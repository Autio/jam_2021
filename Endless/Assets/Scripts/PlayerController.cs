﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class PlayerController : CharacterBase
{
    public enum PlayerStates {idle, attacking, building, turreting};

    public Vector3 positionBeforeGettingInTurret { get; private set; }

    public PlayerStates playerState = PlayerStates.idle;
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


    // Building
    [SerializeField] 
    private GameObject placeableObjectPrefab;
    private GameObject currentPlaceableObject;
    public Material buildingPlacementMaterial;
    private Material originalModelMaterial;
    private Material originalBaseMaterial;


    // Tick for player refresh
    float staminaTick = 0.6f;
    private Collider[] results = new Collider[100];
    private Structure hostStructure;

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
            if (playerState == PlayerStates.turreting){
                return;
            }
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

        if (playerState == PlayerStates.turreting ){
            if (moveVec.magnitude < 0.2f){
                return;
            }
            else{
                // NavMeshHit hit = new NavMeshHit();
                Debug.Log($"Logging: TRYING TO GET OUT ");

                // if (NavMesh.SamplePosition(transform.position,out hit,20,NavMesh.GetAreaFromName("Walkable"))){
                    Debug.Log($"Logging: HITPOSITINION");
                    transform.position = positionBeforeGettingInTurret;
                    playerState = PlayerStates.idle;
                    navmeshAgent.enabled = true;
                    hostStructure.RemovePlayerInTurret(this);
                // }
                return;
            }
        }
        var lookVec = inputActions.Player.Look.ReadValue<Vector2>();

        navmeshAgent.Move(new Vector3(moveVec.x,0,moveVec.y) * Time.deltaTime * CharacterData.MovementSpeed);       
        
        // Move the building you are placing 
        if (playerState == PlayerStates.building && currentPlaceableObject != null)
        {
            MoveCurrentPlaceableObject();
        }
        
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

        if (inputActions.Player.PlaceBuilding.triggered && playerState != PlayerStates.building){
            // Debug.DrawLine(transform.position + playerModel.transform.forward * 2 + Vector3.up * 10, transform.position + playerModel.transform.forward * 2 + Vector3.up * 10 + Vector3.down * 20, Color.red, 4);
            var ray = new Ray(transform.position + playerModel.transform.forward * 1 + Vector3.up * 10, Vector3.down * 20);
            Debug.DrawRay(transform.position + playerModel.transform.forward * 2 + Vector3.up * 10, Vector3.down * 20, Color.blue, 4);
            RaycastHit hit;
            if (Physics.Raycast(ray,out hit,20,1 << LayerMask.NameToLayer("Structure"))){
                Debug.Log($"Logging: WE GOT BULDISNG");
                var structure = hit.collider.GetComponent<Structure>();
                if(structure.StructureData.IsTurret){
                    Debug.Log($"Logging: AND S TURRET");
                    positionBeforeGettingInTurret = transform.position;
                    playerState = PlayerStates.turreting;
                    navmeshAgent.enabled = false;
                    structure.PlacePlayerInTurret(this);
                    hostStructure = structure;
                    
                    return;
                }
            }
        }

        if(playerState != PlayerStates.building)
        // Periodically recharge stamina
        {
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
        }

        // Camera
        if (inputActions.Player.ToggleCamera.triggered){
            ToggleCameraView();
        }

        // Buildings
        if (inputActions.Player.ToggleBuildMode.triggered){
            // Should probably not allow switching to buildmode when attacking
            // Should check whether player is near base? 
            if(currentPlaceableObject == null)
            {
                currentPlaceableObject = Instantiate(placeableObjectPrefab); // Todo - Grab from pool?
                ShowCurrentPlaceableObject();
                playerState = PlayerStates.building;
            } else 
            {
                playerState = PlayerStates.idle;
                Destroy(currentPlaceableObject);
            }

        }
        if (inputActions.Player.PlaceBuilding.triggered){
            PlaceCurrentPlaceableObject();
        }

    }

    private void ShowCurrentPlaceableObject()
    {
        Vector3 down = transform.TransformDirection(Vector3.down);
        RaycastHit hit;
        Physics.Raycast(new Vector3(transform.position.x + 2, transform.position.y + 10, transform.position.z), down, out hit); 
        currentPlaceableObject.transform.position = hit.point + new Vector3(0, .5f, 0);

        // Disable collider
        foreach(Transform child in currentPlaceableObject.transform)
        {
            try{ 
                child.GetComponent<Collider>().enabled = false;
            } catch 
            {

            }
        }

        // Assumes buildings have a base and a model
        originalModelMaterial = currentPlaceableObject.transform.Find("model").GetComponent<Renderer>().material;
        originalBaseMaterial = currentPlaceableObject.transform.Find("Base").GetComponent<Renderer>().material;
        currentPlaceableObject.transform.Find("model").GetComponent<Renderer>().material = buildingPlacementMaterial;
        currentPlaceableObject.transform.Find("Base").GetComponent<Renderer>().material = buildingPlacementMaterial;
    }

    private void MoveCurrentPlaceableObject(){
        
        // The building is always a bit ahead of the player's facing
        Vector3 down = transform.TransformDirection(Vector3.down);
        RaycastHit hit;
        Physics.Raycast(new Vector3(transform.position.x + 2, transform.position.y + 10, transform.position.z), down, out hit); 
        currentPlaceableObject.transform.position = hit.point + new Vector3(0, .5f, 0);

        // Shoulder buttons rotate
        int dir = (int)inputActions.Player.ChangeCharacter.ReadValue<float>();
        float rotationSpeed = 180f;
        float rotation = dir * Time.deltaTime * rotationSpeed;

        currentPlaceableObject.transform.Rotate(0, rotation, 0);

    }

    // Method to move the building around freely, not just close to the player
    // private void MoveCurrentPlaceableObject(Vector3 moveVec, Vector3 lookVec)
    // {
    //     currentPlaceableObject.transform.Translate(new Vector3(moveVec.x, 0, moveVec.y) * Time.deltaTime * CharacterData.MovementSpeed, Space.World);
    //     if (lookVec.magnitude > 0.1f){
    //         currentPlaceableObject.transform.rotation = Quaternion.LookRotation(new Vector3(lookVec.x, 0, lookVec.y),transform.up);
    //     }

    // }

    private void PlaceCurrentPlaceableObject()
    {
        currentPlaceableObject.transform.Find("model").GetComponent<Renderer>().material = originalModelMaterial;
        currentPlaceableObject.transform.Find("Base").GetComponent<Renderer>().material = originalBaseMaterial;

        // Enable the collider

        // Disable collider
        foreach(Transform child in currentPlaceableObject.transform)
        {
            try{ 
                child.GetComponent<Collider>().enabled = true;
            } catch 
            {
                
            }
        }
        currentPlaceableObject = null;

        playerState = PlayerStates.idle;
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
