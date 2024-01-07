using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class flash : NetworkBehaviour {
    public float Ftime;                             //発光までの時間
    public float FEtime;                            //エフェクトの持続時間
    public float Farea;                             //発光範囲
    private float sa;                               //角度の差
    
    public PlayerStatus playerStatus;
	// Use this for initialization
	void Start () {
        StartCoroutine("pika");
	}
	
    IEnumerator pika()
    {
        yield return new WaitForSeconds(Ftime);     //爆発待機


        flashAttack();                              //目潰し判定
        


        Destroy(gameObject);                        //本体削除
    }

    private void flashAttack(){

        Collider[] targets = Physics.OverlapSphere(transform.position, Farea, 1 << 8);  //自分から半径Farea以内のcolliderを格納

        foreach(Collider ply in targets)                        //targets配列を順番に処理
        {
            if(ply.tag == "Player")                              //タグがenemyであるとき
            {
                playerStatus = ply.GetComponent<PlayerStatus>();

                Vector3 v1 = transform.position;                //自分の位置

                Vector3 v2 = ply.transform.position;            //相手の位置

                Vector3 v3 = v2 - v1;                           //自分から見た相手の方向
                v3.Normalize();
                Vector3 v4 = v3.normalized + ply.transform.forward;        //角度の差分

                Ray ray = new Ray(transform.position, v3);      //相手に向かってRayを飛ばす

                RaycastHit hit;                                 //当たったオブジェクトの情報

                if(Physics.Raycast(ray,out hit, Farea))         //距離Fareaまでrayを飛ばし、当たったpbject情報を格納
                {
                    if(hit.collider.tag == "Player")             //tagがenemyの時
                    {
                        float playerDeg = Mathf.Rad2Deg * Mathf.Atan2(ply.transform.forward.x, ply.transform.forward.z);
                        float nadeDeg = Mathf.Rad2Deg * Mathf.Atan2(-v3.x, -v3.z);

                        sa = Mathf.Abs(nadeDeg - playerDeg);
                        if(sa > 180)
                        {
                            sa = 360 - sa;
                        }
                        playerStatus.TakeFlashClientRpc(sa);
    
                    }
                }
            }
        }
    }
}
