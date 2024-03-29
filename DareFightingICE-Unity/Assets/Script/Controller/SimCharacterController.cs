using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class SimCharacterController : MonoBehaviour
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
    public SimCharacterController otherPlayer { get; set; }
    
    public List<GameObject> AttackDeque;

    public TextAsset csvFile;
    
    // Action Flags
    public bool canWalk = true;
    public bool canBlock = true;
    public bool canAttack = true;
    public bool canDash = true;
    public bool isGuard = false;
    public bool canJump = true;

    // Combo System
    private List<string> inputBuffer = new List<string>();
    private Dictionary<string, List<string>> combos = new Dictionary<string, List<string>>();
    private int MAX_BUFFER_SIZE = 7;
    private string[] allTriggers = { "JUMP", "STAND_B", "STAND_A","STAND_FA","STAND_FB","GETHIT","GETKNOCK","GET_THROW","AIR_A","AIR_B","AIR_FA","AIR_FB","BACK_STEP","DASH","FORWARD_JUMP","STAND_THROW_A","STAND_THROW_B","CROUCH_A","CROUCH_B","CROUCH_FA","CROUCH_FB","AIR_DA","AIR_DB","AIR_UA","AIR_UB" };
    private string[] comboTriggers = {"STAND_F_D_DFA","STAND_F_D_DFB","STAND_D_DB_BA","STAND_D_DB_BB","STAND_D_DF_FA","STAND_D_DF_FB","STAND_D_DF_FC","AIR_D_DF_FB","AIR_F_D_DFA","AIR_F_D_DFB","AIR_D_DB_BA","AIR_D_DB_BB","AIR_D_DF_FA"};
    private float lastInputTime;
    private float bufferResetDelay = 0f;
    
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
    [SerializeField]private float jumpDelay = 0.1f;
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
    [SerializeField] private SimHitBoxController leftHand;
    [SerializeField] private SimHitBoxController rightHand;
    [SerializeField] private SimHitBoxController leftFoot;
    [SerializeField] private SimHitBoxController rightFoot;
    
    // SoundControl
  

    private GameSetting gameSetting;
    private SimInputManager inputManager;
    void Start()
    {
        gameSetting = GameSetting.Instance;
        inputManager = SimInputManager.Instance;

        rb = GetComponent<Rigidbody2D>();
        
        combos.Add("F_D_DFA", new List<string> {"F","D","D","F","A"});
        combos.Add("F_D_DFB", new List<string> {"F","D","D","F","B"});
        combos.Add("D_DB_BA", new List<string> {"D","D","B","B","A"});
        combos.Add("D_DB_BB", new List<string> {"D","D","B","B","B"});
        combos.Add("D_DF_FA", new List<string> {"D","D","F","F","A"});
        combos.Add("D_DF_FB", new List<string> {"D","D","F","F","B"});
        combos.Add("D_DF_FC", new List<string> {"D","D","F","F","C"});
        
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
        CheckState();
        CheckCombo();
        ResetBuffer();
        UpdateComboTimer();
    }
     public void HandleAIInput(Key aiInput)
    {
       
        float currentTime = Time.time;
        inputManager.SetInput(PlayerNumber, aiInput);
        Key key = inputManager.GetInput(PlayerNumber);
        if (key.D)
        {
            if (currentTime - lastCrouchInputTime > CrouchInputDelay && !isCrouching)
            {
                lastCrouchInputTime = currentTime;
                Debug.Log("Action in");
                AddInput("D");
            }
            if (isGrounded)
            {
                //Debug.Log("Action Performed");
                PerformCrounch();
            }
        }
        else
        {
            isCrouching = false;
            //_animator.SetBool("CROUCH", false);
        }
        // Movement input
        if (IsFront)
        {
            
            if (inputManager.IsKeyPressed(PlayerNumber, "R"))
            {
              
                if (currentTime - lastForwardDashTime < doubleTapInterval && canDash)
                {
                    canWalk = false;
                    PerformFrontDash(new Vector2(1, 0));
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

                if (canWalk)
                {
                    canWalk = false;
                    PerformWalk(1);
                }
            }
            else if (inputManager.IsKeyPressed(PlayerNumber, "L"))
            {
                if (PlayerNumber)
                      Debug.Log("Action Player1");
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
            else if (inputManager.IsKeyRelease(PlayerNumber, "L"))
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
               
                //_animator.SetBool("FORWARD_WALK", false);
                //_animator.SetBool("GUARD", false);
            }
        }
        else
        {
          
            if (inputManager.IsKeyPressed(PlayerNumber, "L"))
            {
                   
                if (currentTime - lastForwardDashTime < doubleTapInterval && canDash)
                {
                    canWalk = false;
                    PerformFrontDash(new Vector2(-1, 0));
                }
                else
                {
                    lastForwardDashTime = currentTime;
                }
            }
            else if (inputManager.IsKeyHeld(PlayerNumber, "L") && canWalk)
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
                //_animator.SetBool("FORWARD_WALK", false);
                //_animator.SetBool("GUARD", false);
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
        CheckState();
        CheckCombo();
        ResetBuffer();
        UpdateComboTimer();
        
    }
    
   

    private void AddInput(string action)
    {
        lastInputTime = Time.time;
        if (inputBuffer.Count >= MAX_BUFFER_SIZE)
        {
            inputBuffer.RemoveAt(0);
        }
        inputBuffer.Add(action);
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
                    //_animator.SetBool("INAIR",false);
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
                    //_animator.SetBool("INAIR",false);
                }
            }
            else
            { 
                SetState(State.Air);
                //_animator.SetBool("INAIR",true);
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

        var sortedCombos = combos.OrderByDescending(c => c.Value.Count);

        foreach (var combo in sortedCombos)
        {
            if (inputBuffer.Count >= combo.Value.Count && IsComboMatch(combo.Value))
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
    }

    public void ResetSpacialCombo()
    {
        foreach (var trigger in comboTriggers)
        {
            //_animator.ResetTrigger(trigger);
        }
    }
    private bool IsComboMatch(List<string> comboSequence)
    {
        for (int i = 0; i < comboSequence.Count; i++)
        {
            if (inputBuffer[inputBuffer.Count - comboSequence.Count + i] != comboSequence[i])
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
        
        int requireEnergy = MotionManager.Instance.GetStartGiveEnergyForMotion("zen",actionname);
        if (canAttack && !isGuard)
        {
            if (Energy >= Mathf.Abs(requireEnergy) && requireEnergy != 1)
            {
                SetMotionData(MotionManager.Instance.GetMotionAttributes("zen",actionname));
                Energy += requireEnergy;
                //_animator.SetTrigger(actionname);
                canDash = false;
                canAttack = false;
                inputBuffer.Clear();
                
            }
            else
            {
                Debug.LogWarning("Not Enough Energy");
                inputBuffer.Clear();
            }
        }
    }
    
    private void PerformWalk(float direction)
    {

        rb.mass = 8;
        Vector2 movement = new Vector2(direction * speed, rb.velocity.y);
        rb.velocity = movement;

        // Trigger walking animation
        //_animator.SetBool("GUARD", false);
        //_animator.SetBool("FORWARD_WALK", true);
        
    }

    private void PerformBackStep(Vector2 direction)
    {
        rb.velocity = direction * dashforce;
        canBlock = false;
        //_animator.SetTrigger("BACK_STEP");
    }
    
    private void PerformFrontDash(Vector2 direction)
    {
        rb.velocity = direction * dashforce;
        canBlock = false;
        //_animator.SetTrigger("DASH");
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
            //_animator.ResetTrigger(trigger);
        }
    }
    private void PerformBlock()
    {
        // Blocking logic and animation trigger
        //_animator.SetBool("FORWARD_WALK", false);
        //_animator.SetBool("GUARD",true);
        // Set canBlock to false if the block animation should only be triggered once per key press
    }

    private void PerformJump()
    {
       
        // Jumping logic and animation trigger
        if (canJump)
        {
           //_animator.SetTrigger("JUMP");
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
                    if(IsFront) 
                    {
                        Vector2 movement = new Vector2(-1 * speed, rb.velocity.y);
                        rb.velocity = movement;
                    }
                    else 
                    {
                        Vector2 movement = new Vector2(-1 * speed, rb.velocity.y);
                        rb.velocity = movement;
                    }
                }
            }
        }

    
    private void PerformCrounch()
    {
      
        if (!isCrouching)
        {
        }
        isCrouching = true;
        //_animator.SetBool("CROUCH",true);
        
    }

    public void ResetCounch()
    {
    }
    private void PerformForwardJump(float direction)
    {
        // Apply both upward and forward force
        if (canJump)
        {
            Vector2 forwardJumpVelocity = new Vector2(direction * speed, jumpForce);
            rb.velocity = forwardJumpVelocity;
            //_animator.SetTrigger("FORWARD_JUMP");
            jumpTimer = jumpDelay;
            canJump = false;
            Debug.Log("Action called");
        }
        
    }
    
    private void PerformBackwardJump(float direction)
    {
        // Apply both upward and forward force
        if (canJump)
        {
            Vector2 forwardJumpVelocity = new Vector2(direction * speed, jumpForce);
            rb.velocity = forwardJumpVelocity;
            //_animator.SetTrigger("FORWARD_JUMP");
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
        Vector2 forwardJumpVelocity = new Vector2(direction * speed, jumpForce);
        rb.velocity = forwardJumpVelocity;
    }

    public void SetTarget(string target)
    {
        leftHand.zenCharacterController = this;
        rightHand.zenCharacterController = this;
        leftFoot.zenCharacterController = this;
        rightFoot.zenCharacterController = this;
        
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
    public void TakeHit(SimCharacterController attacker,int getHitEnergy,int damage,int getEnegy,int guardDamage,int guardEnergy,AttackType type,bool isDown)
    {
        switch (state)
        {
            case State.Air:
                if (isGuard && (type == AttackType.HIGH || type == AttackType.MIDDLE))
                {
                    Hp -= guardDamage;
                    AddEnergy(getHitEnergy);
                    attacker.GetReward(guardEnergy);
                    //_animator.SetTrigger("GETHIT");
                }
                else
                {
                    if (isDown)
                    {
                        //_animator.SetTrigger("GETKNOCK");
                    }
                    else
                    {
                        //_animator.SetTrigger("GETHIT");
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
                    //_animator.SetTrigger("GETHIT");
                }
                else
                {
                    if (isDown)
                    {
                        //_animator.SetTrigger("GETKNOCK");
                    }
                    else
                    {
                        //_animator.SetTrigger("GETHIT");
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
                    //_animator.SetTrigger("GETHIT");
                }
                else
                {
                    if (isDown)
                    {
                        //_animator.SetTrigger("GETKNOCK");
                    }
                    else
                    {
                        //_animator.SetTrigger("GETHIT");
                    }
                    Hp -= damage;
                    AddEnergy(getHitEnergy);
                    attacker.GetReward(getEnegy);
                }
                break;
        }
        
    }

    public void TakeThrow(SimCharacterController attacker,int giveEnergy,int damage,int getEnegy)
    {
        if (isGrounded)
        {
            Hp -= damage;
            Energy += giveEnergy;
            attacker.GetReward(getEnegy);
            //_animator.SetTrigger("GET_THROW");
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
        if (IsFront)
        {
            fireballDirection = new Vector2(1, 0);
        }
        else
        {
            fireballDirection = new Vector2(0, 1);
        }
        float fireballForce = 10f;
        AttackDeque.Add(leftHand.SpawnBigProjectile(fireballDirection,fireballForce));
    }

    public void SpawnSmallFireball()
    {
        Vector2 fireballDirection = new Vector2(1, 0);
        if (IsFront)
        {
            fireballDirection = new Vector2(1, 0);
        }
        else
        {
            fireballDirection = new Vector2(0, 1);
        }
        float fireballForce = 10f;
        AttackDeque.Add(leftHand.SpawnSmallProjectile(fireballDirection,fireballForce));
    }

}
