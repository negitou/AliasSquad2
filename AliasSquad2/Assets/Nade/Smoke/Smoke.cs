using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Smoke : NetworkBehaviour {

    public float Stime;             //持続時間
    public float Sarea;             //範囲
    public float StriggerTime;      //着弾後の時間
    private float Sdis;             //距離
    private float Dis;              //呼びかけ用

    private bool trigger1;

    private bool flag = false;              //衝突判定

    [SerializeField] private GameObject SParticle;

    ParticleSystem particle;
    public PlayerStatus playerStatus;

    // Use this for initialization
    void Start () {
        trigger1 = false;
	}


    private void OnCollisionEnter(Collision collision)
    {   

        if(flag == false)
        {
            
            flag = true;
            Invoke("moku", StriggerTime);
        }
    }

    private void moku()
    {
        trigger1 = true;

        SParticle.transform.parent = null;
        SParticle.SetActive(true);


        GetComponent<MeshRenderer>().enabled = false;       //本体を見えなくする

        StartCoroutine("mokuDel");

    }

    IEnumerator mokuDel()
    {
        yield return new WaitForSeconds(Stime);

        //playerStatus.TakeSmokeClientRpc(Dis);

        Destroy(gameObject);
    }


    private void Update()
    {
        if (!IsServer)
        {
            return;
        }

        if (trigger1) { 
        Collider[] targets = Physics.OverlapSphere(transform.position, Sarea); //自分から半径Barea以内のcolliderを格納

            foreach (Collider ply in targets)        //targets配列を順番に処理
            {
                if (ply.tag == "Player")             //タグがenemyであるとき
                {
                    playerStatus = ply.GetComponent<PlayerStatus>();

                    Vector3 v1 = transform.position;        //自分の位置

                    Vector3 v2 = ply.transform.position;    //相手の位置

                    Vector3 v3 = v2 - v1;                   //相手の方向

                    Ray ray = new Ray(transform.position, v3);         //相手に向かってrayを飛ばす

                    RaycastHit hit;                         //当たったオブジェクトの情報

                    if (Physics.Raycast(ray, out hit, Sarea))     //距離Bareaまでrayを飛ばし、当たったobject情報を格納
                    {
                        if (hit.collider.tag == "Player")         //tagがenemyの場合
                        {
                            Sdis = Mathf.Abs(Sarea - (v1 - v2).magnitude);
                            Dis = Mathf.Clamp(Mathf.Clamp01(Sdis) *4,0.3f,1);
                            playerStatus.TakeSmokeClientRpc(Dis);

                        }
                    }
                }
            }
        }
    }
}
