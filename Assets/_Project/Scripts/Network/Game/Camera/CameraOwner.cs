using System;
using LindoNoxStudio.Network.Simulation;
using Unity.Netcode;
using UnityEngine;

namespace LindoNoxStudio.Network.Game.Camera
{
    public class CameraOwner : NetworkBehaviour
    {
        #if Client
        
        [SerializeField] private CameraElement _cameraElementPrefab;
        
        private CameraElement _cameraElement;

        public override void OnNetworkSpawn()
        {
            _cameraElement = Instantiate(_cameraElementPrefab);
            _cameraElement.Register(IsOwner);
        }

        public void UpdateCam(Vector3 position, Quaternion rotation)
        {
            /*
            _cameraElement.transform.position = Vector3.Lerp(
                _cameraElement.transform.position, 
                position,
                SimulationManager.PhysicsTickSystem.TimeBetweenTicks * 3);
            */
            _cameraElement.transform.position = position;
            _cameraElement.transform.rotation = rotation;
        }

        #endif
    }
}