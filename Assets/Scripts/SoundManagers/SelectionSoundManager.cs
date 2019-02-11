using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionSoundManager : MonoBehaviour {
	
	public AudioSource select;

	public AudioClip fortress_select;
	public AudioClip poison_select; 
	public AudioClip ranger_select;
	public AudioClip tank_select;
	public AudioClip warrior_select;
	public AudioClip healer_select;

	// Use this for initialization
	void Start () {
		
	}
	
	public void fortress()
	{
		if(fortress_select != null)
		{
			select.clip = fortress_select;
			select.Play();
		}
		
	}

	public void poison()
	{
		if(poison_select != null)
		{
			select.clip = poison_select;
			select.Play();
		}
		
	}

	public void ranger()
	{
		if(ranger_select != null)
		{
			select.clip = ranger_select;
			select.Play();
		}
		
	}

	public void tank()
	{
		if( tank_select != null)
		{
			select.clip = tank_select;
			select.Play();
		}
		
	}

	public void warrior()
	{
		if(warrior_select != null)
		{
			select.clip = warrior_select;
			select.Play();
		}
		
	}

	public void healer()
	{
		if(healer_select != null)
		{
			select.clip = healer_select;
			select.Play();
		}
		
	}
	// Update is called once per frame
	void Update () {
		
	}
}
