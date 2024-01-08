using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerPart : NetworkBehaviour {

    [SerializeField]private GameObject playerObj;

    [SerializeField] private float damageMultiplier;

    public GameObject GetPlayerObj {get { return playerObj; } }

    private void Reset()
    {
        playerObj = GameObject.Find("Player");
    }

    public void PlayerHit(float damage)
    {
        Debug.Log(damageMultiplier.ToString() + ":" + damage.ToString());
        playerObj.GetComponent<PlayerStatus>().TakeDamage(Mathf.FloorToInt(damage * damageMultiplier));
    }

}
