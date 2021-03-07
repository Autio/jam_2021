using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Structure : MonoBehaviour
{
    void Awake()
    {
        StructuresManager.Instance.AddNewStructure(this);
    }

    void Update()
    {
        
    }
}
