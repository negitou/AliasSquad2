using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameQuitSystem : MonoBehaviour {
	public GameObject FirstLoadStage;
	public GameObject OffLoadStage1;
	public GameObject OffLoadStage2;
	public GameObject OffLoadStage3;
	public GameObject OffLoadStage4;
	public GameObject OffLoadStage5;
	void Start () {
		FirstLoadStage.SetActive (true);
		OffLoadStage1.SetActive (false);
		OffLoadStage2.SetActive (false);
		OffLoadStage3.SetActive (false);
		OffLoadStage4.SetActive (false);
		OffLoadStage5.SetActive (false);
	}

}
