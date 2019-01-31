using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconStats : MonoBehaviour {
    public float health;

    public float attack;

    public float range;

    public float movement;

    public string description;

    private void Start()
    {
        health = this.gameObject.GetComponent<StartUnit>().health;
        attack = this.gameObject.GetComponent<StartUnit>().attack;
        range = this.gameObject.GetComponent<StartUnit>().attackRange;
        movement = this.gameObject.GetComponent<StartUnit>().mobility;
        description = this.gameObject.GetComponent<StartUnit>().description;
    }
}
