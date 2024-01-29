using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkHUD : MonoBehaviour
{
    [SerializeField] Button _hostButton;
    [SerializeField] Button _clientButton;

    [SerializeField] GameObject InGameManager;

    void Start()
    {
        _hostButton.onClick.AddListener(StartHost);
        _clientButton.onClick.AddListener(StartClient);
        NetworkManager.Singleton.OnServerStarted += () => Instantiate(InGameManager).GetComponent<NetworkObject>().Spawn();
        NetworkManager.Singleton.OnClientStarted += () => GetComponent<Canvas>().enabled = false;
        NetworkManager.Singleton.OnClientStopped += (b) => GetComponent<Canvas>().enabled = true;
    }

    public void StartHost()
    {
        NetworkManager.Singleton.StartHost();
        
    }

    public void StartClient()
    {
        NetworkManager.Singleton.StartClient();
    }
}
