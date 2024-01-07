using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class BulletController : NetworkBehaviour {

    private NetworkVariable<float> defaultDamage = new NetworkVariable<float>();
    private float currentDamage;

    public NetworkVariable<GameObject> firePlayer = new NetworkVariable<GameObject>();

    private GameObject hitObj;
    [SerializeField] private GameObject bloodParticle;
    [SerializeField] private GameObject woodParticle;
    [SerializeField] private GameObject metalParticle;

    public GameObject SetHitObj { set{ hitObj = value; } }

    private float time;

    Vector3 position;
    Quaternion rotation;


    void Start () {
        currentDamage = defaultDamage.Value;
        if (IsServer)
        {
            firePlayer.Value = hitObj;
        }
        Invoke("ObjectDestroy",3f);
	}

    private void Update()
    {
        if (!IsServer)
        {
            return;
        }
        time += Time.deltaTime;
        if (time > 2)
        {
            //NetworkServer.Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        if(!IsServer)
        {
            return;
        }

        if (col.gameObject.layer == 8)
        {
            return;
        }

        RaycastHit hit;

        
        if (col.gameObject.layer == 9)
        {
            PlayerPart playerPart = col.GetComponent<PlayerPart>();
            if (hitObj != playerPart.GetPlayerObj)
            {
                LayerMask layerMask = LayerMask.GetMask(new string[] { "Dmg Collision" });
                if (Physics.Raycast(transform.position - transform.forward * 2, transform.forward, out hit, 200f))
                {
                    rotation = Quaternion.FromToRotation(Vector3.forward, hit.normal);
                    position = hit.point;
                }

                var particle = (GameObject)Instantiate(bloodParticle, position, rotation);

                hitObj = playerPart.GetPlayerObj;
                playerPart.PlayerHit(currentDamage);
                currentDamage = (int)(currentDamage * 0.6f);
            }
        }
        else if (col.tag == "Wood")
        {
            LayerMask layerMask = LayerMask.GetMask(new string[] { "Object" });
            if (Physics.Raycast(transform.position - transform.forward * 2, transform.forward, out hit, 200f, layerMask))
            {
                rotation = Quaternion.FromToRotation(Vector3.forward, hit.normal);
                position = hit.point;
            }

            var particle = (GameObject)Instantiate(woodParticle, position, rotation);
            //NetworkServer.Spawn(particle);

            currentDamage = (int)(currentDamage * 0.4f);
        }
        else if (col.tag == "Metal")
        {
            LayerMask layerMask = LayerMask.GetMask(new string[] { "Object" });
            if (Physics.Raycast(transform.position - transform.forward * 2, transform.forward, out hit, 200f, layerMask))
            {
                rotation = Quaternion.FromToRotation(Vector3.forward, hit.normal);
                position = hit.point;
            }

            var particle = (GameObject)Instantiate(metalParticle, position, rotation);
            //NetworkServer.Spawn(particle);

            if (currentDamage >= 20)
            {
                currentDamage = (int)(currentDamage * 0.2f);
            }
            else
            {
                currentDamage = (int)(currentDamage * 0f);
            }
        }
        else if (col.tag == "BulletProof")
        {
            LayerMask layerMask = LayerMask.GetMask(new string[] { "Object" });
            if (Physics.Raycast(transform.position - transform.forward * 2, transform.forward, out hit, 200f, layerMask))
            {
                rotation = Quaternion.FromToRotation(Vector3.forward, hit.normal);
                position = hit.point;
            }

            var particle = (GameObject)Instantiate(metalParticle, position, rotation);
            //NetworkServer.Spawn(particle);

            currentDamage = (int)(currentDamage * 0f);
        }

        if (currentDamage <= 0)
        {
            //NetworkServer.Destroy(gameObject);
        }
    }

    public void SetDefaultDamage(int amount)
    {
        if (!IsServer)
        {
            return;
        }

        if (defaultDamage.Value == 0)
        {
            defaultDamage.Value = amount;

            RaycastHit hit;

            LayerMask layerMask = LayerMask.GetMask(new string[] { "Object" });
            if (Physics.Raycast(transform.position - transform.forward * 2, transform.forward, out hit, 200f, layerMask))
            {
                rotation = Quaternion.FromToRotation(Vector3.forward, hit.normal);
                position = hit.point;
            }
        }
    }
}
