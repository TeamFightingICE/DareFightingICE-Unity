using System.IO;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;

public class ReplaySystemController : MonoBehaviour
{
    public ReplayData replayData = new ReplayData();


    public void ReadData() 
    {
        replayData.Player1Data.Add(FrameDataManager.Instance.characterData[0]);
        replayData.Player2Data.Add(FrameDataManager.Instance.characterData[1]);
    }
    public void Save() 
    {
        string FilePath = Application.persistentDataPath +"/Replay1.dat";
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream fileStream = File.Create(FilePath);
        formatter.Serialize(fileStream, replayData);
        fileStream.Close();   
    }
    public ReplayData Load() 
    {
        string FilePath = Application.persistentDataPath +"/Replay1.dat";
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream fileStream = File.Open(FilePath, FileMode.Open);
        ReplayData playerData = (ReplayData)formatter.Deserialize(fileStream);
        fileStream.Close();
        return playerData;
    }



}
