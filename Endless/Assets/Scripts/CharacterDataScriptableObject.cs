using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterData", menuName = "ScriptableObjects/CharacterData", order = 1)]
public class CharacterDataScriptableObject : ScriptableObject
{
    public float MovementSpeed;
    public float Health;
    public float Stamina;
    public float AttackStaminaCost;

    public float StaminaRechargeRate;
    public float HitDamage;
    public float StunTimeAfterBeingHit;
    //This is fucked up, comments per line should explain
    public float KnockBack; //this is the knockback we INFLICT upon hitting something
    public float KnockBackSpeed = 1f; //This is the knockback speed we are SUBJECT to when hit
    public float MaxDistanceToChasePlayer;
    public float MaxDistanceToChaseStructures;
    public float MaxPathedDistanceToChaseCrystal;
    public float AttackRadiusAsTurret;

}
