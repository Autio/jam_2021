using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loot : MonoBehaviour
{
    // Loot contains the resource the player can pick up 
    // Once they have smashed the resource item
    // Maybe later on something more like equipment
    public ResourceController.ResourceTypes resourceType;
    public int amount;

    // Add loot amount to the controller
    void CollectLoot()
    {
        ResourceController.Instance.AddResource(resourceType, amount);
        Destroy(gameObject);
    }

}
