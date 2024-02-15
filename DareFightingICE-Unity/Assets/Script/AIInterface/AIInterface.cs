using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAIInterface
{
    void Initialize(GameData gameData, bool isPlayerOne);
    void GetInformation(FrameData frameData);
    void GetAudioData(AudioData audioData);
    void GetScreenData(ScreenData screenData);
    void Processing();
    Key Input();
    void Close();
    void RoundEnd(RoundResult roundResult);
}
