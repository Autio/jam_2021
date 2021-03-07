﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public enum EnemyStates {idle, attacking, retreating, stunned };

[Serializable]
public class CharacterBase : MonoBehaviour
{
    public CharacterDataScriptableObject CharacterData;
    public event Action<float> OnHealthPctChanged = delegate {};
    public event Action<float> OnStaminaPctChanged = delegate {};

    protected Vector3 knockBackDestination;

    protected NavMeshAgent navmeshAgent;

    protected float currentHealth, maxHealth, currentStamina, maxStamina, staminaRechargeRate;
    protected EnemyStates enemyState = EnemyStates.idle;
    protected NavMeshHit hit;

    protected float stunStartTime;

    public virtual void Awake(){
        currentHealth = CharacterData.Health;
        maxHealth = currentHealth;
        currentStamina = CharacterData.Stamina;
        maxStamina = currentStamina;
        staminaRechargeRate = CharacterData.StaminaRechargeRate;

        navmeshAgent = GetComponent<NavMeshAgent>();
    }

    public virtual void GetHit(float damage, Vector3 knockBack) { 
        ModifyHealth(-(int)damage); // Healthbar tracks as int
        if (currentHealth <= 0){
            Die();
            return;
        }
        if (CharacterData.StunTimeAfterBeingHit >0){
            stunStartTime = Time.time;
            enemyState = EnemyStates.stunned;
        }
        ApplyKnockBack(knockBack);
    }

    public void ModifyHealth(float amount) {
        currentHealth += amount;
        float currentHealthPct = currentHealth / maxHealth; 
        OnHealthPctChanged(currentHealthPct);
    }   
    public void ModifyStamina(float amount) {
        currentStamina += amount;
        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
        float currentStaminaPct = currentStamina / maxStamina; 
        OnStaminaPctChanged(currentStaminaPct);
    }   

    public void ApplyKnockBack(Vector3 knockBack){
        var tentativeKnockbackDestination = transform.position + knockBack;
        NavMesh.Raycast(transform.position, tentativeKnockbackDestination, out hit, NavMesh.AllAreas);
        if (hit.hit){
            tentativeKnockbackDestination = hit.position;
        }
        knockBackDestination = tentativeKnockbackDestination;
        //navmeshAgent.SetDestination(tentativeKnockbackDestination);
    }

    protected void ExecuteKnockBack(){
        transform.position = Vector3.MoveTowards(transform.position, knockBackDestination, CharacterData.KnockBackSpeed * Time.deltaTime);
    }

    public virtual void Die(){
        
        GameObject.Destroy(gameObject);
    }

}
