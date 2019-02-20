using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Win_Buttons : MonoBehaviour {
    public GameObject loadingScreen;
    public Slider loadingSlider;
    public GameObject Manager;

    private void Start()
    {
        Manager = GameObject.Find("GameManager");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void RestartGame(int sceneIndex)
    {
        StartCoroutine(UsingLoadingBar(sceneIndex));
        Manager.GetComponent<PlayerInformation>().reset = true;
        Manager.GetComponent<PlayerInformation>().currentState = PlayerInformation.DraftStates.P1_Pick_1;
    }

    IEnumerator UsingLoadingBar(int sceneIndex)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);//prepares the upcoming scene in the background
        loadingScreen.SetActive(true);
        while (!operation.isDone)
        {
            loadingSlider.value = Mathf.Clamp01(operation.progress / .9f);//creates the percentage for the loading bar
            yield return null;
        }
    }
}
