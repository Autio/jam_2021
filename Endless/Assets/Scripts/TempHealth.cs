using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempHealth : MonoBehaviour
{
    
    [SerializeField]
    private int maxHealth = 100;
    private int currentHealth;

    private float timer = 3.0f;


    public event Action<float> OnHealthPctChanged = delegate {};
    // Start is called before the first frame update
    
    void OnEnable()
    {
        currentHealth = maxHealth;
    }

    public void ModifyHealth(int amount)
    {
        currentHealth += amount;
        float currentHealthPct = (float)currentHealth / (float)maxHealth; 
        OnHealthPctChanged(currentHealthPct);
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        if(timer < 0)
        {
            ModifyHealth(-30);
            timer = 1f;
        }
    }
}
