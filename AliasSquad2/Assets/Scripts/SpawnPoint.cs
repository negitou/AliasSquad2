using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class SpawnPoint : NetworkBehaviour {

    private bool spawnPointEnabled = true;

    void Start () {
		
	}
	
	void Update () {
		
	}

    private void OnTriggerStay(Collider other)
    {
        if (!IsServer)
        {
            return;
        }

        if(other.tag == "Player")
        {
            spawnPointEnabled = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!IsServer)
        {
            return;
        }

        if (other.tag == "Player")
        {
            spawnPointEnabled = true;
        }
    }
}
