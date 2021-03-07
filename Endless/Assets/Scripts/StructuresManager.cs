using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StructuresManager : Singleton<StructuresManager>
{
    public List<Structure> Structures;
    void Awake(){
        Structures = GameObject.FindObjectsOfType<Structure>().ToList();
    }
    public Structure GetClosestStructure(Vector3 fromPosition){
        float closest = float.PositiveInfinity;
        Structure closestStructure = null;
        foreach (var structure in Structures)
        {
            float currentDistance = Vector3.Distance(fromPosition, structure.transform.position);
            if ( currentDistance< closest){
                closest = currentDistance;
                closestStructure = structure;
            }
        }
        return closestStructure;
    }
}
