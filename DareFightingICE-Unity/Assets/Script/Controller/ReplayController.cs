using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class ReplayController : MonoBehaviour
{
    [SerializeField] private GameObject zen;
    [SerializeField] private GameObject spawnP1;
    [SerializeField] private GameObject spawnP2;
    [SerializeField] private GameObject LeftBorder;
    [SerializeField] private GameObject RightBorder;
    [SerializeField] private float flipThreshold = 1.0f;
    [SerializeField] private AudioSource BackgroundMusic;
    [SerializeField] private AudioSource P1HeartBeat;
    [SerializeField] private AudioSource P2HeartBeat;
    [SerializeField] private AudioSource P1EnergyIncrease;
    [SerializeField] private AudioSource P2EnergyIncrease;
    [SerializeField] private int P1EnergyLevel;
    [SerializeField] private int P2EnergyLevel;
    private readonly ReplayCharacterController[] _controllers = new ReplayCharacterController[2];

    public List<GameObject> character;

    private int currentFrameNumber;
    private int currentRound;

    public ReplayCharacterController player1; // Reference to player 1's controller
    public ReplayCharacterController player2; // Reference to player 2's controller
    public int currentFrame;
    public int remainingSecond;
    [SerializeField] private ReplayData replayData;

    [SerializeField] private Image hp1;
    [SerializeField] private Image hp2;
    [SerializeField] private Image energy1;
    [SerializeField] private Image energy2;
    [SerializeField] private TMP_Text fpsText;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text roundText;
    [SerializeField] private TMP_Text p1Status;
    [SerializeField] private TMP_Text p2Status;
    
    float deltaTime = 0.0f;
    private readonly bool[] heartBeatFlag = { false, false };

    private void Awake()
    {
        Application.targetFrameRate = 60;
        replayData = Load();
        SetupScene();
    }

    void Update()
    {
        UpdateCharacterData(true, replayData.Player1Data[currentFrameNumber]);
        UpdateCharacterData(false, replayData.Player2Data[currentFrameNumber]);
        currentFrameNumber++;
        float msec = deltaTime * 1000.0f;
        float fps = 1.0f / deltaTime;
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        fpsText.text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
        roundText.text = $"Round: {currentRound}";
        hp1.fillAmount = (float)player1.Hp / GameSetting.Instance.P1HP;
        hp2.fillAmount = (float)player2.Hp / GameSetting.Instance.P2HP;
        energy1.fillAmount = (float)player1.Energy / 300;
        energy2.fillAmount = (float)player2.Energy / 300;
        // timerText.text = "Frame Limit: " + currentFrame;
        timerText.text = string.Format("{0:0.000}", Math.Round(GetRemainingFrame() / 60.0, 3));
        p1Status.text = $"P1 HP: {player1.Hp} Energy: {player1.Energy}";
        p2Status.text = $"P2 HP: {player2.Hp} Energy: {player2.Energy}";
        SetEnergyColor();

        if (_controllers[0].Hp < 50 && !heartBeatFlag[0])
        {
            heartBeatFlag[0] = true;
            P1HeartBeat.Play();
        }
        // heartbeat for player2
        if (_controllers[1].Hp < 50 && !heartBeatFlag[1])
        {
            heartBeatFlag[1] = true;
            P2HeartBeat.Play();
        }
        //Energy Increase for player1
        if(_controllers[0].Energy >= 50)
        {
            if (_controllers[0].Energy >= P1EnergyLevel + 50)
            {
                P1EnergyLevel += 50;
                P1EnergyIncrease.Play();
            }
        }
        //Energy Increase for player2
        if (_controllers[1].Energy >= 50)
        {
            if (_controllers[1].Energy >= P2EnergyLevel + 50)
            {
                P2EnergyLevel += 50;
                P2EnergyIncrease.Play();
            }
        }
    }

    private void SetupScene()
    {
        currentFrameNumber = 0;
        currentRound = DataManager.Instance.CurrentRound;
        GameObject zen1 = Instantiate(zen, spawnP1.transform.position, spawnP1.transform.rotation);
        zen1.tag = "Player1";

        GameObject zen2 = Instantiate(zen, spawnP2.transform.position, spawnP2.transform.rotation);
        zen2.tag = "Player2";
        zen2.transform.localScale = new Vector3(-3.5f, 3.5f,3.5f);

        _controllers[0] = zen1.GetComponent<ReplayCharacterController>();
        _controllers[1] = zen2.GetComponent<ReplayCharacterController>();

        _controllers[0].PlayerNumber = true;
        _controllers[0].IsFront = true;
        _controllers[0].Hp = GameSetting.Instance.P1HP;
        _controllers[0].Energy = 0;

        _controllers[1].PlayerNumber = false;
        _controllers[1].IsFront = false;
        _controllers[1].Hp = GameSetting.Instance.P2HP;
        _controllers[1].Energy = 0;

        character.Add(zen1);
        character.Add(zen2);  

        P1EnergyLevel = 0;
        P2EnergyLevel = 0;
        heartBeatFlag[0] = false;
        heartBeatFlag[1] = false;
        SetPlayerController(_controllers[0], _controllers[1]);
    }

    private void SetEnergyColor()
    {
        if (player1.Energy == 300)
        {
            energy1.color = Color.blue;
        }
        else if (player1.Energy > 150)
        {
            energy1.color = Color.yellow;
        }
        else
        {
            energy1.color = Color.red;
        }
        
        if (player2.Energy == 300)
        {
            energy2.color = Color.blue;
        }
        else if (player2.Energy > 150)
        {
            energy2.color = Color.yellow;
        }
        else
        {
            energy2.color = Color.red;
        }
    }

    private int GetRemainingFrame()
    {
        return GameSetting.Instance.FrameLimit - currentFrame;
    }

    private void UpdateCharacterData(bool playerNumber, CharacterData data) 
    {
        int idx = playerNumber ? 0 : 1;
        var controller = _controllers[idx];

        controller.PlayerNumber = data.PlayerNumber;
        controller.Hp = data.Hp;
        controller.Energy = data.Energy;
        character[idx].transform.position = new Vector3(data.X,  data.Y, character[idx].transform.position.z);
        character[idx].GetComponent<Rigidbody2D>().velocity = new Vector2 (data.SpeedX,data.SpeedY);
        controller.Action = data.Action;

        Debug.Log(data.Action.ToString());

        if (!data.Front && controller.IsFront) 
        {
            FlipCharacter(controller.gameObject);
        }
        else if(data.Front && !controller.IsFront) 
        {
            FlipCharacter(controller.gameObject);
        }

        controller.IsFront = data.Front;
    }

    public ReplayData Load() 
    {
        string FilePath = Application.persistentDataPath + "/Replay1.dat";
        BinaryFormatter formatter = new();
        FileStream fileStream = File.Open(FilePath, FileMode.Open);
        ReplayData playerData = (ReplayData)formatter.Deserialize(fileStream);
        fileStream.Close();
        return playerData;
    }

    private void FlipCharacter(GameObject character)
    {
        ReplayCharacterController charController = character.GetComponent<ReplayCharacterController>();
        charController.IsFront = !charController.IsFront;
        Vector3 scale = character.transform.localScale;
        scale.x *= -1;
        character.transform.localScale = scale;
    }

    public void SetPlayerController(ReplayCharacterController p1, ReplayCharacterController p2)
    {
        player1 = p1;
        player2 = p2;
    }

}
