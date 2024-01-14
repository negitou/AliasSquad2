using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class TeamManager : NetworkBehaviour {

    public static TeamManager teamManager;

    public const int maxTeamMember = 5;

    [SerializeField] public GameObject[] gameObjectAT = new GameObject[maxTeamMember];
    [SerializeField] public GameObject[] gameObjectFT = new GameObject[maxTeamMember];

    [SerializeField] private GameObject scoreboard;

    private void Awake()
    {
        teamManager = this;
    }

    private void Update()
    {
        if (IsServer)
        {
            SetScoreboardClientRpc();
        }
    }

    [ClientRpc]
    void SetScoreboardClientRpc()
    {
        for (int i = 0; i < maxTeamMember; i++)
        {
            if (gameObjectAT[i])
            {
                //scoreboard.transform.Find("");
            }

            if (gameObjectFT[i])
            {
                
            }
        }
    }

    public void SetGameObject(GameObject player)
    {
        for (int i = 0; i < maxTeamMember; i++)
        {
            if (!gameObjectAT[i])
            {
                gameObjectAT[i] = player;
                gameObjectAT[i].GetComponent<PlayerManager>().TeamSelect(1);
                break;
            }
            else if (!gameObjectFT[i])
            {
                gameObjectFT[i] = player;
                gameObjectFT[i].GetComponent<PlayerManager>().TeamSelect(2);
                break;
            }
        }
    }
}
