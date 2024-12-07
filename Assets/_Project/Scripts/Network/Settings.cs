using UnityEngine;
using UnityEngine.Serialization;

namespace _Project.Scripts.Network
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
        public static bool UseAutoFillInputFieldsInputFields => Instance._useAutoFillInputFields;

        public static ushort DefaultPort => Instance._defaultPort;
        public static readonly string AutoFillIP = "127.0.0.1";
        
        // Inspector Fields
        [Header("Tick System")]
        [SerializeField] private uint _physicsTickRate = 64;
        [SerializeField] private uint _networkTickRate = 64;
        [SerializeField] private bool _useAutoFillInputFields = true;
        [SerializeField] private ushort _defaultPort = 7000;

        private void Awake()
        {
            // Singleton referencing
            Instance = this;
        }
    }
}