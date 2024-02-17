using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleAI : IAIInterface
{
    private bool isPlayerOne;
    private FrameData frameData;
    private AudioData audioData;
    private ScreenData screenData;
    private Key key;
    private CommandCenter commandCenter;

    public bool IsBlind()
    {
        return false;
    }

    public void Initialize(GameData gameData, bool isPlayerOne)
    {
        this.isPlayerOne = isPlayerOne;
        this.key = new Key();
        this.commandCenter = new CommandCenter();
    }

    public void GetNonDelayFrameData(FrameData frameData)
    {
        
    }

    public void GetInformation(FrameData frameData, bool isControl)
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

        if (commandCenter.GetSkillFlag())
        {
            key = commandCenter.GetSkillKey();
        }
        else
        {
            key.Empty();
            commandCenter.SkillCancel();

            commandCenter.CommandCall("B");
        }
    }

    public Key Input()
    {
        return key;
    }

    public void RoundEnd(RoundResult result)
    {

    }

    public void Close()
    {

    }
}
