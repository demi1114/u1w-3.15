using UnityEngine;

public static class CamScreenShot
{
    public static Texture2D Capture(Camera cam, int width, int height)
    {
        var targettex = cam.targetTexture;

        RenderTexture rt = new RenderTexture(width, height, 24);
        cam.targetTexture = rt;

        Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);

        cam.Render();

        var prev = RenderTexture.active;

        RenderTexture.active = rt;
        tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        tex.Apply();

        cam.targetTexture = targettex;
        RenderTexture.active = prev;

        Object.Destroy(rt);

        return tex;
    }
}
