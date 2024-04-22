using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public enum AttackType
{
    MOVE,
    HIGH,
    MIDDLE,
    LOW,
    THROW,
    
}
/// <summary>
/// This script inside HitBox you can control it damage type of damage directly inside animation
/// </summary>
public class HitBoxController : MonoBehaviour
{
    [FormerlySerializedAs("_characterController")] public ZenCharacterController zenCharacterController;
    public FightingController fightingController;
    public bool isActive = false;
    public string target = "";
    public bool isHit = false;
    public bool isThrow = false;
    public int damage;
    public int getEnergy;
    public int guardDamage;
    public int guardEnergy;
    public int giveEnergy;
    public int startAddEnergy;
    public GameObject hitEffect1;
    public GameObject hitEffect2;
    public GameObject hitEffect3;
    public GameObject hitEffect4;
    public AttackType attackType = AttackType.MIDDLE;
    public bool isDown;
    public int impactX;
    public int impactY;
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
            ComboChecker(zenCharacterController.currentCombo);

            // Handle collision, e.g., apply damage
            if (isHit)
            {
                other.GetComponent<ZenCharacterController>().TakeHit(zenCharacterController,giveEnergy,tempDamage,getEnergy,guardDamage,guardEnergy,attackType,isDown);
                if (isProjectile)
                {
                    fightingController.attackDeque.Remove(this.gameObject);
                    Destroy(this.gameObject);
                }
            }
            else if (isThrow)
            {
                other.GetComponent<ZenCharacterController>().TakeThrow(zenCharacterController,giveEnergy,tempDamage,getEnergy);
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

    public void CopyData(HitBoxController parent)
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
        fightingController = parent.fightingController;
        isProjectile = true;
    }
    public void SetData(MotionAttribute motionAttribute)
    {

        isHit = false;
        isThrow = false;

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
        startAddEnergy = motionAttribute.startAddEnergy;

        if (motionAttribute.attackType == AttackType.THROW)
        {
            isThrow = true;
        }
        else
        {
            isHit = true;
        }

        /*if (attackType != AttackType.THROW && motionAttribute.activeTime < 100)
        {
            isHit = true;
        }
        else if (attackType == AttackType.THROW)
        {
            isThrow = true;
        }*/
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
        fireballInstance.GetComponent<HitBoxController>().CopyData(this);
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
        fireballInstance.GetComponent<HitBoxController>().CopyData(this);
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
