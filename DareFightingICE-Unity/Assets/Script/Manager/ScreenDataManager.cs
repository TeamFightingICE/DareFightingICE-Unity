using UnityEngine;
using System.IO;
using System.IO.Compression;
using UnityEngine.UI;
public class ScreenDataManager : Singleton<ScreenDataManager>
{
    private byte[] screenDataAsBytes;
    private byte[] compressBytes;
    public void ProcessScreenData(RenderTexture rTex, Texture2D tex)
    {
        RenderTexture.active = rTex;
        tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
        tex.Apply();
        //var correctedTexture = ApplyGammaCorrection(tex);
        screenDataAsBytes = tex.GetRawTextureData();
        compressBytes = GrpcUtil.CompressBytes(screenDataAsBytes);
    }

    private Texture2D ApplyGammaCorrection(Texture2D source, float gamma = 2.2f)
    {
        Texture2D correctedTexture = new Texture2D(source.width, source.height, source.format, false);
        Color[] pixels = source.GetPixels();

        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i].r = Mathf.Pow(pixels[i].r, 1 / gamma);
            pixels[i].g = Mathf.Pow(pixels[i].g, 1 / gamma);
            pixels[i].b = Mathf.Pow(pixels[i].b, 1 / gamma);
        }

        correctedTexture.SetPixels(pixels);
        correctedTexture.Apply();

        return correctedTexture;
    }

    public ScreenData GetScreenData()
    {
        ScreenData data = new()
        {
            DisplayBytes = compressBytes
        };
        return data;
    }
}
