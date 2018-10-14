using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour {
    public string ctr_num;
    public string next_scene;
	// Use this for initialization
	void Update () {
        ChangeScenes(next_scene);
        Exitgame();
	}
    public void ChangeScenes(string scene)
    {
       //if(Input.GetButtonDown(ctr_num + " A Button"))
        if (Input.GetMouseButtonDown(0))
            SceneManager.LoadScene(scene);

    }
    public void Exitgame()
    {
        //if(Input.GetButtonDown(ctr_num + " A Button"))
        if (Input.GetMouseButtonDown(0))
            Application.Quit();
    }
}
