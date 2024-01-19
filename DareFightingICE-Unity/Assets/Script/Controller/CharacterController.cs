using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class CharacterController : MonoBehaviour
{
    // Character Info
    [SerializeField]
    public bool PlayerNumber{ get; set; }

    public bool IsFront;
    public int Hp { get; set; }
    public int Energy { get; set; }
    
    // Action Flags
    public bool canWalk = true;
    public bool canBlock = true;
    public bool canAttack = true;
    public bool canDash = true;
    public bool isGuard = false;

    // Combo System
    private List<string> inputBuffer = new List<string>();
    private Dictionary<string, List<string>> combos = new Dictionary<string, List<string>>();
    private int MAX_BUFFER_SIZE = 6;
    
    // Character Control
    public float speed = 5.0f;
    public float jumpForce = 7.0f;
    public Transform groundCheck;
    public LayerMask groundLayer;
    public float groundCheckRadius = 0.2f;
    private Rigidbody2D rb;
    public bool isGrounded;
    public bool isCrouching = false;
    [SerializeField]private State _state = State.Stand;
    [SerializeField] private Animator _animator;
    private float _timer = 0f;
    private float lastDirectionalInputTime = 0f;
    private float directionalInputDelay = 0.1f;
    
    private float lastForwardDashTime = 0f;
    private float lastBackwardDashTime = 0f;
    private float doubleTapInterval = 0.3f;
    [SerializeField]private float dashforce = 0;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        IsFront = true;
        combos.Add("Combo1", new List<string> {"A","B","C" });
        combos.Add("STAND_FA", new List<string> {"F","A"});
        combos.Add("STAND_FB", new List<string> {"F","B"});
        combos.Add("STAND_A", new List<string> {"A"});
        combos.Add("STAND_B", new List<string> {"B"});
        // Add more combos as needed
    }

    void Update()
    {
        CheckState();
        HandleInput();
        CheckCombo();
        ResetBuffer();
    }

    private void HandleInput()
    {
        float currentTime = Time.time;
        // Movement input
        if (IsFront)
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                if (currentTime - lastForwardDashTime < doubleTapInterval)
                {
                    canWalk = false;
                    PerformFrontDash(new Vector2(1,0));
                }
                else
                {
                    lastForwardDashTime = currentTime;
                }
            }
            else if (Input.GetKey(KeyCode.RightArrow) && canWalk)
            {
                canWalk = false;
                if (currentTime - lastDirectionalInputTime > directionalInputDelay)
                {
                    lastDirectionalInputTime = currentTime;
                    AddInput("F");
                }
                PerformWalk(Input.GetAxis("Horizontal"));
                
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                //_animator.SetBool("FORWARD_WALK", false);
                if (currentTime - lastBackwardDashTime < doubleTapInterval)
                {
                    if (currentTime - lastBackwardDashTime < doubleTapInterval && canDash)
                    {
                        var direction = new Vector2(-1, 0); // Dash left
                        canDash = false;
                        PerformBackStep(direction);
                    }
                }
                else
                {
                    lastBackwardDashTime = currentTime;
                }
            }
            else if (Input.GetKey(KeyCode.LeftArrow) && canBlock)
            { 
                if (Input.GetKey(KeyCode.UpArrow) && isGrounded)
                {
                    PerformBackwardJump(-1);
                }
                else
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
                if (currentTime - lastForwardDashTime < doubleTapInterval)
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
                canWalk = false;
                if (currentTime - lastDirectionalInputTime > directionalInputDelay)
                {
                    lastDirectionalInputTime = currentTime;
                    AddInput("F");
                }
                PerformWalk(Input.GetAxis("Horizontal"));
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                if (currentTime - lastBackwardDashTime < doubleTapInterval)
                {
                    if (currentTime - lastBackwardDashTime < doubleTapInterval && canDash)
                    {
                        var direction = new Vector2(1, 0); // Dash left
                        canDash = false;
                        PerformBackStep(direction);
                    }
                }
                else
                {
                    lastBackwardDashTime = currentTime;
                }
                
            }else if (Input.GetKey(KeyCode.RightArrow) && canBlock)
            { 
                if (Input.GetKey(KeyCode.UpArrow) && isGrounded)
                {
                    PerformBackwardJump(1);
                }
                else
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
        
        
        
        // Combat input
        if (Input.GetKeyDown(KeyCode.Z) && canAttack) { AddInput("A"); }
        if (Input.GetKeyDown(KeyCode.X) && canAttack) { AddInput("B"); }
        if (Input.GetKeyDown(KeyCode.C) && canAttack) { AddInput("C"); }
    }
    

    private void AddInput(string action)
    {
        if (inputBuffer.Count >= MAX_BUFFER_SIZE)
        {
            inputBuffer.RemoveAt(0); // Remove the oldest input
        }
        inputBuffer.Add(action);
        Debug.Log("Input Added: " + action + ",\n Buffer: " + string.Join(", ", inputBuffer));

    }

    
    private void CheckState()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        if (_state != State.Down)
        {
            if (isGrounded)
            {
                if (isCrouching)
                {
                    SetState(State.Crouch);
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
        var sortedCombos = combos.OrderByDescending(c => c.Value.Count);

        foreach (var combo in sortedCombos)
        {
            if (inputBuffer.Count >= combo.Value.Count)
            {
                if (IsComboMatch(combo.Value))
                {
                    Debug.Log("Executing Combo: " + combo.Key);
                    ExecuteCombo(combo.Key);
                    inputBuffer.Clear();
                    break;
                }
            }
        }
    }

    private void ResetBuffer()
    {
        _timer += Time.deltaTime;
        if (_timer > 0.5)
        {
            _timer = 0;
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

    private void ExecuteCombo(string comboName)
    {
        print(comboName);
        PerformActtack(comboName);
    }

    private void SetState(State state)
    {
        _state = state;
    }
    
    private void PerformActtack(string actionname)
    {
        if (canAttack)
        {
            _animator.SetTrigger(actionname);
            canAttack = false;
            inputBuffer.Clear();
        }
    }
    
    private void PerformWalk(float direction)
    {
        Vector2 movement = new Vector2(direction * speed, rb.velocity.y);
        rb.velocity = movement;
        if (direction > 0)
            transform.localScale = new Vector3(4, 4, 4);

        // Trigger walking animation
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

    private void PerformBlock()
    {
        // Blocking logic and animation trigger
        _animator.SetBool("GUARD",true);
        // Set canBlock to false if the block animation should only be triggered once per key press
    }

    private void PerformJump()
    {
        // Jumping logic and animation trigger
        _animator.SetTrigger("JUMP");
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    }
    
    private void PerformForwardJump(float direction)
    {
        // Apply both upward and forward force
        Vector2 forwardJumpVelocity = new Vector2(direction * speed, jumpForce);
        rb.velocity = forwardJumpVelocity;
        _animator.SetTrigger("FORWARD_JUMP");
    }
    
    private void PerformBackwardJump(float direction)
    {
        // Apply both upward and forward force
        Vector2 forwardJumpVelocity = new Vector2(direction * speed, jumpForce);
        rb.velocity = forwardJumpVelocity;
    }
}
