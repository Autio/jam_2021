using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileData", menuName = "ScriptableObjects/ProjectileData", order = 1)]
public class ProjectileDataScriptableObject : ScriptableObject
{
    public float lifetime = .6f; // Before projectile dissipates 
    public float knockBack = 2.0f;
    public float damage = 10.0f;
    public float speed = 12f;

}
