using System;
using _Project.Scripts.Network.NetworkObjects;
using Unity.Netcode;

namespace _Project.Scripts.Network.Connection
{
    /// <summary>
    /// A static class that listens to ConnectionEvents and handles them.
    /// </summary>
    public static class ConnectionListener
    {
        /// <summary>
        /// Should be called from the CustomNetworkManager when a ConnectionEvent is triggered.
        /// </summary>
        /// <param name="networkManager"></param>
        /// <param name="eventData"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static void OnConnectionEvent(NetworkManager networkManager, ConnectionEventData eventData)
        {
            // Switching through the ConnectionEvent for easier handling.
            switch (eventData.EventType)
            {
                case ConnectionEvent.ClientConnected:
                    OnClientJoinedServerEvent(eventData);
                    break;
                case ConnectionEvent.ClientDisconnected:
                    OnClientLeaveServerEvent(eventData);
                    break;
                
                // Not used right now
                case ConnectionEvent.PeerConnected:
                    break;
                case ConnectionEvent.PeerDisconnected:
                    break;
                
                // Hopefully never used
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Is called from the OnConnectionEvent method when a client connects.
        /// </summary>
        /// <param name="eventData"></param>
        private static void OnClientJoinedServerEvent(ConnectionEventData eventData)
        {
            #if Server
            NetworkObjectSpawner.InstantiateClient(eventData.ClientId);
            #endif
        }
        
        /// <summary>
        /// Is called from the OnConnectionEvent method when a client disconnects.
        /// </summary>
        /// <param name="eventData"></param>
        private static void OnClientLeaveServerEvent(ConnectionEventData eventData)
        {
            
        }
    }
}