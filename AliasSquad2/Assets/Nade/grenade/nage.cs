using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class nage : MonoBehaviour {
    private int trg1;            //構え判定
    private int trg2;            //左右クリック判定
    public GameObject HEG;      //HEグレprefab
    public Transform hand1;     //投げ位置 左
    public Transform hand2;     //投げ位置 右
	public Transform shotRT;		//投げ向き
    public float GSL;           //投擲速度 左
    public float GSR;           //投擲速度 右

	// Use this for initialization
	void Start () {
        trg1 = 0;
        trg2 = 0;
	}
	
	// Update is called once per frame
	void Update () {
		
        if (Input.GetMouseButtonDown(0))
        {
            trg1 = 1;
            trg2 = 1;                       //左クリック
        }

        if (Input.GetMouseButtonUp(0))
        {
            trg1 = 2;
        }

        if (Input.GetMouseButtonDown(1))
        {
            trg1 = 1;
            trg2 = 2;                       //右クリック
        }

        if (Input.GetMouseButtonUp(1))
        {
            trg1 = 2;
        }

        if(trg1 == 2)
        {
            trg1 = 0;
            GameObject HEGs = Instantiate(HEG) as GameObject;

            Vector3 force;
            
            if(trg2 == 1)
            {
				force = shotRT.gameObject.transform.forward * GSL;
                HEGs.GetComponent<Rigidbody>().AddForce(force);
                HEGs.transform.position = hand1.position;
            }

            if(trg2 == 2)
            {
				force = shotRT.gameObject.transform.forward * GSR;
                HEGs.GetComponent<Rigidbody>().AddForce(force);
                HEGs.transform.position = hand2.position;
            }
        }
    }
}