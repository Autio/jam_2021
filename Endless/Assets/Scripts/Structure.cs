using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Structure : MonoBehaviour
{
    public StructureDataScriptableObject StructureData;
    public Collider StructureCollider;
    // Handle health in the separate component
    Health health;
    public GameObject projectileObject;
    private Collider[] results = new Collider[100];
    private float fireCountdown;
    private List<MaterialMap> materialMaps = new List<MaterialMap>();
    public Transform TurretStandPoint;
    private bool isManned;
    private PlayerController manningPlayer;

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
        // Store all the original renderers and materials 
        foreach(Transform child in transform)
        {
            if (child.TryGetComponent(out Renderer renderer))
            {
                MaterialMap m = new MaterialMap();
                m.renderer = renderer;
                m.originalMaterial = renderer.material;
                materialMaps.Add(m);
            }
        }
    }

    void Update(){

        fireCountdown -= Time.deltaTime;

        if (StructureData.IsTurret && fireCountdown < 0f){

            var turretRange = isManned ? StructureData.AttackRadiusAsTurret * manningPlayer.CharacterData.AttackRadiusModifierWhenInTurret : StructureData.AttackRadiusAsTurret;
            var turretDamage = isManned ? StructureData.AttackDamageAsTurret * manningPlayer.CharacterData.AttackDamageModifierWhenInTurret : StructureData.AttackDamageAsTurret;
            var turretRateOfFire = isManned ? StructureData.AttackRateAsTurret * manningPlayer.CharacterData.AttackSpeedModifierWhenInTurret : StructureData.AttackRateAsTurret;
            
            fireCountdown = turretRateOfFire;
            var numberOfSurroundingEnemies = Physics.OverlapCapsuleNonAlloc(transform.position + Vector3.up * 5, transform.position + Vector3.down * 5, turretRange, results, LayerMask.GetMask("Enemy"), QueryTriggerInteraction.Ignore);
            if ( numberOfSurroundingEnemies > 0){
                float closest = turretRange;
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
                    var projectileComponent = projectile.GetComponent<Projectile>();
                    projectileComponent.SetProjectileDamage(turretDamage);
                    projectileComponent.target = target.transform;
                    projectile.SetActive(true);
                }
            }

        }
    }

    public void PlacePlayerInTurret(PlayerController player){
        isManned = true;
        manningPlayer = player;
        
        player.transform.position = TurretStandPoint.transform.position;
      
        // player.playerModel.transform.position = TurretStandPoint.transform.position;
    }
    public void RemovePlayerInTurret(PlayerController player){
        isManned = false;
        manningPlayer = null;
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

    // Set all the materials in the renderers of this building back to what they were at Awake
    public void ResetMaterials()
    {
        foreach(MaterialMap m in materialMaps)
        {
            m.renderer.material = m.originalMaterial;
        }
    }

    // Set all the materials in the renderers of this building to one uniform material
    // Used for valid and invalid placement colouration
    public void SetMaterials(Material material)
    {
        foreach(MaterialMap m in materialMaps)
        {
            m.renderer.material = material;
        }
    }

}

class MaterialMap 
{
    // Stores the original materials of a structure's parts so that they can be swapped
    // To show valid and invalid placement instead
    public Renderer renderer;
    public Material originalMaterial;
}
