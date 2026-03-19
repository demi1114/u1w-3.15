using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using TMPro;

public class GallerySys : MonoBehaviour
{
    // ギャラリーの画像選択用

    public int Selected;
    [SerializeField] List<GalleryPicture> list;

    [SerializeField] SpriteRenderer target;

    [SerializeField] Slider slider;

    [SerializeField] TextMeshProUGUI picTitle, picDesc;

    public bool HideUI;

    [SerializeField] RectTransform UIStuff;

    public int posX, posY;

    private void Awake()
    {
        PictureChangeTo(0);
    }

    private void Update()
    {
        target.gameObject.transform.localScale = Vector3.one * slider.value;

        //キー入力受付(仮)
        if (Input.GetKeyDown(KeyCode.D))
        {
            NextPic();
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            PrevPic();
        }

        if (HideUI)//UI隠すやつ
        {
            UIStuff.localScale = Vector3.Lerp(UIStuff.localScale, new Vector3(1, 1.4f, 1), Time.deltaTime*6);
        }
        else
        {
            UIStuff.localScale = Vector3.Lerp(UIStuff.localScale, new Vector3(1, 1, 1), Time.deltaTime*6);
        }

        CheckOutRange();
        Vector2 mousepos = Mouse.current.delta.ReadValue();
        if (Input.GetMouseButton(0))//クリック中
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return; // UI触ってる間は処理しない
            }
            target.transform.localPosition += (Vector3)(mousepos * 0.015f);
            CheckOutRange();
        }
    }

    void CheckOutRange()
    {
        if (target.transform.localPosition.x > target.sprite.texture.width * 0.005f * slider.value)
        {
            target.transform.localPosition = new Vector2(target.sprite.texture.width * 0.005f * slider.value, target.transform.localPosition.y);
        }
        if (target.transform.localPosition.x < target.sprite.texture.width * -0.005f * slider.value)
        {
            target.transform.localPosition = new Vector2(target.sprite.texture.width * -0.005f * slider.value, target.transform.localPosition.y);
        }
        if (target.transform.localPosition.y > target.sprite.texture.height * 0.005f * slider.value)
        {
            target.transform.localPosition = new Vector2(target.transform.localPosition.x, target.sprite.texture.height * 0.005f * slider.value);
        }
        if (target.transform.localPosition.y < target.sprite.texture.height * -0.005f * slider.value)
        {
            target.transform.localPosition = new Vector2(target.transform.localPosition.x, target.sprite.texture.height * -0.005f * slider.value);
        }
    }

    public void NextPic()
    {
        Selected++;
        Debug.Log("Inc");
        if (Selected >= list.Count) Selected = 0;
        PictureChangeTo(Selected);
    }
    public void PrevPic()
    {
        Selected--;
        Debug.Log("Dec");
        if (Selected < 0) Selected = list.Count - 1;
        PictureChangeTo(Selected);
    }
    public void ResetPos()
    {
        target.transform.localPosition = Vector3.zero;
    }

    public void PictureChangeTo(int Num)
    {
        target.sprite = list[Num].picture;
        picTitle.SetText(list[Num].Title);
        picDesc.SetText(list[Num].Description);
    }

    public void SliderIncrease()
    {
        slider.value += 0.05f;
    }
    public void SliderDecrease()
    {
        slider.value -= 0.05f;
    }

    public void HideUnhideUI()
    {
        HideUI = !HideUI;
    }
}