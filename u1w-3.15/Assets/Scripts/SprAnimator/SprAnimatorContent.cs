using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "SprAnimatorContent", menuName = "Scriptable Objects/SprAnimatorContent")]
public class SprAnimatorContent : ScriptableObject
{
    public List<AnimContainer> ANIMLIST;
}

[System.Serializable]
public class AnimContainer
{
    public string NAME;
    public List<SPRITEANDLENGTH> SPRITES;
    public bool LOOP;

    public int GetTotalFrame()
    {
        return SPRITES.Sum(s => s.LENGTH);
    }
    public SPRITEANDLENGTH GetSprite(int targetFrame)
    {
        int frame = targetFrame;

        for (int i = 0; i < SPRITES.Count; i++)
        {
            if (frame < SPRITES[i].LENGTH)
            {
                return SPRITES[i];
            }

            frame -= SPRITES[i].LENGTH;
        }

        //エラー回避
        return SPRITES[SPRITES.Count - 1];
    }
}

[System.Serializable]
public class SPRITEANDLENGTH
{
    public Sprite SPRITE;
    public int LENGTH; // 60fpsベース
    public bool FLIPX;
    public bool FLIPY;
}