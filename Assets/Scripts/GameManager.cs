using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{

    [SerializeField] private Door _finalDoor;

    private bool _switchPlayer1 = false;
    private bool _switchPlayer2 = false;

    private void TurnOnOff(PlayerController.Player player)
    {
        switch(player)
        {
            case PlayerController.Player.Player1: _switchPlayer1 = true; break;
            case PlayerController.Player.Player2: _switchPlayer2 = true; break;
        }

        CheckVictory();
    }

    private void CheckVictory()
    {
        if (_switchPlayer1 && _switchPlayer2)
            _finalDoor.Open();
    }

    private void OnEnable()
    {
        Observer.GameManager.TurnOnOff += TurnOnOff;
    }

    private void OnDisable()
    {
        Observer.GameManager.TurnOnOff -= TurnOnOff;
    }

}
