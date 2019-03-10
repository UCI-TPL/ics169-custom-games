using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingLoadingScreenImg : MonoBehaviour {

    public GameObject background;
    public GameObject tip;
    public List<Sprite> backgroundImg;
    public List<string> TipText;
    public bool allowed;

	// Use this for initialization
	void Start () {
        allowed = true;
	}
	
	// Update is called once per frame
	void Update () {
        if (allowed)
        {
            allowed = false;
            SetImage();
        }
	}

    void SetImage()
    {
        int selectedImg = Random.Range(0, backgroundImg.Count);
        background.GetComponent<Image>().sprite = backgroundImg[selectedImg];
        tip.GetComponent<Text>().text = TipText[selectedImg];
    }

}
