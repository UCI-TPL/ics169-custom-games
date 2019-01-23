using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StartButton : MonoBehaviour {
    public GameObject FirstObject, LoadingScreen;
    public Slider slider;
    void Start()
    {
        GameObject.Find("EventSystem").GetComponent<EventSystem>().SetSelectedGameObject(FirstObject);
    }
    public void ChangeScenes(int sceneIndex)
    {
        StartCoroutine(StartLoadingScene(sceneIndex));
    }

    IEnumerator StartLoadingScene(int sceneIndex)
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
