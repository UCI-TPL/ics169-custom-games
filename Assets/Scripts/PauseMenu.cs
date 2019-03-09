using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour {
    public GameObject pause_menu;
    public GameObject editor;
    public GameObject cursor;
    public GameObject controls_menu;
    public GameObject FirstObject;
    public GameObject ResumeButton, ControlsButton, QuitButton;
    public PlayerInformation Manager;
    public GameObject LoadingScreen;
    public Slider slider;
    // Use this for initialization
    void Awake () {
        LoadingScreen = GameObject.Find("LoadingScreen");
        slider = LoadingScreen.gameObject.GetComponentInChildren<Slider>();

    }

    private void Start()
    {
        LoadingScreen.SetActive(false);
        controls_menu.SetActive(false);
    }

    // Update is called once per frame
    void Update () {
		if(Input.GetButtonDown("J1 Start Button") || Input.GetButtonDown("J2 Start Button"))
        {
            if(pause_menu.gameObject.activeInHierarchy == false)
            {
                pause_menu.gameObject.SetActive(true);
                editor.gameObject.SetActive(false); //turns off editor so that players cant select units while the pause menu is open
                cursor.gameObject.SetActive(false);// turns off cursor so that it stays in the position before the pause
                ResumeButton.gameObject.GetComponent<PauseMenu>().LoadingScreen = editor.gameObject.GetComponent<PauseMenu>().LoadingScreen;
                ControlsButton.gameObject.GetComponent<PauseMenu>().LoadingScreen = editor.gameObject.GetComponent<PauseMenu>().LoadingScreen;
                QuitButton.gameObject.GetComponent<PauseMenu>().LoadingScreen = editor.gameObject.GetComponent<PauseMenu>().LoadingScreen;
                GameObject.Find("EventSystem").GetComponent<EventSystem>().SetSelectedGameObject(FirstObject);
                Time.timeScale = 0;
            }
            else
            {
                editor.gameObject.SetActive(true);
                cursor.gameObject.SetActive(true);
                pause_menu.gameObject.SetActive(false);
                Time.timeScale = 1;
            }
        }
    }

    public void Options()
    {
        controls_menu.gameObject.SetActive(true);
        pause_menu.gameObject.SetActive(false);
        //controls_menu.gameObject.GetComponent<PauseMenu>().inside_controls_menu = true;
        GameObject.Find("EventSystem").GetComponent<EventSystem>().SetSelectedGameObject(FirstObject);
    }

    public void GoBack()
    {
        if ((Input.GetButtonDown("J1 B Button") || Input.GetButtonDown("J2 B Button")) && controls_menu.activeInHierarchy)
        {
            pause_menu.gameObject.SetActive(true);
            controls_menu.gameObject.SetActive(false);
            GameObject.Find("EventSystem").GetComponent<EventSystem>().SetSelectedGameObject(FirstObject);
        }

        else if ((Input.GetButtonDown("J1 B Button") || Input.GetButtonDown("J2 B Button")) && pause_menu.activeInHierarchy)
        {
            editor.SetActive(true);
            cursor.SetActive(true);
            pause_menu.gameObject.SetActive(false);
            Time.timeScale = 1;
            //GameObject.Find("EventSystem").GetComponent<EventSystem>().SetSelectedGameObject(FirstObject);
        }
    }

    public void Resume()
    {
        editor.SetActive(true);
        cursor.SetActive(true);
        pause_menu.gameObject.SetActive(false);
        Time.timeScale = 1;
    }

    public void QuitGame(int sceneIndex)
    {
        StartCoroutine(Send2MainMenu(sceneIndex));
        Manager.reset = true;
        Manager.currentState = PlayerInformation.DraftStates.P1_Pick_1;
        Time.timeScale = 1;
    }

    IEnumerator Send2MainMenu(int sceneIndex)
    {
        AsyncOperation async = SceneManager.LoadSceneAsync(sceneIndex);
        LoadingScreen.SetActive(true);
        while (!async.isDone)
        {
            slider.value = Mathf.Clamp01(async.progress / .9f);
            yield return null;
        }
    }
}
