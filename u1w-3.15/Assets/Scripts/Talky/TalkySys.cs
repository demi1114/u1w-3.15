using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TalkySys : MonoBehaviour
{
    TypeWriter writer;
    [SerializeField] TextMeshProUGUI nameui;

    [SerializeField] RectTransform Container, CharL, CharR;
    [SerializeField] CanvasGroup BG;

    private void Awake()
    {
        writer = GetComponent<TypeWriter>();
        this.transform.localScale = new Vector3(0, 1, 1);
    }

    public void Speak(string name, string text)
    {
        nameui.text = name;
        writer.StartCoroutine(writer.Play(text));
    }

    public bool Gonext()
    {
        if (writer.Ready)
        {
            return true;
        }
        else
        {
            writer.Skip();
            return false;
        }
    }

    public void ChangeCharacter(string ch, Sprite pic)
    {
        if(ch == "LEFT")
        {
            CharL.GetComponent<Image>().sprite = pic;
        }
        else if(ch == "RIGHT")
        {
            CharR.GetComponent<Image>().sprite = pic;
        }
    }

    public bool IsReady()
    {
        return writer.Ready;
    }

    public void BringUpUI(float Sec = 1.0f, bool ignorecharL = false, bool ignorecharR = false)
    {
        writer.tmp.text = "";
        writer.shdw.text = "";
        nameui.text = "";
        StartCoroutine(ShowUI(Sec, ignorecharL, ignorecharR));
        this.transform.localScale = new Vector3(1, 1, 1);
    }
    public void BringOutUI(float Sec = 1.0f, bool ignorecharL = false, bool ignorecharR = false)
    {
        writer.tmp.text = "";
        writer.shdw.text = "";
        nameui.text = "";
        StartCoroutine(HideUI(Sec, ignorecharL, ignorecharR));
        this.transform.localScale = new Vector3(1, 1, 1);
    }

    public void HideLeft()
    {
        StartCoroutine(HideLeftCharacter());
    }
    public void HideRight()
    {
        StartCoroutine(HideRightCharacter());
    }
    public void ShowLeft()
    {
        StartCoroutine (ShowLeftCharacter());
    }
    public void ShowRight()
    {
        StartCoroutine (ShowRightCharacter());
    }

    IEnumerator ShowUI(float Sec, bool ignorecharL = false, bool ignorecharR = false)
    {
        Container.anchoredPosition = new Vector2(0, -160);
        for (float t = 0f; t < Sec; t += Time.unscaledDeltaTime)
        {
            float ease = Mathf.Sin((t / Sec) * Mathf.PI * 0.5f);
            float easefast = 1.0f;
            if (t<Sec*0.5f)
            {
                easefast = Mathf.Sin((t / (Sec * 0.5f)) * Mathf.PI * 0.5f);
            }

            Container.anchoredPosition = new Vector2(0, Mathf.Lerp(-160, 160,ease));
            if(!ignorecharR) CharR.anchoredPosition = new Vector2(Mathf.Lerp(800, 400, easefast), -250);
            if(!ignorecharL) CharL.anchoredPosition = new Vector2(-Mathf.Lerp(800, 400, easefast), -250);

            BG.alpha = (t / Sec);

            yield return null;
        }
        BG.alpha = 1.0f;
        if (!ignorecharR) CharR.anchoredPosition = new Vector2(400, -250);
        if (!ignorecharL) CharL.anchoredPosition = new Vector2(-400, -250);
        Container.anchoredPosition = new Vector2(0, 160);
    }

    IEnumerator HideUI(float Sec, bool ignorecharL = false, bool ignorecharR = false)
    {
        Container.anchoredPosition = new Vector2(0, -160);
        for (float t = 0f; t < Sec; t += Time.unscaledDeltaTime)
        {
            float ease = Mathf.Sin((t / Sec) * Mathf.PI * 0.5f);
            float easefast = 1.0f;
            if (t < Sec * 0.5f)
            {
                easefast = Mathf.Sin((t / (Sec * 0.5f)) * Mathf.PI * 0.5f);
            }

            Container.anchoredPosition = new Vector2(0, Mathf.Lerp(160, -160, ease));
            if (!ignorecharR) CharR.anchoredPosition = new Vector2(Mathf.Lerp(400, 800, easefast), -250);
            if(!ignorecharL)CharL.anchoredPosition = new Vector2(-Mathf.Lerp(400, 800, easefast), -250);

            BG.alpha = 1.0f - (t / Sec);

            yield return null;
        }
        BG.alpha = 0.0f;
        if (!ignorecharR) CharR.anchoredPosition = new Vector2(800, -250);
        if(!ignorecharL)CharL.anchoredPosition = new Vector2(-800, -250);
        Container.anchoredPosition = new Vector2(0, -160);
        this.transform.localScale = new Vector2(0, 1);
    }

    IEnumerator HideLeftCharacter()
    {
        for (float t = 0f; t < 1; t += Time.unscaledDeltaTime)
        {
            float ease = Mathf.Sin((t / 1) * Mathf.PI * 0.5f);
            CharL.anchoredPosition = new Vector2(-Mathf.Lerp(400, 800, ease), -250);
            yield return null;
        }
        CharL.anchoredPosition = new Vector2(-800, -250);
    }
    IEnumerator ShowLeftCharacter()
    {
        for (float t = 0f; t < 1; t += Time.unscaledDeltaTime)
        {
            float ease = Mathf.Sin((t / 1) * Mathf.PI * 0.5f);
            CharL.anchoredPosition = new Vector2(-Mathf.Lerp(800, 400, ease), -250);
            yield return null;
        }
        CharL.anchoredPosition = new Vector2(-400, -250);
    }
    IEnumerator HideRightCharacter()
    {
        for (float t = 0f; t < 1; t += Time.unscaledDeltaTime)
        {
            float ease = Mathf.Sin((t / 1) * Mathf.PI * 0.5f);
            CharR.anchoredPosition = new Vector2(Mathf.Lerp(400, 800, ease), -250);
            yield return null;
        }
        CharR.anchoredPosition = new Vector2(800, -250);
    }
    IEnumerator ShowRightCharacter()
    {
        for (float t = 0f; t < 1; t += Time.unscaledDeltaTime)
        {
            float ease = Mathf.Sin((t / 1) * Mathf.PI * 0.5f);
            CharR.anchoredPosition = new Vector2(Mathf.Lerp(800, 400, ease), -250);
            yield return null;
        }
        CharR.anchoredPosition = new Vector2(400, -250);
    }
}
