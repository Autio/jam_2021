using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    // A generic class for taking damage
    // Add this to anything that should take damage
    // And anything that should have a health bar
    private float current, max;
    public event Action<float> OnHealthPctChanged = delegate {};
    public void ModifyHealth(float amount) {
        current += amount;
        float currentHealthPct = current / max; 
        OnHealthPctChanged(currentHealthPct);
    }      

    public void SetCurrentHealth(float health)
    {
        current = (int)health;
    } 
    public float GetCurrentHealth()
    {
        return current;
    }
    public void SetMaxHealth(float health)
    {
        max = health;
    } 


}
