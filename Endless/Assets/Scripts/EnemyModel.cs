using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyModel : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {
//        Debug.Log(collision.gameObject.layer);
        if(collision.gameObject.layer == 8)
        {
            transform.parent.GetComponent<Enemy>().SetJumping(false);
            transform.parent.GetComponent<Enemy>().CreateGroundParticles(this.transform.position);
        }
    }

}
