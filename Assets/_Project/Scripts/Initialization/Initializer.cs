using LindoNoxStudio.Scenes;
using UnityEngine;

namespace LindoNoxStudio.Initialization
{
    public class Initializer : MonoBehaviour
    {
        private void Start()
        {
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