using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAIInterface
{
    public bool IsBlind();
    void Initialize(GameData gameData, bool isPlayerOne);
    void GetNonDelayFrameData(FrameData frameData);
    void GetInformation(FrameData frameData);
    void GetAudioData(AudioData audioData);
    void GetScreenData(ScreenData screenData);
    void Processing();
    Key Input();
    void Close();
    void RoundEnd(RoundResult roundResult);
}
