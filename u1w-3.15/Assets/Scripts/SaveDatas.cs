using System.Collections.Generic;
using UnityEngine;

public class SaveDatas : MonoBehaviour
{
    public List<Album> album;

    public bool[] Flags = new bool[256];
    [Range(0, 255)] public byte[] NumVar = new byte[32];

    public bool HavePositionSettingByVector;
    public bool HavePositionSettingByTransform;
    public string PositionTransform;
    public Vector2 PositionVector;

    public static SaveDatas instance;
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
    }

    public bool FindAlbum(string[] NeedObj, bool MakePhotoUnDelistable = false)
    {
        var needSet = new HashSet<string>(NeedObj);

        // まず「同じ条件で既にロックされてるものがあるか」チェック
        foreach (var alb in album)
        {
            var targetSet = new HashSet<string>(alb.targets);

            if (needSet.IsSubsetOf(targetSet) && alb.UnDelistable)
            {
                return true; // ← もうロック済みなので何もしない
            }
        }

        // ロックされてないなら、最初の1件だけロック
        foreach (var alb in album)
        {
            var targetSet = new HashSet<string>(alb.targets);

            if (needSet.IsSubsetOf(targetSet))
            {
                if (MakePhotoUnDelistable)
                {
                    alb.UnDelistable = true;
                }
                return true;
            }
        }

        return false;
    }

    public int DelistAlbum(int Number)
    {
        if (album[Number].UnDelistable) return 1;
        album.RemoveAt(Number);
        return 0;
    }
}
