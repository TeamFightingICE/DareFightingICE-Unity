using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using UnityEngine;

public class FrameDataManager : Singleton<FrameDataManager>
{
    private FrameData currentFrameData = new();
    public readonly CharacterData[] characterData = new CharacterData[2];
    private readonly bool[] front = new bool[2];
    public GameObject[] character = new GameObject[2];
    private readonly Rigidbody2D[] rb = new Rigidbody2D[2];
    private readonly ZenCharacterController[] _controllers = new ZenCharacterController[2];
    private InterfaceDisplay _interfaceDisplay;
    private FightingController _fightingController;

    public int currentFrameNumber;

    public int currentRound;
    
    public GameObject[] projectileData;

    public bool emptyFlag;

    private Action action = new Action();

    private int GetRemainingFrame()
    {
        return GameSetting.Instance.FrameLimit - currentFrameNumber;
    }
    
    public void SetupFrameData(GameObject character1, GameObject character2, InterfaceDisplay frameInfo, FightingController fightingController)
    {
        character[0] = character1;
        character[1] = character2;
        rb[0] = character1.GetComponent<Rigidbody2D>();
        rb[1] = character2.GetComponent<Rigidbody2D>();
        _controllers[0] = character1.GetComponent<ZenCharacterController>();
        _controllers[1] = character2.GetComponent<ZenCharacterController>();
        _interfaceDisplay = frameInfo;
        currentFrameNumber = 0;
        currentRound = DataManager.Instance.CurrentRound;
        _fightingController = fightingController;
    }

    private AttackData GetAttackData(HitBoxController controller)
    {
        return new AttackData()
        {
            SettingHitArea = new HitArea(),  // dont have
            SettingSpeedX = 0,  // dont have
            SettingSpeedY = 0,  // dont have
            CurrentHitArea = new HitArea(),  // dont have
            CurrentFrame = 0,  // dont have
            PlayerNumber = false,  // dont have
            SpeedX = 0,  // dont have
            SpeedY = 0,  // dont have
            StartUp = 0,  // dont have
            Active = controller.isActive,
            HitDamage = controller.damage,  // not sure
            GuardDamage = 0,  // dont have
            StartAddEnergy = 0,  // dont have
            HitAddEnergy = controller.getEnergy,  // not sure
            GuardAddEnergy = controller.guardEnergy, // not sure
            GiveEnergy = controller.giveEnergy,
            ImpactX = controller.impactX,
            ImpactY = controller.impactY,
            GiveGuardRecov = 0,  // dont have
            AttackType = controller.attackType,
            DownProp = controller.isDown,
            IsProjectile = controller.isProjectile,
        };
    }
    
    private CharacterData GetCharacterData(bool playerNumber)
    {
        int idx = playerNumber ? 0 : 1;
        var controller = _controllers[idx];
        var _rb = rb[idx];

        return new CharacterData
        {
            PlayerNumber = controller.PlayerNumber,
            Hp = controller.Hp,
            Energy = controller.Energy,
            X = (int)controller.transform.position.x,
            Y = (int)controller.transform.position.y,
            Left = 0,  // dont have
            Right = 0,  // dont have
            Top = 0,  // dont have
            Bottom = 0,  // dont have
            SpeedX = (int)_rb.velocity.x,
            SpeedY = (int)_rb.velocity.y,
            State = controller.state,
            Action = controller.Action,
            Front = controller.IsFront,
            Control = true,  // dont have
            AttackData = GetAttackData(controller.leftHand),  // not sure
            RemainingFrame = 0,  // dont have
            HitConfirm = false,  // dont have
            GraphicSizeX = 0,  // dont have
            GraphicSizeY = 0,  // dont have
            GraphicAdjustX = 0,  // dont have
            HitCount = 0,  // dont have
            LastHitFrame = 0,  // dont have
        };
    }

    public void UpdateCharacterData()
    {
        int remainingFrame = GetRemainingFrame();  // this is not remaining frame of Java version

        characterData[0] = GetCharacterData(true);
        front[0] = characterData[0].Front;

        characterData[1] = GetCharacterData(false);
        front[1] = characterData[1].Front;
    }
    
    public void ProcessFrameData()
    {
        UpdateCharacterData();
        currentFrameNumber = _interfaceDisplay.currentFrame;

        List<AttackData> projectileList = new();
        foreach (var attack in _fightingController.attackDeque)
        {
            var proj = attack.GetComponent<HitBoxController>();
            projectileList.Add(GetAttackData(proj));
        }

        currentFrameData = new()
        {
            CharacterData = characterData,
            CurrentFrameNumber = currentFrameNumber,
            CurrentRound = currentRound,
            ProjectileData = projectileList,
            EmptyFlag = false,
            Front = front
        };
    }

    public FrameData GetFrameData()
    {
        return new FrameData(currentFrameData);
    }
}
