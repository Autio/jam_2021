using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    // Checks for impact and tries to do stuff
    // Disappears after a while
    public Transform target;
    public ProjectileDataScriptableObject ProjectileData;
    float lifetime, damage, speed, knockBack;
    // Start is called before the first frame update
    void Start()
    {
        damage = ProjectileData.damage;
        speed = ProjectileData.speed;
        knockBack = ProjectileData.knockBack;

        transform.LookAt(target);
    }

    private void OnEnable() {
        lifetime = ProjectileData.lifetime;

    }

    // Update is called once per frame
    void Update()
    {
        lifetime -= Time.deltaTime;
        if(lifetime < 0)
        {
            gameObject.SetActive(false);
            
        } else {
            try{
                transform.position = Vector3.MoveTowards(transform.position, target.position, Time.deltaTime * speed);
            }
            catch{
                gameObject.SetActive(false);
            }
        }
    }


    void OnTriggerEnter(Collider other) {
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy")) { 
            var enemy = other.transform.GetComponentInParent<CharacterBase>(); // We'll need some weird shit to know we're calling the right function here, the specific enemy's rather than the character base one. 
            Vector3 knockBackVector = (enemy.transform.position - transform.position) * knockBack;
            enemy.GetHit(damage, knockBackVector);
            gameObject.SetActive(false);

        }      

    }
}
}
