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
    public override void Awake()
    {
        inputActions = new EndlessInputActions();
        inputActions.Player.Enable();
        base.Awake();

    }

    // Tick for player refresh
    float staminaTick = 0.6f;

    void Update()
    {
        if (!isCurrentlySelected){
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
            staminaTick = 0.6f;
            playerState = PlayerStates.idle;
            if(currentStamina < maxStamina)
            {
                ModifyStamina(staminaRechargeRate);
            }
        }
    }

    public override void GetHit(float damage, Vector3 knockback)
    {
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
}
