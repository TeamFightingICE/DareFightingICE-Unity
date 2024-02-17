using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MctsAi23i : IAIInterface
{
    private FrameData frameData;
    private AudioData audioData;
    private ScreenData screenData;
    private Key input;
    public void Initialize(GameData gameData, bool isPlayerOne)
    {
        input = new Key();
    }

    public void GetInformation(FrameData frameData)
    {
        this.frameData = frameData;
    }

    public void GetAudioData(AudioData audioData)
    {
        this.audioData = audioData;
    }

    public void GetScreenData(ScreenData screenData)
    {
        this.screenData = screenData;
    }

    public void Processing()
    {
        if (frameData.EmptyFlag || frameData.RemainingFrameNumber <= 0)
        {
            return;
        }

        // implement Mcts logic here
        input.A = !input.A; // perform punch
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
