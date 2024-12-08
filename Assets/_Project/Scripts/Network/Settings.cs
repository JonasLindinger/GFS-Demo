using UnityEngine;
using UnityEngine.Serialization;

namespace _Project.Scripts.Network
{
    /// <summary>
    /// Contains some default settings for the project such as TickRate, etc.
    /// </summary>
    public class Settings : MonoBehaviour
    {
        // Todo: Add Connection Approval, Timeout and Reconnecting
        // Todo: Add Actual Player Model
        // Singleton reference
        private static Settings Instance { get; set; }
        
        // Getters
        public static uint PhysicsTickRate => Instance._physicsTickRate;
        public static uint NetworkTickRate => Instance._physicsTickRate;
        public static uint TickAdjustmentRate => Instance._tickAdjustmentRate;
        public static bool ControlPhysics => Instance._controlPhysics;
        public static bool UseAutoFillInputFieldsInputFields => Instance._useAutoFillInputFields;
        public static ushort DefaultPort => Instance._defaultPort;
        public static string AutoFillIP => Instance._autoFillIP;

        // Inspector Fields
        [Header("Tick System")]
        [SerializeField] private uint _physicsTickRate = 64; // 64 Ticks per Second
        [SerializeField] private uint _networkTickRate = 64; // 64 Ticks per Second
        [SerializeField] private uint _tickAdjustmentRate = 1; // Once every Second
        [Space(5)] 
        [SerializeField] private bool _controlPhysics = false;
        [Space(5)]
        [SerializeField] private bool _useAutoFillInputFields = true;
        [Space(5)]
        [SerializeField] private ushort _defaultPort = 7000;
        [SerializeField] private string _autoFillIP = "127.0.0.1";

        private void Awake()
        {
            // Singleton referencing
            Instance = this;
        }
    }
}