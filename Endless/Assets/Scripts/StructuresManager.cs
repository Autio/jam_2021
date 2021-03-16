using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class StructuresManager : Singleton<StructuresManager>
{
    public List<Structure> Structures;
    [SerializeField] TextMeshProUGUI buildingCostText; 
    // Structure types the player can build
    public List<GameObject> AllowedStructures;

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

    public void AddNewStructure(Structure structure){
        if (!Structures.Contains(structure)){
            Structures.Add(structure);
        }
    }

    public void RemoveDeadStructure(Structure deadStructure){
        Structures.Remove(deadStructure);
    }

    public void HandleNewObject()
    {


    }


    void Update() 
    {
    }

    // Should GUI be handled in a separate controller entirely? 
    // Shows the costs of the building you are wanting to build
    public bool CanAffordToBuild(StructureDataScriptableObject StructureData)
    {
        if(StructureData.ResourceCosts.wood > ResourceController.Instance.GetWood())
        {
            return false;
        }
        if(StructureData.ResourceCosts.stone > ResourceController.Instance.GetStone())
        {
            return false;
        }

        return true;
        
        
    }

    public void ShowBuildingCosts(StructureDataScriptableObject StructureData)
    {
        string woodCost = StructureData.ResourceCosts.wood > 0 ? StructureData.ResourceCosts.wood + " wood\n" : "";
        string stoneCost = StructureData.ResourceCosts.stone > 0 ? StructureData.ResourceCosts.stone + " stone\n" : "";

        buildingCostText.text = "Cost:\n" + woodCost + stoneCost;
    }

    public void ClearBuildingCosts()
    {
        buildingCostText.text = "";
    }


}
