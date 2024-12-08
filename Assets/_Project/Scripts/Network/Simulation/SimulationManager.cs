using _Project.Scripts.Network.Utils;
using UnityEngine;

namespace _Project.Scripts.Network.Simulation
{
    /// <summary>
    /// This class is responsible for managing the simulation and Tick Systems.
    /// </summary>
    public class SimulationManager : MonoBehaviour
    {
        // Singleton reference
        public static SimulationManager Instance { get; private set; }
        
        // Tick Systems
        private TickSystem _physicsTickSystem;
        private TickSystem _networkTickSystem;     
        #if Server
        private TickSystem _tickAdjustmentTickSystem;
        #endif

        private void Awake()
        {
            Instance = this;
        }

        public void Update()
        {
            // Updating the Tick System(s)
            if (_physicsTickSystem != null)
                _physicsTickSystem.Update(Time.deltaTime);
            if (_networkTickSystem != null)
                _networkTickSystem.Update(Time.deltaTime);
            #if Server
            if (_tickAdjustmentTickSystem != null)
                _tickAdjustmentTickSystem.Update(Time.deltaTime);
            #endif
        }
        
        /// <summary>
        /// Starts both Tick Systems.
        /// </summary>
        /// <param name="physicsStartingTick">Optional starting Tick for the Physics Tick System. This should only be used on the Client</param>
        public static void StartSimulation(uint physicsStartingTick = 0)
        {
            // Controlling the Physics Simulation manually
            Physics.simulationMode = Settings.ControlPhysics ? SimulationMode.Script : SimulationMode.FixedUpdate;
            
            // Create new Tick Systems with the default Values
            Instance._physicsTickSystem = new TickSystem(Settings.PhysicsTickRate, physicsStartingTick);
            Instance._networkTickSystem = new TickSystem(Settings.PhysicsTickRate);
            #if Server
            _tickAdjustmentTickSystem = new TickSystem(Settings.TickAdjustmentRate);
            #endif
            
            // Subscribe to the OnTick event
            Instance._physicsTickSystem.OnTick += Instance.HandlePhysicsTick;
            Instance._networkTickSystem.OnTick += Instance.HandleNetworkTick;
            #if Server
            _tickAdjustmentTickSystem.OnTick += Instance.HandleAdjustmentTick;
            #endif
        }

        /// <summary>
        /// Runs every physics tick.
        /// Controls physics if the Simulation Mode is set to Script.
        /// Moves player or players depending on if we are a Server or Client.
        /// </summary>
        /// <param name="tick"></param>
        private void HandlePhysicsTick(uint tick)
        {
            // Running physics if the Simulation Mode is set to Script.
            // This enables us to make more precise calculations.
            if (Physics.simulationMode == SimulationMode.Script) 
                Physics.Simulate(_physicsTickSystem.TimeBetweenTicks);
        }

        /// <summary>
        /// Runs every network tick.
        /// Sends local states.
        /// </summary>
        /// <param name="tick"></param>
        private void HandleNetworkTick(uint tick)
        {
            
        }
        
        #if Server
        /// <summary>
        /// We send the buffer Size to every client, so that they can adjust there tick system
        /// </summary>
        /// <param name="tick"></param>
        private static void HandleAdjustmentTick(uint tick)
        {
            
        }
        #endif
    }
}