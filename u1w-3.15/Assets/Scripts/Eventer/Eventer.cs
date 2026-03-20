using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eventer : MonoBehaviour
{
    [SerializeReference] List<Ev> list;

    EventContext context;

    private void Awake()
    {
        context = new EventContext();
        context.Player = FindFirstObjectByType<PlayerMovement>().gameObject;
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

    private void Start()
    {
        StartCoroutine(RunEvents());
    }

    IEnumerator RunEvents()
    {
        foreach (var ev in list)
        {
            yield return StartCoroutine(ev.Execute(this, context));
        }
    }
}

//必要なデータを送る
public class EventContext
{
    public GameObject Player;
    public bool PressEnter;
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
    public bool WaitUntilEnd;

    public string speaker;
    [TextArea(4,4)]public string text;

    public override IEnumerator Execute(MonoBehaviour runner, EventContext context)
    {
        if (WaitUntilEnd)
        {
            //yield return runner.StartCoroutine(Move());
        }
        else
        {
            //runner.StartCoroutine(Move());
            //yield break;
        }

        yield break;
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