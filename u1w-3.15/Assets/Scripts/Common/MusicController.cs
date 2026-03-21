using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    public static MusicController instance;

    private void Awake()
    {
        // ƒVƒ“ƒOƒ‹ƒgƒ“‰»
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    //BGM
    public AudioSource BGMChannel;
    public int MainBGMtype = 0;
    public AudioClip TitleBGM;
    public AudioClip[] MainBGMs;

    public BGMType playingBGMType = BGMType.Other;

    public enum BGMType
    {
        Title,
        Main,
        Other
    }

    public void ChangeBGM(BGMType mus, float Duration = 1.0f, bool Force = false)
    {
        switch (mus)
        {
            case BGMType.Title:
                if (playingBGMType == mus&&!Force) break;
                playingBGMType = mus;
                StartCoroutine(eChangeBGM(TitleBGM, Duration));
                break;
            case BGMType.Main:
                if (playingBGMType == mus&&!Force) break;
                playingBGMType = mus;
                StartCoroutine(eChangeBGM(MainBGMs[MainBGMtype], Duration));
                break;

            case BGMType.Other:
                Debug.LogWarning("You must not chose `BGMType.Other` via ChangeBGM");
                break;
        }
    }
    public void ChangeBGM(AudioClip mus, float Duration = 1.0f)
    {
        playingBGMType = BGMType.Other;
        StartCoroutine(eChangeBGM(mus, Duration));
    }

    IEnumerator eChangeBGM(AudioClip mus, float Duration = 1.0f)
    {
        for(float t=0.0f; t<Duration; t+=Time.unscaledDeltaTime) 
        {
            BGMChannel.volume = 1.0f - t / Duration;
            yield return null;
        }
        BGMChannel.volume = 0.0f;
        BGMChannel.Stop();
        BGMChannel.clip = mus;
        BGMChannel.volume = 1.0f;
        BGMChannel.Play();
    }

    //SE

    int SelectedSEChannel = 0;
    public AudioSource[] SEChannel;

    public void PlaySE(AudioClip aud, float Vol)
    {
        SEChannel[SelectedSEChannel].PlayOneShot(aud, Vol);
        SelectedSEChannel++;
        if(SelectedSEChannel >= SEChannel.Length)
        {
            SelectedSEChannel = 0;
        }
    }
}
