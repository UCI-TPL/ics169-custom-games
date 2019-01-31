using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssigningStats : MonoBehaviour {
    public GameObject manager, ranger,tank,melee;
	// Use this for initialization
	void Awake () {
        manager = GameObject.Find("GameManager");
        ranger = GameObject.Find("Ranger_Highlight");
        
	}

    private void Start()
    {
        
    }

    // Update is called once per frame
    void Update () {
		
	}
}
