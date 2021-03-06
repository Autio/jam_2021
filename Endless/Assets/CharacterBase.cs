using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public enum EnemyStates {idle, attacking, retreating };

public class CharacterBase : MonoBehaviour
{
    public CharacterDataScriptableObject CharacterData;

    protected NavMeshAgent navmeshAgent;

    protected float currentHealth;
    protected EnemyStates enemyState = EnemyStates.idle;

    public virtual void Awake(){
        currentHealth = CharacterData.Health;
        navmeshAgent = GetComponent<NavMeshAgent>();
    }

    public virtual void GetHit(float damage, Vector3 knockback) { 
        currentHealth -= damage;
        if (currentHealth <= 0){
            Die();
        }
    }

    public virtual void Die(){
        GameObject.Destroy(gameObject);
    }

}
