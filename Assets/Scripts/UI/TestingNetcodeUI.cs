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
            KitchenGameMultiplayer.Instance.StartHost();
            Hide();
        });
          startClientButton.onClick.AddListener(() =>
        {
            Debug.Log("Client");
            KitchenGameMultiplayer.Instance.StartClient();
            Hide();
        });

    }
    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
