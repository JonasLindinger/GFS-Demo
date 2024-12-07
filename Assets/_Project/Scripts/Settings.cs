using UnityEngine;

namespace _Project.Scripts
{
    /// <summary>
    /// Contains some default settings for the project such as TickRate, etc.
    /// </summary>
    public class Settings : MonoBehaviour
    {
        // Singleton reference
        private static Settings Instance { get; set; }
        
        // Getters
        public static uint PhysicsTickRate => Instance._physicsTickRate;
        public static uint NetworkTickRate => Instance._physicsTickRate;
        
        // Inspector Fields
        [Header("Tick System")]
        [SerializeField] private uint _physicsTickRate = 64;
        [SerializeField] private uint _networkTickRate = 64;

        private void Awake()
        {
            // Singleton referencing
            Instance = this;
        }
    }
}