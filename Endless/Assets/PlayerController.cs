using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // private InputAction moveAction;
    // private PlayerInput playerInput;

    private EndlessInputActions inputActions;

    void Awake()
    {
        inputActions = new EndlessInputActions();
        inputActions.Player.Enable();
    }

    void Update()
    {
        var moveVec = inputActions.Player.Move.ReadValue<Vector2>();
        Debug.Log(moveVec);
        GetComponent<NavMeshAgent>().Move(new Vector3(moveVec.x,0,moveVec.y) * Time.deltaTime * 10);
    }
}
