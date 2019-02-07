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
        base.TakeDamage(attacked_unit, damage); // the normal function from StartUnit

        DecrementCounter();
        if (specialAttackCounter == 0)
        {
            ShootPoisonGas(HexagonCoord.FromPosition(attacked_unit.transform.position));
        }
    }


    public void ShootPoisonGas(HexagonCoord coord) // spawn the environmental hazard "PoisonGas"
    {
        print("shooting poison  from hero");
        if(gameObject.tag == "Player 1")
            editor.P1StatusOnGrid.Add(poisonGas.CreateHazardAt(coord));
        else if(gameObject.tag == "Player 2")
            editor.P2StatusOnGrid.Add(poisonGas.CreateHazardAt(coord));

        specialAttackCounter = 5;
    }
    public void DecrementCounter() // decrease the counter in a nicer way? don't know why i wrote this function honestly
    {
            specialAttackCounter -= 1;
    }
}
