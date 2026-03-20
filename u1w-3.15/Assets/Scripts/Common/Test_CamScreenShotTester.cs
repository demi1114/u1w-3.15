using UnityEngine;
using UnityEngine.Rendering;

public class Test_CamScreenShotTester : MonoBehaviour
{
    private void Awake()
    {
        Texture2D texture = CamScreenShot.Capture(Camera.main, 1280, 720, true);

        Sprite sprite = Sprite.Create(
        texture,                  // Texture2D
        new Rect(0, 0, texture.width, texture.height),
        new Vector2(0.5f, 0.5f)   // ピボット（中央）
        );


        GetComponent<SpriteRenderer>().sprite = sprite;
    }
}
