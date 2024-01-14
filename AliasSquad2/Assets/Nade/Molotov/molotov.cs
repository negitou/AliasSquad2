using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class molotov : MonoBehaviour
{
    public float MEtime;            //エフェクトの持続時間 
    public float Marea;             //炎上範囲
    public float Mtime;             //炎上時間
    private float time;             //経過時間
    public float Mkan;              //炎上間隔
    public float Mdamage;           //炎上ダメージ
    public GameObject prefab_Msys;  //炎上エフェクト
    private bool flag = false;              //衝突判定
    private bool atk = false;               //攻撃判定
    ParticleSystem particle;


     void Start()
    {
        time = Mkan;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (flag == false)
        {
            flag = true;
            enjo();
        }   
    }

    private void enjo()
    {
        GameObject effect = Instantiate(prefab_Msys, transform.position, Quaternion.identity) as GameObject;    //パーティクル生成

        GetComponent<MeshRenderer>().enabled = false;       //本体を見えなくする

        atk = true;
        Destroy(effect, MEtime);
        Destroy(gameObject, MEtime);
    }

    private void Update()
    {
        
    }
}
