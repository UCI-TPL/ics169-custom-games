using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartUnit : MonoBehaviour {

    public int mobility; // how far a unit can move
    public int attackRange; // how far a unit can attack
    public int health;

    public bool dead = false;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if(dead)
            StartCoroutine(Dead());
	}

    IEnumerator Dead()
    {
        yield return new WaitForSeconds(0.5f);
        Destroy(this.gameObject);
    
    }
}
