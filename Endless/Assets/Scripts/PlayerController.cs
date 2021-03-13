using System.Collections;
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


    // Building placement
    [SerializeField] 
    private GameObject placeableObjectPrefab;
    private GameObject currentPlaceableObject;
    private Structure placeableStructure;
    private bool validPlacement;
    private Vector3 buildingExtents;
    private Quaternion previousBuildingOrientation;
    public Material buildingPlacementMaterial;
    public Material invalidPlacementMaterial;


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
        else if (moveVec.magnitude > 0.1f && playerState != PlayerStates.building){
            playerModel.transform.rotation = Quaternion.LookRotation(new Vector3(moveVec.x, 0, moveVec.y),transform.up);
        }
        if(playerState != PlayerStates.building)
        {      
            if (inputActions.Player.Fire.triggered){
                // Needs to check against something dynamic 
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
                currentPlaceableObject = Instantiate(placeableObjectPrefab); 
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

        if (playerState == PlayerStates.building && inputActions.Player.ChangeBuilding.triggered)
        {
            ChangeBuilding();
        }
    }

    // Change building being placed
    private void ChangeBuilding()
    {
            Debug.Log($"Logging: CHANGE BUILDING");
            var allowedStructures = StructuresManager.Instance.AllowedStructures;
            var currentStructureIndex = 0;
            for (int i = 0; i < allowedStructures.Count; i++)
            {
                if(allowedStructures[i].name == placeableObjectPrefab.name)
                {
                    currentStructureIndex = i;
                }
            }

            var newSelectedStructureIndex = Mathf.Abs((currentStructureIndex + (int) inputActions.Player.ChangeBuilding.ReadValue<float>()) % allowedStructures.Count);
            Destroy(currentPlaceableObject);
            placeableObjectPrefab = allowedStructures[newSelectedStructureIndex];
            currentPlaceableObject = null;
            currentPlaceableObject = Instantiate(placeableObjectPrefab); 
            currentPlaceableObject.GetComponent<Structure>().StructureCollider.enabled = true;

            ShowCurrentPlaceableObject();
    }

    // Sets the materials of the building to be placed to show if it's a valid placement or not
    private void SetPlacementValidity(bool isValid){
        if(isValid)
        {
            currentPlaceableObject.GetComponent<Structure>().SetMaterials(buildingPlacementMaterial);
        }   else 
        {
            currentPlaceableObject.GetComponent<Structure>().SetMaterials(invalidPlacementMaterial);
        }
        
    }

    private void ShowCurrentPlaceableObject()
    {

        // TODO: Building switching
        // Grab extents before the collider is disabled
        placeableStructure = currentPlaceableObject.GetComponent<Structure>();
        buildingExtents = placeableStructure.StructureCollider.bounds.extents;
        // Only check for the ground
        int layer_mask = LayerMask.GetMask("Ground");
        Vector3 down = transform.TransformDirection(Vector3.down);
        RaycastHit hit;
        float distanceFromPlayer = buildingExtents.x * 1.5f;

        Physics.Raycast((playerModel.transform.forward * distanceFromPlayer + transform.position) + new Vector3(0, 20f, 0), down, out hit, 100f, layer_mask); 
        currentPlaceableObject.transform.position = hit.point + new Vector3(0, buildingExtents.y / 4, 0);
        if(previousBuildingOrientation != null)
        {
            currentPlaceableObject.transform.rotation = previousBuildingOrientation;
        }

        // Disable collider
        currentPlaceableObject.GetComponent<Structure>().StructureCollider.enabled = false;

        // If valid
        SetPlacementValidity(true);

    }

    private void MoveCurrentPlaceableObject(){

        // Only check for the ground
        int layer_mask = LayerMask.GetMask("Ground");

        // The building is always a bit ahead of the player's facing
        Vector3 down = transform.TransformDirection(Vector3.down);
        RaycastHit hit;
        float distanceFromPlayer = buildingExtents.x * 1.6f; // Depends on the extents
        Physics.Raycast((playerModel.transform.forward * distanceFromPlayer + transform.position) + new Vector3(0, 20f, 0), down, out hit, 100f, layer_mask); 
        
        // Make sure aligns with ground 
        var slopeRotation = Quaternion.FromToRotation(currentPlaceableObject.transform.up, hit.normal);
        var slopeIncline = Vector3.Angle(Vector3.up, hit.normal);

        // Shoulder buttons rotate
        int dir = (int)inputActions.Player.RotateBuilding.ReadValue<float>();
        if(dir != 0)
        {
            float rotationSpeed = 180f;
            float rotation = dir * Time.deltaTime * rotationSpeed;
            currentPlaceableObject.transform.Rotate(0, rotation, 0);       
        }

        float yBuffer = .1f;

        // Only apply sloping for WALLS
        // Otherwise folks build things vertically even if they are on an incline base
        if(placeableStructure.StructureData.Type == StructureDataScriptableObject.BuildingType.wall)
        {
            currentPlaceableObject.transform.rotation = Quaternion.Slerp(currentPlaceableObject.transform.rotation, slopeRotation * currentPlaceableObject.transform.rotation, 10 * Time.deltaTime);        
            yBuffer = -0.2f;
        }   
         
         
        currentPlaceableObject.transform.position = hit.point + new Vector3(0, buildingExtents.y + yBuffer, 0);

        validPlacement = true;
        // Check if too far
        float allowedRadius = 10f; //TODO: Get from somewhere better, like the level settings or sth
        if(transform.position.magnitude > allowedRadius)
        {
            validPlacement = false;
        }
        // Check if overlapping with existing buildings
        int s_layerMask = LayerMask.GetMask("Structure"); 
        float overlapBuffer = 1.3f;
        if(placeableStructure.StructureData.Type == StructureDataScriptableObject.BuildingType.wall){
            overlapBuffer = 0.1f;
        }
        Debug.Log(buildingExtents / overlapBuffer);
        Collider[] hitColliders = Physics.OverlapBox(
            currentPlaceableObject.transform.position, buildingExtents * overlapBuffer, currentPlaceableObject.transform.rotation, s_layerMask);

        foreach(Collider c in hitColliders)
        {
            // Check isn't self
            if (c.transform != currentPlaceableObject.transform)
            {
                validPlacement = false;
            }
        }

        //Check on a valid incline
        float allowedIncline = placeableStructure.StructureData.MaxIncline;
        
        if(slopeIncline > allowedIncline && allowedIncline != 0)
        {
            validPlacement = false;
        }
        
        SetPlacementValidity(validPlacement);

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
        if(validPlacement)
        {
            currentPlaceableObject.GetComponent<Structure>().ResetMaterials();
            currentPlaceableObject.GetComponent<Structure>().StructureCollider.enabled = true;
            // Store the rotation to be ready for the next placement
            previousBuildingOrientation = currentPlaceableObject.transform.rotation;

            currentPlaceableObject = null;

            playerState = PlayerStates.idle;
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
