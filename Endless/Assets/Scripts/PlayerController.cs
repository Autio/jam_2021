using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class PlayerController : CharacterBase
{
    enum PlayerStates {idle, attacking};
    PlayerStates playerState = PlayerStates.idle;
    public WeaponBase Weapon;
    // public InputAction stikazzi;
    public GameObject playerModel;
    public Camera playerCamera;
    private EndlessInputActions inputActions;
    private bool isCurrentlySelected = true;
    enum CameraStates {near, far};
    
    CameraStates cameraState = CameraStates.near;

    // Temp for firing as a turret when idle
    public float fireRate = 2f; // Should come from a weapon eventually
    public GameObject muzzlePoint; // Where the projectile is spawned
    public GameObject projectileObject;
    private float fireCountdown = 1f;

    // Tick for player refresh
    float staminaTick = 0.6f;
    private Collider[] results = new Collider[100];


    public override void Awake()
    {
        inputActions = new EndlessInputActions();
        inputActions.Player.Enable();
        base.Awake();
    }


    void Update()
    {
        if (enemyState == EnemyStates.stunned){ //awful awful workaround. Awful
            if ( (Time.time - stunStartTime ) >= CharacterData.StunTimeAfterBeingHit){
                enemyState = EnemyStates.idle;
            }
            else{
                ExecuteKnockBack();
                return;
            }
        }
        if (!isCurrentlySelected){
            // Do idle stuff
            // By default, behave like a turret
            fireCountdown -= Time.deltaTime;
            if(fireCountdown < 0f)
            {
                Turret();
                fireCountdown = 1f / fireRate;
            }

            return;
        }

        var moveVec = inputActions.Player.Move.ReadValue<Vector2>();
        var lookVec = inputActions.Player.Look.ReadValue<Vector2>();
        navmeshAgent.Move(new Vector3(moveVec.x,0,moveVec.y) * Time.deltaTime * CharacterData.MovementSpeed);
        if (lookVec.magnitude > 0.1f){
            playerModel.transform.rotation = Quaternion.LookRotation(new Vector3(lookVec.x, 0, lookVec.y),transform.up);
        }
        else if (moveVec.magnitude > 0.1f){
            playerModel.transform.rotation = Quaternion.LookRotation(new Vector3(moveVec.x, 0, moveVec.y),transform.up);
        }
        if (inputActions.Player.Fire.triggered){
            if(currentStamina > 4)
            {
                // Swing if not already swinging
                if(playerState != PlayerStates.attacking)
                    {
                        Weapon.TrySwing();
                        // Deplete stamina
                        ModifyStamina(-CharacterData.AttackStaminaCost); // Temp fixed arbitrary value
                        staminaTick = 0.6f;
                    }
                playerState = PlayerStates.attacking;
            }
            
        }

        // Periodically recharge stamina
        staminaTick -= Time.deltaTime;
        if(staminaTick < 0)
        {
            staminaTick = 0.2f;
            playerState = PlayerStates.idle;
            if(currentStamina < maxStamina)
            {
                ModifyStamina(staminaRechargeRate);
            }
        }

        if (inputActions.Player.ToggleCamera.triggered){
            ToggleCameraView();
        }
    }


    void Turret()
    {
        var numberOfSurroundingEnemies = Physics.OverlapCapsuleNonAlloc(transform.position + Vector3.up * 5, transform.position + Vector3.down * 5, CharacterData.AttackRadiusAsTurret, results, LayerMask.GetMask("Enemy"), QueryTriggerInteraction.Ignore);
        if ( numberOfSurroundingEnemies > 0){
            float closest = CharacterData.AttackRadiusAsTurret;
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
                playerModel.transform.LookAt(target.transform);
                // Fire a projectile
                GameObject projectile = ObjectPooler.Instance.GetPooledObject(projectileObject);
                projectile.transform.position = muzzlePoint.transform.position + new Vector3(0, 0.4f, 0);
                projectile.GetComponent<Projectile>().target = target.transform;
                projectile.SetActive(true);

            }
        }
    }

    public override void GetHit(float damage, Vector3 knockback)
    {
        Debug.Log($"Logging: GOTHITIHTHIGN");
        base.GetHit(damage, knockback);
        
    }
    public override void Die()
    {
        PlayerManager.Instance.PlayerDied(this);
        base.Die();
    }
    public void SetSelectedState(bool isSelected){
        playerCamera.gameObject.SetActive(isSelected);
        isCurrentlySelected = isSelected;
    }

    public void ToggleCameraView()
    {
        // Switches between preset camera positions
        Vector3 cameraNear = new Vector3(0, 5.85f, -4.5f);
        Vector3 cameraFar = new Vector3(0, 10, -8);
        if(cameraState == CameraStates.near)
        {
            cameraState = CameraStates.far;
            Camera.main.transform.localPosition = cameraFar;
        } else 
        {
            cameraState = CameraStates.near;
            Camera.main.transform.localPosition = cameraNear;
        }


    }

}
