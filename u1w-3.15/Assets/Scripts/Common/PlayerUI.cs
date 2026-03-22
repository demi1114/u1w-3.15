using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] GameObject VirtualPads;

    public void UIHide()
    {
        this.transform.localScale = Vector2.zero;
    }
    public void UIShow()
    {
        this.transform.localScale = Vector2.one;
    }

    private void Update()
    {
        if (GameMaster.instance.VirtualPad)
        {
            VirtualPads.transform.localScale = Vector2.one;
        }
        else
        {
            VirtualPads.transform.localScale = Vector2.zero;
        }
    }
}
