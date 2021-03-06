using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Transform model;
    enum enemyStates {idle, attacking, retreating };
    enemyStates enemyState = enemyStates.idle;
    private float jumpTicker = 1.0f; 
    bool jumping = false;

    // Start is called before the first frame update
    void Start()
    {
        
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
        jumpTicker -= Time.deltaTime;
        if(jumpTicker < 0 && !jumping)
        {
            Jump(); // Go ahead and jump
            jumpTicker = Random.Range(0.2f,0.6f);
        }
    }

    void Jump()
    {
        jumping = true;
        model.GetComponent<Rigidbody>().AddForce(transform.up * Random.Range(60, 120));

    }

    // void OnCollisionEnter(Collision collision)
    // {
    //     if(collision.collider.gameobject.layer == 8)
    //     {
    //         jumping = false;
    //     }
    // }

}
