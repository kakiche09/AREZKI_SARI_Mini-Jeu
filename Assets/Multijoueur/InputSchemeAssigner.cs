using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;


public class InputSchemeAssigner : MonoBehaviour 
{
    // Attention, ce système ne peut pas gérer plus de 2 joueurs sur le même clavier.

    private void Start()
    {
        // Nombre de joueurs
        int playerCount = PlayerInput.all.Count;

        // Nombre de gamepads
        int gamePadCount = Gamepad.all.Count;

        print($"Partie de {playerCount} joueurs avec {gamePadCount} GamePads");

        int remainingPlayers = playerCount;

        if (playerCount > 0)
        {
            // Assigner les gamepad
            for (int i = 0; i < gamePadCount; i++)
            {
                PlayerInput.all[remainingPlayers - 1].SwitchCurrentControlScheme("GamePad", Gamepad.all[i]);

                remainingPlayers--;

                if (remainingPlayers == 0)
                    return;
            }

            // Assigner les côtés de clavier
            for (int i = remainingPlayers; i > 0; i--)
            {
                switch (i)
                {
                    case 1:
                        PlayerInput.all[remainingPlayers - 1].SwitchCurrentControlScheme("Keyboard Left", Keyboard.current);
                        break;
                    case 2:
                        PlayerInput.all[remainingPlayers - 1].SwitchCurrentControlScheme("Keyboard Right", Keyboard.current);
                        break;
                    default:
                        break;
                }

                remainingPlayers--;
            }
        }
	}
}
