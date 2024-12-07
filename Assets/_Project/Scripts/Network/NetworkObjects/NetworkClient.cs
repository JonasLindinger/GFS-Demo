using Unity.Netcode;

namespace _Project.Scripts.Network.NetworkObjects
{
    /// <summary>
    /// NetworkClient is a NetworkBehaviour that represents a client in the network and is used as a transport layer between the server and the client.
    /// </summary>
    public class NetworkClient : NetworkBehaviour
    {
        private void Start()
        {
            #if Server
            NetworkObjectSpawner.InstantiatePlayer(OwnerClientId);
            #endif
        }
    }
}