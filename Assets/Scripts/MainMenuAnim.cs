using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuAnim : MonoBehaviour {
    public List<GameObject> left_units;
    public List<GameObject> right_units;

    public bool play,resetAllowed;
    public int num,speed;
    public int timeAmount;
    public GameObject offScreen;
	// Use this for initialization
	void Start () {
        num = 0;
        play = true;
        resetAllowed = false;
	}
	
	// Update is called once per frame
	void Update () {
        //if (resetAllowed)
        //{
        //    resetAllowed = false;
        //    StartCoroutine(Reset(num));
        //    //play = true;

        //}
        if (play)
        {
            play = false;
            StartCoroutine(AnimationPlayer(num));
        }
	}

    IEnumerator AnimationPlayer(int index)
    {
        yield return new WaitForSeconds(1f);
        left_units[index].GetComponentInChildren<StartUnit>().anim.SetBool("Attacking", true);
        yield return new WaitForSeconds(1f);

        left_units[index].GetComponentInChildren<StartUnit>().anim.SetBool("Attacking", false);
        right_units[index].GetComponentInChildren<StartUnit>().anim.SetBool("Hurt", true);

        yield return new WaitForSeconds(.4f);

        right_units[index].GetComponentInChildren<StartUnit>().anim.SetBool("Hurt", false);
        right_units[index].GetComponent<Animator>().SetBool("Start", true);//reset

        yield return new WaitForSeconds(1f);
        right_units[index].GetComponent<Animator>().SetBool("Start", false);//reset
        left_units[index].GetComponentInChildren<StartUnit>().anim.SetBool("Attacking", false);
        right_units[index].GetComponentInChildren<StartUnit>().anim.SetBool("Hurt", false);

        right_units[index + 1].GetComponentInChildren<StartUnit>().anim.SetBool("Moving", true);
        right_units[index + 1].GetComponent<Animator>().SetBool("Start", true);//reset
        yield return new WaitForSeconds(1f);
        right_units[index + 1].GetComponent<Animator>().SetBool("Start", false);
        right_units[index + 1].GetComponentInChildren<StartUnit>().anim.SetBool("Moving", false);
        right_units[index + 1].GetComponentInChildren<StartUnit>().anim.SetBool("Attacking", true);
        yield return new WaitForSeconds(1f);
        left_units[index].GetComponentInChildren<StartUnit>().anim.SetBool("Hurt", true);
        yield return new WaitForSeconds(.2f);
        left_units[index].GetComponentInChildren<StartUnit>().anim.SetBool("Hurt", false);//
        right_units[index + 1].GetComponentInChildren<StartUnit>().anim.SetBool("Attacking", false);
        left_units[index].GetComponent<Animator>().SetBool("Start", true);//reset

        yield return new WaitForSeconds(1f);

        left_units[index].GetComponent<Animator>().SetBool("Start", false);
        left_units[index + 1].GetComponentInChildren<StartUnit>().anim.SetBool("Moving", true);
        left_units[index + 1].GetComponent<Animator>().SetBool("Start", true);//reset
        yield return new WaitForSeconds(1f);
        left_units[index + 1].GetComponent<Animator>().SetBool("Start", false);
        left_units[index + 1].GetComponentInChildren<StartUnit>().anim.SetBool("Moving", false);
        left_units[index + 1].GetComponentInChildren<StartUnit>().anim.SetBool("Attacking", true);
        yield return new WaitForSeconds(1);
        left_units[index + 1].GetComponentInChildren<StartUnit>().anim.SetBool("Attacking", false);
        right_units[index + 1].GetComponentInChildren<StartUnit>().anim.SetBool("Hurt", true);
        yield return new WaitForSeconds(.4f);
        right_units[index + 1].GetComponentInChildren<StartUnit>().anim.SetBool("Hurt", false);
        yield return new WaitForSeconds(.3f);
        right_units[index + 1].GetComponent<Animator>().SetBool("goBack", true);//reset
        right_units[index].GetComponent<Animator>().SetBool("Activate", true);//reset
        yield return new WaitForSeconds(1f);
        right_units[index + 1].GetComponent<Animator>().SetBool("goBack", false);
        right_units[index].GetComponent<Animator>().SetBool("Activate", false);
        right_units[index].GetComponentInChildren<StartUnit>().anim.SetBool("Attacking", true);
        yield return new WaitForSeconds(1f);
        right_units[index].GetComponentInChildren<StartUnit>().anim.SetBool("Attacking", false);
        left_units[index + 1].GetComponentInChildren<StartUnit>().anim.SetBool("Hurt", true);
        yield return new WaitForSeconds(.3f);
        left_units[index + 1].GetComponentInChildren<StartUnit>().anim.SetBool("Hurt", false);
        left_units[index + 1].GetComponent<Animator>().SetBool("goBack", true);//reset
        yield return new WaitForSeconds(1f);
        left_units[index + 1].GetComponent<Animator>().SetBool("goBack", false);
        left_units[index].GetComponent<Animator>().SetBool("MoveForward", true);//reset
        yield return new WaitForSeconds(1f);
        left_units[index].GetComponent<Animator>().SetBool("MoveForward", false);
        //resetAllowed = true;
        play = true;

    }

    IEnumerator Reset(int index)
    {
        left_units[index].GetComponent<Animator>().SetBool("Reset", true);//reset
        left_units[index + 1].GetComponent<Animator>().SetBool("Reset", true);//reset
        right_units[index].GetComponent<Animator>().SetBool("Reset", true);//reset
        right_units[index + 1].GetComponent<Animator>().SetBool("Reset", true);//reset
        yield return new WaitForSeconds(1f);
        left_units[index].GetComponent<Animator>().SetBool("MoveForward", false);//reset
        left_units[index].GetComponent<Animator>().SetBool("Start", false);//reset
        left_units[index + 1].GetComponent<Animator>().SetBool("goBack", false);//reset
        left_units[index + 1].GetComponent<Animator>().SetBool("Start", false);//reset
        right_units[index].GetComponent<Animator>().SetBool("Activate", false);//reset
        right_units[index].GetComponent<Animator>().SetBool("Start", false);//reset
        right_units[index + 1].GetComponent<Animator>().SetBool("goBack", false);//reset
        right_units[index + 1].GetComponent<Animator>().SetBool("Start", false);//reset


        play = true;
    }
}
