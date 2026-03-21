using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class Eventer : MonoBehaviour, InputSystem_Actions.IENTERActions
{
    [SerializeReference] List<Ev> list;

    EventContext context;

    public bool PressEnter;

    [SerializeField] GameObject textbubblePrefab;

    bool ThisRunning;

    InputSystem_Actions input;

    private void Awake()
    {
        input = new InputSystem_Actions();
        input.ENTER.SetCallbacks(this);
        context = new EventContext();
        context.Player = FindFirstObjectByType<PlayerMovement>().gameObject;
        context.evt = this;
        context.camScript = Camera.main.GetComponent<PlayerCamera>();
    }

    [ContextMenu("0 一時停止")]
    void AddWait()
    {
        list.Add(new eWait());
    }

    [ContextMenu("1 会話")]
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

    private void Start()
    {
        StartCoroutine(RunEvents());
    }

    public void OnEnter(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            PressEnter = true;
        }
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
            PressEnter = false;
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

// BGM変更
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