using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StructureData", menuName = "ScriptableObjects/StructureData", order = 1)]
public class StructureDataScriptableObject : ScriptableObject
{
    public float Health;
    public float KnockBackInflictedUponAttacker;
    public float StuntimeInflictedUponAttacker;

}
