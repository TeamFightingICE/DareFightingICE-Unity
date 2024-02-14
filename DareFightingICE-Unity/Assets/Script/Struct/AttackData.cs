using DareFightingICE.Grpc.Proto;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackData
{
	public HitArea SettingHitArea { get; set; }

	/**
	 * The absolute value of the horizontal speed of the attack hit box (zero
	 * means the attack hit box will track the character).
	 */
	public int SettingSpeedX { get; set; }

	/**
	 * The absolute value of the vertical speed of the attack hit box (zero
	 * means the attack hit box will track the character).
	 */
	public int SettingSpeedY { get; set; }

	/**
	 * The HitArea's information of this attack hit box in the current frame.
	 *
	 * @see HitArea
	 */
	public HitArea CurrentHitArea { get; set; }

	/**
	 * The number of frames since this attack was used.
	 */
	public int CurrentFrame { get; set; }

	/**
	 * The player side's flag.<br>
	 * {@code True} if the character is P1, or {@code false} if P2.
	 */
	public bool PlayerNumber { get; set; }

	/**
	 * The horizontal speed of the attack hit box (minus when moving left and
	 * plus when moving right).
	 */
	public int SpeedX { get; set; }

	/**
	 * The vertical speed of the attack hit box (minus when moving up and plus
	 * when moving down).
	 */
	public int SpeedY { get; set; }

	/**
	 * The number of frames in Startup.
	 *
	 * @see MotionData#attackStartUp
	 */
	public int StartUp { get; set; }

	/**
	 * The number of frames in Active.
	 *
	 * @see MotionData#attackActive
	 */
	public int Active { get; set; }

	/**
	 * The damage value to the unguarded opponent hit by this skill.
	 */
	public int HitDamage { get; set; }

	/**
	 * The damage value to the guarded opponent hit by this skill.
	 */
	public int GuardDamage { get; set; }

	/**
	 * The value of the energy added to the character when it uses this skill.
	 */
	public int StartAddEnergy { get; set; }

	/**
	 * The value of the energy added to the character when this skill hits the
	 * opponent.
	 */
	public int HitAddEnergy { get; set; }

	/**
	 * The value of the energy added to the character when this skill is blocked
	 * by the opponent.
	 */
	public int GuardAddEnergy { get; set; }

	/**
	 * The value of the energy added to the opponent when it is hit by this
	 * skill.
	 */
	public int GiveEnergy { get; set; }

	/**
	 * The change in the horizontal speed of the opponent when it is hit by this
	 * skill.
	 */
	public int ImpactX { get; set; }

	/**
	 * The change in the vertical speed of the opponent when it is hit by this
	 * skill.
	 */
	public int ImpactY { get; set; }

	/**
	 * The number of frames that the guarded opponent needs to resume to his
	 * normal status after being hit by this skill.
	 */
	public int GiveGuardRecov { get; set; }

	/**
	 * The value of the attack type: 1 = high, 2 = middle, 3 = low, 4 = throw.
	 */
	public int AttackType { get; set; }

	/**
	 * The flag whether this skill can push down the opponent when hit.<br>
	 * {@code true} if this skill can push down, {@code false} otherwise.
	 */
	public bool DownProp { get; set; }

	/**
	 * The flag whether this skill is projectile or not.<br>
	 * {@code true} if this skill is projectile, {@code false} otherwise.
	 */
	public bool IsProjectile { get; set; }

	/**
	 * The class constructor.
	 */
	public AttackData()
	{
		this.SettingHitArea = new HitArea();
		this.SettingSpeedX = 0;
		this.SettingSpeedY = 0;
		this.CurrentHitArea = new HitArea();
		this.CurrentFrame = -1;
		this.PlayerNumber = true;
		this.SpeedX = 0;
		this.SpeedY = 0;
		this.StartUp = 0;
		this.Active = 0;
		this.HitDamage = 0;
		this.GuardDamage = 0;
		this.StartAddEnergy = 0;
		this.HitAddEnergy = 0;
		this.GuardAddEnergy = 0;
		this.GiveEnergy = 0;
		this.ImpactX = 0;
		this.ImpactY = 0;
		this.GiveGuardRecov = 0;
		this.AttackType = 0;
		this.DownProp = false;
		this.IsProjectile = false;
	}

	public AttackData(AttackData other)
	{
		this.SettingHitArea = new HitArea(other.SettingHitArea);
		this.SettingSpeedX = other.SettingSpeedX;
		this.SettingSpeedY = other.SettingSpeedY;
		this.CurrentHitArea = new HitArea(other.CurrentHitArea);
		this.CurrentFrame = other.CurrentFrame;
		this.PlayerNumber = other.PlayerNumber;
		this.SpeedX = other.SpeedX;
		this.SpeedY = other.SpeedY;
		this.StartUp = other.StartUp;
		this.Active = other.Active;
		this.HitDamage = other.HitDamage;
		this.GuardDamage = other.GuardDamage;
		this.StartAddEnergy = other.StartAddEnergy;
		this.HitAddEnergy = other.HitAddEnergy;
		this.GuardAddEnergy = other.GuardAddEnergy;
		this.GiveEnergy = other.GiveEnergy;
		this.ImpactX = other.ImpactX;
		this.ImpactY = other.ImpactY;
		this.GiveGuardRecov = other.GiveGuardRecov;
		this.AttackType = other.AttackType;
		this.DownProp = other.DownProp;
		this.IsProjectile = other.IsProjectile;
	}

	public GrpcAttackData ToProto()
	{
		return new GrpcAttackData
		{
			SettingHitArea = SettingHitArea.ToProto(),
			SettingSpeedX = SettingSpeedX,
			SettingSpeedY = SettingSpeedY,
			CurrentHitArea = CurrentHitArea.ToProto(),
			CurrentFrame = CurrentFrame,
			PlayerNumber = PlayerNumber,
			SpeedX = SpeedX,
			SpeedY = SpeedY,
			StartUp = StartUp,
			Active = Active,
			HitDamage = HitDamage,
			GuardDamage = GuardDamage,
			StartAddEnergy = StartAddEnergy,
			HitAddEnergy = HitAddEnergy,
			GuardAddEnergy = GuardAddEnergy,
			GiveEnergy = GiveEnergy,
			ImpactX = ImpactX,
			ImpactY = ImpactY,
			GiveGuardRecov = GiveGuardRecov,
			AttackType = AttackType,
			DownProp = DownProp,
			IsProjectile = IsProjectile,
		};
	}
}
