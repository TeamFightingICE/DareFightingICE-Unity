using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotionLoader : MonoBehaviour
{
    public TextAsset zenMotion;
    public TextAsset garnetMotion;
    public TextAsset ludMotion;
    // Start is called before the first frame update

    private void Start()
    {
        if (MotionManager.Instance.isLoad == false)
        {
            print("Work?");
            FlagSetting.Instance.ResetData();
            MotionManager.Instance.LoadMotion(zenMotion, garnetMotion, ludMotion);
            MotionManager.Instance.LoadMotionData();
            MotionManager.Instance.isLoad = true;
        }
    }
}
