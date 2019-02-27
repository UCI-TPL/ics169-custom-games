using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missle_Script : MonoBehaviour {
    public GameObject explosion_prefab;
    public GameObject contact_plane;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnCollisionEnter(Collision collision)
    {
        //expload
        if(collision.gameObject == contact_plane)
        {
            GameObject exp_obj = Instantiate(explosion_prefab, transform.position, Quaternion.identity);
            Destroy(this.gameObject);
        }
        
    }
}
