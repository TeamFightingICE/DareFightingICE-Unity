using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FightingController : MonoBehaviour
{
    /// <summary>
    /// Control everything in FightingScene
    /// include timer ending and flip character
    /// </summary>
    [SerializeField] private GameObject zen;
    [SerializeField] private GameObject spawnP1;
    [SerializeField] private GameObject spawnP2;
    [SerializeField] private GameObject LeftBorder;
    [SerializeField] private GameObject RightBorder;
    [SerializeField] private float flipThreshold = 1.0f;
    private bool isStart = false;
    private int currentRound;
    public List<GameObject> character;
    private AIController[] _aiControllers = new AIController[2];
    private ZenCharacterController[] _controllers = new ZenCharacterController[2];
    private int currentFrameNumber;
    public InterfaceDisplay _display;
    [SerializeField] private AudioSource P1HeartBeat;
    [SerializeField] private AudioSource P2HeartBeat;
    [SerializeField] private AudioSource P1EnergyIncrease;
    [SerializeField] private AudioSource P2EnergyIncrease;
    [SerializeField] private int P1EnergyLevel;
    [SerializeField] private int P2EnergyLevel;
    
    void Start()
    {
        SetupScene();

        if (DataManager.Instance.CurrentRound == 1) {
            _aiControllers[0].Initialize(GameDataManager.Instance.GameData, true);
            _aiControllers[1].Initialize(GameDataManager.Instance.GameData, false);
        }
        _aiControllers[0].InitRound();
        _aiControllers[1].InitRound();

        Thread.Sleep(1000); // wait for 1 second to let the AI initialize
    }

    private void SetupScene()
    {
        ClearList();
        currentFrameNumber = 0;
        currentRound = DataManager.Instance.CurrentRound;
        GameObject zen1 = Instantiate(zen, spawnP1.transform.position, spawnP1.transform.rotation);
        zen1.tag = "Player1";

        GameObject zen2 = Instantiate(zen, spawnP2.transform.position, spawnP2.transform.rotation);
        zen2.tag = "Player2";
        zen2.transform.localScale = new Vector3(-3.5f, 3.5f,3.5f);

        _aiControllers[0] = zen1.GetComponent<AIController>();
        _aiControllers[1] = zen2.GetComponent<AIController>();
        _controllers[0] = zen1.GetComponent<ZenCharacterController>();
        _controllers[1] = zen2.GetComponent<ZenCharacterController>();

        _controllers[0].PlayerNumber = true;
        _controllers[0].IsFront = true;
        _controllers[0].Hp = GameSetting.Instance.P1HP;
        _controllers[0].Energy = 0;
        _controllers[0].otherPlayer = _controllers[1];
        _controllers[0].SetTarget("Player2");

        _controllers[1].PlayerNumber = false;
        _controllers[1].IsFront = false;
        _controllers[1].Hp = GameSetting.Instance.P2HP;
        _controllers[1].Energy = 0;
        _controllers[1].otherPlayer = _controllers[0];
        _controllers[1].SetTarget("Player1");

        character.Add(zen1);
        character.Add(zen2);

        P1EnergyLevel = 0;
        P2EnergyLevel = 0;

        _display.SetPlayerController(_controllers[0], _controllers[1]);
        _display.currentRound = currentRound;

        FrameDataManager.Instance.SetupFrameData(character[0], character[1], _display);
        AudioDataManager.Instance.Initialize();

        isStart = true;
    }

    public void ResetRound()
    {
        _controllers[0].IsFront = true;
        _controllers[0].Hp = GameSetting.Instance.P1HP;
        _controllers[0].Energy = 0;
        character[0].transform.position = spawnP1.transform.position;
        character[0].transform.rotation = spawnP1.transform.rotation;
        
        _controllers[1].IsFront = true;
        _controllers[1].Hp = GameSetting.Instance.P2HP;
        _controllers[1].Energy = 0;
        character[1].transform.position = spawnP2.transform.position;
        character[1].transform.rotation = spawnP2.transform.rotation;

        Vector3 scaleP1 = character[0].transform.localScale;
        scaleP1.x = Mathf.Abs(scaleP1.x) * (_controllers[0].IsFront ? 1 : -1);
        character[0].transform.localScale = scaleP1;

        Vector3 scaleP2 = character[1].transform.localScale;
        scaleP2.x = Mathf.Abs(scaleP2.x) * (_controllers[1].IsFront ? 1 : -1);
        character[1].transform.localScale = scaleP2;

        currentFrameNumber = 0;
        currentRound++;
        _display.currentRound = currentRound;
        isStart = true;
    }
    // Update is called once per frame
    void Update()
    {
        // Special Sound effects Part
        // heartbeat for player1
        if (_controllers[0].Hp < 50)
        {
            P1HeartBeat.Play();
        }
        // heartbeat for player2
        if (_controllers[1].Hp < 50)
        {
            P2HeartBeat.Play();
        }
        //Energy Increase for PLayer1
        if(_controllers[0].Energy >= 50)
        {
            if (_controllers[0].Energy >= P1EnergyLevel + 50)
            {
                P1EnergyLevel += 50;
                P1EnergyIncrease.Play();
            }
        }
        //Energy Increase for PLayer2
        if (_controllers[1].Energy >= 50)
        {
            if (_controllers[1].Energy >= P2EnergyLevel + 50)
            {
                P2EnergyLevel += 50;
                P2EnergyIncrease.Play();
            }
        }

        if (currentFrameNumber >= GameSetting.Instance.FrameLimit || CheckField())
        {
            OnRoundEnd();
        }
        else if (isStart)
        {
            _display.currentFrame = currentFrameNumber++;
            FrameDataManager.Instance.ProcessFrameData();
            AudioDataManager.Instance.ProcessAudioData();
            HandlePositionOverlap();
        }
    }

    private void ClearList()
    {
        foreach (GameObject obj in character)
        {
            Destroy(obj);
        }
        character.Clear();
    }
    void OnRoundEnd()
    {
        Thread.Sleep(100);

        RoundResult result = new RoundResult
        {
            CurrentRound = currentRound,
            ElaspedFrame = currentFrameNumber,
            RemainingHPs = new int[]{
                Math.Max(0, _controllers[0].Hp),
                Math.Max(0, _controllers[1].Hp)
            }
        };

        isStart = false;
        _aiControllers[0].RoundEnd(result);
        _aiControllers[1].RoundEnd(result);

        DataManager.Instance.RoundResults.Add(result);
       
        if (currentRound < GameSetting.Instance.RoundLimit)
        {
            //ResetRound();
            DataManager.Instance.CurrentRound += 1;
            SceneManager.LoadScene("StartingGamePlay");
        }
        else
        {
            OnGameEnd();
        }
    }

    void OnGameEnd() {
        SceneManager.LoadScene("GameEnd");
    }

    bool CheckField()
    {
        if (_controllers[0].Hp <= 0 || _controllers[1].Hp <= 0)
        {
            return true;
        }
        else
        {
            return false;
        }
        
    }
    private void HandlePositionOverlap()
    {
        if (character.Count < 2) return; // Ensure there are two characters

        // Get positions of the players
        float player1Position = character[0].transform.position.x;
        float player2Position = character[1].transform.position.x;

        // Calculate the distance between the players
        float distanceBetweenPlayers = Mathf.Abs(player1Position - player2Position);

        // Only flip characters if they are beyond the threshold distance
        if (distanceBetweenPlayers > flipThreshold)
        {
            // Determine if players need to flip based on their relative positions
            if (player1Position < player2Position && !_controllers[0].IsFront)
            {
                FlipCharacter(character[0]);
            }
            else if (player1Position > player2Position && _controllers[0].IsFront)
            {
                FlipCharacter(character[0]);
            }

            if (player2Position < player1Position && !_controllers[1].IsFront)
            {
                FlipCharacter(character[1]);
            }
            else if (player2Position > player1Position && _controllers[1].IsFront)
            {
                FlipCharacter(character[1]);
            }
        }
    }

    private void FlipCharacter(GameObject character)
    {
        ZenCharacterController charController = character.GetComponent<ZenCharacterController>();
        charController.IsFront = !charController.IsFront;
        Vector3 scale = character.transform.localScale;
        scale.x *= -1;
        character.transform.localScale = scale;
    }
    
    // private void FlipCharacter(GameObject character)
    // {
    //     SpriteRenderer spriteRenderer = character.GetComponent<SpriteRenderer>();
    //     if (spriteRenderer != null)
    //     {
    //         spriteRenderer.flipX = !spriteRenderer.flipX;
    //
    //         // Optionally, adjust facing direction property if you have one
    //         ZenCharacterController charController = character.GetComponent<ZenCharacterController>();
    //         if (charController != null)
    //         {
    //             charController.IsFront = !charController.IsFront;
    //         }
    //     }
    //     else
    //     {
    //         //Debug.LogError("SpriteRenderer not found on " + character.name);
    //     }
    // }
}
