using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class C4bomb : NetworkBehaviour {

    public static C4bomb c4bomb;

    public NetworkVariable<int> c4State = new NetworkVariable<int>();//現在の状態 0で未使用 1で起動済み 2で解除中 3で解除済み 4爆破
    public float c4Time;            //爆発までの時間
    public float c4Area;            //爆発範囲
    public float c4Etime;           //エフェクトの持続時間
    public GameObject c4Effect;     //爆発エフェクト

    [SerializeField]private float maxDamege;    //最大ダメージ
    private int c4Damage;        //ダメージ演算用の入れ物

    private bool startFlag = false; //起動呼び出し用
    private bool diffuseFlag = false;

    ParticleSystem particle;

    Coroutine c4start;

    private void Start()
    {
        c4bomb = this;
    }

    void Update()
    {
        switch (c4State.Value)
        {

            case 1:
                if (!startFlag)
                {
                    startFlag = true;
                    c4start = StartCoroutine(C4start());
                }
                break;


            case 2:

                break;


            case 3:
                if (!diffuseFlag)
                {
                    diffuseFlag = true;
                    StopCoroutine(c4start);
                }
                break;

        }

    }

    IEnumerator C4start()
    {
        //ここに残り時間をC4timeに置き換える記述を記入してください

        //爆発待機
        yield return new WaitForSeconds(c4Time);

        c4State.Value = 4;

        //パーティクル生成
        c4Effect.transform.parent = null;
        c4Effect.SetActive(true);

        //攻撃判定
        C4Attack();

        c4bomb = null;


        //本体削除
        yield return new WaitForSeconds(3.0f);
        Destroy(gameObject);

    }



    public void SetC4State(int num)
    {
        c4State.Value = num;
    }

    private void C4Attack()
    {
        //自分から半径C4area以内のcolliderをtargetsに格納
        Collider[] targets = Physics.OverlapSphere(transform.position, c4Area);

        //targets配列を順番に処理
        foreach (Collider ply in targets)
        {
            //タグがenemyであるとき
            if (ply.tag == "Player")
            {
                //相手の方向を取得
                Vector3 v1 = ply.transform.position - transform.position;

                //相手に向かってrayを飛ばす
                Ray ray = new Ray(transform.position, v1);

                //当たったオブジェクトの情報の入れ物
                RaycastHit hit;

                //距離C4areaまでrayを飛ばし,当たったobject情報を格納
                if (Physics.Raycast(ray, out hit, c4Area))
                {
                    //tagがenemyの場合
                    if (hit.collider.tag == "Player")
                    {
                        //ダメージ処理

                        //ダメージ減衰式
                        c4Damage = Mathf.FloorToInt(maxDamege * (c4Area - (transform.position - ply.transform.position).magnitude) / c4Area);

                        //ダメージが0以上の時
                        if (c4Damage > 0)
                        {
                            PlayerStatus obj = ply.GetComponent<PlayerStatus>();

                            obj.TakeDamage(c4Damage);

                        }

                    }

                }

            }

        }

    }
}
