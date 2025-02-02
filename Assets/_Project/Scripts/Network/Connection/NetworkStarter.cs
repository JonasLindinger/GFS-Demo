using System;
using System.Threading.Tasks;
using LindoNoxStudio.Network.Simulation;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Multiplay;
using Unity.Services.Multiplayer;
using UnityEngine;
using Random = UnityEngine.Random;

namespace LindoNoxStudio.Network.Connection
{
    public class NetworkStarter : MonoBehaviour
    {
        private static bool _alreadyAutoAllocated = true;
        
        #if Server
        private IServerQueryHandler _serverQueryHandler;
        #endif
        
        // Todo: Add player visual rotation
        // Todo: Add different Client Side Prediction modes (Predict local, Predict Everything, don't Predict)
        // Make Override the Connection data for real use
        // Connection Data to connect to
        public static ConnectionData ConnectionData = new ConnectionData()
        {
            IP = "127.0.0.1", // Default ip = 127.0.0.1 (localHost) for testing purposes
            Port = 7778 // Default Port = 7778 for testing purposes
        };

        public static string Username = "Client" + Random.Range(1111, 9999).ToString();
        
        private async void Start()
        {
            InitUnity();
            
            #if Client
            StartClient();
            #elif Server
            if (!await CheckMultiplay())
                // This isn't a multiplay Server
                StartServer(false);
            #endif
        }

        private void Update()
        {
            #if Server
            if (_serverQueryHandler == null) return;
            _serverQueryHandler.CurrentPlayers = (ushort) Client.Clients.Count;
            _serverQueryHandler?.UpdateServerCheck();
            #endif
        }

        /// <summary>
        /// Getting the Unity Transport of the NetworkManager and setting the ip and port to the connection data
        /// </summary>
        private void SetConnectionDataOnTransport(bool isMultiplayServer)
        {
            // Getting the Unity Transport
            var unityTransport = NetworkManager.Singleton.GetComponent<UnityTransport>();

            // Setting the connection data
            if (isMultiplayServer)
                unityTransport.SetConnectionData(ConnectionData.IP, ConnectionData.Port, "0.0.0.0");
            else
                unityTransport.SetConnectionData(ConnectionData.IP, ConnectionData.Port);
        }

        #if Server
        private async Task<bool> CheckMultiplay()
        {
            MultiplayEventCallbacks multiplayEventCallbacks = new MultiplayEventCallbacks();
            multiplayEventCallbacks.Allocate += MultiplayEventCallback_Allocate;
            multiplayEventCallbacks.Deallocate += MultiplayEventCallback_Deallocate;
            multiplayEventCallbacks.Error += MultiplayEventCallback_Error;
            multiplayEventCallbacks.SubscriptionStateChanged += MultiplayEventCallback_SubscriptionStateChanged;
            IServerEvents serverEvents = await MultiplayService.Instance.SubscribeToServerEventsAsync(multiplayEventCallbacks);
            
            _serverQueryHandler = await MultiplayService.Instance.StartServerQueryHandlerAsync(
                4, 
                "GameServer: " + Random.Range(1111, 9999).ToString(), 
                "Drone", 
                "1", 
                "Playground"
                );
            
            ServerConfig serverConfig = MultiplayService.Instance.ServerConfig;
            if (serverConfig.AllocationId != "")
            {
                Debug.Log("This is a multiplay server");
                MultiplayEventCallback_Allocate(new MultiplayAllocation(
                    "", 
                    serverConfig.ServerId,
                    serverConfig.AllocationId));
                return true;
            }
            else return false;
        }

        private void MultiplayEventCallback_SubscriptionStateChanged(MultiplayServerSubscriptionState obj)
        {
            
        }

        private void MultiplayEventCallback_Error(MultiplayError obj)
        {
            
        }

        private void MultiplayEventCallback_Deallocate(MultiplayDeallocation obj)
        {
            
        }

        private void MultiplayEventCallback_Allocate(MultiplayAllocation obj)
        {
            if (_alreadyAutoAllocated)
                return;
            _alreadyAutoAllocated = true;
            
            var serverConfig = MultiplayService.Instance.ServerConfig;
            Debug.Log("Server Id: " + serverConfig.ServerId);
            Debug.Log("Allocation Id: " + serverConfig.AllocationId);
            Debug.Log("Port: " + serverConfig.Port);
            Debug.Log("IP: " + serverConfig.IpAddress);
            Debug.Log("QueryPort: " + serverConfig.QueryPort);
            Debug.Log("LogDirectory: " + serverConfig.ServerLogDirectory);
            
            string ip4Address = "0.0.0.0";
            ushort port = serverConfig.Port;
            ConnectionData = new ConnectionData()
            {
                IP = ip4Address,
                Port = port
            };

            StartServer(true);
        }
        #endif

        private async void InitUnity()
        {
            InitializationOptions initializationOptions = new InitializationOptions();
            initializationOptions.SetProfile(Random.Range(1111, 9999).ToString());
            await UnityServices.InitializeAsync(initializationOptions);
            #if Client
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            #endif
        }
        
        #if Client
        /// <summary>
        /// Subscribes to events, creates connection payload for server and starting the client
        /// </summary>
        private void StartClient()
        {
            // Setting the connection data
            SetConnectionDataOnTransport(false);
            
            // Subscribe to Client Connect / Disconnect event
            NetworkManager.Singleton.OnConnectionEvent +=
                (NetworkManager networkManager, ConnectionEventData connectionEvent) =>
                {
                    // Only do something if we connect or disconnect
                    if (connectionEvent.ClientId != NetworkManager.Singleton.LocalClientId) return;
                    
                    switch (connectionEvent.EventType)
                    {
                        // On Connected to Server Event
                        case ConnectionEvent.ClientConnected:
                            Debug.Log("Client connected.");
                            break;
                        
                        // On Disconnected from Server Event
                        case ConnectionEvent.ClientDisconnected:
                            Debug.Log("Client disconnected. Reason: " + NetworkManager.Singleton.DisconnectReason);
                            break;
                    }
                };
            
            // Creating Connection Data, saving it
            // Set clientId to SteamId or something like that
            ulong clientId = (ulong) Random.Range(11111, 99999);
            NetworkManager.Singleton.NetworkConfig.ConnectionData = ConnectionPayload.Encode(clientId, Username);
            
            // Starting client and logging the client status
            if (NetworkManager.Singleton.StartClient())
                Debug.Log("Client started.");
            else
                Debug.Log("Failed to start client.");
        }
        #elif Server
        private async void StartServer(bool isMultiplayServer = false)
        {
            SetConnectionDataOnTransport(isMultiplayServer);
            
            // Subscribing to the connection approval event
            NetworkManager.Singleton.ConnectionApprovalCallback = ConnectionManager.OnClientJoinRequest;
            // Subscribe to Client Connect / Disconnect event
            NetworkManager.Singleton.OnConnectionEvent +=
                (NetworkManager networkManager, ConnectionEventData connectionEvent) =>
                {
                    switch (connectionEvent.EventType)
                    {
                        case ConnectionEvent.ClientConnected:
                            // We approved the client, so we mark him as joined
                            ConnectionManager.OnClientJoined(Client.GetClientByClientId(connectionEvent.ClientId));
                            break;
                        case ConnectionEvent.ClientDisconnected:
                            // If we registered the client, we remove the client
                            Client leftClient = Client.GetClientByClientId(connectionEvent.ClientId);
                            if (leftClient == null) return;
                            ConnectionManager.OnClientLeft(leftClient);
                            break;
                    }
                };

            // Starting the Server, logging the server status and starting tick System
            if (NetworkManager.Singleton.StartServer())
            {
                SimulationManager.StartTickSystem();
                Debug.Log("Server started.");

                await MultiplayService.Instance.ReadyServerForPlayersAsync();
            }
            else
                Debug.Log("Failed to start server.");
        } 
        #endif
    }
}