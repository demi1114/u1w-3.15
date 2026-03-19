using UnityEngine;

[CreateAssetMenu(fileName = "GalleryPicture", menuName = "Scriptable Objects/GalleryPicture")]
public class GalleryPicture : ScriptableObject
{
    public Sprite picture;//画像本体
    [TextArea(1, 1)] public string Title;//ギャラリーの画像のモチーフ名などタイトル
    [TextArea(3, 3)] public string Description;//説明文
}
