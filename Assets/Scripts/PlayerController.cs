using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerController : MonoBehaviour
{
    private Player _player;

    void Start()
    {
        _player = GetComponent<Player>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
            _player.Move(0);
        else if (Input.GetKeyDown(KeyCode.D))
            _player.Move(1);
        else if (Input.GetKeyDown(KeyCode.S))
            _player.Move(2);
        else if (Input.GetKeyDown(KeyCode.A))
            _player.Move(3);
    }
}
