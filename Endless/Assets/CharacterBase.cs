using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public enum EnemyStates {idle, attacking, retreating };

public class CharacterBase : MonoBehaviour
{
    public CharacterDataScriptableObject CharacterData;
    public event Action<float> OnHealthPctChanged = delegate {};


    protected NavMeshAgent navmeshAgent;

    protected float currentHealth, maxHealth;
    protected EnemyStates enemyState = EnemyStates.idle;

    public virtual void Awake(){
        currentHealth = CharacterData.Health;
        maxHealth = currentHealth;
        navmeshAgent = GetComponent<NavMeshAgent>();
    }

    public virtual void GetHit(float damage, Vector3 knockback) { 
        ModifyHealth(-(int)damage); // Healthbar tracks as int
        Debug.Log(currentHealth);
        if (currentHealth <= 0){
            Die();
        }
    }

    public void ModifyHealth(int amount) {
        currentHealth += amount;
        float currentHealthPct = (float)currentHealth / (float)maxHealth; 
        OnHealthPctChanged(currentHealthPct);
    }   
    public virtual void Die(){
        
        GameObject.Destroy(gameObject);
    }

}
