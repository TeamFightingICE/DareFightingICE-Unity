using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ReplayCharacterController : MonoBehaviour
{
    public bool PlayerNumber { get; set; }
    public bool IsFront { get; set; }
    public int Hp { get; set; }
    public int Energy { get; set; }
    public Action Action = Action.NEUTRAL;
    public Action PreAction = Action.NEUTRAL;
    public Animator animator;
    public TextAsset csvFile;
    public TextMeshPro PlayerNum;
    public int currentCombo = 0;
        // HitBoxController
    [SerializeField] public HitBoxControllerReplay leftHand;
    [SerializeField] public HitBoxControllerReplay rightHand;
    [SerializeField] public HitBoxControllerReplay leftFoot;
    [SerializeField] public HitBoxControllerReplay rightFoot;

    void Update ()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 || PreAction != Action)
        {
            //Debug.Log(Action.ToString());
            animator.Play(Action.ToString());
            PreAction = Action;
        }
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
}
