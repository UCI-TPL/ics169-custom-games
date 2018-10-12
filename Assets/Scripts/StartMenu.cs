using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour {
    public string ctr_num;
	// Use this for initialization
	void Update () {
        ChangeScenes("Game");
	}
    public void ChangeScenes(string scene)
    {
        if (Input.GetButtonDown(ctr_num + " A Button"))
            SceneManager.LoadScene(scene);
    }
}
