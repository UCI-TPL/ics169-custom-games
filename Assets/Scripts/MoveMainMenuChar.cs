using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveMainMenuChar : MonoBehaviour {
    private void Start()
    {
        //this.GetComponent<Animator>().SetBool("Activate", false);
    }
    public void MoveinPositiveDirection()
    {
        if(this.gameObject.transform.position.x >= 10)
        {
            //this.gameObject.GetComponent<Animator>().enabled = false;
            //this.gameObject.transform.position = new Vector3(10, this.gameObject.transform.position.y, this.gameObject.transform.position.z);
        }
    }

    public void MoveinNegativeDirection()
    {
        if (this.gameObject.transform.position.x <= -10)
        {
            this.gameObject.GetComponent<Animator>().SetBool("MovingForwards", false);
        }
    }
}
