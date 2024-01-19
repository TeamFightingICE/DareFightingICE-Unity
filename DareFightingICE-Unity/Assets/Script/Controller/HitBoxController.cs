using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBoxController : MonoBehaviour
{
    public bool isActive = false;
    private bool hasHit = false;

    void OnTriggerEnter(Collider other)
    {
        if (isActive && !hasHit && other.CompareTag("Enemy"))
        {
            hasHit = true;
            // Handle collision, e.g., apply damage
            Debug.Log("Hit detected on enemy with " + gameObject.name);
        }
    }

    public void Activate()
    {
        isActive = true;
        hasHit = false;
    }

    public void Deactivate()
    {
        isActive = false;
    }
}
