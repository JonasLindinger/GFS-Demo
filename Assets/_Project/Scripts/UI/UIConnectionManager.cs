using LindoNoxStudio.Network.Connection;
using LindoNoxStudio.Scenes;
using TMPro;
using UnityEngine;

namespace _Project.Scripts.UI
{
    public class UIConnectionManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TMP_InputField _ipText;
        [SerializeField] private TMP_InputField _portText;
        [SerializeField] private TMP_InputField _usernameText;
        
        public void ConnectionBtn()
        {
            SetConnectionData(_ipText.text, ushort.Parse(_portText.text));
            Connect();
        }
        
        private void SetConnectionData(string ip, ushort port)
        {
            NetworkStarter.ConnectionData = new ConnectionData() 
                { 
                    IP = ip, 
                    Port = port 
                };
            
            NetworkStarter.Username = _usernameText.text;
        }
        
        private void Connect()
        {
            // Adding NetworkLayer to the Scene loading queue
            SceneManager.Instance.AddSceneOperationToQueue(SceneOperationType.Loading, (int)SceneIndexes.NetworkLayer, (int)SceneIndexes.NetworkLayer);
            // Adding Game to the Scene loading queue
            SceneManager.Instance.AddSceneOperationToQueue(SceneOperationType.Loading, (int)SceneIndexes.Game);
            
            // Loading scene operations
            SceneManager.Instance.RunSceneOperations();
        }
    }
}