using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Serialization;


/// <summary>
/// This script inside HitBox you can control it damage type of damage directly inside animation
/// </summary>
public class HitBoxControllerReplay : MonoBehaviour
{
    [FormerlySerializedAs("_characterController")] public ReplayCharacterController zenCharacterController;
    public bool isActive = false;
    public string target = "";
    public bool isHit = false;
    public GameObject hitEffect1;
    public GameObject hitEffect2;
    public GameObject hitEffect3;
    public GameObject hitEffect4;

    public bool isProjectile;
    [SerializeField] private GameObject smallFireball;
    [SerializeField] private GameObject largeFireball;
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
        if (isActive && other.gameObject.CompareTag(target))
        {

            ComboChecker(zenCharacterController.currentCombo);

            // Handle collision, e.g., apply damage
            if (isHit)
            {
                if (isProjectile)
                {

                    Destroy(this.gameObject);
                }
            }

        }
        else if (other.gameObject.name == "Border" && isProjectile)
        {
            Destroy(this.gameObject);
        }
    }

    public void Activate()
    {
        isActive = true;
    }

    public void Deactivate()
    {
        isHit = false;
        isActive = false;
    }

    private void ComboChecker(int comboCount)
    {
        switch (comboCount)
        {
            case 1:
                Instantiate(hitEffect1, this.transform,false);
                break;
            case 2:
                Instantiate(hitEffect2, this.transform,false);
                break;
            case 3:
                Instantiate(hitEffect3, this.transform,false);
                break;
            case >3:
                Instantiate(hitEffect4, this.transform,false);
                break;
            default:
                Instantiate(hitEffect1, this.transform,false);
                break;
        }
    }


    public GameObject SpawnSmallProjectile(Vector2 direction, float force,bool flipSprite)
    {
        // Instantiate the fireball at the position of the hitbox with the same rotation
        GameObject fireballInstance = Instantiate(smallFireball, transform.position, Quaternion.identity);

        if (flipSprite)
        {
            fireballInstance.transform.localScale = new Vector3(-1 * Mathf.Sign(direction.x), 1, 1);
        }

        // Get the Rigidbody2D component of the fireball
        Rigidbody2D rb = fireballInstance.GetComponent<Rigidbody2D>();
        // Check if the Rigidbody2D component exists to avoid null reference errors
        if (rb != null)
        {
            // Add force to the fireball to propel it in the specified direction
            rb.AddForce(direction * force, ForceMode2D.Impulse);
            return fireballInstance;
        }
        else
        {
            Debug.LogError("Spawned fireball does not have a Rigidbody2D component.");
            return null;
        }
    }
    
    public GameObject SpawnBigProjectile(Vector2 direction, float force, bool flipSprite)
    {
        // Instantiate the fireball at the position of the hitbox with the same rotation
        GameObject fireballInstance = Instantiate(largeFireball, transform.position, Quaternion.identity);

        if (flipSprite)
        {
            fireballInstance.transform.localScale = new Vector3(-1 * Mathf.Sign(direction.x), 1, 1);
        }

        // Get the Rigidbody2D component of the fireball
        Rigidbody2D rb = fireballInstance.GetComponent<Rigidbody2D>();
        fireballInstance.GetComponent<HitBoxController>().isDown = true;
        // Check if the Rigidbody2D component exists to avoid null reference errors
        if (rb != null)
        {
            // Add force to the fireball to propel it in the specified direction
            rb.AddForce(direction * force, ForceMode2D.Impulse);
            return fireballInstance;
        }
        else
        {
            Debug.LogError("Spawned fireball does not have a Rigidbody2D component.");
            return fireballInstance;
        }
    }
    
}
