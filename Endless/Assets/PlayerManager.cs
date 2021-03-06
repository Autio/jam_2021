using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerManager : Singleton<PlayerManager>
{
    private EndlessInputActions inputActions;
    private List<PlayerController> players;
    private PlayerController currentlySelectedPlayer;
    // Start is called before the first frame update
    void Awake(){
        inputActions = new EndlessInputActions();
        inputActions.Player.Enable();

        players = GameObject.FindObjectsOfType<PlayerController>().ToList();
        players.ForEach(player => { player.SetSelectedState(false); });
        currentlySelectedPlayer = players[0];
        currentlySelectedPlayer.SetSelectedState(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (inputActions.Player.PrevCharacter.triggered){
            Debug.Log($"Logging: CHANGE CHAR");
            var currentPlayerIndex = players.IndexOf(currentlySelectedPlayer);
            currentlySelectedPlayer.SetSelectedState(false);
            var newSelectedPlayerIndex = (currentPlayerIndex + (int) inputActions.Player.PrevCharacter.ReadValue<float>()) % players.Count;
            currentlySelectedPlayer = players[newSelectedPlayerIndex];
            currentlySelectedPlayer.SetSelectedState(true);
        }
    }

    public void PlayerDied(PlayerController deadPlayer){
        players.Remove(deadPlayer);
        if (players.Count > 0){
            currentlySelectedPlayer = players[0];
            currentlySelectedPlayer.SetSelectedState(true);
        }
        else{
            Debug.LogError("ALL PLAYERS DEAD YOU LOSE AND WE DON'T HAVE AN END SCREEN");
        }

    }

}
