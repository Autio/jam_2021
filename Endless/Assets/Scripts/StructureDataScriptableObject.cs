using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StructureData", menuName = "ScriptableObjects/StructureData", order = 1)]
public class StructureDataScriptableObject : ScriptableObject
{
    public float Health;
    public enum BuildingType {turret, wall, other};
    public BuildingType Type;
    // How much of an incline can the building be on and still be accepted 
    public float maxIncline;
    public float KnockBackInflictedUponAttacker;
    public float StuntimeInflictedUponAttacker;
    public bool IsTurret = false;
    public float AttackRadiusAsTurret;
    public float AttackRateAsTurret;

    public float AttackDamageAsTurret; 
}
