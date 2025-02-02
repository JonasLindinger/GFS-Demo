using LindoNoxStudio.Scenes;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

namespace LindoNoxStudio.Initialization
{
    public class Initializer : MonoBehaviour
    {
        private async void Start()
        {
            InitializationOptions initializationOptions = new InitializationOptions();
            initializationOptions.SetProfile(Random.Range(1111, 9999).ToString());
            await UnityServices.Instance.InitializeAsync();
            
            #if Client
            // Authenticated
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log("Authenticated Anonymously");
            #endif
            
            #if Client
            SceneManager.Instance.LoadScene((int) SceneIndexes.Menu, (int) SceneIndexes.Menu);
            #elif Server
            // Adding NetworkLayer to the Scene loading queue
            SceneManager.Instance.AddSceneOperationToQueue(SceneOperationType.Loading, (int)SceneIndexes.NetworkLayer, (int)SceneIndexes.NetworkLayer);
            // Adding Game to the Scene loading queue
            SceneManager.Instance.AddSceneOperationToQueue(SceneOperationType.Loading, (int)SceneIndexes.Game);
            
            // Loading scene operations
            SceneManager.Instance.RunSceneOperations();
            #endif
        }
    }
}