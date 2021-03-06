using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Blob : CharacterBase
{
    public Transform model;
    private float jumpTicker = 1.0f;
    private float sinceLastJump = 0f;
    bool jumping = false;

    public GameObject groundParticles;
    private GameObject crystal;
    private NavMeshPath pathToCrystal;
    private Rigidbody modelRb;

    public override void Awake(){
        base.Awake();
        crystal = GameObject.FindGameObjectWithTag("Crystal");
        pathToCrystal = new NavMeshPath();
        modelRb = model.GetComponent<Rigidbody>();
    }

    void Update()
    {
        if(enemyState != EnemyStates.idle)
        {
            modelRb.isKinematic = true; //else they fall through the ground when hit or knocked or whatever.
        }

        if (enemyState == EnemyStates.stunned){
            if ( (Time.time - stunStartTime ) >= CharacterData.StunTimeAfterBeingHit){
                enemyState = EnemyStates.idle;
            }
            else{
                ExecuteKnockBack();
                return;
            }
        }

        
        var closestPlayerCharacter = PlayerManager.Instance.GetClosestPlayerCharacter(transform.position);
        var distanceToPlayerChar = Vector3.Distance(transform.position,closestPlayerCharacter.transform.position);
        if (distanceToPlayerChar <= CharacterData.MaxDistanceToChasePlayer){
            enemyState = EnemyStates.attacking;
            navmeshAgent.SetDestination(closestPlayerCharacter.transform.position);
        }
        else{
            var directDistanceToCrystal = Vector3.Distance(transform.position, crystal.transform.position);
            if (directDistanceToCrystal < CharacterData.MaxPathedDistanceToChaseCrystal){
                navmeshAgent.CalculatePath(crystal.transform.position, pathToCrystal);
                if(pathToCrystal.status == NavMeshPathStatus.PathComplete){
                    Debug.Log($"We got path to crystal");
                    navmeshAgent.SetPath(pathToCrystal);
                    //if we are here, there _is_ a path to crystal
                    if (navmeshAgent.remainingDistance > CharacterData.MaxPathedDistanceToChaseCrystal){
                        navmeshAgent.ResetPath();
                    }
                    else{
                        Debug.Log($"We are going to crystal");
                        return; //terrible but else they don't go to the crystal.
                    }
                }
            }
            var closestStructure = StructuresManager.Instance.GetClosestStructure(transform.position);
            var distanceToStructure = Vector3.Distance(transform.position,closestPlayerCharacter.transform.position);
            if (distanceToStructure <= CharacterData.MaxDistanceToChaseStructures){
                enemyState = EnemyStates.attacking;
                navmeshAgent.SetDestination(closestStructure.transform.position);
            }
            else{
                enemyState = EnemyStates.idle;
            }
        }

        if(enemyState == EnemyStates.idle)
        {
            IdleBehaviour();
        }
    }

    // Get the enemy to jump in an agitated state
    void IdleBehaviour()
    {
        // TODO: Replace with an animation
        modelRb.isKinematic = false; //else they don't fall when they jump. I really wish this was a normal animation already :D 
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

    void Jump(){
        jumping = true;
        sinceLastJump = 0;
        model.GetComponent<Rigidbody>().AddForce(transform.up * Random.Range(50, 130));

    }

    public void SetJumping(bool b){
        jumping = b;
    }

    public void CreateGroundParticles(Vector3 pos){
        GameObject gp = GameObject.Instantiate(groundParticles, pos, Quaternion.identity) as GameObject;
        Destroy(gp, .6f);
    }

    public override void GetHit(float damage, Vector3 knockback){
        
        
        base.GetHit(damage,knockback); //this takes care of dying
    }

    void OnTriggerEnter(Collider other) {
        //We hit a player
        if (other.gameObject.layer == LayerMask.NameToLayer("Player")) { 
            Debug.Log($"Logging:  YO IMMA PLAYER II GOT IT  ");

            var playerCharacter = other.GetComponentInParent<CharacterBase>();//We'll need some weird shit to know we're calling the right function here, the specific enemy's rather than the character base one. 
            Vector3 knockBackVector = (playerCharacter.transform.position - transform.position) * CharacterData.KnockBack;
            playerCharacter.GetHit(CharacterData.HitDamage, knockBackVector);
        }
        //We hit a structure
        else if (other.gameObject.layer == LayerMask.NameToLayer("Structure") ){
            var structure = other.GetComponentInParent<Structure>();
            Vector3 knockBackVector = (transform.position - structure.transform.position) * structure.StructureData.KnockBackInflictedUponAttacker;
            this.GetHit(0, knockBackVector);
            //really bad hack to give this a different stuntime, as demanded not by the blob's own data, but by the structure's. this is really horrible.
            stunStartTime = Time.time + ( CharacterData.StunTimeAfterBeingHit - structure.StructureData.StuntimeInflictedUponAttacker ); // ha ha lol! 
            structure.GetHit(CharacterData.HitDamage);
        }
    }

}