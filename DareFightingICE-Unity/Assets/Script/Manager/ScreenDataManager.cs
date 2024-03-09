using UnityEngine;
using System.IO;
using System.IO.Compression;
using UnityEngine.UI;
public class ScreenDataManager : Singleton<ScreenDataManager>
{
    private byte[] picturebyte;
    public void ProcessScreenData(RenderTexture rTex, Texture2D tex)
    {
        RenderTexture.active = rTex;
        tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
        tex.Apply();
        //Debug.Log("ScreenData Processed!");
        picturebyte = tex.GetRawTextureData();
        
    }

    public ScreenData GetScreenData()
    {
        //Debug.Log("ScreenData Taken!");
        byte[] compress = GrpcUtil.CompressBytes(picturebyte);
        ScreenData data = new ScreenData
        {
            DisplayBytes = compress
        };
        return data;
    }
}
