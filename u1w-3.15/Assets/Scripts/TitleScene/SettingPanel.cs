using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SettingPanel : MonoBehaviour
{
    public enum Status
    {
        Waiting,
        Open,
        Close
    }

    public Status state;
    CanvasGroup group;
    GameObject childObj;

    [SerializeField] Slider Master, BGM, SE;

    private void Awake()
    {
        childObj = transform.GetChild(0).gameObject;
        childObj.gameObject.SetActive(false);
        group = GetComponent<CanvasGroup>();

        Master.value = GameMaster.instance.GetVolume("Master");
        BGM.value = GameMaster.instance.GetVolume("BGM");
        SE.value = GameMaster.instance.GetVolume("SE");
    }

    public void SetStatusOpen()
    {
        SetStatus(Status.Open);
    }

    public void SetStatusClose()
    {
        SetStatus(Status.Close);
    }

    public void SetStatus(Status status)
    {
        if(state != Status.Waiting)
        {
            return;
        }


        switch (status)
        {
            case Status.Open:
                childObj.SetActive(true);
                StartCoroutine(CanvasGroupAlpha(0, 1, 0.5f, true));
                break;
            case Status.Close:
                StartCoroutine(CanvasGroupAlpha(1, 0, 0.5f));
                break;
        }
    }
    IEnumerator CanvasGroupAlpha(float from, float to, float timer=1.0f, bool Enables = false)
    {
        for (float t = 0; t < timer; t += Time.unscaledDeltaTime)
        {
            float GetPercentage = t / timer;
            group.alpha = Mathf.Lerp(from, to, GetPercentage);
            yield return null;
        }
        group.interactable = Enables;
        group.alpha = to;
        childObj.SetActive(Enables);
    }

    public void GetSetMasterVolume(Slider slider)
    {
        GameMaster.instance.SetVolume("Master", slider.value);
    }
    public void GetSetBGMVolume(Slider slider)
    {
        GameMaster.instance.SetVolume("BGM", slider.value);
    }
    public void GetSetSEVolume(Slider slider)
    {
        GameMaster.instance.SetVolume("SE", slider.value);
    }
}
