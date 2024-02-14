using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

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
    public List<GameObject> character;
    private AIController[] _aiControllers = new AIController[2];
    private ZenCharacterController[] _controllers = new ZenCharacterController[2];
    

    private float framelimit;

    public InterfaceDisplay _display;

    [SerializeField] private AudioSource P1HeartBeat;
    [SerializeField] private AudioSource P2HeartBeat;
    [SerializeField] private AudioSource P1EnegryIncrease;
    [SerializeField] private AudioSource P2EnegryIncrease;
    [SerializeField] private int P1EnergyLevel;
    [SerializeField] private int P2EnergyLevel ;
    
    void Start()
    {
        SetupScene();
        _aiControllers[0].Initialize(GameDataManager.Instance.GameData,true);
        _aiControllers[1].Initialize(GameDataManager.Instance.GameData,false);

    }

    private void SetupScene()
    {
        ClearList();
        framelimit = GameSetting.Instance.frameLimit;
        GameObject zen1 = Instantiate(zen, spawnP1.transform.position, spawnP1.transform.rotation);
        zen1.GetComponent<ZenCharacterController>().PlayerNumber = true;
        zen1.GetComponent<ZenCharacterController>().IsFront = true;
        zen1.GetComponent<ZenCharacterController>().Hp = GameSetting.Instance.p1Hp;
        zen1.GetComponent<ZenCharacterController>().Energy = 0;
        GameObject zen2 = Instantiate(zen, spawnP2.transform.position, spawnP2.transform.rotation);
        zen2.GetComponent<ZenCharacterController>().PlayerNumber = false;
        zen2.GetComponent<ZenCharacterController>().IsFront = false;
        zen2.GetComponent<ZenCharacterController>().Hp = GameSetting.Instance.p2Hp;
        zen2.GetComponent<ZenCharacterController>().Energy = 0;
        Vector3 scale = zen2.transform.localScale;
        scale.x *= -1;
        zen2.transform.localScale = scale;
        zen1.GetComponent<ZenCharacterController>().otherPlayer = zen2.GetComponent<ZenCharacterController>();
        zen2.GetComponent<ZenCharacterController>().otherPlayer = zen1.GetComponent<ZenCharacterController>();
        zen1.tag = "Player1";
        zen2.tag = "Player2";
        zen1.GetComponent<ZenCharacterController>().SetTarget("Player2");
        zen2.GetComponent<ZenCharacterController>().SetTarget("Player1");
        _aiControllers[0] = zen1.GetComponent<AIController>();
        _aiControllers[1] = zen2.GetComponent<AIController>();
        _controllers[0] = zen1.GetComponent<ZenCharacterController>();
        _controllers[1] = zen2.GetComponent<ZenCharacterController>();
        character.Add(zen1);
        character.Add(zen2);
        isStart = true;
        P1EnergyLevel = 0;
        P2EnergyLevel = 0;
        _display.SetPlayerController(_controllers[0],_controllers[1]);
        FrameDataManager.Instance.SetupFrameData(character[0],character[1],_display);
    }

    public void ResetRound()
    {
        character[0].transform.position = spawnP1.transform.position;
        character[0].transform.rotation = spawnP1.transform.rotation;
        _controllers[0].IsFront = true;
        _controllers[0].Hp = GameSetting.Instance.p1Hp;
        _controllers[0].Energy = 0;
        
        character[1].transform.position = spawnP2.transform.position;
        character[1].transform.rotation = spawnP2.transform.rotation;
        _controllers[1].IsFront = true;
        _controllers[1].Hp = GameSetting.Instance.p2Hp;
        _controllers[1].Energy = 0;
        Vector3 scale = character[1].transform.localScale;
        scale.x *= -1;
    }
    // Update is called once per frame
    void Update()
    {
        // Special Sound effects Part
        // heartbeat for player1
        if (character[0].GetComponent<ZenCharacterController>().Hp < 50)
        {
            P1HeartBeat.Play();
        }
        // heartbeat for player2
        if (character[1].GetComponent<ZenCharacterController>().Hp < 50)
        {
            P2HeartBeat.Play();
        }
        //Energy Increase for PLayer1
        if(character[0].GetComponent<ZenCharacterController>().Energy >= 50)
        {
            if (character[0].GetComponent<ZenCharacterController>().Energy >= P1EnergyLevel+50)
            {
                P1EnergyLevel = P1EnergyLevel + 50;
                P1EnegryIncrease.Play();
            }
        }
        //Energy Increase for PLayer2
        if (character[1].GetComponent<ZenCharacterController>().Energy >= 50)
        {
            if (character[1].GetComponent<ZenCharacterController>().Energy >= P2EnergyLevel + 50)
            {
                P2EnergyLevel = P2EnergyLevel + 50;
                P2EnegryIncrease.Play();
            }
        }
        if (framelimit <= 0 || CheckField())
        {
            OnRoundEnd();
        }
        else if (isStart)
        {
            FrameDataManager.Instance.ProcessFrameData();
            framelimit -= 1;
            _display.currentFrame = framelimit;
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
        RoundResult result = new RoundResult
        {
            CurrentRound = 0,
            ElaspedFrame = _display.GetElaspedFrame(),
            RemainingHPs = new int[]{_controllers[0].Hp,_controllers[1].Hp}
        };
        isStart = false;
        _aiControllers[0].RoundEnd(result);
        _aiControllers[1].RoundEnd(result);
        ResetRound();
    }

    bool CheckField()
    {
        GameObject player1 = character[0];
        GameObject player2 = character[1];
        if (player1.GetComponent<ZenCharacterController>().Hp <= 0 || player2.GetComponent<ZenCharacterController>().Hp <= 0)
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

        GameObject player1 = character[0];
        GameObject player2 = character[1];

        // Get positions of the players
        float player1Position = player1.transform.position.x;
        float player2Position = player2.transform.position.x;

        // Calculate the distance between the players
        float distanceBetweenPlayers = Mathf.Abs(player1Position - player2Position);

        // Only flip characters if they are beyond the threshold distance
        if (distanceBetweenPlayers > flipThreshold)
        {
            // Determine if players need to flip based on their relative positions
            if (player1Position < player2Position && !player1.GetComponent<ZenCharacterController>().IsFront)
            {
                FlipCharacter(player1);
            }
            else if (player1Position > player2Position && player1.GetComponent<ZenCharacterController>().IsFront)
            {
                FlipCharacter(player1);
            }

            if (player2Position < player1Position && !player2.GetComponent<ZenCharacterController>().IsFront)
            {
                FlipCharacter(player2);
            }
            else if (player2Position > player1Position && player2.GetComponent<ZenCharacterController>().IsFront)
            {
                FlipCharacter(player2);
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
