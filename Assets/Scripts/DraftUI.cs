using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DraftUI : MonoBehaviour {
    private PlayerInformation playerinfo;
    public Text P1CostText; //done
    public Text P2CostText; //done
    public Text PoolText; //done
    public Text PChoice; //done
    public Text StatText; //done
    public Image Panel; //done
    public Image UnitIcon; //done but could be better
    public Image P1Choice1; //done
    public Image P1Choice2; //done
    public Image P1Choice3; //done
    public Image P1Choice4; //done
    public Image P1Choice5; //done
    public Image P1Choice6;
    public Image P1Choice7;
    public Image P2Choice1; //done
    public Image P2Choice2; //done
    public Image P2Choice3; //done
    public Image P2Choice4; //done
    public Image P2Choice5; //done
    public Image P2Choice6;
    public Image P2Choice7;

    public Color baby_blue = new Color(0.49f,0.74f,1f);
    public bool blinking = false;

    private void Awake()
    {
        playerinfo = GameObject.FindObjectOfType<PlayerInformation>();
        playerinfo.draftUI = this;
    }


    // Update is called once per frame
    void Update() {
        //ChangePlayer();
        StartUnit temp;
        if(playerinfo.currentState == PlayerInformation.DraftStates.P1_Pick_1 || (playerinfo.currentState == PlayerInformation.DraftStates.P2_Pick_1 && playerinfo.Player2Chosen.Count == 0))
        {
            temp = playerinfo.HeroUnits[playerinfo.p1ScrollValue];
        }
        else
            temp = playerinfo.AllP1Units[playerinfo.p1ScrollValue];


        P1CostText.text = "Cost: " + playerinfo.p1Cost.ToString();
        P2CostText.text = "Cost: " + playerinfo.p2Cost.ToString();
        PChoice.text = temp.name + "  /  Cost:" + temp.cost;
        UnitIcon.sprite = temp.Icon;
        StatText.text = "Hlth:" + temp.health.ToString("000") + "\t Att:" + temp.attack.ToString("000")
            + "\nRange:" + temp.attackRange + "\t Move:" + temp.mobility;
        if (playerinfo.pool)
        {
            PoolText.text = "x" + CheckPool().ToString();
        }

    }

    public IEnumerator Blink(Image i)
    {
        i.enabled = false;
        blinking = true;
        yield return new WaitForSeconds(0.25f);
        i.enabled = true;
        yield return new WaitForSeconds(0.25f);
        blinking = false;
    }

    private int CheckPool()
    {
        int count = 0;
        for (int j = 0; j < playerinfo.PoolUnits.Count; j++)
        {
    
            if ( playerinfo.AllP1Units[playerinfo.p1ScrollValue]  == playerinfo.PoolUnits[j])
                count++;
            
        }
        return count;
    }

    public void P1Pick1()
    {
        P1Choice1.sprite = playerinfo.Player1Chosen[0].Icon;
        P1Choice1.color = Color.white;
        P1Choice1.enabled = true;
    }
    public void P1Pick2()
    {
        P1Choice2.sprite = playerinfo.Player1Chosen[1].Icon;
        P1Choice2.color = Color.white;
        P1Choice2.enabled = true;
    }
    public void P1Pick3()
    {
        P1Choice3.sprite = playerinfo.Player1Chosen[2].Icon;
        P1Choice3.color = Color.white;
        P1Choice3.enabled = true;
    }
    public void P1Pick4()
    {
        P1Choice4.sprite = playerinfo.Player1Chosen[3].Icon;
        P1Choice4.color = Color.white;
    }
    public void P1Pick5()
    {
        P1Choice5.sprite = playerinfo.Player1Chosen[4].Icon;
        P1Choice5.color = Color.white;
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
    public void P2Pick4()
    {
        P2Choice4.sprite = playerinfo.Player2Chosen[3].Icon;
        P2Choice4.color = Color.white;
    }
    public void P2Pick5()
    {
        P2Choice5.sprite = playerinfo.Player2Chosen[4].Icon;
        P2Choice5.color = Color.white;
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
