using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpearWeaponController : WeaponBase,IWeaponController
{
    public Animation SwingAnimation;
    public PlayerController SpearOwner;
    private Rigidbody spearRb;
    private bool canSwing;
    private bool isSwinging;

    void Awake(){
        spearRb = GetComponent<Rigidbody>();
        canSwing = true;
    }

    void Update()
    {
        
    }

    public  override void TrySwing(){
        if (canSwing){
            StartSwing();
        }
    }

    private void StartSwing(){
        SwingAnimation.Play();
        spearRb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        canSwing = false;
        isSwinging = true;
    }
    public void EndSwing(){ //called by animation event. meh.
        spearRb.collisionDetectionMode = CollisionDetectionMode.Discrete;
        canSwing = true;
        isSwinging = false;
    }


//DEPRECATE
    // void OnCollisionEnter(Collision other) {
    //     Debug.Log($"Logging: AMCOLLIDE");
    //     if (!isSwinging) return;
    //     if (other.gameObject.layer == LayerMask.NameToLayer("Enemy")) { 
    //         Debug.Log($"We hit monster");
    //     }
    // }
    void OnTriggerEnter(Collider other) {
        if (!isSwinging) return;
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy")) { 
            Debug.Log($"We hit monster");
            var enemy = other.GetComponentInParent<Enemy>();
            Vector3 knockBackVector = (enemy.transform.position - SpearOwner.transform.position) * SpearOwner.CharacterData.KnockBack;
            enemy.GetHit(SpearOwner.CharacterData.HitDamage, knockBackVector);
        }
    }
}
