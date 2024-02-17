using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameEndController : MonoBehaviour
{
    [SerializeField] private TMP_Text[] R1Hps;
    [SerializeField] private GameObject[] R1Win;
    [SerializeField] private TMP_Text[] R2Hps;
    [SerializeField] private GameObject[] R2Win;
    [SerializeField] private TMP_Text[] R3Hps;
    [SerializeField] private GameObject[] R3Win;
    [SerializeField] private GameObject R1Draw;
    [SerializeField] private GameObject R2Draw;
    [SerializeField] private GameObject R3Draw;
    private List<GameObject> _winObjects = new List<GameObject>();
    private List<GameObject> _drawObjects = new List<GameObject>();
    private List<TMP_Text> _hpTexts = new List<TMP_Text>();
    // Start is called before the first frame update
    void Start()
    {
        _winObjects.AddRange(R1Win);
        _winObjects.AddRange(R2Win);
        _winObjects.AddRange(R3Win);
        _drawObjects.Add(R1Draw);
        _drawObjects.Add(R2Draw);
        _drawObjects.Add(R3Draw);
        _hpTexts.AddRange(R1Hps);
        _hpTexts.AddRange(R2Hps);
        _hpTexts.AddRange(R3Hps);
        StartCoroutine(ProcessingGameEnd());
    }
    IEnumerator ProcessingGameEnd() 
    {
        for (int i = 0; i < DataManager.Instance.RoundResults.Count; i++) 
        {
            _hpTexts[i * 2].text = DataManager.Instance.RoundResults[i].RemainingHPs[0].ToString();
            _hpTexts[i * 2 + 1].text = DataManager.Instance.RoundResults[i].RemainingHPs[1].ToString();
            if (DataManager.Instance.RoundResults[i].RemainingHPs[0] > DataManager.Instance.RoundResults[i].RemainingHPs[1]) 
            {
                _winObjects[i * 2].SetActive(true);
            }
            else if (DataManager.Instance.RoundResults[i].RemainingHPs[0] == DataManager.Instance.RoundResults[i].RemainingHPs[1]) 
            {
                _drawObjects[i].SetActive(true);
            }
            else 
            {
                _winObjects[i * 2 + 1].SetActive(true);
            }
        }
        
        yield return new WaitForSeconds(4f);

        DataManager.Instance.CurrentRound = 1;
        DataManager.Instance.RoundResults.Clear();
        Debug.Log("CurrentGame: " + DataManager.Instance.CurrentGame + " / " + GameSetting.Instance.GameRepeatCount);
        if (DataManager.Instance.CurrentGame >= GameSetting.Instance.GameRepeatCount) 
        {
            DataManager.Instance.CurrentGame = 1;
            SceneManager.LoadScene("Launch");
        }
        else 
        {
            DataManager.Instance.CurrentGame++;
            SceneManager.LoadScene("StartingGameplay");
        }
    }
}
