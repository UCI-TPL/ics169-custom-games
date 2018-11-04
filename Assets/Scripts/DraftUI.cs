using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DraftUI : MonoBehaviour {
    private PlayerInformation playerinfo;
    public Text P1CostText; //done
    public Text P2CostText; //done
    public Text PoolText;
    public Text PChoice; //done
    public Text StatText;
    public Image Panel; //done
    public Image UnitIcon;
    public Image P1Choice1; //done
    public Image P1Choice2; //done
    public Image P1Choice3; //done
    public Image P2Choice1; //done
    public Image P2Choice2; //done
    public Image P2Choice3; //done

    private void Awake()
    {
        playerinfo = GameObject.FindObjectOfType<PlayerInformation>();
        playerinfo.draftUI = this;
    }

	
	// Update is called once per frame
	void Update () {
        //ChangePlayer();
        StartUnit temp = playerinfo.AllP1Units[playerinfo.p1ScrollValue];
        P1CostText.text = "Cost: " + playerinfo.p1Cost.ToString();
        P2CostText.text = "Cost: " + playerinfo.p2Cost.ToString();
        PChoice.text = temp.name;
        UnitIcon.sprite = temp.Icon;
        StatText.text = "Health:" + temp.health.ToString("000") + "\t Attack:" + temp.attack.ToString("000") + "\t Cost:" + temp.cost.ToString("0")
            + "\nRange:" + temp.attackRange + "\t Mobility:" + temp.mobility;
    }

    public void P1Pick1()
    {
        P1Choice1.sprite = playerinfo.Player1Chosen[0].Icon;
        P1Choice1.color = Color.white;
    }
    public void P1Pick2()
    {
        P1Choice2.sprite = playerinfo.Player1Chosen[1].Icon;
        P1Choice2.color = Color.white;
    }
    public void P1Pick3()
    {
        P1Choice3.sprite = playerinfo.Player1Chosen[2].Icon;
        P1Choice3.color = Color.white;
    }

    public void P2Pick1()
    {
        P2Choice1.sprite = playerinfo.Player2Chosen[0].Icon;
        P2Choice1.color = Color.white;
    }
    public void P2Pick2()
    {
        P2Choice2.sprite = playerinfo.Player2Chosen[1].Icon;
        P2Choice2.color = Color.white;
    }
    public void P2Pick3()
    {
        P2Choice3.sprite = playerinfo.Player2Chosen[2].Icon;
        P2Choice3.color = Color.white;
    }

    public void ChangePlayer()
    {
        if(Panel.GetComponent<Image>().color == Color.blue)
        {
            Panel.GetComponent<Image>().color = Color.red;
        }
        else
        {
            Panel.GetComponent<Image>().color = Color.blue;
        }
    }
}
