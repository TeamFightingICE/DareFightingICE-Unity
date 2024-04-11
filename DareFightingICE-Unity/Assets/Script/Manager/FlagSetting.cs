using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
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
    public bool grpc = true;
    public bool grpcAuto = false;
    public bool grpcAutoReady = false;
    public bool socket = false;
    public int port = 50051;
    public bool loadArgs = false;
    
    public void ResetData()
    {
        slowmotion = false;
        debugActionFlag = false;
        debugFrameDataFlag = false;
        muteFlag = false;
    }

    public void SetData(Toggle slow, Toggle debug, Toggle frame, Toggle mute)
    {
        slowmotion = slow;
        debugActionFlag = debug;
        debugFrameDataFlag = frame;
        muteFlag = mute;
    }

    public void LoadArgs(string[] args) {
        if (loadArgs) return;

        for (int i = 0; i < args.Length; i++)
        {
            switch (args[i].ToLower())
            {
                case "--limithp":
                    limitHpFlag = true;
                    GameSetting.Instance.P1HP = int.Parse(args[++i]);
                    GameSetting.Instance.P2HP = int.Parse(args[++i]);
                    break;
                case "--a1":
                    GameSetting.Instance.P1AIName = args[++i];
                    break;
                case "--a2":
                    GameSetting.Instance.P2AIName = args[++i];
                    break;
                case "-r":
                    GameSetting.Instance.RoundLimit = int.Parse(args[++i]);
                    break;
                case "-f":
                    GameSetting.Instance.FrameLimit = int.Parse(args[++i]);
                    break;
                case "--port":
                    port = int.Parse(args[++i]);
                    break;
                case "--enable-auto":
                    grpcAuto = true;
                    break;
                case "--use-socket":
                    grpc = false;
                    socket = true;
                    break;
                case "--blind-player":
                    int player = int.Parse(args[++i]);
                    if (player == 2) {
                        GameSetting.Instance.IsBlind[0] = true; // player 1
                        GameSetting.Instance.IsBlind[1] = true; // player 2
                    } else {
                        GameSetting.Instance.IsBlind[player] = true;
                    }
                    break;
                case "--non-delay":
                    player = int.Parse(args[++i]);
                	if (player == 2) {
                        GameSetting.Instance.IsNonDelay[0] = true; // player 1
                        GameSetting.Instance.IsNonDelay[1] = true; // player 2
                    } else {
                        GameSetting.Instance.IsNonDelay[player] = true;
                    }
                	break;
                case "--keep-connection":
                    player = int.Parse(args[++i]);
                    if (player == 2) {
                        GameSetting.Instance.IsKeepConnection[0] = true; // player 1
                        GameSetting.Instance.IsKeepConnection[1] = true; // player 2
                    } else {
                        GameSetting.Instance.IsKeepConnection[player] = true;
                    }
                    break;
            }
        }
        loadArgs = true;
    }
}
