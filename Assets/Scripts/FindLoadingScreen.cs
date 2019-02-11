using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FindLoadingScreen : MonoBehaviour {
    public GameObject manager;
	// Use this for initialization
	void Awake () {
        manager = GameObject.Find("GameManager");
        manager.GetComponent<PlayerInformation>().loadingScreen = GameObject.Find("LoadingScreen");
        manager.GetComponent<PlayerInformation>().loadingSlider = manager.GetComponent<PlayerInformation>().loadingScreen.GetComponentInChildren<Slider>();
        manager.GetComponent<PlayerInformation>().selectionSounds = FindObjectOfType<SelectionSoundManager>();

    }

    private void Start()
    {
        manager.GetComponent<PlayerInformation>().loadingScreen.SetActive(false);
    }

    // Update is called once per frame
    void Update () {
		
	}
}
