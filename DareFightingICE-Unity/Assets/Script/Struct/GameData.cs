using DareFightingICE.Grpc.Proto;
using System.Collections.Generic;
using UnityEngine;

public class GameData
{
    public int StageWidth { get; private set; }
    public int StageHeight { get; private set; }
    public int[] MaxHPs { get; private set; }
    public int[] MaxEnergies { get; private set; }
    public List<MotionAttribute>[] CharacterMotions { get; private set; }
    public string[] CharacterNames { get; private set; }
    public string[] AiNames { get; private set; }
    public int RepeatCount { get; private set; }

    // Constructor that allows overriding initial values
    public GameData(string[] characterNames = null, string[] aiNames = null, int? repeatCount = null, int? stageWidth = null, int? stageHeight = null)
    {
        MaxHPs = new int[2];
        MaxEnergies = new int[2];
        CharacterMotions = new List<MotionAttribute>[2];

        // Use provided values or default ones if null
        StageWidth = stageWidth ?? 960;
        StageHeight = stageHeight ?? 640;
        CharacterNames = characterNames ?? new string[] { "Zen", "Zen" }; // Default names
        AiNames = aiNames ?? new string[] { "SampleAI", "SampleAI" }; // Default AI names
        RepeatCount = repeatCount ?? 1; // Default repeat count

        InitializeGameData();
    }

    public GameData(GameData other)
    {
        StageWidth = other.StageWidth;
        StageHeight = other.StageHeight;
        MaxHPs = new int[2] { other.MaxHPs[0], other.MaxHPs[1] };
        MaxEnergies = new int[2] { other.MaxEnergies[0], other.MaxEnergies[1] };
        CharacterMotions = new List<MotionAttribute>[2];
        for (int i = 0; i < 2; i++)
        {
            List<MotionAttribute> motionDataList = new();
            foreach (var motion in other.CharacterMotions[i])
            {
                motionDataList.Add(new MotionAttribute(motion));
            }
            CharacterMotions[i] = motionDataList;
        }
        CharacterNames = new string[2] { other.CharacterNames[0], other.CharacterNames[1] };
        AiNames = new string[2] { other.AiNames[0], other.AiNames[1] };
        RepeatCount = other.RepeatCount;
    }

    private void InitializeGameData()
    {
        // Initialize HPs, Energies, and Motions
        MaxHPs[0] = GameSetting.Instance.P1HP;
        MaxHPs[1] = GameSetting.Instance.P2HP;
        MaxEnergies[0] = 300; // Default maximum energy
        MaxEnergies[1] = 300; // Default maximum energy

        // Initialize motions for each character
        for (int i = 0; i < 2; i++)
        {
            List<MotionAttribute> motionDataList = new List<MotionAttribute>();
            // Populate motionDataList with character-specific motions
            CharacterMotions[i] = motionDataList;
        }
    }

    public List<MotionAttribute> GetMotionData(bool playerNumber)
    {
        return new List<MotionAttribute>(CharacterMotions[playerNumber ? 0 : 1]);
    }

    // Additional methods (e.g., GetMaxHP, GetMaxEnergy, GetCharacterName, GetAiName) can be added as needed

    public GrpcGameData ToProto()
    {
        GrpcGameData gameData = new()
        {
            MaxHps = { MaxHPs },
            MaxEnergies = { MaxEnergies },
            CharacterNames = { CharacterNames},
            AiNames = { AiNames }
        };
        return gameData;
    }
}
