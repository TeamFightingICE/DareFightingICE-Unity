using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlagSetting : Singleton<FlagSetting>
{
    /**
     * Enable AI control inside game folder?
     */
    public bool automationFlag = false;

    /**
     * Enable All Flag include AI
     */
    public bool allCombinationFlag = false;

    /**
     * Open BG
     */
    public bool enableBackground = true;

    /**
     * Python AI
     */
    public bool py4j = false;

    /**
     * Count perform action
     */
    public bool debugActionFlag = false;

    /**
     * Enable Framedata
     */
    public bool debugFrameDataFlag = false;

    /**
     * Trainning Mode．
     */
    public bool trainingModeFlag = false;

    /**
     * P1,P2 Setplayer HP．
     */
    public bool limitHpFlag = false;

    /**
     * Mute Mode．
     */
    public bool muteFlag = false;

    /**
     * Write Json Infomation : log
     */
    public bool jsonFlag = false;

    /**
     * Write outputAndLog．
     */
    public bool outputErrorAndLogFlag = false;

    /**
     * FastMod
     */
    public bool fastModeFlag = false;

    /**
     * Open Game window Not important?．
     */
    public bool enableWindow = true;

    /**
     * Slow motion mode
     */
    public bool slowmotion = false;
    
    /**
     *  Enable gRPC
     */
    public bool grpc = false;
    public bool grpcAuto = false;
    
    public void ResetData()
    {
     slowmotion = false;
     debugActionFlag = false;
     debugFrameDataFlag = false;
     muteFlag = false;
    }

    public void SetData(Toggle slow, Toggle debug, Toggle frame,Toggle mute)
    {
     slowmotion = slow;
     debugActionFlag = debug;
     debugFrameDataFlag = frame;
     muteFlag = mute;
    }
}
