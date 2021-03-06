using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public Transform model;
    public CharacterDataScriptableObject EnemyData;
    enum enemyStates {idle, attacking, retreating };
    enemyStates enemyState = enemyStates.idle;
    private float jumpTicker = 1.0f;
    private float sinceLastJump = 0f;
    bool jumping = false;
    private NavMeshAgent navmeshAgent;

    public GameObject groundParticles;

    // Start is called before the first frame update
    void Awake()
    {
        navmeshAgent = GetComponent<NavMeshAgent>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if(enemyState == enemyStates.idle)
        {
            IdleBehaviour();
        }
    }

    // Get the enemy to jump in an agitated state
    void IdleBehaviour()
    {
        sinceLastJump += Time.deltaTime;
        jumpTicker -= Time.deltaTime;
        if(jumpTicker < 0 && !jumping)
        {
            Jump(); // Go ahead and jump
            jumpTicker = Random.Range(0.4f,0.7f);
        } else if (sinceLastJump > 2.5f) {
            jumping = false;
            Jump();
            jumpTicker = Random.Range(0.4f,0.7f);
        }
    }

    void Jump()
    {
        jumping = true;
        sinceLastJump = 0;
        model.GetComponent<Rigidbody>().AddForce(transform.up * Random.Range(50, 130));

    }

    public void SetJumping(bool b)
    {
        jumping = b;
    }

    public void CreateGroundParticles(Vector3 pos)
    {
        GameObject gp = GameObject.Instantiate(groundParticles, pos, Quaternion.identity) as GameObject;
        Destroy(gp, .6f);
    }

    NavMeshHit hit;
    public void GetHit(float damage, Vector3 knockback){
        EnemyData.Health -= damage;
        // navmeshAgent.speed *= 10;
        var tentativeKnockbackDestination = transform.position + knockback;
        NavMesh.Raycast(transform.position, tentativeKnockbackDestination, out hit, NavMesh.AllAreas);
        if (hit.hit){
            Debug.Log($"Logging: WE HIT THE THING");
            tentativeKnockbackDestination = hit.position;
        }
        navmeshAgent.SetDestination(tentativeKnockbackDestination);
    }

}
