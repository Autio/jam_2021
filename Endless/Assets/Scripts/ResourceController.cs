using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum ResourceType {stone, wood};

public class ResourceController : Singleton<ResourceController>
{
    // Keeps track of level resources
    public int initialWood, initialStone;

    [SerializeField] TextMeshProUGUI stoneText, woodText;
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
        stoneText.text = stone.ToString();
    }

    public int GetWood()
    {
        return wood;
    }
    public int GetStone()
    {
        return stone;
    }

    public void AddResource(ResourceType resourceType, int amount)
    {
        Debug.Log(resourceType);
        switch (resourceType)
        {
            case ResourceType.wood:
                wood += amount;
                break;
            case ResourceType.stone:
                stone += amount;
                break;
        
        default:
            Debug.Log("Couldn't add that resource");
            break;
        }
        UpdateResourceGUI();

    }

    public void RemoveResource(ResourceType resourceType, int amount)
    {
        switch (resourceType)
        {
            case ResourceType.wood:
                wood -= amount;
                break;
            case ResourceType.stone:
                stone -= amount;
                break;
        
        default:
            Debug.Log("Couldn't subtract that resource");
            break;
        }

        UpdateResourceGUI();
    }

    public void PayForBuilding(StructureDataScriptableObject StructureData)
    {
        if(StructureData.ResourceCosts.wood > 0)
        {
            RemoveResource(ResourceType.wood, StructureData.ResourceCosts.wood);
        }
        if(StructureData.ResourceCosts.stone > 0)
        {
            RemoveResource(ResourceType.stone, StructureData.ResourceCosts.stone);
        }
    }

    // Render Resources in the GUI
    public void UpdateResourceGUI()
    {
        stoneText.text = stone.ToString();
        woodText.text = wood.ToString();
    }
}
