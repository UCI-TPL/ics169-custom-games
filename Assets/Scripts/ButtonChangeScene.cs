﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ButtonChangeScene : MonoBehaviour 
{

	public Button b;
	public string name;
	//public AudioSource click;

	// Use this for initialization
	void Start () 
	{
		//click = GetComponent<AudioSource>();
		Button b1 = b.GetComponent<Button>();
		b1.onClick.AddListener(TaskOnClick);
	}
	
	void TaskOnClick()
	{
		//click.Play();
		SceneManager.LoadScene(name);	
	}
}
