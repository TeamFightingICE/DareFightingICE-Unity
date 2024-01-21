using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class HitBoxController : MonoBehaviour
{
    public bool isActive = false;
    public string target = "";
    public bool isHit = false;
    public bool isThrow = false;
    public int damage;
    private void OnEnable()
    {
        Activate();
    }

    private void OnDisable()
    {
        Deactivate();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.LogWarning("Hit");
        if (isActive && other.gameObject.CompareTag(target))
        {
            // Handle collision, e.g., apply damage
            if (isHit)
            {
                other.GetComponent<CharacterController>().TakeHit(damage);
            }else if (isThrow)
            {
                other.GetComponent<CharacterController>().TakeThorw(damage);
            }
        }
    }

    public void Activate()
    {
        isActive = true;
    }

    public void Deactivate()
    {
        isHit = false;
        isThrow = false;
        damage = 0;
        isActive = false;
    }
}
