using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SimFightingController : MonoBehaviour
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

    [SerializeField] private int P1EnergyLevel;
    [SerializeField] private int P2EnergyLevel;
    [SerializeField] private List<Key> P1Keys;
    [SerializeField] private List<Key> P2Keys;
    [SerializeField] private Action[] P1Commands;
    [SerializeField] private Action[] P2Commands;
    [SerializeField] private Action[] DummyActionsList = {Action.FORWARD_WALK,Action.FORWARD_WALK,Action.FORWARD_WALK,Action.FORWARD_WALK};
    [SerializeField] private LinkedList<Action> DummyActions;


    public List<GameObject> character;
    private readonly SimCharacterController[] _controllers = new SimCharacterController[2];
    private CommandCenter commandCenter;


    private int currentFrameNumber;
    private int currentRound;

    void Start()
    {
        DummyActions = new LinkedList<Action>(DummyActionsList);

        SetupScene();

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

        _controllers[0] = zen1.GetComponent<SimCharacterController>();
        _controllers[1] = zen2.GetComponent<SimCharacterController>();

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
        commandCenter = new CommandCenter();

        SimFrameDataManager.Instance.SetupFrameData(character[0], character[1], 0);
    }
    void Update() 
    {
        if(Input.GetMouseButton(0)) 
        {
            Debug.Log("Simulator called");
            SimFrameDataManager.Instance.ProcessFrameData(currentFrameNumber);
            Simulate(SimFrameDataManager.Instance.GetFrameData(),true,DummyActions,DummyActions,60);
        }
    }

     
    public void ResetRound()
    {
        _controllers[0].IsFront = true;
        _controllers[0].Hp = GameSetting.Instance.P1HP;
        _controllers[0].Energy = 300;
        character[0].transform.position = spawnP1.transform.position;
        character[0].transform.rotation = spawnP1.transform.rotation;
        
        _controllers[1].IsFront = true;
        _controllers[1].Hp = GameSetting.Instance.P2HP;
        _controllers[1].Energy = 300;
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

    }
    public FrameData Simulate(FrameData frameData, bool playerNumber, LinkedList<Action> myAct, LinkedList<Action> oppAct, int simulatorLimit)
    {
        currentFrameNumber = frameData.CurrentFrameNumber;
        currentRound = frameData.CurrentRound;
        UpdateCharacterDataPlayer1(frameData.CharacterData[0]);
        UpdateCharacterDataPlayer2(frameData.CharacterData[1]);
        if(playerNumber) 
        {
            P1Commands = myAct.ToArray() ;
            P2Commands = oppAct.ToArray();
        }
        else 
        {
            P2Commands = myAct.ToArray() ;
            P1Commands = oppAct.ToArray();
        }
        P1Keys = new List<Key>();
        P2Keys = new List<Key>();

        ProcessingActions(P1Commands, P1Keys);
        ProcessingActions(P2Commands, P2Keys);

        for(int i = 0; i < simulatorLimit; i++) 
        {
            if(i < P1Keys.Count)
                _controllers[0].HandleAIInput(P1Keys[i]);
            if(i < P2Keys.Count)
                _controllers[1].HandleAIInput(P2Keys[i]);
            currentFrameNumber++;
           // Debug.Log("P1 Action: " + p1Keys[i] + " P2 Action: " + p2Keys[i] + " Total Length " + P1Keys.Count);
            //Debug.Log("1 Frame complete");
        }
        SimFrameDataManager.Instance.ProcessFrameData(currentFrameNumber);
        
        return SimFrameDataManager.Instance.GetFrameData();
    }


    private void ClearList()
    {
        foreach (GameObject obj in character)
        {
            Destroy(obj);
        }
        character.Clear();
    }

    private void UpdateCharacterDataPlayer1(CharacterData data) 
    {
        _controllers[0].PlayerNumber = data.PlayerNumber;
        _controllers[0].Hp = data.Hp;
        _controllers[0].Energy = data.Energy;
        character[0].transform.position = new Vector3(data.XPos,  data.YPos, character[0].transform.position.z) ;
        character[0].GetComponent<Rigidbody2D>().velocity = new Vector2 (data.XVelo,data.YVelo);
        _controllers[0].state = data.State;
        _controllers[0].Action = data.Action;
        _controllers[0].IsFront = data.IsFront;
    }

    private void UpdateCharacterDataPlayer2(CharacterData data) 
    {
        _controllers[1].PlayerNumber = data.PlayerNumber;
        _controllers[1].Hp = data.Hp;
        _controllers[1].Energy = data.Energy;
        character[1].transform.position = new Vector3(data.XPos,  data.YPos, character[1].transform.position.z) ;
        character[1].GetComponent<Rigidbody2D>().velocity = new Vector2 (data.XVelo,data.YVelo);
        _controllers[1].state = data.State;
        _controllers[1].Action = data.Action;
        _controllers[1].IsFront = data.IsFront;
    }
    private void ProcessingActions(Action[] myAct, List<Key> keys) 
    {
        for(int i = 0; i < myAct.Length; i++) 
        {
            commandCenter.CommandCall(myAct[i].ToString());
            while (commandCenter.GetSkillFlag()) {
                keys.Add(commandCenter.GetSkillKey());
            }
        }
    }
}
