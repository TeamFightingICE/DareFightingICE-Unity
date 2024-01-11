using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagSetting : Singleton<FlagSetting>
{
    /**
     * Enable AI control inside game folder?
     */
    public static bool automationFlag = false;

    /**
     * Enable All Flag include AI
     */
    public static bool allCombinationFlag = false;

    /**
     * Open BG
     */
    public static bool enableBackground = true;

    /**
     * Python AI
     */
    public static bool py4j = false;

    /**
     * Count perform action
     */
    public static bool debugActionFlag = false;

    /**
     * Enable Framedata
     */
    public static bool debugFrameDataFlag = false;

    /**
     * Trainning Mode．
     */
    public static bool trainingModeFlag = false;

    /**
     * P1,P2 Setplayer HP．
     */
    public static bool limitHpFlag = false;

    /**
     * Mute Mode．
     */
    public static bool muteFlag = false;

    /**
     * Write Json Infomation : log
     */
    public static bool jsonFlag = false;

    /**
     * Write outputAndLog．
     */
    public static bool outputErrorAndLogFlag = false;

    /**
     * FastMod
     */
    public static bool fastModeFlag = false;

    /**
     * Open Game window Not important?．
     */
    public static bool enableWindow = true;

    /**
     * Slow motion mode
     */
    public static bool slowmotion = false;
    
    /**
     *  Enable gRPC
     */
    public static bool grpc = false;
    public static bool grpcAuto = false;
}
