using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Text_Pulse : MonoBehaviour {

    public float timeScale;
    public float maxScale;
    public float minScale;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        GetComponent<Text>().rectTransform.localScale = new Vector3(Mathf.PingPong(Time.time * timeScale, maxScale) + minScale, Mathf.PingPong(Time.time * timeScale, maxScale) + minScale, 1);
	}
}
