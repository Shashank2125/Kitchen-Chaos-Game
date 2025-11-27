using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using System.Runtime.CompilerServices;

public class TestingNetcodeUI : MonoBehaviour
{
    [SerializeField] private Button startHostButton;
    [SerializeField] private Button startClientButton;
    private void Awake()
    {
        startHostButton.onClick.AddListener(() =>
        {
            Debug.Log("Host");
            NetworkManager.Singleton.StartHost();
            Hide();
        });
          startClientButton.onClick.AddListener(() =>
        {
            Debug.Log("Client");
            NetworkManager.Singleton.StartClient();
            Hide();
        });

    }
    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
