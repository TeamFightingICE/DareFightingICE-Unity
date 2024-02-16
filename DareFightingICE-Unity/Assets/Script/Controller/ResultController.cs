using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResultController : MonoBehaviour
{
    private int displayTime;
    // Start is called before the first frame update
    void Start()
    {
        this.displayTime = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (++this.displayTime > 300)
        {
            if (GameSetting.Instance.IsRunWithGrpcAuto)
            {
                SceneManager.LoadScene("GrpcAuto");
            }
            else
            {
                SceneManager.LoadScene("Start");
            }
        }
    }
}
