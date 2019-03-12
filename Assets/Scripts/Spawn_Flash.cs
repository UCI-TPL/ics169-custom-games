using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn_Flash : MonoBehaviour {
    public GameObject MuzzFlash;
    public GameObject MuzzFlash_Hit;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Show_Flash()
    {
        GameObject Muzzle_Flash = Instantiate(MuzzFlash) as GameObject;
        Muzzle_Flash.transform.SetParent(this.gameObject.transform);
        Muzzle_Flash.transform.localPosition = new Vector3(0,0,0);
        Muzzle_Flash.transform.localScale = new Vector3(25, 25, 0);
        StartCoroutine(Delete_Flash(Muzzle_Flash));
    }

    public IEnumerator Delete_Flash(GameObject Muzzle_Flash)
    {
        yield return new WaitForSeconds(0.1f);
        GameObject.Destroy(Muzzle_Flash);
    }

    public GameObject Show_Flash_Hit(GameObject Unit_Getting_Attacked)
    {
        GameObject Muzzle_Flash_Hit = Instantiate(MuzzFlash_Hit) as GameObject;
        Muzzle_Flash_Hit.transform.SetParent(Unit_Getting_Attacked.transform);
        if(Muzzle_Flash_Hit.tag == "Lightening_Effect")
        {
            Muzzle_Flash_Hit.transform.localPosition = new Vector3(0, 5.4f, 0);
            Muzzle_Flash_Hit.transform.localScale = new Vector3(0.6f, 0.6f, 0);
        }
        else
        {
            Muzzle_Flash_Hit.transform.localPosition = new Vector3(0, 0, 0);
            Muzzle_Flash_Hit.transform.localScale = new Vector3(0.5f, 0.5f, 0);
        }

        float rand_rotate = Random.Range(0f, 360f);
        StartCoroutine(Delete_Flash(Muzzle_Flash_Hit));
        return Muzzle_Flash_Hit;
    }
}
