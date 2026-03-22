using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// 究極に可読性悪し、覚悟を決めて読破せよ。

public class Eventer : MonoBehaviour, InputSystem_Actions.IENTERActions
{
    [SerializeReference] List<Ev> list;

    EventContext context;

    public bool PressEnter;

    TalkySys talkySys;
    public bool TalkySkippable;

    [SerializeField] GameObject textbubblePrefab;

    bool ThisRunning;

    InputSystem_Actions input;

    private void Awake()
    {
        this.gameObject.tag = "Eventer";

        input = new InputSystem_Actions();
        input.ENTER.SetCallbacks(this);
        context = new EventContext();
        context.Player = FindFirstObjectByType<PlayerMovement>().gameObject;
        context.evt = this;
        context.camScript = Camera.main.GetComponent<PlayerCamera>();
        talkySys = FindFirstObjectByType<TalkySys>();
        context.tlk = talkySys;
        context.camMan = FindFirstObjectByType<CameraMan>();
    }

    [ContextMenu("SAVE_0 フラグ変更")]
    void AddSavesFlag()
    {
        list.Add(new eSavesFlag());
    }
    [ContextMenu("0 一時停止")]
    void AddWait()
    {
        list.Add(new eWait());
    }

    [ContextMenu("1 テキストバブル")]
    void AddChat()
    {
        list.Add(new eChat());
    }

    [ContextMenu("2A オブジェクト移動(Transform)")]
    void MoveOBJTransform()
    {
        list.Add(new eMoveOBJTTrns());
    }
    [ContextMenu("2B オブジェクト移動(Vector3)")]
    void MoveOBJVector()
    {
        list.Add(new eMoveOBJTVec());
    }

    [ContextMenu("3A 会話UI出す")]
    void ShowTalkUI()
    {
        list.Add(new eTalkUIShow());
    }
    [ContextMenu("3B 会話UI隠す")]
    void HideTalkUI()
    {
        list.Add(new eTalkUIHide());
    }
    [ContextMenu("3Main 会話書き込み")]
    void SpeakTalkUI()
    {
        list.Add(new eTalkUISpeak());
    }
    [ContextMenu("3C キャラ非表示")]
    void HideCharTalkUI()
    {
        list.Add(new eTalkUIHideChar());
    }
    [ContextMenu("3D キャラ絵変更")]
    void ChangeCharTalkUI()
    {
        list.Add(new eTalkUIChangeChar());
    }

    [ContextMenu("Z_CMN_A プレイヤーのRIGIDBODYを停止")]
    void PlrDisableRB()
    {
        list.Add(new eDisableRB());
    }
    [ContextMenu("Z_CMN_B プレイヤーのINPUTを停止")]
    void PlrDisableInput()
    {
        list.Add(new eDisableInput());
    }
    [ContextMenu("Z_CMN_C カメラ追加シフト")]
    void CamAdditionalShifter()
    {
        list.Add(new eCamShifter());
    }
    [ContextMenu("Z_CMN_D カメラ速度 通常or瞬間")]
    void CamSpdChange()
    {
        list.Add(new eCamSpd());
    }
    [ContextMenu("Z_CMN_E カメラモード 通常or固定")]
    void CamModeChange()
    {
        list.Add(new eCamMode());
    }
    [ContextMenu("MUS BGM変更 (リストから)")]
    void BGMChangeByList()
    {
        list.Add(new eBgmChangeList());
    }
    [ContextMenu("MUS BGM変更")]
    void BGMChangeByAudioClip()
    {
        list.Add(new eBgmChange());
    }
    [ContextMenu("MUS SE再生")]
    void PlaySEonUI()
    {
        list.Add(new ePlaySE());
    }

    private void Start()
    {
        //StartCoroutine(RunEvents());
    }

    public void OnEnter(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            PressEnter = true;
        }
    }

    public bool TalkyForceExit;

    private void Update()
    {
        context.TalkyForceExit = TalkyForceExit;

        if (ForceRunAfterCamClose && context.camMan.CamEnd)
        {
            Run();
        }
    }

    private void LateUpdate()
    {
        PressEnter = false;
    }

    public bool ForceRunOnTouch;
    public bool ForceRunOnEnable;

    //カメラマンチェック
    public bool ForceRunAfterCamClose;

    //アルバムチェック
    public enum AlbumChecker
    {
        None,EnableIfExist,DisableIfExist,
    }
    public AlbumChecker IfIsAlbum;
    public string[] targetObjectName;

    //セーブのフラグチェック
    public enum SFlagChecker
    {
        None, EnableIfFlagIsOn, DisableIfFlagIsOn,
    }
    public SFlagChecker IfSaveFlag;
    public int FlagTarget;

    private void OnEnable()
    {
        if (ForceRunOnEnable)
        {
            Run();
        }
    }

    public bool Run()
    {
        if (IfSaveFlag == SFlagChecker.EnableIfFlagIsOn && !SaveDatas.instance.Flags[FlagTarget])
        {
            return false;
        }
        if (IfSaveFlag == SFlagChecker.DisableIfFlagIsOn && SaveDatas.instance.Flags[FlagTarget])
        {
            return false;
        }
        if(IfIsAlbum == AlbumChecker.EnableIfExist && !SaveDatas.instance.FindAlbum(targetObjectName))
        {
            return false;
        }
        if (IfIsAlbum == AlbumChecker.DisableIfExist && SaveDatas.instance.FindAlbum(targetObjectName))
        {
            return false;
        }
        if (ThisRunning) return false;
        StartCoroutine(RunEvents());
        return true;
    }

    void SetRunning(bool running)
    {
        ThisRunning = running;

        if (running)
            input.ENTER.Enable();
        else
            input.ENTER.Disable();
    }

    IEnumerator RunEvents() // 実行
    {
        SetRunning(true);
        foreach (var ev in list)
        {
            yield return StartCoroutine(ev.Execute(this, context));
        }
        SetRunning(false);
    }

    // チャットバブル召喚
    public GameObject SpawnChatBubble(string Name, string Text, Vector2 Pos)
    {
        GameObject genobj = Instantiate(textbubblePrefab, (Vector3)Pos, Quaternion.identity);
        ChatBubble cBub = genobj.GetComponent<ChatBubble>();
        cBub.SetName(Name);
        cBub.SetText(Text);

        return genobj;
    }
}

//必要なデータを送る
public class EventContext
{
    public GameObject Player;
    public PlayerCamera camScript;
    public Eventer evt;
    public TalkySys tlk;
    public CameraMan camMan;

    public bool TalkyForceExit;
}

public class EvDict
{
    public static Dictionary<string, string> nameDict = new Dictionary<string, string>()
    {
        { "eWait", "イベントリスト進行待機(秒)" },
        { "eChat", "テキストバブル" },
        { "eMoveOBJTTrns", "移動(Transform)" },
        { "eMoveOBJTVec", "移動(Vector3)" },
        { "eDisableRB", "プレイヤーRigidBody無効化" },
        { "eDisableInput", "プレイヤー操作無効化" },
        { "eCamShifter", "カメラ位置ズラシ(カメラ基本座標を中心にずらす)" },
        { "eCamSpd", "カメラ速度" },
        { "eCamMode", "カメラのモード(Fixed=固定)" },
        { "eBgmChangeList", "BGMを変更 (リストから)" },
        { "eBgmChange", "BGMを変更 (AudioClip)" },
        { "ePlaySE", "SE再生2D (AudioClip)" },
        { "eTalkUIShow", "会話UI 表示" },
        { "eTalkUIHide", "会話UI 非表示" },
        { "eTalkUISpeak", "会話UI テキスト描画" },
        { "eTalkUIChangeChar", "会話UI キャラ絵変更" },
        { "eTalkUIHideChar", "会話UI キャラ非表示" },
        { "eSavesFlag", "セーブデータ フラグを変更" },
        { "", "" }
    };
}

[System.Serializable]
public abstract class Ev
{
    public abstract IEnumerator Execute(MonoBehaviour runner, EventContext context);
}

// 一定時間待機
[System.Serializable]
public class eWait : Ev
{
    public float time;

    public override IEnumerator Execute(MonoBehaviour runner, EventContext context)
    {
        yield return new WaitForSeconds(time);
    }
}

// 会話文表示
[System.Serializable]
public class eChat : Ev
{
    public bool WaitUntilBtn;
    public bool DeleteByTimer;

    public string speaker;
    [TextArea(4,4)]public string text;

    public float Timer;

    public bool UseTransformAsCenter;
    public Vector2 Position;
    public Transform transform;

    public override IEnumerator Execute(MonoBehaviour runner, EventContext context)
    {
        if (WaitUntilBtn)
        {
            yield return runner.StartCoroutine(Chat(context));
        }
        else if (DeleteByTimer)
        {
            yield return runner.StartCoroutine(WaitTimeChat(context,Timer));
        }
        else if (!WaitUntilBtn && !DeleteByTimer)
        {
            if(UseTransformAsCenter) context.evt.SpawnChatBubble(speaker, text, Position+(Vector2)transform.position);
            else context.evt.SpawnChatBubble(speaker, text, Position);
            yield break;
        }
    }

    IEnumerator Chat(EventContext context)
    {
        GameObject chatBubble;
        if (UseTransformAsCenter) chatBubble = context.evt.SpawnChatBubble(speaker, text, Position + (Vector2)transform.position);
        else chatBubble = context.evt.SpawnChatBubble(speaker, text, Position);
        while (!context.evt.PressEnter) //ボタン押されるまで待機
        {
            yield return null;
        }
        chatBubble.GetComponent<ChatBubble>().Destroy();
    }

    IEnumerator WaitTimeChat(EventContext context, float t)
    {
        GameObject chatBubble;
        if (UseTransformAsCenter) chatBubble = context.evt.SpawnChatBubble(speaker, text, Position + (Vector2)transform.position);
        else chatBubble = context.evt.SpawnChatBubble(speaker, text, Position);
        yield return new WaitForSeconds(t);
        chatBubble.GetComponent<ChatBubble>().Destroy();
    }
}

// Transform へ オブジェクト移動
[System.Serializable]
public class eMoveOBJTTrns : Ev
{
    public bool WaitUntilEnd;

    public GameObject Target;
    public Transform Destination;

    public float Timer;

    public override IEnumerator Execute(MonoBehaviour runner, EventContext context)
    {
        if (WaitUntilEnd)
        {
            yield return runner.StartCoroutine(Move());
        }
        else
        {
            runner.StartCoroutine(Move());
            yield break;
        }
    }
    IEnumerator Move()
    {
        float t = 0;
        Vector3 start = Target.transform.position;

        while (t < Timer)
        {
            t += Time.deltaTime;
            float rate = t / Timer;

            Target.transform.position = Vector3.Lerp(start, Destination.position, rate);

            yield return null;
        }

        Target.transform.position = Destination.position;
    }
}

// Vector3 へ オブジェクト移動
[System.Serializable]
public class eMoveOBJTVec : Ev
{
    public bool WaitUntilEnd;

    public GameObject Target;
    public Vector3 Destination;

    public float Timer;

    public override IEnumerator Execute(MonoBehaviour runner, EventContext context)
    {
        if (WaitUntilEnd)
        {
            yield return runner.StartCoroutine(Move());
        }
        else
        {
            runner.StartCoroutine(Move());
            yield break;
        }
    }
    IEnumerator Move()
    {
        float t = 0;
        Vector3 start = Target.transform.position;

        while (t < Timer)
        {
            t += Time.deltaTime;
            float rate = t / Timer;

            Target.transform.position = Vector3.Lerp(start, Destination, rate);

            yield return null;
        }

        Target.transform.position = Destination;
    }
}

// Player の RigidBody 無効化
[System.Serializable]
public class eDisableRB : Ev
{
    public bool DisableRigidBody;

    public override IEnumerator Execute(MonoBehaviour runner, EventContext context)
    {
        if(DisableRigidBody)
        {
            context.Player.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        }
        else
        {
            context.Player.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        }
        yield break;
    }
}

// Player の 操作 無効化
[System.Serializable]
public class eDisableInput : Ev
{
    public bool DisableInput;

    public override IEnumerator Execute(MonoBehaviour runner, EventContext context)
    {
        context.Player.GetComponent<PlayerMovement>().DisablePlayerInput(DisableInput);
        yield break;
    }
}

// カメラのシフト書き換え
[System.Serializable]
public class eCamShifter : Ev
{
    public Vector2 Shift;

    public override IEnumerator Execute(MonoBehaviour runner, EventContext context)
    {
        context.camScript.AdditionalShift = Shift;
        yield break;
    }
}

// カメラの速度
[System.Serializable]
public class eCamSpd : Ev
{
    public PCamSpd Mode;

    public override IEnumerator Execute(MonoBehaviour runner, EventContext context)
    {
        context.camScript.CamSpd = Mode;
        yield break;
    }
}
// カメラのモード
[System.Serializable]
public class eCamMode : Ev
{
    public PCamMode Mode;
    public bool UseTransformAsCenter;
    public Vector2 Position;
    public Transform transform;

    public override IEnumerator Execute(MonoBehaviour runner, EventContext context)
    {
        context.camScript.CamMode = Mode;
        if(UseTransformAsCenter) context.camScript.FixedCamPos = (Vector2)transform.position + Position;
        else context.camScript.FixedCamPos = Position;
        yield break;
    }
}

// BGM変更(リストから)
[System.Serializable]
public class eBgmChangeList : Ev
{
    public MusicController.BGMType BGMType;

    public override IEnumerator Execute(MonoBehaviour runner, EventContext context)
    {
        MusicController.instance.ChangeBGM(BGMType);
        yield break;
    }
}
// BGM変更
[System.Serializable]
public class eBgmChange : Ev
{
    public AudioClip BGMType;

    public override IEnumerator Execute(MonoBehaviour runner, EventContext context)
    {
        MusicController.instance.ChangeBGM(BGMType);
        yield break;
    }
}

// SE再生
[System.Serializable]
public class ePlaySE : Ev
{
    public AudioClip SE;
    public float Volume = 1.0f;

    public override IEnumerator Execute(MonoBehaviour runner, EventContext context)
    {
        MusicController.instance.PlaySE(SE, Volume);
        yield break;
    }
}

// 会話UI呼び出し
[System.Serializable]
public class eTalkUIShow : Ev
{
    public float Timer = 1.0f;

    public bool WaitTime = true;

    [Header("アニメーションさせない")]
    public bool IgnoreCharL = false;
    public bool IgnoreCharR = false;

    public override IEnumerator Execute(MonoBehaviour runner, EventContext context)
    {
        context.tlk.BringUpUI(Timer, IgnoreCharL, IgnoreCharR);
        if (WaitTime) yield return new WaitForSecondsRealtime(Timer);
        yield break;
    }
}

// 会話UI隠す
[System.Serializable]
public class eTalkUIHide : Ev
{
    public float Timer = 1.0f;

    public bool WaitTime = true;

    [Header("アニメーションさせない")]
    public bool IgnoreCharL = false;
    public bool IgnoreCharR = false;

    public override IEnumerator Execute(MonoBehaviour runner, EventContext context)
    {
        context.tlk.BringOutUI(Timer, IgnoreCharL, IgnoreCharR);
        if (WaitTime) yield return new WaitForSecondsRealtime(Timer);
        yield break;
    }
}

// 会話UI 描画
[System.Serializable]
public class eTalkUISpeak : Ev
{
    public string Name;
    [TextArea(4,4)]public string Text;

    public bool Skippable = true;

    public override IEnumerator Execute(MonoBehaviour runner, EventContext context)
    {
        context.evt.TalkySkippable = Skippable;
        context.tlk.Speak(Name, Text);
        yield return runner.StartCoroutine(TalkySkip(context));
        yield break;
    }

    IEnumerator TalkySkip(EventContext context)
    {
        while (!context.TalkyForceExit) //ループ
        {
            if (context.evt.PressEnter)//ボタン押されるまで待機
            {
                if (Skippable)
                {
                    if (context.tlk.Gonext())
                    {
                        yield break;
                    }
                }
                else
                {
                    if (context.tlk.IsReady())
                    {
                        yield break;
                    }
                }
            }
            yield return null;
        }
    }
}

// 会話UI キャラ非表示
[System.Serializable]
public class eTalkUIHideChar : Ev
{
    public bool Hide = true;

    public enum whichChar { CharL, CharR };
    public whichChar Char;

    public bool WaitUntilEnd;

    public override IEnumerator Execute(MonoBehaviour runner, EventContext context)
    {
        switch (Char)
        {
            case (whichChar.CharL):
                if(Hide) context.tlk.HideLeft();
                else context.tlk.ShowLeft();
                break;
            case (whichChar.CharR):
                if (Hide) context.tlk.HideRight();
                else context.tlk.ShowRight();
                break;
        }
        if (WaitUntilEnd) yield return new WaitForSecondsRealtime(1.0f);
        yield break;
    }
}

// 会話UI キャラ変更
[System.Serializable]
public class eTalkUIChangeChar : Ev
{
    public enum whichChar { CharL, CharR };
    public whichChar Char;
    public Sprite picture;

    public override IEnumerator Execute(MonoBehaviour runner, EventContext context)
    {
        switch (Char)
        {
            case (whichChar.CharL):
                context.tlk.ChangeCharacter("LEFT", picture);
                break;
            case (whichChar.CharR):
                context.tlk.ChangeCharacter("RIGHT", picture);
                break;
        }
        yield break;
    }
}

// セーブデータ フラグ変更
[System.Serializable]
public class eSavesFlag : Ev
{
    public int FlagNum;
    public bool Flag = true;

    public override IEnumerator Execute(MonoBehaviour runner, EventContext context)
    {
        SaveDatas.instance.Flags[FlagNum] = Flag;
        yield break;
    }
}