using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Serialization;


/// <summary>
/// This script inside HitBox you can control it damage type of damage directly inside animation
/// </summary>
public class SimHitBoxController : MonoBehaviour
{
    [FormerlySerializedAs("_characterController")] public SimCharacterController zenCharacterController;
    private bool isActive = false;
    public string target = "";
    private bool isHit = false;
    private bool isThrow = false;
    private int damage;
    private int getEnergy;
    private int guardDamage;
    private int guardEnergy;
    private int giveEnergy;
    private AttackType attackType = AttackType.MIDDLE;
    private bool isDown;
    private int impactX;
    private int impactY;
    public bool isProjectile;
    private int projDamage;
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
            //Call hit effect when character gets hit.
            int tempDamage = damage;
            if (zenCharacterController.currentCombo > 3)
            {
                tempDamage += (int)(5 * (4f / zenCharacterController.currentCombo));
            }

            // Handle collision, e.g., apply damage
            if (isHit)
            {
                other.GetComponent<SimCharacterController>().TakeHit(zenCharacterController,giveEnergy,tempDamage,getEnergy,guardDamage,guardEnergy,attackType,isDown);
                if (isProjectile)
                {
                    zenCharacterController.AttackDeque.Remove(this.gameObject);
                    Destroy(this.gameObject);
                }
            }
            else if (isThrow)
            {
                zenCharacterController.TakeThrow(zenCharacterController,giveEnergy,tempDamage,getEnergy);
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
        isThrow = false;
        damage = 0;
        isActive = false;
    }


    public void CopyData(SimHitBoxController parent)
    {
        damage = parent.projDamage;
        getEnergy = parent.getEnergy;
        guardDamage = parent.guardDamage;
        guardEnergy = parent.guardEnergy;
        giveEnergy = parent.giveEnergy;
        attackType = parent.attackType;
        isDown = parent.isDown;
        impactX = parent.impactX;
        impactY = parent.impactY;
        isHit = true;
        target = parent.target;
        zenCharacterController = parent.zenCharacterController;
        isProjectile = true;
    }
    public void SetData(MotionAttribute motionAttribute)
    {
        if (motionAttribute.activeTime >= 100)
        {
            projDamage = motionAttribute.hitDamage;
        }
        else
        {
            damage = motionAttribute.hitDamage;
        }
        getEnergy = motionAttribute.hitAddEnergy;
        guardDamage = motionAttribute.guardDamage;
        guardEnergy = motionAttribute.guardAddEnergy;
        giveEnergy = motionAttribute.giveEnergy;
        attackType = motionAttribute.attackType;
        isDown = motionAttribute.isDown;
        impactX = motionAttribute.impactX;
        impactY = motionAttribute.impactY;
        if (attackType != AttackType.THROW && motionAttribute.activeTime < 100)
        {
            isHit = true;
        }
        else if (attackType == AttackType.THROW)
        {
            isThrow = true;
        }
    }
    public GameObject SpawnSmallProjectile(Vector2 direction, float force)
    {
        // Instantiate the fireball at the position of the hitbox with the same rotation
        GameObject fireballInstance = Instantiate(smallFireball, transform.position, Quaternion.identity);

        // Get the Rigidbody2D component of the fireball
        Rigidbody2D rb = fireballInstance.GetComponent<Rigidbody2D>();
        fireballInstance.GetComponent<SimHitBoxController>().CopyData(this);
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
    
    public GameObject SpawnBigProjectile(Vector2 direction, float force)
    {
        // Instantiate the fireball at the position of the hitbox with the same rotation
        GameObject fireballInstance = Instantiate(largeFireball, transform.position, Quaternion.identity);

        // Get the Rigidbody2D component of the fireball
        Rigidbody2D rb = fireballInstance.GetComponent<Rigidbody2D>();
        fireballInstance.GetComponent<SimHitBoxController>().CopyData(this);
        fireballInstance.GetComponent<SimHitBoxController>().isDown = true;
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
