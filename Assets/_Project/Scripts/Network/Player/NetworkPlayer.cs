using System.Collections.Generic;
using System.Linq;
using LindoNoxStudio.Network.Game.Camera;
using LindoNoxStudio.Network.Input;
using LindoNoxStudio.Network.Simulation;
using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;
using Client = LindoNoxStudio.Network.Connection.Client;
using NetworkClient = LindoNoxStudio.Network.Connection.NetworkClient;

namespace LindoNoxStudio.Network.Player
{
    [RequireComponent(typeof(PlayerController))]
    [RequireComponent(typeof(CameraOwner))]
    public class NetworkPlayer : NetworkBehaviour
    {
        #if Client
        // Local Network Player Singleton reference
        public static NetworkPlayer LocalNetworkPlayer { get; private set; }
        
        // Values
        private uint _lastReceavedGameStateTick;
        
        private CameraOwner _cameraOwner;
        
        #elif Server
        // Client info reference
        private Client _networkClient;
        #endif
        
        // References
        [HideInInspector] public PlayerController playerController;
        
        public override void OnNetworkSpawn()
        {
            #if Client
            _cameraOwner = GetComponent<CameraOwner>();
            
            // Referencing Singleton
            if (IsOwner)
                LocalNetworkPlayer = this;
            #elif Server
            // Client Info referencing
            _networkClient = Client.GetClientByClientId(OwnerClientId);
            _networkClient.NetworkPlayer = this;
            #endif

            // Referencing
            playerController = GetComponent<PlayerController>();
        }
        
        public override void OnNetworkDespawn()
        {
            #if Client
            // Removing Singleton reference
            if (IsOwner)
                LocalNetworkPlayer = null;
            #endif
        }

        #if Client
        /// <summary>
        /// Predicts and saves the local Game State
        /// </summary>
        /// <param name="tick"></param>
        public ClientInputState PredictLocalState(uint tick)
        {
            // Getting input to process
            ClientInputState input = NetworkClient.LocalClient._input.GetClientInputState(tick);
            
            // Process new input
            playerController.OnInput(input);
            _cameraOwner.UpdateCam(transform.position, transform.rotation);
            
            return input;
        }
        
        #elif Server
        /// <summary>
        /// Sets and saves the Game State of this Player
        /// </summary>
        /// <param name="tick"></param>
        public void HandleState(uint tick)
        {   
            // Getting input to process
            ClientInputState input = _networkClient.NetworkClient._input.GetClientInputState(tick);
            
            // Process new input
            playerController.OnInput(input);
        }
        #endif

        /// <summary>
        /// Remote procedural call.
        /// Server sends the client the current GameState and the Client applys it and check his local movement
        /// </summary>
        [Rpc(SendTo.Owner, Delivery = RpcDelivery.Reliable)]
        public void OnServerGameStateRPC(GameState gameState)
        {
            #if Client
            foreach (var kvp in gameState.States)
            {
                ulong networkId = kvp.Key;
                IState state = kvp.Value;

                SnapshotManager.ApplyState(gameState.Tick, networkId, state);
            }

            SnapshotManager.TakeSnapshot(gameState.Tick); 

            for (uint tick = gameState.Tick + 1; tick <= SimulationManager.CurrentTick; tick++)
                SimulationManager.RunPhysicsTick(tick, true);
            #endif
        }
    }
}