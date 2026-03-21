using System.Collections;
using TMPro;
using UnityEngine;

public class TypeWriter : MonoBehaviour
{
    public TextMeshProUGUI tmp;
    public TextMeshProUGUI shdw;

    public float defaultSpeed = 0.1f; // 1文字0.1秒
    float currentSpeed;

    public bool Ready;

    void Awake()
    {
        //テスト用
        //StartCoroutine(Play("なあああああああ\nええええええ[SPEED=10]\nうううううううううううう"));
    }

    private void Update()
    {
        shdw.text = tmp.text;
    }

    public void Skip()
    {
        currentSpeed = 0.001f;
    }

    public IEnumerator Play(string text)
    {
        Ready = false;
        tmp.text = "";
        currentSpeed = defaultSpeed;

        for (int i = 0; i < text.Length; i++)
        {
            char c = text[i];

            // コマンド開始
            if (c == '[')
            {
                int end = text.IndexOf(']', i);
                if (end != -1)
                {
                    string command = text.Substring(i + 1, end - i - 1);
                    yield return ExecuteCommand(command);

                    i = end; // コマンド分スキップ
                    continue;
                }
            }

            // 通常文字
            tmp.text += c;

            if (c != '\n') // 改行は待たない
                yield return new WaitForSeconds(currentSpeed);
        }
        Ready = true;
    }

    IEnumerator ExecuteCommand(string cmd)
    {
        // WAIT=10
        if (cmd.StartsWith("WAIT="))
        {
            string val = cmd.Replace("WAIT=", "");
            if (float.TryParse(val, out float v))
            {
                yield return new WaitForSeconds(v * 0.1f);
            }
        }

        // SPEED=5
        else if (cmd.StartsWith("SPEED="))
        {
            string val = cmd.Replace("SPEED=", "");
            if (float.TryParse(val, out float v))
            {
                currentSpeed = v * 0.1f; // 1で0.1秒
            }
        }
    }
}