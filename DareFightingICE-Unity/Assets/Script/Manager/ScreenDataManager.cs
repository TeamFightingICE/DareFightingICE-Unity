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
        screenDataAsBytes = tex.GetRawTextureData();
        //compressBytes = GrpcUtil.CompressBytes(screenDataAsBytes);
    }

    public ScreenData GetScreenData()
    {
        ScreenData data = new()
        {
            DisplayBytes = screenDataAsBytes
        };
        return data;
    }
}
