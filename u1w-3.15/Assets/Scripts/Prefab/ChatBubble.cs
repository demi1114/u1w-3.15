using TMPro;
using UnityEngine;

public class ChatBubble : MonoBehaviour
{
    [SerializeField] TextMeshPro NameTMP, ContextTMP;
    [SerializeField] GameObject NameOBJ, ContextOBJ;
    
    public float charHeight = 1f, charWidth = 1f;

    private void Update()
    {
        FixBubbleDraw();
    }

    public void SetText(string text)
    {
        ContextTMP.text = text;
    }
    public void SetName(string text)
    {
        NameTMP.text = text;
    }

    public void FixBubbleDraw()
    {
        string text = ContextTMP.text;//テキスト受け取りテスト
        // string text = yes;

        string[] lines = text.Split('\n');

        int lineCount = lines.Length;
        int maxLineLength = 0;

        foreach (var line in lines)
        {
            if (line.Length > maxLineLength)
                maxLineLength = line.Length;
        }

        float width = maxLineLength * charWidth;
        float height = lineCount * charHeight;

        SpriteRenderer spriteRenderer = ContextOBJ.GetComponent<SpriteRenderer>();
        spriteRenderer.size = new Vector2(width + 1, height);
        ContextOBJ.transform.localPosition = new Vector2(0,1 + height*0.5f);
        ContextTMP.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
        NameOBJ.transform.localPosition = new Vector2(2 - width*0.5f, 1.35f + height);
    }

    public void Destroy()
    {
        Destroy(this.gameObject);
    }
}
