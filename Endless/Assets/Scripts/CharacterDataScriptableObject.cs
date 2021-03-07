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
    public float KnockBack;
    public float MaxDistanceToChasePlayer;
    public float MaxDistanceToChaseStructures;

}
