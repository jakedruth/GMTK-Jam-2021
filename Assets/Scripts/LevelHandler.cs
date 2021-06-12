using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelHandler : MonoBehaviour
{
    public Player player;
    public Player otherPlayer;
    
    public string otherInput;
    private int _inputIndex;

    void Awake()
    {
        player.onEndMoving.AddListener(OnPlayerEndMove);
        otherPlayer.onEndMoving.AddListener(OnOtherEndMove);
    }

    public void OnPlayerEndMove()
    {
        CheckForLevelCompleted();
        SetPlayerToCanMove();
    }

    public void OnOtherEndMove()
    {
        CheckForLevelCompleted();
        SendInputToOtherPlayer();
    }

    public void SetPlayerToCanMove()
    {
        player.canMove = true;
    }

    public void SendInputToOtherPlayer()
    {
        int command = otherInput[_inputIndex % otherInput.Length] - 48; // the int value for char '0' is 48

        player.canMove = false;
        otherPlayer.Move(command);

        _inputIndex++;
    }

    public void CheckForLevelCompleted()
    {
        foreach (GoalMarker goalMarker in FindObjectsOfType<GoalMarker>())
        {
            if (!goalMarker.isOverlapping)
            {
                return;
            }
        }

        Debug.Log("Victory!");
    }
}
