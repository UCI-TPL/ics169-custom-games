using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Portrait_Script : MonoBehaviour {
    public Image Portrait;
    public Image Portrait_BG;
    public bool In_Use;

    

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Change_portrait(Sprite _new_image)
    {
        Portrait.sprite = _new_image;
    }
}
