using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PoisonHero : HeroUnit {
    public PoisonGas poisonGas;
    public int specialAttackCounter = 5; // counter to keep track of when to fire off his load

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public override void TakeDamage(StartUnit attacked_unit, float damage) // will occur when the hero retaliates and attacks (2 per turn)
    {
        print("using the TakeDamage() function");
        base.TakeDamage(attacked_unit, damage); // the normal function from StartUnit

        DecrementCounter();
        if (specialAttackCounter == 0)
        {
            
            ShootPoisonGas(editor.hexGrid.GetCell(attacked_unit.transform.position));
        }
    }


    public void ShootPoisonGas(HexagonCell cell) // spawn the environmental hazard "PoisonGas"
    {
        editor.hazardsOnGrid.Add(poisonGas.CreateHazardAt(cell, editor.hexGrid));
       
        specialAttackCounter = 5;
    }
    public void DecrementCounter() // decrease the counter in a nicer way? don't know why i wrote this function honestly
    {
            specialAttackCounter -= 1;
    }
}
