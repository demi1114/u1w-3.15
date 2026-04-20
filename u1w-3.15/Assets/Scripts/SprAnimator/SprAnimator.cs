using System.Linq;
using UnityEngine;

public class SprAnimator : MonoBehaviour
{
    SpriteRenderer _spr;

    // 必ず入れるべし。
    public SprAnimatorContent CONTENT;

    private void Awake()
    {
        var GETANIMATOR = GetComponent<SpriteRenderer>();
        if(GETANIMATOR != null)
        {
            _spr = GETANIMATOR;
        }
        else
        {
            Debug.LogError("オブジェクト [ "+this.gameObject.name+" ] にSpriteRendererコンポーネントが有りません。");
        }
    }

    private void Start()
    {
        PlayAnim("IDLE");
    }

    AnimContainer ANIM_PLAYING;

    public bool PlayAnim(string NAME)
    {
        // 同じアニメーション名ならスキップ
        if (ANIM_PLAYING != null && ANIM_PLAYING.NAME == NAME) return false;

        foreach (var anim in CONTENT.ANIMLIST)
        {
            if(anim.NAME == NAME)
            {
                ANIM_PLAYING = anim;
                DELTATIME = 0;
                return true;
            }
        }

        return false;
    }

    [SerializeField] float DELTATIME;
    [SerializeField] int FRAME;

    private void Update()
    {
        //if (ANIM_PLAYING == null) return;
        //if (_spr == null) return;

        DELTATIME += Time.deltaTime;

        var MAX = ANIM_PLAYING.GetTotalFrame()/60f;

        // 仮処理( 必要あれば修正 ) !!!!!!!
        if (ANIM_PLAYING.LOOP)
        {
            if (ANIM_PLAYING.LOOP && DELTATIME > MAX)
            {
                DELTATIME %= MAX;
            }
        }

        FRAME = (int)(DELTATIME * 60);

        _spr.sprite = ANIM_PLAYING.GetSprite(FRAME).SPRITE;
        _spr.flipX = ANIM_PLAYING.GetSprite(FRAME).FLIPX;
        _spr.flipY = ANIM_PLAYING.GetSprite(FRAME).FLIPY;
    }
}
