using Unity.Netcode;
using UnityEngine;

namespace LindoNoxStudio.Network.Game.Camera
{
    public class CameraElement : MonoBehaviour
    {
        #if Client
        private bool _isRegistered = false;
        
        private void OnDestroy()
        {
            RemoveRegistration();
        }

        /// <summary>
        /// Adding this camera element to list
        /// </summary>
        public void Register(bool isOwner)
        {
            if (_isRegistered) return;
            _isRegistered = true;
            CameraManager.Instance.AddCameraElement(this, isOwner);
        }
        
        /// <summary>
        /// Removing this camera element from list
        /// </summary>
        private void RemoveRegistration()
        {
            if (!_isRegistered) return;
            _isRegistered = false;
            CameraManager.Instance.RemoveCameraElement(this);
        }
        #endif
    }
}