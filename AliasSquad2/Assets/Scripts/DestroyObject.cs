using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class DestroyObject : NetworkBehaviour {

	void Start () {
		Invoke ("ObjectDestroy", 5f);
	}
		
	private void ObjectDestroy(){
		Destroy (gameObject);
	}
}
