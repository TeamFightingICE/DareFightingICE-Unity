using UnityEngine;
using System.IO;
using System.IO.Compression;

public class AIScreenData : MonoBehaviour
{
    public Texture2D CaptureScreen(int width, int height)
    {
        Texture2D screenCapture = new Texture2D(width, height, TextureFormat.RGB24, false);
        screenCapture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        screenCapture.Apply();

        Texture2D resizedScreenCapture = ResizeTexture(screenCapture, width, height);
        Destroy(screenCapture);

        return resizedScreenCapture;
    }

    Texture2D ResizeTexture(Texture2D source, int newWidth, int newHeight)
    {
        source.filterMode = FilterMode.Bilinear;
        RenderTexture rt = RenderTexture.GetTemporary(newWidth, newHeight);
        rt.filterMode = FilterMode.Bilinear;
        RenderTexture.active = rt;
        Graphics.Blit(source, rt);
        Texture2D nTex = new Texture2D(newWidth, newHeight);
        nTex.ReadPixels(new Rect(0, 0, newWidth, newHeight), 0, 0);
        nTex.Apply();
        RenderTexture.active = null;
        RenderTexture.ReleaseTemporary(rt);
        return nTex;
    }

    public byte[] GetScreenDataAsBytes(Texture2D texture, bool grayscale = false)
    {
        byte[] pixels;
        if (grayscale)
        {
            pixels = new byte[texture.width * texture.height];
            for (int i = 0; i < pixels.Length; i++)
            {
                Color pixelColor = texture.GetPixel(i % texture.width, i / texture.width);
                pixels[i] = (byte)(pixelColor.grayscale * 255);
            }
        }
        else
        {
            pixels = texture.EncodeToPNG();
        }

        return pixels;
    }

    public byte[] CompressBytes(byte[] data)
    {
        using (var memoryStream = new MemoryStream())
        {
            using (var gzip = new GZipStream(memoryStream, CompressionMode.Compress, true))
            {
                gzip.Write(data, 0, data.Length);
            }
            return memoryStream.ToArray();
        }
    }
}