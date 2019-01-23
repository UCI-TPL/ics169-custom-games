using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class StartMenu : MonoBehaviour {
    public string ctr_num;
    //public string next_scene;
    public GameObject FirstObject, controlsMenu, maincanvas, creditsScreen;

    // Use this for initialization
    //void Start() {
    //       GameObject.Find("EventSystem").GetComponent<EventSystem>().SetSelectedGameObject(FirstObject);
    //   }
    //   public void ChangeScenes(string scene)
    //   {
    //       SceneManager.LoadScene(scene);
    //   }

    private void Update()
    {
        Back2Menu();
    }
    public void Exitgame()
    {
        Application.Quit();
    }

    public void How2Play()
    {
        GameObject.Find("EventSystem").GetComponent<EventSystem>().SetSelectedGameObject(FirstObject);
        controlsMenu.SetActive(true);
        maincanvas.SetActive(false);
    }

    public void Back2Menu()
    {
        if (Input.GetButtonDown("Cancel") && controlsMenu.activeInHierarchy)
        {
            controlsMenu.SetActive(false);
            maincanvas.SetActive(true);
            GameObject.Find("EventSystem").GetComponent<EventSystem>().SetSelectedGameObject(FirstObject);
        }
        
        else if(Input.GetButtonDown("Cancel") && creditsScreen.activeInHierarchy)
        {
            creditsScreen.SetActive(false);
            maincanvas.SetActive(true);
            GameObject.Find("EventSystem").GetComponent<EventSystem>().SetSelectedGameObject(FirstObject);
        }
        
    }

    public void Credits()
    {
        maincanvas.SetActive(false);
        creditsScreen.SetActive(true);
        GameObject.Find("EventSystem").GetComponent<EventSystem>().SetSelectedGameObject(FirstObject);
    }
}
