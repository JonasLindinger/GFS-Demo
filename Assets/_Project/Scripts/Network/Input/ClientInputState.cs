using Unity.Netcode;
using UnityEngine;

namespace _Project.Scripts.Network.Input
{
    /// <summary>
    /// This class is responsible for holding the client's input state.
    /// </summary>
    public abstract class ClientInputState : INetworkSerializable
    {
        public uint Tick;
        public Vector2 Rotation;
        private byte _essentialKeys;
        
        public bool IsSprinting => (_essentialKeys & (1 << 4)) != 0;
        public bool IsJumping => (_essentialKeys & (1 << 5)) != 0;
        public bool IsCrouching => (_essentialKeys & (1 << 6)) != 0;
        
        // Decode moveInput
        public Vector2 GetMoveInput()
        {
            return new Vector2(
                (_essentialKeys & (1 << 1)) != 0 ? -1 :    // Left
                (_essentialKeys & (1 << 3)) != 0 ? 1 : 0,  // Right
                (_essentialKeys & (1 << 0)) != 0 ? 1 :     // Forward
                (_essentialKeys & (1 << 2)) != 0 ? -1 : 0  // Backward
            );
        }
        
        public void SetUp(uint tick, Vector2 rotation, Vector2 moveInput, bool isSprinting, bool isJumping, bool isCrouching)
        {
            Tick = tick;
            Rotation = rotation;
            
            // Reset the essentialKeys to 0 before setting new values
            _essentialKeys = 0;

            // Encode moveInput
            if (moveInput.y > 0) _essentialKeys |= 1 << 0; // Forward (y > 0)
            if (moveInput.x < 0) _essentialKeys |= 1 << 1; // Left (x < 0)
            if (moveInput.y < 0) _essentialKeys |= 1 << 2; // Backward (y < 0)
            if (moveInput.x > 0) _essentialKeys |= 1 << 3; // Right (x > 0)
            
            // Encoding other keys
            if (isSprinting) _essentialKeys |= 1 << 4; // Sprinting 0 or 1
            if (isJumping) _essentialKeys |= 1 << 5;   // Jumping 0 or 1
            if (isCrouching) _essentialKeys |= 1 << 6; // Crouching 0 or 1
            // if (isCrouching) essentialKeys |= 1 << 7; Empty Slot to use in the future // Crouching 0 or 1
        }
        
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref Tick);
            serializer.SerializeValue(ref Rotation);
            serializer.SerializeValue(ref _essentialKeys);
        }
    }
}