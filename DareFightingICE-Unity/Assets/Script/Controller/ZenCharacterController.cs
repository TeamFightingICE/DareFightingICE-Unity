using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using TMPro;

public class ZenCharacterController : MonoBehaviour
{
    /// <summary>
    /// zenCharacter Info contain information to send to FightingController
    /// Action Flags are hardcode bridge that connect animator with this script
    /// Combo System
    /// MAX_BUFFER_SIZE to control max input inside buffer when it exceed it will start remove first buffer
    /// allTrigger is to control animation when add more trigger inside animator please add those trigger here too
    /// Reset Buffer time is to clear buffer after player stop press button
    /// </summary>
    
    // zenCharacter Info
    public bool PlayerNumber { get; set; }
    public bool IsFront { get; set; }
    public int Hp { get; set; }
    public int Energy { get; set; }
    public ZenCharacterController otherPlayer { get; set; }
    public FightingController fightingController;
    public TextAsset csvFile;
    public TextMeshPro PlayerNum;
    
    // Action Flags
    public bool canWalk = true;
    public bool canBlock = true;
    public bool canAttack = true;
    public bool canDash = true;
    public bool isGuard = false;
    public bool canJump = true;

    // Combo System
    private List<string> inputBuffer = new List<string>();
    private List<string> sInputBuffer = new List<string>();
    private Dictionary<string, List<string>> combos = new Dictionary<string, List<string>>();
    private Dictionary<string, List<string>> Scombos = new Dictionary<string, List<string>>();
    private int MAX_BUFFER_SIZE = 15;
    private int MAX_SBUFFER_SIZE = 3;
    private string[] allTriggers = { "JUMP", "STAND_B", "STAND_A","STAND_FA","STAND_FB","GETHIT","GETKNOCK","GET_THROW","AIR_A","AIR_B","AIR_FA","AIR_FB","BACK_STEP","DASH","FORWARD_JUMP","STAND_THROW_A","STAND_THROW_B","CROUCH_A","CROUCH_B","CROUCH_FA","CROUCH_FB","AIR_DA","AIR_DB","AIR_UA","AIR_UB" };
    private string[] comboTriggers = {"STAND_F_D_DFA","STAND_F_D_DFB","STAND_D_DB_BA","STAND_D_DB_BB","STAND_D_DF_FA","STAND_D_DF_FB","STAND_D_DF_FC","AIR_D_DF_FB","AIR_F_D_DFA","AIR_F_D_DFB","AIR_D_DB_BA","AIR_D_DB_BB","AIR_D_DF_FA"};
    private float lastInputTime;
    private float lastSInputTime;
    private float bufferResetDelay = 0.3f;
    private float SbufferResetDelay = 1f;

    public int currentCombo = 0;
    private float timeSinceLastHit = 0f;
    private float comboResetTime = 0.5f;
    
    // zenCharacter Control
    public float speed = 5.0f;
    public float jumpForce = 7.0f;
    public Transform groundCheck;
    public LayerMask groundLayer;
    public float groundCheckRadius = 0.2f;
    private Rigidbody2D rb;
    public bool isGrounded;
    public bool isCrouching = false;
    public State state = State.Stand;
    public Action Action = Action.NEUTRAL;
    [SerializeField] private Animator _animator;
    private float jumpTimer = 0f;
    private float jumpDelay = 0.5f;
    private float lastDirectionalInputTime = 0.5f;
    private float directionalInputDelay = 0f;
    
    private float lastCrouchInputTime = 0.5f;
    private float CrouchInputDelay = 0.1f;
    
    private float lastThrowInputTime = 0.5f;
    private float throwInputDelay = 0.1f;
    
    private float lastForwardDashTime = -1f;
    private float doubleTapInterval = 0.3f;
    
    private float lastBackwardDashTime = -1f;
    private float keyPressTime;
    private float tapThreshold = 0.2f;


    [SerializeField] private float dashforce = 0;
    // HitBoxController
    [SerializeField] public HitBoxController leftHand;
    [SerializeField] public HitBoxController rightHand;
    [SerializeField] public HitBoxController leftFoot;
    [SerializeField] public HitBoxController rightFoot;
    
    // SoundControl
    [SerializeField] private GameObject counchSound;
    public AIController aiController;
    private GameSetting gameSetting;
    private InputManager inputManager;
    void Start()
    {
        gameSetting = GameSetting.Instance;
        inputManager = InputManager.Instance;

        rb = GetComponent<Rigidbody2D>();
        
        Scombos.Add("F_D_DFA", new List<string> {"F","D","A"});
        Scombos.Add("F_D_DFB", new List<string> {"F","D","B"});
        Scombos.Add("D_DB_BA", new List<string> {"D","B","A"});
        Scombos.Add("D_DB_BB", new List<string> {"D","B","F"});
        Scombos.Add("D_DF_FA", new List<string> {"D","F","A"});
        Scombos.Add("D_DF_FB", new List<string> {"D","F","B"});
        Scombos.Add("D_DF_FC", new List<string> {"D","F","C"});

        combos.Add("F_D_DFA", new List<string> { "F", "D", "A" });
        combos.Add("F_D_DFB", new List<string> { "F", "D", "B" });
        combos.Add("D_DB_BA", new List<string> { "D", "B", "A" });
        combos.Add("D_DB_BB", new List<string> { "D", "B", "F" });
        combos.Add("D_DF_FA", new List<string> { "D", "F", "A" });
        combos.Add("D_DF_FB", new List<string> { "D", "F", "B" });
        combos.Add("D_DF_FC", new List<string> { "D", "F", "C" });

        combos.Add("FA", new List<string> {"F","A"});
        combos.Add("FB", new List<string> {"F","B"});
        combos.Add("DA", new List<string> {"D","A"});
        combos.Add("DB", new List<string> {"D","B"});
        combos.Add("UA", new List<string> {"U","A"});
        combos.Add("UB", new List<string> {"U","B"});
        combos.Add("THROW_A", new List<string> {"THROW","A"});
        combos.Add("THROW_B", new List<string> {"THROW","B"});
        combos.Add("A", new List<string> {"A"});
        combos.Add("B", new List<string> {"B"});
        // Add more combos as needed
    }

    void Update()
    {
        //print(jumpTimer);
        //print(inputBuffer);
        CheckState();
        if (gameSetting.P1ControlType != ControlType.KEYBOARD || gameSetting.P2ControlType != ControlType.KEYBOARD)
        {
            UpdateAI();
        }

        if (gameSetting.P1ControlType == ControlType.KEYBOARD && PlayerNumber)
        {
            HandleInputP1();
        }
        else if (gameSetting.P2ControlType == ControlType.KEYBOARD && !PlayerNumber)
        {
            HandleInputP2();
        }


        CheckCombo();
        ResetBuffer();
        UpdateComboTimer();
    }
    void UpdateAI()
    {
        // Assuming aiController.Input() returns an object similar to the Key class in your Java example
        Key aiInput = aiController.Input();
        inputManager.SetInput(PlayerNumber, aiInput);
        HandleAIInput();
    }

    private void HandleInputP1()
    {
        float currentTime = Time.time;
        if (Input.GetKey(KeyCode.DownArrow))
        {
            if (currentTime - lastCrouchInputTime > CrouchInputDelay && !isCrouching)
            {
                lastCrouchInputTime = currentTime;
                AddInput("D");
            }
            if (isGrounded)
            {
                PerformCrounch();
            }
        }
        else
        {
            isCrouching = false;
            _animator.SetBool("CROUCH",false);
        }
        // Movement input
        if (IsFront)
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                if (currentTime - lastForwardDashTime < doubleTapInterval && canDash)
                {
                    canWalk = false;
                    PerformFrontDash(new Vector2(1,0));
                }
                else
                {
                    lastForwardDashTime = currentTime;
                }
            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                if (currentTime - lastDirectionalInputTime > directionalInputDelay)
                {
                    lastDirectionalInputTime = currentTime;
                    AddInput("F");
                }

                if (canWalk)
                {
                    canWalk = false;
                    PerformWalk(1);
                }
                
                
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                keyPressTime = currentTime;
            }
            else if (Input.GetKey(KeyCode.LeftArrow) && canBlock)
            { 
                if (Input.GetKeyDown(KeyCode.UpArrow) && isGrounded)
                {
                    PerformBackwardJump(-1);
                }
                else
                {
                    if (currentTime - lastThrowInputTime > throwInputDelay)
                    {
                        lastThrowInputTime = currentTime;
                        AddInput("THROW");
                    }
                    PerformBlock();
                }
            }
            else if (Input.GetKeyUp(KeyCode.LeftArrow))
            {
                if (currentTime - keyPressTime <= tapThreshold && canDash)
                {
                    var direction = new Vector2(-1, 0);
                    PerformBackStep(direction);
                    canDash = false;
                }
                else if (currentTime - keyPressTime > tapThreshold && canBlock)
                {
                    PerformBlock();
                }
            }
            else
            {
                _animator.SetBool("FORWARD_WALK", false);
                _animator.SetBool("GUARD", false);
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                if (currentTime - lastForwardDashTime < doubleTapInterval && canDash)
                {
                    canWalk = false;
                    PerformFrontDash(new Vector2(-1,0));
                }
                else
                {
                    lastForwardDashTime = currentTime;
                }
            }
            else if (Input.GetKey(KeyCode.LeftArrow) && canWalk)
            {
                if (currentTime - lastDirectionalInputTime > directionalInputDelay)
                {
                    lastDirectionalInputTime = currentTime;
                    AddInput("F");
                }
                if (canWalk)
                {
                    canWalk = false;
                    PerformWalk(-1);
                }
                
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                keyPressTime = currentTime;
            }
            else if (Input.GetKey(KeyCode.RightArrow) && canBlock)
            { 
                if (Input.GetKeyDown(KeyCode.UpArrow) && isGrounded)
                {
                    PerformBackwardJump(1);
                }
                else
                {
                    if (currentTime - lastThrowInputTime > throwInputDelay)
                    {
                        lastThrowInputTime = currentTime;
                        AddInput("THROW");
                    }
                    PerformBlock();
                }
            }
            else if (Input.GetKeyUp(KeyCode.RightArrow))
            {
                if (currentTime - keyPressTime <= tapThreshold && canDash)
                {
                    var direction = new Vector2(1, 0);
                    PerformBackStep(direction);
                    canDash = false;
                }
                else if (currentTime - keyPressTime > tapThreshold && canBlock)
                {
                    PerformBlock();
                }
            }
            else
            {
                _animator.SetBool("FORWARD_WALK", false);
                _animator.SetBool("GUARD", false);
            }
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (Mathf.Abs(rb.velocity.x) > 0 && isGrounded)
            {
                AddInput("U");
                PerformForwardJump(rb.velocity.x > 0 ? 1 : -1);
            }
            else if (isGrounded)
            {
                AddInput("U");
                PerformJump();
            }
        }
        
        // TESTING
        /*
         if (Input.GetKeyDown(KeyCode.M) && canAttack) { _animator.SetTrigger("GETHIT"); }
         if (Input.GetKeyDown(KeyCode.N) && canAttack) { _animator.SetTrigger("GETKNOCK"); }
         if (Input.GetKeyDown(KeyCode.B) && canAttack) { _animator.SetTrigger("GET_THROW"); }
         if (Input.GetKeyDown(KeyCode.Q) && canAttack)
         {
             ExecuteGivenCombo("F_D_DFA");
         }
         if (Input.GetKeyDown(KeyCode.W) && canAttack)
         {
             ExecuteGivenCombo("F_D_DFB");
         }
         if (Input.GetKeyDown(KeyCode.E) && canAttack)
         {
             ExecuteGivenCombo("D_DB_BA");
         }
         if (Input.GetKeyDown(KeyCode.R) && canAttack)
         {
             ExecuteGivenCombo("D_DB_BB");
         }
         if (Input.GetKeyDown(KeyCode.A) && canAttack)
         {
             ExecuteGivenCombo("D_DF_FA");
         }
         if (Input.GetKeyDown(KeyCode.S) && canAttack)
         {
             ExecuteGivenCombo("D_DF_FB");
         }
        if (Input.GetKeyDown(KeyCode.D) && canAttack)
        {
            ExecuteGivenCombo("D_DF_FC");
        }
        */
        // Combat input
        if (Input.GetKeyDown(KeyCode.Z) && canAttack) { AddInput("A"); }
        if (Input.GetKeyDown(KeyCode.X) && canAttack) { AddInput("B"); }
        if (Input.GetKeyDown(KeyCode.C) && canAttack) { AddInput("C"); }
    }
    
    private void HandleInputP2()
    {
        float currentTime = Time.time;
        if (Input.GetKey(KeyCode.K))
        {
            if (currentTime - lastCrouchInputTime > CrouchInputDelay && !isCrouching)
            {
                lastCrouchInputTime = currentTime;
                AddInput("D");
            }
            if (isGrounded)
            {
                PerformCrounch();
            }
        }else
        {
            isCrouching = false;
            _animator.SetBool("CROUCH",false);
        }
        // Movement input
        if (IsFront)
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                if (currentTime - lastForwardDashTime < doubleTapInterval && canDash)
                {
                    canWalk = false;
                    PerformFrontDash(new Vector2(1,0));
                }
                else
                {
                    lastForwardDashTime = currentTime;
                }
            }
            else if (Input.GetKey(KeyCode.L))
            {
                if (currentTime - lastDirectionalInputTime > directionalInputDelay)
                {
                    lastDirectionalInputTime = currentTime;
                    AddInput("F");
                }

                if (canWalk)
                {
                    canWalk = false;
                    PerformWalk(1);
                }
                
                
            }
            else if (Input.GetKeyDown(KeyCode.J))
            {
                keyPressTime = currentTime;
            }
            else if (Input.GetKey(KeyCode.J) && canBlock)
            { 
                if (Input.GetKeyDown(KeyCode.I) && isGrounded)
                {
                    PerformBackwardJump(-1);
                }
                else
                {
                    if (currentTime - lastThrowInputTime > throwInputDelay)
                    {
                        lastThrowInputTime = currentTime;
                        AddInput("THROW");
                    }
                    PerformBlock();
                }
            }
            else if (Input.GetKeyUp(KeyCode.J))
            {
                if (currentTime - keyPressTime <= tapThreshold && canDash)
                {
                    var direction = new Vector2(-1, 0);
                    PerformBackStep(direction);
                    canDash = false;
                }
                else if (currentTime - keyPressTime > tapThreshold && canBlock)
                {
                    PerformBlock();
                }
            }
            else
            {
                _animator.SetBool("FORWARD_WALK", false);
                _animator.SetBool("GUARD", false);
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.J))
            {
                if (currentTime - lastForwardDashTime < doubleTapInterval && canDash)
                {
                    canWalk = false;
                    PerformFrontDash(new Vector2(-1,0));
                }
                else
                {
                    lastForwardDashTime = currentTime;
                }
            }
            else if (Input.GetKey(KeyCode.J) && canWalk)
            {
                if (currentTime - lastDirectionalInputTime > directionalInputDelay)
                {
                    lastDirectionalInputTime = currentTime;
                    AddInput("F");
                }
                if (canWalk)
                {
                    canWalk = false;
                    PerformWalk(-1);
                }
                
            }
            else if (Input.GetKeyDown(KeyCode.L))
            {
                keyPressTime = currentTime;
            }
            else if (Input.GetKey(KeyCode.L) && canBlock)
            { 
                if (Input.GetKeyDown(KeyCode.I) && isGrounded)
                {
                    PerformBackwardJump(1);
                }
                else
                {
                    if (currentTime - lastThrowInputTime > throwInputDelay)
                    {
                        lastThrowInputTime = currentTime;
                        AddInput("THROW");
                    }
                    PerformBlock();
                }
            }
            else if (Input.GetKeyUp(KeyCode.L))
            {
                if (currentTime - keyPressTime <= tapThreshold && canDash)
                {
                    var direction = new Vector2(1, 0);
                    PerformBackStep(direction);
                    canDash = false;
                }
                else if (currentTime - keyPressTime > tapThreshold && canBlock)
                {
                    PerformBlock();
                }
            }
            else
            {
                _animator.SetBool("FORWARD_WALK", false);
                _animator.SetBool("GUARD", false);
            }
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            if (Mathf.Abs(rb.velocity.x) > 0 && isGrounded)
            {
                AddInput("U");
                PerformForwardJump(rb.velocity.x > 0 ? 1 : -1);
            }
            else if (isGrounded)
            {
                AddInput("U");
                PerformJump();
            }
        }
        
        // TESTING
        // if (Input.GetKeyDown(KeyCode.M) && canAttack) { _animator.SetTrigger("GETHIT"); }
        // if (Input.GetKeyDown(KeyCode.N) && canAttack) { _animator.SetTrigger("GETKNOCK"); }
        // if (Input.GetKeyDown(KeyCode.B) && canAttack) { _animator.SetTrigger("GET_THROW"); }
        // Combat input
        if (Input.GetKeyDown(KeyCode.T) && canAttack) { AddInput("A");}
        if (Input.GetKeyDown(KeyCode.Y) && canAttack) { AddInput("B");}
        if (Input.GetKeyDown(KeyCode.U) && canAttack) { AddInput("C");}
    }
    

    private void AddInput(string action)
    {
        AddSpacialInput(action);
        lastInputTime = Time.time;
        if (inputBuffer.Count >= MAX_BUFFER_SIZE)
        {
            inputBuffer.RemoveAt(0);
        }
        inputBuffer.Add(action);
        //Debug.Log("Input Added: " + action + ",\n Buffer: " + string.Join(", ", inputBuffer));

    }

    private void AddSpacialInput(string action)
    {
        lastSInputTime = Time.time;
        if (!sInputBuffer.Contains(action))  // Check if the action is not already in the buffer
        {
            if (sInputBuffer.Count >= MAX_SBUFFER_SIZE)
            {
                sInputBuffer.RemoveAt(0); // Remove the first element to make space if the buffer is full
            }
            sInputBuffer.Add(action); // Add the new action since it's not already in the buffer
            //Debug.Log("Special Input Added: " + action + ",\n Buffer: " + string.Join(", ", sInputBuffer));
        }
        else
        {
            //Debug.Log("Action " + action + " already in buffer, not added again.");
        }
        //Debug.Log("Spacial Input Added: " + action + ",\n Buffer: " + string.Join(", ", sInputBuffer));
        //Debug.Log("Input Added: " + action + ",\n Buffer: " + string.Join(", ", inputBuffer));

    }

    private void CheckState()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        if (state != State.Down)
        {
            if (isGrounded)
            {
                if (isCrouching)
                {
                    SetState(State.Crouch);
                    _animator.SetBool("INAIR",false);
                }
                else if (!isCrouching)
                {
                    SetState(State.Stand);
                    if (jumpTimer > 0)
                    {
                        jumpTimer -= Time.deltaTime;
                        canJump = false;
                    }
                    else
                    {
                        canJump = true;
                    }
                    _animator.SetBool("INAIR",false);
                }
            }
            else
            { 
                SetState(State.Air);
                _animator.SetBool("INAIR",true);
                canWalk = false;
            }
        }
    }
    private void CheckCombo()
    {
        // if (inputBuffer.Count < 2)
        // {
        //     return; // Not enough inputs for a combo
        // }
        if (sInputBuffer.Count == 3)
        {
            var SsortedCombos = Scombos.OrderByDescending(c => c.Value.Count);
               foreach (var combo in SsortedCombos)
                {
                    if (sInputBuffer.Count >= combo.Value.Count && IsComboMatch(combo.Value, sInputBuffer))
                    {
                        //Debug.Log("Executing Combo: " + combo.Key);
                        ExecuteCombo(combo.Key);
                        return; // Exit the loop after executing a combo
                    }
                }
        }
        

        var sortedCombos = combos.OrderByDescending(c => c.Value.Count);

        foreach (var combo in sortedCombos)
        {
            if (inputBuffer.Count >= combo.Value.Count && IsComboMatch(combo.Value,inputBuffer))
            {
                //Debug.Log("Executing Combo: " + combo.Key);
                ExecuteCombo(combo.Key);
                return; // Exit the loop after executing a combo
            }
        }

        
    }

    private void ResetBuffer()
    {
        if (Time.time - lastInputTime > bufferResetDelay)
        {
            inputBuffer.Clear();
        }
        if (Time.time - lastSInputTime > SbufferResetDelay)
        {
            sInputBuffer.Clear();
        }
    }

    public void ResetSpacialCombo()
    {
        foreach (var trigger in comboTriggers)
        {
            _animator.ResetTrigger(trigger);
        }
    }
    /*private bool IsComboMatch(List<string> comboSequence)
    {
        for (int i = 0; i < comboSequence.Count; i++)
        {
            if (inputBuffer[inputBuffer.Count - comboSequence.Count + i] != comboSequence[i])
                return false;
        }
        return true;
    }*/


    private bool IsComboMatch(List<string> comboSequence, List<string> inputList)
    {
        // Check if there are enough inputs in the list to match the combo
        if (inputList.Count < comboSequence.Count)
            return false;

        int startIndex = inputList.Count - comboSequence.Count;
        for (int i = 0; i < comboSequence.Count; i++)
        {
            if (inputList[startIndex + i] != comboSequence[i])
                return false;
        }
        return true;
    }
    private void ExecuteGivenCombo(string comboName)
    {
        string action = "";
        
        switch (state)
        {
            case State.Stand:
                action = "STAND_" + comboName;
                PerformActtack(action);
                break;
            case State.Air:
                action = "AIR_" + comboName;
                PerformActtack(action);
                break;
            case State.Crouch:
                action = "CROUCH_" + comboName;
                PerformActtack(action);
                break;
        }
    }
    private void ExecuteCombo(string comboName)
    {
        string action = "";
        
        switch (state)
        {
            case State.Stand:
                action = "STAND_" + comboName;
                PerformActtack(action);
                break;
            case State.Air:
                action = "AIR_" + comboName;
                PerformActtack(action);
                break;
            case State.Crouch:
                action = "CROUCH_" + comboName;
                PerformActtack(action);
                break;
        }
    }

    private void SetState(State state)
    {
        this.state = state;
    }
    
    private void PerformActtack(string actionname)
    {
        int requireEnergy = MotionManager.Instance.GetStartGiveEnergyForMotion("zen", actionname);
        if (canAttack && !isGuard)
        {
            if (Energy >= Mathf.Abs(requireEnergy) && requireEnergy != 1)
            {
                SetMotionData(MotionManager.Instance.GetMotionAttributes("zen", actionname));
                Energy += requireEnergy;
                _animator.SetTrigger(actionname);
                canDash = false;
                canAttack = false;
                inputBuffer.Clear();
                sInputBuffer.Clear();
            }
            else
            {
                Debug.LogWarning("Not Enough Energy");
                inputBuffer.Clear();
                sInputBuffer.Clear();
            }
        }
    }
    
    private void PerformWalk(float direction)
    {
        if (state == State.Crouch)
        {
            _animator.SetBool("CROUCH", false);
        }
        else if (state == State.Stand)
        {
            rb.mass = 8;
            Vector2 movement = new(direction * speed, rb.velocity.y);
            rb.velocity = movement;

            // Trigger walking animation
            _animator.SetBool("GUARD", false);
            _animator.SetBool("FORWARD_WALK", true);
        }
    }

    private void PerformBackStep(Vector2 direction)
    {
        rb.velocity = direction * dashforce;
        canBlock = false;
        _animator.SetTrigger("BACK_STEP");
    }
    
    private void PerformFrontDash(Vector2 direction)
    {
        if (state == State.Stand)
        {
            rb.velocity = direction * dashforce;
            canBlock = false;
            _animator.SetTrigger("DASH");
        }
    }

    public void ResetVelocity()
    {
        rb.velocity = new Vector2(0,0);
    }
    // This is very hardcode for bug fixing
    // Please don't try to use it too much
    public void ResetTrigger()
    {
        foreach (var trigger in allTriggers)
        {
            _animator.ResetTrigger(trigger);
        }
    }
    private void PerformBlock()
    {
        // Blocking logic and animation trigger
        _animator.SetBool("CROUCH", false);
        _animator.SetBool("FORWARD_WALK", false);
        _animator.SetBool("GUARD", true);
        // Set canBlock to false if the block animation should only be triggered once per key press
    }

    private void PerformJump()
    {
        // Jumping logic and animation trigger
        if (state == State.Crouch)
        {
            _animator.SetBool("CROUCH", false);
        }
        else if (isGrounded && jumpTimer <= 0 && canJump && state == State.Stand)
        {
            _animator.SetTrigger("JUMP");
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpTimer = jumpDelay;
            canJump = false;
        }
    }

    //Added to prevent Player from getting stuck on top.
     private void OnCollisionStay2D(Collision2D collision)
        { 
            if (LayerMask.LayerToName(collision.transform.gameObject.layer) == "Player")
            {
                if(!isGrounded) 
                {
                    int signed = IsFront ? -1 : 1;
                    Vector2 movement = new(signed * speed, rb.velocity.y);
                    rb.velocity = movement;
                }
            }
        }

    
    private void PerformCrounch()
    {
        if (!isCrouching)
        {
            counchSound.SetActive(true);
        }
        isCrouching = true;
        _animator.SetBool("FORWARD_WALK", false);
        _animator.SetBool("CROUCH", true);
    }

    public void ResetCounch()
    {
        counchSound.SetActive(false);
    }
    private void PerformForwardJump(float direction)
    {
        // Apply both upward and forward force
        if (canJump && isGrounded && jumpTimer <= 0)
        {
            Vector2 forwardJumpVelocity = new Vector2(direction * speed, jumpForce);
            rb.velocity = forwardJumpVelocity;
            _animator.SetTrigger("FORWARD_JUMP");
            jumpTimer = jumpDelay;
            canJump = false;
        }
    }
    
    private void PerformBackwardJump(float direction)
    {
        // Apply both upward and forward force
        if (canJump && isGrounded && jumpTimer <= 0)
        {
            Vector2 forwardJumpVelocity = new Vector2(direction * speed, jumpForce);
            rb.velocity = forwardJumpVelocity;
            _animator.SetTrigger("FORWARD_JUMP");
            jumpTimer = jumpDelay;
            canJump = false;
        }   
    }

    public void GetThrow()
    {
        float direction;
        if (IsFront)
        {
            direction = -1;
        }
        else
        {
            direction = 1;
        }
        Vector2 forwardJumpVelocity = new Vector2(direction * 3, jumpForce);
        rb.velocity = forwardJumpVelocity;
    }

    public void SetTarget(string target,FightingController fighting)
    {
        leftHand.zenCharacterController = this;
        rightHand.zenCharacterController = this;
        leftFoot.zenCharacterController = this;
        rightFoot.zenCharacterController = this;
        
        leftHand.fightingController = fighting;
        rightHand.fightingController = fighting;
        leftFoot.fightingController = fighting;
        rightFoot.fightingController = fighting;


        leftHand.target = target;
        rightHand.target = target;
        leftFoot.target = target;
        rightFoot.target = target;
    }

    public void SetMotionData(MotionAttribute motionAttribute)
    {
        leftHand.SetData(motionAttribute);
        rightHand.SetData(motionAttribute);
        leftFoot.SetData(motionAttribute);
        rightFoot.SetData(motionAttribute);
    }
    public void TakeHit(ZenCharacterController attacker,int getHitEnergy,int damage,int getEnegy,int guardDamage,int guardEnergy,AttackType type,bool isDown)
    {
        switch (state)
        {
            case State.Air:
                if (isGuard && (type == AttackType.HIGH || type == AttackType.MIDDLE))
                {
                    Hp -= guardDamage;
                    AddEnergy(getHitEnergy);
                    attacker.GetReward(guardEnergy);
                    _animator.SetTrigger("GETHIT");
                }
                else
                {
                    if (isDown)
                    {
                        _animator.SetTrigger("GETKNOCK");
                    }
                    else
                    {
                        _animator.SetTrigger("GETHIT");
                    }
                    Hp -= damage;
                    AddEnergy(getHitEnergy);
                    attacker.GetReward(getEnegy);
                }
                break;
            case State.Stand:
                if (isGuard && (type == AttackType.HIGH || type == AttackType.MIDDLE))
                {
                    Hp -= guardDamage;
                    AddEnergy(getHitEnergy);
                    attacker.GetReward(guardEnergy);
                    _animator.SetTrigger("GETHIT");
                }
                else
                {
                    if (isDown)
                    {
                        _animator.SetTrigger("GETKNOCK");
                    }
                    else
                    {
                        _animator.SetTrigger("GETHIT");
                    }
                    Hp -= damage;
                    AddEnergy(getHitEnergy);
                    attacker.GetReward(getEnegy);
                }
                break;
            case State.Crouch:
                if (isGuard && (type == AttackType.HIGH || type == AttackType.LOW))
                {
                    Hp -= guardDamage;
                    AddEnergy(getHitEnergy);
                    attacker.GetReward(guardEnergy);
                    _animator.SetTrigger("GETHIT");
                }
                else
                {
                    if (isDown)
                    {
                        _animator.SetTrigger("GETKNOCK");
                        print("GetKnock");
                    }
                    else
                    {
                        _animator.SetTrigger("GETHIT");
                    }
                    Hp -= damage;
                    AddEnergy(getHitEnergy);
                    attacker.GetReward(getEnegy);
                }
                break;
        }
        
    }

    public void TakeThrow(ZenCharacterController attacker,int giveEnergy,int damage,int getEnegy)
    {
        if (isGrounded)
        {
            Hp -= damage;
            Energy += giveEnergy;
            attacker.GetReward(getEnegy);
            GetThrow();
            _animator.SetTrigger("GET_THROW");
            print("GetThrow");
        }
    }
    void AddEnergy(int amount)
    {
        Energy += amount;
        if (Energy > 300)
        {
            Energy = 300;
        }

    }
    public void GetReward(int energy)
    {
        timeSinceLastHit = 0f;
        if (currentCombo < 9)
        {
            currentCombo ++;
        }
        else
        {
            currentCombo = 9;
        }
        this.Energy += energy;
        if (this.Energy >= 300)
        {
            Energy = 300;
        }
    }

    private void ResetCombo()
    {
        currentCombo = 0;
    }

    private void UpdateComboTimer()
    {
        timeSinceLastHit += Time.deltaTime;
        if (timeSinceLastHit >= comboResetTime)
        {
            ResetCombo();
        }
    }
    
    public void SpawnLargeFireball()
    {
        Vector2 fireballDirection = new Vector2(1, 0);
        float fireballForce = 10f;
        if (IsFront)
        {
            //fireballDirection = new Vector2(1, 0);
            fireballForce = 10f;
            fightingController.AddAttackDeque(leftHand.SpawnBigProjectile(fireballDirection, fireballForce, false));
        }
        else
        {
            fireballForce = -10f;
            fightingController.AddAttackDeque(leftHand.SpawnBigProjectile(fireballDirection, fireballForce, true));

            //fireballDirection = new Vector2(0, 1);
        }
        
    }

    public void SpawnSmallFireball()
    {
        Vector2 fireballDirection = new Vector2(1, 0);
        float fireballForce = 10f;
        if (IsFront)
        {
            fireballForce = 10f;
            fightingController.AddAttackDeque(leftHand.SpawnSmallProjectile(fireballDirection, fireballForce, false));
            //fireballDirection = new Vector2(1, 0);
        }
        else
        {
            fireballForce = -10f;
            fightingController.AddAttackDeque(leftHand.SpawnSmallProjectile(fireballDirection, fireballForce, true));
            //fireballDirection = new Vector2(0, 1);
        }
        
        
    }

    float uncrouchDelay = 0.1f;
    float uncrouchTimer = 0;
    public void HandleAIInput()
    {
        float currentTime = Time.time;
        Key key = inputManager.GetInput(PlayerNumber);
        if (key.D)
        {
            if (currentTime - lastCrouchInputTime > CrouchInputDelay && !isCrouching)
            {
                lastCrouchInputTime = currentTime;
                AddInput("D");
            }
            if (isGrounded)
            {
                PerformCrounch();
            }
        }
        else
        {
            isCrouching = false;
            _animator.SetBool("CROUCH", false);
            uncrouchTimer = currentTime;
        }
        // Movement input
        if (IsFront)
        {
            if (inputManager.IsKeyPressed(PlayerNumber, "R"))
            {
                if (currentTime - lastForwardDashTime < doubleTapInterval && canDash && isGrounded)
                {
                    if(!isCrouching && currentTime - uncrouchTimer > uncrouchDelay)
                    {
                        canWalk = false;
                        PerformFrontDash(new Vector2(1, 0));
                    }
                }
                else
                {
                    lastForwardDashTime = currentTime;
                }
            }
            else if (inputManager.IsKeyHeld(PlayerNumber, "R"))
            {
                if (currentTime - lastDirectionalInputTime > directionalInputDelay)
                {
                    lastDirectionalInputTime = currentTime;
                    AddInput("F");
                }

                if (canWalk && !isCrouching && currentTime - uncrouchTimer > uncrouchDelay && isGrounded) 
                {
                    canWalk = false;
                    PerformWalk(1);
                }
            }
            else if (inputManager.IsKeyPressed(PlayerNumber, "L"))
            {
                keyPressTime = currentTime;
            }
            else if (inputManager.IsKeyHeld(PlayerNumber, "L") && canBlock)
            { 
                if (Input.GetKeyDown(KeyCode.UpArrow) && isGrounded)
                {
                    PerformBackwardJump(-1);
                }
                else
                {
                    if (currentTime - lastThrowInputTime > throwInputDelay)
                    {
                        lastThrowInputTime = currentTime;
                        AddInput("THROW");
                    }
                    PerformBlock();
                }
            }
            else if (inputManager.IsKeyRelease(PlayerNumber, "L") && isGrounded)
            {
                if (currentTime - keyPressTime <= tapThreshold && canDash)
                {
                    var direction = new Vector2(-1, 0);
                    PerformBackStep(direction);
                    canDash = false;
                }
                else if (currentTime - keyPressTime > tapThreshold && canBlock)
                {
                    PerformBlock();
                }
            }
            else
            {
                _animator.SetBool("FORWARD_WALK", false);
                _animator.SetBool("GUARD", false);
            }
        }
        else
        {
            if (inputManager.IsKeyPressed(PlayerNumber, "L"))
            {
                if (currentTime - lastForwardDashTime < doubleTapInterval && canDash && isGrounded)
                {
                    if (!isCrouching && currentTime - uncrouchTimer > uncrouchDelay)
                    {
                        canWalk = false;
                        PerformFrontDash(new Vector2(-1, 0));  
                    }
                }
                else
                {
                    lastForwardDashTime = currentTime;
                }
            }
            else if (inputManager.IsKeyHeld(PlayerNumber, "L") && canWalk && isGrounded)
            {
                if (currentTime - lastDirectionalInputTime > directionalInputDelay)
                {
                    lastDirectionalInputTime = currentTime;
                    AddInput("F");
                }
                if (canWalk && !isCrouching && currentTime - uncrouchTimer > uncrouchDelay)
                {
                    canWalk = false;
                    PerformWalk(-1);
                }
                
            }
            else if (inputManager.IsKeyPressed(PlayerNumber, "R"))
            {
                keyPressTime = currentTime;
            }
            else if (inputManager.IsKeyHeld(PlayerNumber, "R") && canBlock)
            { 
                if (inputManager.IsKeyPressed(PlayerNumber, "U") && isGrounded)
                {
                    PerformBackwardJump(1);
                }
                else
                {
                    if (currentTime - lastThrowInputTime > throwInputDelay)
                    {
                        lastThrowInputTime = currentTime;
                        AddInput("THROW");
                    }
                    PerformBlock();
                }
            }
            else if (inputManager.IsKeyRelease(PlayerNumber, "R"))
            {
                if (currentTime - keyPressTime <= tapThreshold && canDash && isGrounded)
                {
                    var direction = new Vector2(1, 0);
                    PerformBackStep(direction);
                    canDash = false;
                }
                else if (currentTime - keyPressTime > tapThreshold && canBlock)
                {
                    PerformBlock();
                }
            }
            else
            {
                _animator.SetBool("FORWARD_WALK", false);
                _animator.SetBool("GUARD", false);
            }
        }

        if (inputManager.IsKeyPressed(PlayerNumber, "U"))
        {
            if (Mathf.Abs(rb.velocity.x) > 0 && isGrounded)
            {
                AddInput("U");
                PerformForwardJump(rb.velocity.x > 0 ? 1 : -1);
            }
            else if (isGrounded)
            {
                AddInput("U");
                PerformJump();
            }
        }
        
        if (inputManager.IsKeyPressed(PlayerNumber, "A") && canAttack) { AddInput("A"); }
        if (inputManager.IsKeyPressed(PlayerNumber, "B") && canAttack) { AddInput("B"); }
        if (inputManager.IsKeyPressed(PlayerNumber, "C") && canAttack) { AddInput("C"); }
    }
    public void SetActionName(string actionName) 
    {
        Action = (Action)System.Enum.Parse(typeof(Action), actionName);
    }
}
