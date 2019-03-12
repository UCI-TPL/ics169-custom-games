using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Delete_After : MonoBehaviour {
    public float life_time;

	// Use this for initialization
	void Start () {
        Destroy(this.gameObject, life_time);
	}
	
	// Update is called once per frame
	void Update () {
        
	}
}
