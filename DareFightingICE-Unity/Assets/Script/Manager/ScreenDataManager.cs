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
        Texture2D resizedScreenData = new(96, 64, TextureFormat.R8, false);
        ResizeAndConvertToGrayscale(tex, resizedScreenData);
        screenDataAsBytes = resizedScreenData.GetRawTextureData();
        compressBytes = GrpcUtil.CompressBytes(screenDataAsBytes);
    }

    void ResizeAndConvertToGrayscale(Texture2D originalTexture, Texture2D resizedTexture)
    {
        for (int y = 0; y < resizedTexture.height; y++)
        {
            for (int x = 0; x < resizedTexture.width; x++)
            {
                float origX = x * 1.0f / resizedTexture.width * originalTexture.width;
                float origY = y * 1.0f / resizedTexture.height * originalTexture.height;

                Color sampledColor = originalTexture.GetPixelBilinear(origX / originalTexture.width, origY / originalTexture.height);
                float grayValue = sampledColor.grayscale;
                
                resizedTexture.SetPixel(x, y, new Color(grayValue, grayValue, grayValue));
            }
        }

        resizedTexture.Apply();
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
