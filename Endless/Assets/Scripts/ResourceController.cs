using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceController : MonoBehaviour
{
    // Keeps track of level resources
    public enum ResourceTypes {wood, stone};
    public int initialWood, initialStone;

    private int wood, stone;

    // Start is called before the first frame update
    void Start()
    {
        // Load in initial level values
        wood = initialWood;
        stone = initialStone;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddResource(ResourceTypes resourceType, int amount)
    {
        switch (resourceType)
        {
            case ResourceTypes.wood:
                wood += amount;
                break;
            case ResourceTypes.stone:
                stone += amount;
                break;
        
        default:
            Debug.Log("Couldn't add that resource");
            break;
        }
    }

    public void RemoveResource(ResourceTypes resourceType, int amount)
    {
        switch (resourceType)
        {
            case ResourceTypes.wood:
                wood -= amount;
                break;
            case ResourceTypes.stone:
                stone -= amount;
                break;
        
        default:
            Debug.Log("Couldn't subtract that resource");
            break;
        }
    }
}
