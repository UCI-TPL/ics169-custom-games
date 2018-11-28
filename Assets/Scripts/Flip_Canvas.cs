using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flip_Canvas : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        flip_if_neg();
	}

    public void flip_if_neg()
    {
        if (Mathf.Sign(this.transform.parent.localScale.x) == -1 && Mathf.Sign(this.transform.localScale.x) == 1)
        {
            this.transform.localScale = new Vector3(this.transform.localScale.x * -1, this.transform.localScale.y, this.transform.localScale.z);
        }
        else
        {
            if (Mathf.Sign(this.transform.parent.localScale.x) == 1 && Mathf.Sign(this.transform.localScale.x) == -1)
            {
                this.transform.localScale = new Vector3(this.transform.localScale.x * -1, this.transform.localScale.y, this.transform.localScale.z);
            }
        }
    }
}
