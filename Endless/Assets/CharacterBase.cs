using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public enum EnemyStates {idle, attacking, retreating, stunned };

public class CharacterBase : MonoBehaviour
{
    public CharacterDataScriptableObject CharacterData;
    public event Action<float> OnHealthPctChanged = delegate {};
    public event Action<float> OnStaminaPctChanged = delegate {};



    protected NavMeshAgent navmeshAgent;

    protected float currentHealth, maxHealth, currentStamina, maxStamina, staminaRechargeRate;
    protected EnemyStates enemyState = EnemyStates.idle;

    protected float stunStartTime;

    public virtual void Awake(){
        currentHealth = CharacterData.Health;
        maxHealth = currentHealth;
        currentStamina = CharacterData.Stamina;
        maxStamina = currentStamina;
        staminaRechargeRate = CharacterData.StaminaRechargeRate;

        navmeshAgent = GetComponent<NavMeshAgent>();
    }

    public virtual void GetHit(float damage, Vector3 knockback) { 
        ModifyHealth(-(int)damage); // Healthbar tracks as int
        if (currentHealth <= 0){
            Die();
            return;
        }
        if (CharacterData.StunTimeAfterBeingHit >0){
            stunStartTime = Time.time;
            enemyState = EnemyStates.stunned;
        }
        
    }

    public void ModifyHealth(int amount) {
        currentHealth += amount;
        float currentHealthPct = (float)currentHealth / (float)maxHealth; 
        OnHealthPctChanged(currentHealthPct);
    }   
    public void ModifyStamina(int amount) {
        currentStamina += amount;
        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
        float currentStaminaPct = (float)currentStamina / (float)maxStamina; 
        OnStaminaPctChanged(currentStaminaPct);
    }   
    public virtual void Die(){
        
        GameObject.Destroy(gameObject);
    }

}
