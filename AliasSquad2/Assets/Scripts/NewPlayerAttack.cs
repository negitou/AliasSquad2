using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.Netcode;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class NewPlayerAttack : NetworkBehaviour
{
    [SerializeField] private Transform firstPersonPlayer;
    //武器情報
    //
    //
    //弾 オブジェクト 
    [SerializeField] private GameObject bulletObj;
    //所持スロット
    private NetworkVariable<int> currentEquipSlotIndex = new NetworkVariable<int>();

    private Transform cameraTransform;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            return;
        }
        cameraTransform = transform.Find("RecoilObj").Find("PlayerCamera");
    }

    void Update()
    {
        if (!IsOwner)
        {
            return;
        }

        if (Input.GetMouseButton(0))
        {
            FireServerRpc(30, cameraTransform.position, cameraTransform.rotation);
        }
    }

    private void ChangeBefore()
    {

    }

    private void ChangePrev()
    {

    }

    private void ChangeNext()
    {

    }

    [ServerRpc]
    private void FireServerRpc(int damage, Vector3 position, Quaternion quaternion)
    {
        Fire(damage, position, quaternion);
    }

    private void Fire(int damage, Vector3 position, Quaternion quaternion)
    {
        var bullet = Instantiate(bulletObj, position, quaternion);

        bullet.GetComponent<Rigidbody>().AddForce(bullet.transform.forward * 100, ForceMode.VelocityChange);
        bullet.GetComponent<BulletController>().SetDefaultDamage(damage);
        bullet.GetComponent<BulletController>().SetHitObj = gameObject;

        bullet.GetComponent<NetworkObject>().Spawn();
    }
}