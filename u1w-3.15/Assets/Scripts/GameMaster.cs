using UnityEngine;
using UnityEngine.Audio;
using static UnityEngine.Rendering.DebugUI;

public class GameMaster : MonoBehaviour
{
    public static GameMaster instance { get; private set; }

    private void Awake()
    {
        // シングルトン化
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        SetTargetFramerate(60);
    }

    public bool VirtualPad;


    public void SetTargetFramerate(int val=60)
    {
        Application.targetFrameRate = val;
    }
    public int GetTargetFrameRate()
    {
        return Application.targetFrameRate;
    }

    [SerializeField] AudioMixer mixer;

    public void SetVolume(string target, float volume)
    {
        float dB = Mathf.Log10(volume) * 20;
        if (volume <= 0.0001f)
            mixer.SetFloat(target, -80f); // 完全ミュート
        else
            mixer.SetFloat(target, dB);
    }
    public float GetVolume(string target)
    {
        float dB;
        if (mixer.GetFloat(target, out dB))
        {
            if (dB <= -80f) return 0f; // ミュート扱い
            return Mathf.Pow(10f, dB / 20f);
        }
        return 1f;
    }
}
