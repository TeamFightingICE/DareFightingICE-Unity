using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAIInterface
{
    void Initialize(GameData gameData, bool isPlayerOne);
    void GetInformation(FrameData frameData);
    void Processing();
    Key Input();
    void Close();
    void RoundEnd(int p1Hp, int p2Hp, int frames);
}
