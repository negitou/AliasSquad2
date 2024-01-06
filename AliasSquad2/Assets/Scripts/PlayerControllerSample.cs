using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerControllerSample : NetworkBehaviour
{
    private static readonly string HORIZONTAL = "Horizontal";
    private static readonly string VERTICAL = "Vertical";
    public override void OnNetworkSpawn()
    {
        if (!IsLocalPlayer)
        {
            return;
        }
        transform.position = new Vector3(Random.Range(-5, 5), 1, Random.Range(-5, 5));
    }
    void Update()
    {
        if (!IsLocalPlayer)
        {
            return;
        }
        var moveX = Input.GetAxis(HORIZONTAL) * Time.deltaTime * 110.0f;
        var moveZ = Input.GetAxis(VERTICAL) * Time.deltaTime * 4f;

        transform.Rotate(0, moveX, 0);
        transform.Translate(0, 0, moveZ);
    }
}
