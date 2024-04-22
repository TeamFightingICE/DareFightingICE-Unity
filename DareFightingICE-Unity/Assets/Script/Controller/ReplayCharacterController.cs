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

    void Update ()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 || PreAction != Action)
        {
            //Debug.Log(Action.ToString());
            animator.Play(Action.ToString());
            PreAction = Action;
        }
    }
    
}
