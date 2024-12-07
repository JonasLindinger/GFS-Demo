using System.Collections.Generic;
using UnityEngine;

namespace _Project.Scripts.Network.NetworkObjects
{
    /// <summary>
    /// This class is responsible for spawning network objects.
    /// </summary>
    public class NetworkObjectSpawner : MonoBehaviour
    {
        // Singleton reference
        private static NetworkObjectSpawner Instance { get; set; }
        
        #if Server
        // ReSharper disable once CollectionNeverQueried.Local
        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        private static Dictionary<ulong, NetworkClient> _networkClients = new Dictionary<ulong, NetworkClient>();
        // ReSharper disable once CollectionNeverQueried.Local
        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        private static Dictionary<ulong, NetworkPlayer> _networkPlayers = new Dictionary<ulong, NetworkPlayer>();
        #endif

        // Inspector fields
        [Header("Network Objects")]
        [SerializeField] private NetworkClient _networkClient;
        [SerializeField] private NetworkPlayer _networkPlayer;

        private void Awake()
        {
            Instance = this;
        }

        #if Server
        /// <summary>
        /// Instantiate a new NetworkClient for the given client.
        /// The object can only be seen by the client and the server.
        /// </summary>
        /// <param name="clientId">References the client id</param>
        public static void InstantiateClient(ulong clientId)
        {
            var newClient = Instantiate(Instance._networkClient).NetworkObject;
            
            newClient.SpawnWithObservers = false;
            newClient.SceneMigrationSynchronization = false;
            newClient.ActiveSceneSynchronization = false;
            newClient.SynchronizeTransform = false;
            newClient.AutoObjectParentSync = false;
            newClient.SyncOwnerTransformWhenParented = false;
            
            newClient.SpawnWithOwnership(clientId);
            newClient.NetworkShow(clientId);
            
            _networkClients.Add(clientId, newClient.GetComponent<NetworkClient>());
        }
        
        /// <summary>
        /// Instantiate a new NetworkPlayer for the given client.
        /// The object can only be seen by every client and the server.
        /// </summary>
        /// <param name="clientId">References the client id</param>
        public static void InstantiatePlayer(ulong clientId)
        {
            var newPlayer = Instantiate(Instance._networkPlayer).NetworkObject;
            
            newPlayer.SpawnWithObservers = true;
            newPlayer.SceneMigrationSynchronization = false;
            newPlayer.ActiveSceneSynchronization = false;
            newPlayer.SynchronizeTransform = false;
            newPlayer.AutoObjectParentSync = false;
            newPlayer.SyncOwnerTransformWhenParented = false;
            
            newPlayer.SpawnWithOwnership(clientId);
            
            _networkPlayers.Add(clientId, newPlayer.GetComponent<NetworkPlayer>());
        }
        #endif
    }
}