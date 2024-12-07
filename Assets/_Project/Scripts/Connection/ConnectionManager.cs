using System;
using _Project.Scripts.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.Connection
{
    /// <summary>
    /// A Component that connects the UI with the NetworkManager.
    /// </summary>
    public class ConnectionManager : MonoBehaviour
    {
        // Inspector Fields
        [Header("Input Fields")]
        [SerializeField] private TMP_InputField _usernameInputField;
        [SerializeField] private TMP_InputField _ipInputField;
        [SerializeField] private TMP_InputField _portInputField;
        [Space(5)]
        [SerializeField] private Button _connectButton; 
        
        private void Start()
        {
            #if Client
            try
            {
                if (Settings.UseAutoFillInputFieldsInputFields) AutoFillInputFields();
                
                UpdateButtonInteractableState();
                AddConnectionListener();
                AddInputFieldListeners();
            }
            catch (NullReferenceException e)
            {
                Debug.LogError($"Failed to connect the UI with the NetworkManager. A reference is missing. {e.Message}");
            }
            #elif Server
            CustomNetworkManager.StartServer("", Settings.DefaultPort);
            #endif
        }

        #if Client
        /// <summary>
        /// Fills the input fields with default values.
        /// </summary>
        private void AutoFillInputFields()
        {
            _usernameInputField.text = $"Guest_{UnityEngine.Random.Range(1111, 9999)}";
            _ipInputField.text = Settings.AutoFillIP;
            _portInputField.text = Settings.DefaultPort.ToString();
        }

        /// <summary>
        /// Adds a listener to the connect button to start the client or server.
        /// </summary>
        private void AddConnectionListener()
        {
            try
            {
                _connectButton.onClick.AddListener(() =>
                {
                    string username = _usernameInputField.text;
                    string ip = _ipInputField.text;
                    string port = _portInputField.text;
                    
                    if (!ValidateUserInput(username, ip, port))
                    {
                        Debug.LogError("Invalid input. Please check your input again.");
                        return;
                    }
                    
                    CustomNetworkManager.StartClient(ip, ushort.Parse(port));
                });   
            }
            catch (NullReferenceException e)
            {
                Debug.LogError($"Failed to add a onClick listener to the connection Button. A reference is missing. {e.Message}");
            }
        }

        /// <summary>
        /// Adds listeners to the input fields to update the button interactable state based on the text input.
        /// </summary>
        private void AddInputFieldListeners()
        {
            try
            {
                _usernameInputField.onValueChanged.AddListener((username) => UpdateButtonInteractableState());
                _ipInputField.onValueChanged.AddListener((ip) => UpdateButtonInteractableState());
                _portInputField.onValueChanged.AddListener((port) => UpdateButtonInteractableState());
            }
            catch (NullReferenceException e)
            {
                Debug.LogError($"Failed to add the input field listeners. A reference is missing. {e.Message}");
            }
        }
        
        /// <summary>
        /// Sets the button interactable state based on the text input. This helps the user to know when the input is valid.
        /// </summary>
        private void UpdateButtonInteractableState()
        {
            try
            {
                string username = _usernameInputField.text;
                string ip = _ipInputField.text;
                string port = _portInputField.text;
                
                _connectButton.interactable = ValidateUserInput(username, ip, port);
            }
            catch (NullReferenceException e)
            {
                Debug.LogError($"Failed to update the button interactable state. A reference is missing. {e.Message}");
            }
        }
        
        /// <summary>
        /// Validates the user input for the username, ip and port.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        private bool ValidateUserInput(string username, string ip, string port)
        {
            return Validator.IsValidName(username) && Validator.IsValidIP(ip) && Validator.IsValidPort(port);
        }
        #endif
    }
}