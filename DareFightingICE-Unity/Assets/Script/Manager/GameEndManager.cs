using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameEndManager : MonoBehaviour
{
    [SerializeField] private TMP_Text [] R1Hps;
    [SerializeField] private GameObject [] R1Win;
      [SerializeField] private TMP_Text [] R2Hps;
    [SerializeField] private GameObject [] R2Win;
      [SerializeField] private TMP_Text [] R3Hps;
    [SerializeField] private GameObject [] R3Win;
    [SerializeField] private GameObject R1Draw;
    [SerializeField] private GameObject R2Draw;
    [SerializeField] private GameObject R3Draw;
    // Start is called before the first frame update
    void Start()
    {
    StartCoroutine(ProcessRoundEnd());
    }
IEnumerator ProcessRoundEnd() 
{
GameSetting.Instance.GameRepeatedCount++;
        GameSetting.Instance.RoundNum = 1;
        R1Hps[0].text = GameSetting.Instance.Rount1Results.RemainingHPs[0].ToString();
        R1Hps[1].text = GameSetting.Instance.Rount1Results.RemainingHPs[1].ToString();
        R2Hps[0].text = GameSetting.Instance.Rount2Results.RemainingHPs[0].ToString();
        R3Hps[1].text = GameSetting.Instance.Rount2Results.RemainingHPs[1].ToString();
        R3Hps[0].text = GameSetting.Instance.Rount3Results.RemainingHPs[0].ToString();
        R3Hps[1].text = GameSetting.Instance.Rount3Results.RemainingHPs[1].ToString();

        if(GameSetting.Instance.Rount1Results.RemainingHPs[0] > GameSetting.Instance.Rount1Results.RemainingHPs[1]) 
        {
            R1Win[0].SetActive(true);
        }
        else if (GameSetting.Instance.Rount1Results.RemainingHPs[0] == GameSetting.Instance.Rount1Results.RemainingHPs[1]) 
        {
            R1Draw.SetActive(true);
        }
        else 
        {
            R1Win[1].SetActive(true);
        }

         if(GameSetting.Instance.Rount2Results.RemainingHPs[0] > GameSetting.Instance.Rount2Results.RemainingHPs[1]) 
        {
            R2Win[0].SetActive(true);
        }
        else if (GameSetting.Instance.Rount2Results.RemainingHPs[0] == GameSetting.Instance.Rount2Results.RemainingHPs[1]) 
        {
            R2Draw.SetActive(true);
        }
        else 
        {
            R2Win[1].SetActive(true);
        }

         if(GameSetting.Instance.Rount3Results.RemainingHPs[0] > GameSetting.Instance.Rount3Results.RemainingHPs[1]) 
        {
            R3Win[0].SetActive(true);
        }
        else if (GameSetting.Instance.Rount3Results.RemainingHPs[0] == GameSetting.Instance.Rount3Results.RemainingHPs[1]) 
        {
            R3Draw.SetActive(true);
        }
        else 
        {
            R3Win[1].SetActive(true);
        }
        

        yield return new WaitForSeconds(4f);


        if(GameSetting.Instance.GameRepeatedCount >= GameSetting.Instance.GameRepeatCount) 
        {
            SceneManager.LoadScene("Launch");
        }
        else 
        {
            SceneManager.LoadScene("Gameplay");
        }
}
 
}
