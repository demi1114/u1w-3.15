using TMPro;
using UnityEngine;

public class TaskKeeper : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI TaskText;
    static public string NextText = "“Á‚É‚È‚µ";
    CanvasGroup cG;

    static public void TextRefresh(string targetTEXT)
    {
        textlength = 0;
        NextText = targetTEXT;
    }

    float delta = 0.0f;
    [SerializeField] float tickrate = 0.1f;
    [SerializeField] static int textlength = int.MaxValue;

    void Awake()
    {
        cG = GetComponent<CanvasGroup>();
    }

    private void Update()
    {
        TaskText.text = NextText.Substring(0,Mathf.Min(textlength,NextText.Length));
        TickCheck();
    }

    void TickCheck()
    {
        delta += Time.deltaTime;
        if (delta >= tickrate)
        {
            textlength++;
            delta = 0.0f;
        }
    }

    public void hideUIquick()
    {
        cG.alpha = 0.0f;
    }

    public void showUIquick()
    {
        cG.alpha = 1.0f;
    }
}
