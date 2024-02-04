using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDataManager : Singleton<GameDataManager>
{
    public GameData GameData;

    public void SetGameData(GameData gameData)
    {
        GameData = gameData;
    }
}
