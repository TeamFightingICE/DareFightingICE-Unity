using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    /// <summary>
    /// Character Info contain information to send to FightingController
    /// Action Flags are hardcode bridge that connect animator with this script
    /// Combo System
    /// MAX_BUFFER_SIZE to control max input inside buffer when it exceed it will start remove first buffer
    /// allTrigger is to control animation when add more trigger inside animator please add those trigger here too
    /// Reset Buffer time is to clear buffer after player stop press button
    /// </summary>
    
    // Character Info
    public bool PlayerNumber{ get; set; }
    public bool IsFront { get; set; }
    public int Hp { get; set; }
    public int Energy { get; set; }
    public CharacterController otherPlayer{get; set; }
    
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
    private float lastInputTime;
    private float bufferResetDelay = 0.01f;
    
    // Character Control
    public float speed = 5.0f;
    public float jumpForce = 7.0f;
    public Transform groundCheck;
    public LayerMask groundLayer;
    public float groundCheckRadius = 0.2f;
    private Rigidbody2D rb;
    public bool isGrounded;
    public bool isCrouching = false;
    public State state = State.Stand;
    [SerializeField] private Animator _animator;
    private float _timer = 0f;
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
    
    [SerializeField]private float dashforce = 0;
    // HitBoxController
    [SerializeField] private HitBoxController leftHand;
    [SerializeField] private HitBoxController rightHand;
    [SerializeField] private HitBoxController leftFoot;
    [SerializeField] private HitBoxController rightFoot;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        combos.Add("F_D_DFA", new List<string> {"F","D","D","F","A"});
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
        if (PlayerNumber)
        {
            HandleInputP1();
        }
        else
        {
            HandleInputP2();
        }
        CheckCombo();
        ResetBuffer();

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
        }else
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
                if (Input.GetKey(KeyCode.UpArrow) && isGrounded)
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
                if (Input.GetKey(KeyCode.UpArrow) && isGrounded)
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
        // if (Input.GetKeyDown(KeyCode.M) && canAttack) { _animator.SetTrigger("GETHIT"); }
        // if (Input.GetKeyDown(KeyCode.N) && canAttack) { _animator.SetTrigger("GETKNOCK"); }
        // if (Input.GetKeyDown(KeyCode.B) && canAttack) { _animator.SetTrigger("GET_THROW"); }
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
                if (Input.GetKey(KeyCode.I) && isGrounded)
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
                if (Input.GetKey(KeyCode.I) && isGrounded)
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
        if (Input.GetKeyDown(KeyCode.T) && canAttack) { AddInput("A"); }
        if (Input.GetKeyDown(KeyCode.Y) && canAttack) { AddInput("B"); }
        if (Input.GetKeyDown(KeyCode.U) && canAttack) { AddInput("C"); }
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
                    _animator.SetBool("INAIR",false);
                }
                else
                {
                    SetState(State.Stand);
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
    private bool IsComboMatch(List<string> comboSequence)
    {
        for (int i = 0; i < comboSequence.Count; i++)
        {
            if (inputBuffer[inputBuffer.Count - comboSequence.Count + i] != comboSequence[i])
                return false;
        }
        return true;
    }
    
    private void ExecuteGivenCombo(string comboName,State _state)
    {
        string action = "";
        print(comboName);
        switch (_state)
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
        print(comboName);
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
        if (canAttack && !isGuard)
        {
            _animator.SetTrigger(actionname);
            canDash = false;
            canAttack = false;
            inputBuffer.Clear();
        }
    }
    
    private void PerformWalk(float direction)
    {
        Vector2 movement = new Vector2(direction * speed, rb.velocity.y);
        rb.velocity = movement;

        // Trigger walking animation
        _animator.SetBool("GUARD", false);
        _animator.SetBool("FORWARD_WALK", true);
    }

    private void PerformBackStep(Vector2 direction)
    {
        rb.velocity = direction * dashforce;
        canBlock = false;
        _animator.SetTrigger("BACK_STEP");
    }
    
    private void PerformFrontDash(Vector2 direction)
    {
        rb.velocity = direction * dashforce;
        canBlock = false;
        _animator.SetTrigger("DASH");
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
        _animator.SetBool("FORWARD_WALK", false);
        _animator.SetBool("GUARD",true);
        // Set canBlock to false if the block animation should only be triggered once per key press
    }

    private void PerformJump()
    {
        // Jumping logic and animation trigger
        if (canJump)
        {
           _animator.SetTrigger("JUMP");
            rb.velocity = new Vector2(rb.velocity.x, jumpForce); 
        }
    }
    
    private void PerformCrounch()
    {
        isCrouching = true;
        _animator.SetBool("CROUCH",true);
    }
    private void PerformForwardJump(float direction)
    {
        // Apply both upward and forward force
        if (canJump)
        {
            Vector2 forwardJumpVelocity = new Vector2(direction * speed, jumpForce);
            rb.velocity = forwardJumpVelocity;
            _animator.SetTrigger("FORWARD_JUMP");
        }
        
    }
    
    private void PerformBackwardJump(float direction)
    {
        // Apply both upward and forward force
        if (canJump)
        {
            Vector2 forwardJumpVelocity = new Vector2(direction * speed, jumpForce);
            rb.velocity = forwardJumpVelocity;
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
        leftHand.target = target;
        rightHand.target = target;
        leftFoot.target = target;
        rightFoot.target = target;
    }

    public void TakeHit(int damage)
    {
        Hp -= damage;
        _animator.SetTrigger("GETHIT");
    }

    public void TakeThorw(int damage)
    {
        Hp -= damage;
        _animator.SetTrigger("GET_THROW");
    }
}
