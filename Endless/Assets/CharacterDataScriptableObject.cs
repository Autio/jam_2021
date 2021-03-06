using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterData", menuName = "ScriptableObjects/CharacterData", order = 1)]
public class CharacterDataScriptableObject : ScriptableObject
{
    public float MovementSpeed;
    public float Health;
    public float Stamina;
    public float StaminaRechargeRate;
    public float HitDamage;
    public float KnockBack;

}
