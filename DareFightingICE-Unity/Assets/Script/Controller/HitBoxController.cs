using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
/// <summary>
/// This script inside HitBox you can control it damage type of damage directly inside animation
/// </summary>
public class HitBoxController : MonoBehaviour
{
    public CharacterController _characterController;
    public bool isActive = false;
    public string target = "";
    public bool isHit = false;
    public bool isThrow = false;
    public int damage;
    public int getEnergy;
    public int guardDamage;
    public int guardEnergy;
    public int giveEnergy;
    public GameObject hitEffect1;
    public GameObject hitEffect2;
    public GameObject hitEffect3;
    public GameObject hitEffect4;
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
            //Call hit effect when character gets hit.
            int tempDamage = damage;
            print(_characterController.currentCombo);
            if (_characterController.currentCombo > 3)
            {
                tempDamage += (int)(5 * (4f / _characterController.currentCombo));
                print(tempDamage);
            }
            ComboChecker(_characterController.currentCombo);

            // Handle collision, e.g., apply damage
            if (isHit)
            {
                _characterController.GetReward(giveEnergy);
                other.GetComponent<CharacterController>().TakeHit(tempDamage,getEnergy,guardDamage,guardEnergy);
            }else if (isThrow)
            {
                _characterController.GetReward(giveEnergy);
                other.GetComponent<CharacterController>().TakeThorw(tempDamage,getEnergy);
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
}
