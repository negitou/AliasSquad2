using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class HEbomb : NetworkBehaviour {

	public float Btime;								 //爆発までの時間
    public float Barea;       					     //爆発範囲
	public GameObject particle; 					 //爆破エフェクト
	[SerializeField] private float maxDamege;			 //最大ダメージ
	private int Bdamage;							 //ダメージ演算用の入れ物
	// Use this for initialization
	void Start () {
        StartCoroutine ("bom");
        
    }
	
	// Update is called once per frame
	IEnumerator bom() {

		yield return new WaitForSeconds (Btime);    //爆発待機

        particle.transform.parent = null;
        particle.SetActive(true);

        if (IsServer)
        {
            bomAttack();            //攻撃判定
        }
			
		Destroy (gameObject);                       //本体削除
	}

    
    private void bomAttack()   {

        Collider[] targets = Physics.OverlapSphere(transform.position, Barea); //自分から半径Barea以内のcolliderを格納

        foreach(Collider ply in targets)        //targets配列を順番に処理
        {
            if (ply.tag == "Player")             //タグがenemyであるとき
            {
                Vector3 v1 = transform.position;        //自分の位置

                Vector3 v2 = ply.transform.position;    //相手の位置
                    
                Vector3 v3 = v2 - v1;                   //相手の方向

                Ray ray =  new Ray(transform.position, v3);         //相手に向かってrayを飛ばす

                RaycastHit hit;                         //当たったオブジェクトの情報

                if(Physics.Raycast(ray,out hit, Barea))     //距離Bareaまでrayを飛ばし、当たったobject情報を格納
                {
                    if(hit.collider.tag == "Player")         //tagがenemyの場合
                    {
						
                        //ダメージ処理
						Bdamage =Mathf.FloorToInt( maxDamege * (Barea - (v1 - v2).magnitude) /Barea);　//ダメージ減衰式

						if (Bdamage > 0) {											//ダメージが0以上の時
							PlayerStatus obj = ply.GetComponent<PlayerStatus> ();
							obj.TakeDamage(Bdamage);     
						}
                    }
                }
            }
        }
    }
}