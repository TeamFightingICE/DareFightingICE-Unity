using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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


    void Update ()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 || PreAction != Action)
        {
            animator.Play(Action.ToString());
            PreAction = Action;
        }
    }
}
