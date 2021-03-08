using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Structure : MonoBehaviour
{
    public StructureDataScriptableObject StructureData;

    // Handle health in the separate component
    Health health;
    public GameObject projectileObject;
    private Collider[] results = new Collider[100];
    private float fireCountdown;

    void Awake()
    {
        health = GetComponent<Health>();
        StructuresManager.Instance.AddNewStructure(this);
        try {
        health.SetCurrentHealth(StructureData.Health);
        health.SetMaxHealth(StructureData.Health);
        } catch{
            Debug.Log("Couldn't set health for " + gameObject.name);
        }
    }

    void Update(){

        fireCountdown -= Time.deltaTime;

        if (StructureData.IsTurret && fireCountdown < 0f){
            fireCountdown = StructureData.AttackRateAsTurret;

            var numberOfSurroundingEnemies = Physics.OverlapCapsuleNonAlloc(transform.position + Vector3.up * 5, transform.position + Vector3.down * 5, StructureData.AttackRadiusAsTurret, results, LayerMask.GetMask("Enemy"), QueryTriggerInteraction.Ignore);
            if ( numberOfSurroundingEnemies > 0){
                float closest = StructureData.AttackRadiusAsTurret;
                Collider target = null;
                // Target the closest enemy in range
                for (int i = 0; i < numberOfSurroundingEnemies; i++)
                {
                    var enemy = results[i];
                    float dist = (enemy.transform.position - transform.position).magnitude;
                        if(dist < closest)
                        {
                            closest = dist;
                            target = enemy;
                        }
                }
                if(target != null)
                {
                    // Turn towards the target
                    // Fire a projectile
                    GameObject projectile = ObjectPooler.Instance.GetPooledObject(projectileObject);
                    projectile.transform.position = transform.position + Vector3.up * 3;
                    projectile.GetComponent<Projectile>().target = target.transform;
                    projectile.SetActive(true);

                }
            }

        }
    }

    public void GetHit(float damage){
        health.ModifyHealth(-damage);
        if (health.GetCurrentHealth() <= 0) {
            Die();
        }
    }

    public void Die(){
        GameObject.Destroy(gameObject);
    }
    public void OnDestroy(){
        StructuresManager.Instance.RemoveDeadStructure(this);
    }
}
