using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sandbox : IAIInterface
{
    private Key input;
    public void Initialize(GameData gameData, bool isPlayerOne)
    {
        input = new Key();
    }

    public void GetInformation(FrameData frameData)
    {

    }

    public void GetAudioData(AudioData audioData)
    {

    }

    public void GetScreenData(ScreenData screenData)
    {

    }

    public void Processing()
    {
        
    }

    public Key Input()
    {
        return input;
    }

    public void RoundEnd(RoundResult result)
    {

    }

    public void Close()
    {

    }
}
