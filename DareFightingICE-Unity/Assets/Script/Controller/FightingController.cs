using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    [SerializeField] private AudioSource BackgroundMusic;
    [SerializeField] private AudioSource P1HeartBeat;
    [SerializeField] private AudioSource P2HeartBeat;
    [SerializeField] private AudioSource P1EnergyIncrease;
    [SerializeField] private AudioSource P2EnergyIncrease;
    [SerializeField] private int P1EnergyLevel;
    [SerializeField] private int P2EnergyLevel;
    [SerializeField] private Image endScreen;
    [SerializeField] private RenderTexture ScreenDataRT;

    public List<GameObject> character;
    public List<GameObject> attackDeque;
    private readonly AIController[] _aiControllers = new AIController[2];
    private readonly ZenCharacterController[] _controllers = new ZenCharacterController[2];
    public InterfaceDisplay _display;
    private bool isStart = false;
    private bool isFading = false;
    private bool isEnd = false;
    private int currentFrameNumber;
    private int currentRound;
    private InputManager inputManager;
    private Texture2D ScreenDataTexture;
    private readonly bool[] heartBeatFlag = { false, false };

    void Start()
    {
        inputManager = InputManager.Instance;
        SetupScene();

        if (DataManager.Instance.CurrentRound == 1) {
            _aiControllers[0].Initialize(GameDataManager.Instance.GameData, true);
            _aiControllers[1].Initialize(GameDataManager.Instance.GameData, false);
        }
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
        _controllers[0].SetTarget("Player2",this);
        _controllers[0].fightingController = this;


        _controllers[1].PlayerNumber = false;
        _controllers[1].IsFront = false;
        _controllers[1].Hp = GameSetting.Instance.P2HP;
        _controllers[1].Energy = 0;
        _controllers[1].otherPlayer = _controllers[0];
        _controllers[1].SetTarget("Player1",this);
        _controllers[1].fightingController = this;

        character.Add(zen1);
        character.Add(zen2);  

        P1EnergyLevel = 0;
        P2EnergyLevel = 0;

        _display.SetPlayerController(_controllers[0], _controllers[1]);
        _display.currentRound = currentRound;

        FrameDataManager.Instance.SetupFrameData(character[0], character[1], _display);
        AudioDataManager.Instance.Initialize();
        ScreenDataTexture = new Texture2D(ScreenDataRT.width, ScreenDataRT.height, TextureFormat.RGB24, false);

        isStart = true;
    }

     
    public void ResetRound()
    {
        _controllers[0].IsFront = true;
        _controllers[0].Hp = GameSetting.Instance.P1HP;
        _controllers[0].Energy = 0;
        character[0].transform.SetPositionAndRotation(spawnP1.transform.position, spawnP1.transform.rotation);
        heartBeatFlag[0] = false;
        
        _controllers[1].IsFront = false;
        _controllers[1].Hp = GameSetting.Instance.P2HP;
        _controllers[1].Energy = 0;
        character[1].transform.SetPositionAndRotation(spawnP2.transform.position, spawnP2.transform.rotation);
        heartBeatFlag[1] = false;

        Vector3 scaleP1 = character[0].transform.localScale;
        scaleP1.x = Mathf.Abs(scaleP1.x) * (_controllers[0].IsFront ? 1 : -1);
        character[0].transform.localScale = scaleP1;

        Vector3 scaleP2 = character[1].transform.localScale;
        scaleP2.x = Mathf.Abs(scaleP2.x) * (_controllers[1].IsFront ? 1 : -1);
        character[1].transform.localScale = scaleP2;

        currentFrameNumber = 0;
        currentRound++;
        _display.currentRound = currentRound;
        _aiControllers[0].isRoundEnd = false;
        _aiControllers[1].isRoundEnd = false;
        //SimFighitng.ResetRound();
    }
    // Update is called once per frame
    void Update()
    {
        if (isFading) return;
        // Special Sound effects Part
        // heartbeat for player1
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

        if (!isEnd && !isFading && (currentFrameNumber >= GameSetting.Instance.FrameLimit || CheckField()))
        {
            OnRoundEnd();
        }
        else if (isStart)
        {
            _display.currentFrame = currentFrameNumber++;
            FrameDataManager.Instance.ProcessFrameData();
            AudioDataManager.Instance.ProcessAudioData();
            ScreenDataManager.Instance.ProcessScreenData(ScreenDataRT, ScreenDataTexture);
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
        if (isEnd) return;

        isEnd = true;
        _aiControllers[0].isRoundEnd = true;
        _aiControllers[1].isRoundEnd = true;

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
        inputManager.ClearInput();

        _aiControllers[0].RoundEnd(result);
        _aiControllers[1].RoundEnd(result);

        DataManager.Instance.RoundResults.Add(result);
       
        if (currentRound < GameSetting.Instance.RoundLimit)
        {
            StartCoroutine(GameFadeClose());
            DataManager.Instance.CurrentRound++;
            isEnd = false;
        }
        else
        {
            OnGameEnd();
        }
    }

    void OnGameEnd() {
        if (DataManager.Instance.CurrentGame >= GameSetting.Instance.GameRepeatCount)
        {
            _aiControllers[0].Close();
            _aiControllers[1].Close();
        }

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

    void StopAudioSources() {
        BackgroundMusic.Stop();
        P1HeartBeat.Stop();
        P2HeartBeat.Stop();
    }

    IEnumerator GameFadeClose()
    {
        if (isFading)
        {
            yield break;
        }

        isFading = true;
        AudioListener.pause = true;
        StopAudioSources();
        float elapsed = 0f;

        Color startColor = endScreen.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 1f);

        while (elapsed < 0.1f)
        {
            elapsed += Time.deltaTime;
            float normalizedTime = Mathf.Clamp01(elapsed / 0.1f);
            endScreen.color = Color.Lerp(startColor, endColor, normalizedTime);
            yield return null;
        }
        endScreen.color = endColor;
        yield return new WaitForSeconds(1);
        ResetRound();
        endScreen.color = new Color(startColor.r, startColor.g, startColor.b, 0f);
        isStart = true;
        isFading = false;
        AudioListener.pause = false;
        BackgroundMusic.Play();
    }

    public void AddAttackDeque(GameObject gameobject)
    {
        attackDeque.Add(gameobject);
    }
}
