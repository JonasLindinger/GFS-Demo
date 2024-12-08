using UnityEngine;
using UnityEngine.InputSystem;

namespace _Project.Scripts.Network.Input
{
    /// <summary>
    /// This class is responsible for managing the player's input.
    /// </summary>
    [RequireComponent(typeof(PlayerInput))]
    public class InputManager : MonoBehaviour
    {
        // Singleton reference
        public static InputManager Instance { get; private set; }
        
        // References
        private PlayerInput _playerInput;

        private void Start()
        {
            _playerInput = GetComponent<PlayerInput>();
        }
        
        /// <summary>
        /// This method returns the player's movement input (WASD) as a Vector2.
        /// </summary>
        /// <returns></returns>
        public static Vector2 GetMoveInput()
        {
            return Instance._playerInput.actions["Move"].ReadValue<Vector2>();
        }
    }
}