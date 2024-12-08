using _Project.Scripts.Network.Input;
using Unity.Netcode;
using UnityEngine;

namespace _Project.Scripts.Network.Player
{
    /// <summary>
    /// This class is responsible for controlling the player's movement.
    /// </summary>
    public class PlayerController : NetworkBehaviour
    {
        [Header("SETTINGS")]
        [SerializeField] private float _walkSpeed;
        [SerializeField] private float _sprintSpeed;
        [SerializeField] private float _crouchSpeed;
        [SerializeField] private float _groundDrag;
        [Space(2)]
        [SerializeField] private float _jumpForce; 
        [SerializeField] private float _jumpCooldown;
        [SerializeField] private float _airMultiplier;
        [Space(10)]
        [Header("REFERENCES")]
        [SerializeField] private Camera _playerCamera;
        [SerializeField] private Transform _orientation;
        [SerializeField] private LayerMask _whatIsGround;
        [SerializeField] private float _playerHeight;

        private bool _grounded;
        private bool _readyToJump = true;

        private Rigidbody _rb;

        public override void OnNetworkSpawn()
        {
            // Referencing
            _rb = GetComponent<Rigidbody>();
            _rb.freezeRotation = true;
            _playerCamera.enabled = IsOwner;
            _playerCamera.GetComponent<AudioListener>().enabled = IsOwner;

            // Cursor
            if (!IsLocalPlayer) return;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        public void Controll(ClientInputState input)
        {
            // Todo: Use deltatiming? (Time Between Ticks)
            if (input == null) return;
            
            // Applying movement
            // Setting the drag
            _grounded = Physics.Raycast(transform.position, Vector3.down, _playerHeight * 0.5f + 0.2f, _whatIsGround);

            if (_grounded)
                _rb.linearDamping = _groundDrag;
            else
                _rb.linearDamping = 0;

            // Calculating movement
            Vector2 moveInput = input.GetMoveInput();

            _orientation.rotation = Quaternion.Euler(0, input.Rotation.x, 0);
            Vector3 moveDirection = _orientation.forward * moveInput.y + _orientation.right * moveInput.x;

            // Applying movement

            float moveSpeed = input.IsSprinting ? _sprintSpeed : input.IsCrouching ? _crouchSpeed : _walkSpeed;

            // Grounded
            if (_grounded)
                _rb.AddForce(moveDirection.normalized * (moveSpeed * 10), ForceMode.Force);

            // In air
            else
                _rb.AddForce(moveDirection.normalized * (moveSpeed * 10 * _airMultiplier), ForceMode.Force);

            // Speed Control
            Vector3 flatVel = new Vector3(_rb.linearVelocity.x, 0, _rb.linearVelocity.z);
            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                _rb.linearVelocity = new Vector3(limitedVel.x, _rb.linearVelocity.y, limitedVel.z);
            }

            if (input.IsJumping && _grounded && _readyToJump)
            {
                // Resetting Y velocity
                _rb.linearVelocity = new Vector3(_rb.linearVelocity.x, 0, _rb.linearVelocity.z);

                // Applying Force
                _rb.AddForce(transform.up * _jumpForce, ForceMode.Impulse);

                // Applying Cooldown
                _readyToJump = false;
                Invoke(nameof(ResetJump), _jumpCooldown);
            }
        }
        
        private void ResetJump()
        {
            _readyToJump = true;
        }
    }
}