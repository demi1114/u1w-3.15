using System.Collections.Generic;
using UnityEngine;

public class SaveDatas : MonoBehaviour
{
    public List<Album> album;

    public bool[] Flags = new bool[256];

    public static SaveDatas instance;
    private void Awake()
    {
        // ƒVƒ“ƒOƒ‹ƒgƒ“‰»
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

    public bool FindAlbum(string[] NeedObj)
    {
        var needSet = new HashSet<string>(NeedObj);

        foreach (var alb in album)
        {
            var targetSet = new HashSet<string>(alb.targets);

            if (needSet.IsSubsetOf(targetSet))
            {
                return true;
            }
        }

        return false;
    }
}
