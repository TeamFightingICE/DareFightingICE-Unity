using System.IO;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System;


public class ReplaySystemController : MonoBehaviour
{
    public ReplayData replayData = new();
    

    public void ReadData() 
    {
        replayData.Player1Data.Add(FrameDataManager.Instance.characterData[0]);
        replayData.Player2Data.Add(FrameDataManager.Instance.characterData[1]);
    }

    public void Save() 
    {
        try
        {
            var path = Application.persistentDataPath;
            var folder = "log/replay";
            path = Path.Combine(path, folder);
            if(! Directory.Exists(path)) Directory.CreateDirectory(path);
            string FilePath = Path.Combine(path,  DateTime.Now.ToString("hh-mm-ss") + "_Replay.dat");
            BinaryFormatter formatter = new();
            using FileStream fileStream = File.Create(FilePath);
            formatter.Serialize(fileStream, replayData);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

}
