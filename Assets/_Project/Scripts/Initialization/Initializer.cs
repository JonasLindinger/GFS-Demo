using LindoNoxStudio.Scenes;
using UnityEngine;

namespace LindoNoxStudio.Initialization
{
    public class Initializer : MonoBehaviour
    {
        private void Start()
        {
            SceneManager.Instance.LoadScene((int) SceneIndexes.Menu, (int) SceneIndexes.Menu);
        }
    }
}