using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Transform target;
    // Checks for impact and tries to do stuff
    // Disappears after a while
    public float lifetime;
    public float knockBack = 2.0f;
    public float damage = 2.0f;
    public float speed = 0.3f;
    // Start is called before the first frame update
    void Start()
    {
        transform.LookAt(target);
    }

    // Update is called once per frame
    void Update()
    {
        lifetime -= Time.deltaTime;
        if(lifetime < 0)
        {
            gameObject.SetActive(false);
            
        } else {
                transform.position = Vector3.MoveTowards(transform.position, target.position, Time.deltaTime * speed);
        }
    }


    void OnTriggerEnter(Collider other) {
    {
        Debug.Log("Projectile hit!");
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy")) { 
            Debug.Log("Enemy hit by projectile");
            var enemy = other.transform.GetComponentInParent<CharacterBase>(); // We'll need some weird shit to know we're calling the right function here, the specific enemy's rather than the character base one. 
            Vector3 knockBackVector = (enemy.transform.position - transform.position) * knockBack;
            enemy.GetHit(damage, knockBackVector);
            gameObject.SetActive(false);
        }      
    }
}
}
