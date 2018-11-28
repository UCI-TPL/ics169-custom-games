using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blinking : MonoBehaviour {
    public float interval;
    public float next_blink;
    public Color32 color_1;
    public Color32 color_2;
    public SpriteRenderer Sprite;
    public bool color_bool;
    public bool lerp_color;

	// Use this for initialization
	void Start () {
        next_blink = 0;
        color_bool = true;
	}
	
	// Update is called once per frame
	void Update () {
        //might want to try slowly transitioning from one color to the other
        if (lerp_color)
        {
            Sprite.color = Color.Lerp(color_2, color_1, Mathf.PingPong(Time.time, interval));
        }
        else if (Time.time > next_blink)
        {
            if (color_bool)
            {
                
                Sprite.color = color_1;
                next_blink = Time.time + interval;
                color_bool = !color_bool;
                
            }
            else
            {
                
                Sprite.color = color_2;
                next_blink = Time.time + interval;
                color_bool = !color_bool;
                
            }
            
        }
	}
}
