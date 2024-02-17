using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotionAttribute
{
    public int hitDamage;
    public int guardDamage;
    public int startAddEnergy;
    public int hitAddEnergy;
    public int guardAddEnergy;
    public int giveEnergy;
    public int impactX;
    public int impactY;
    public AttackType attackType;
    public bool isDown;
    public int activeTime;

    public MotionAttribute()
    {
        
    }

    public MotionAttribute(MotionAttribute other)
    {
        this.hitDamage = other.hitDamage;
        this.guardDamage = other.guardDamage;
        this.startAddEnergy = other.startAddEnergy;
        this.hitAddEnergy = other.hitAddEnergy;
        this.guardAddEnergy = other.guardAddEnergy;
        this.giveEnergy = other.giveEnergy;
        this.impactX = other.impactX;
        this.impactY = other.impactY;
        this.attackType = other.attackType;
        this.isDown = other.isDown;
        this.activeTime = other.activeTime;
    }
}
