using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

namespace _Project.Scripts.Connection
{
    /// <summary>
    /// A Custom Network Manager that inherits from the Unity Network Manager.
    /// It sets the Unity Transport, which is a required Component, as the default Network Transport.
    /// </summary>
    [RequireComponent(typeof(UnityTransport))]
    public class CustomNetworkManager : NetworkManager
    {
        private static CustomNetworkManager Instance { get; set; }
        
        // References
        private UnityTransport _transport;
        
        private void Start()
        {
            // Referencing the UnityTransport as default communication, so I don't have to set in the editor
            _transport = GetComponent<UnityTransport>();
            Singleton.NetworkConfig.NetworkTransport = _transport;
            
            // Setting the TickRate for the Network. Therefor RPCs are called with less/no delay.
            Singleton.NetworkConfig.TickRate = Settings.NetworkTickRate;

            // Setting the ConnectionApproval to false, so I don't need to handle that right now.
            Singleton.NetworkConfig.ConnectionApproval = false;
            
            // Singleton referencing
            Instance = this;
        }

        private void SetConnectionData(string ip, ushort port) => _transport.SetConnectionData(ip, port);
        
        #if Client
        /// <summary>
        /// Set's the IP and Port on the Network Transport and starts the client.
        /// </summary>
        /// <param name="ip">Used IP</param>
        /// <param name="port">Used Port</param>
        public static void StartClient(string ip, ushort port)
        {
            Instance.SetConnectionData(ip, port);
            if (Instance.StartClient())
            {
                Debug.Log("Successfully started the client.");
            }
        }
        #endif

        #if Server
        /// <summary>
        /// Set's the IP and Port on the Network Transport and starts the server.
        /// </summary>
        /// <param name="ip">Used IP</param>
        /// <param name="port">Used Port</param>
        public static void StartServer(string ip, ushort port)
        {
            Instance.SetConnectionData(ip, port);
            if (Instance.StartServer())
            {
                Debug.Log("Successfully started the server.");
            }
        }
        #endif
    }
}