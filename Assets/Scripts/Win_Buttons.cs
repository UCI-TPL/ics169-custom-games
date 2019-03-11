using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Win_Buttons : MonoBehaviour {
    public GameObject loadingScreen;
    public Slider loadingSlider;
    public GameObject Manager, Editor;
    public GameObject PlayAgain;

    private void Start()
    {
        Manager = GameObject.Find("GameManager");
        Editor = GameObject.Find("Editor");
        GameObject.Find("EventSystem").GetComponent<EventSystem>().SetSelectedGameObject(PlayAgain);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void RestartGame(int sceneIndex)
    {
        Manager.GetComponent<PlayerInformation>().reset = true;
        Manager.GetComponent<PlayerInformation>().currentState = PlayerInformation.DraftStates.P1_Pick_1;
        StartCoroutine(UsingLoadingBar(sceneIndex));
        Editor.GetComponent<HexagonMapEditor>().P1won = false;
        Editor.GetComponent<HexagonMapEditor>().P2won = false;


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
