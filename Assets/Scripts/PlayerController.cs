using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerController : MonoBehaviour
{
    private Player _player;
    private bool _recordMovement;
    private string _data;

    void Start()
    {
        _player = GetComponent<Player>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (!_recordMovement)
            {
                _recordMovement = true;
                _data = string.Empty;
            }
            else
            {
                _recordMovement = false;
                Debug.Log(_data);
            }
        }

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            SendMovement(0);
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            SendMovement(1);
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            SendMovement(2);
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            SendMovement(3);
        else if (Input.GetKeyDown(KeyCode.Space))
            SendMovement(4);
    }

    void SendMovement(int dir)
    {
        if (_recordMovement)
            _data += dir.ToString();

        _player.Move(dir);
    }
}
