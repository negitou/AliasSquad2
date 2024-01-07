using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;



public class DontDestroyObject : MonoBehaviour {

    [SerializeField] GameObject[] gameObjects;

    [SerializeField] string nextScene;

    private void Awake()
    {
        for (int i = 0; i < gameObjects.Length; i++)
        {
            DontDestroyOnLoad(gameObjects[i]);

        }
    }

    void Start () {
        SceneManager.LoadScene(nextScene);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
