using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DraftUI : MonoBehaviour {
    private PlayerInformation playerinfo;
    public Text ChoiceText; //done
    public Text PChoice; //done
    public Text StatText; //done
    public Text DescriptionText;
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

    public Image FortressHero;
    public Image FortressHeroBack;
    public Text FortressHeroNum;
    public Image PoisonHero;
    public Image PoisonHeroBack;
    public Text PoisonHeroNum;
    public Image Ranger;
    public Image RangerBack;
    public Text RangerNum;
    public Image Tank;
    public Image TankBack;
    public Text TankNum;
    public Image Warrior;
    public Image WarriorBack;
    public Text WarriorNum;
    public Image Healer;
    public Image HealerBack;
    public Text HealerNum;

    public Color baby_blue = new Color(0.49f,0.74f,1f);
    public bool blinking = false;

    private void Awake()
    {
        playerinfo = GameObject.FindObjectOfType<PlayerInformation>();
        playerinfo.draftUI = this;
        Tank.color = Color.grey;
        Healer.color = Color.grey;
        Warrior.color = Color.grey;
        Ranger.color = Color.grey;
    }


    // Update is called once per frame
    void Update() {
        //ChangePlayer();
        StartUnit temp;
        if (playerinfo.currentState == PlayerInformation.DraftStates.P1_Pick_1 || (playerinfo.currentState == PlayerInformation.DraftStates.P2_Pick_1 && playerinfo.Player2Chosen.Count == 0))
        {
            if (playerinfo.currentState == PlayerInformation.DraftStates.P1_Pick_1)
                temp = playerinfo.Hero1Units[playerinfo.p1ScrollValue];
            else
                temp = playerinfo.Hero2Units[playerinfo.p1ScrollValue];

            //FortressHeroNum.text = "1";
            if (temp.unit_type == "Fortress")
            {
                if(PoisonHeroBack.enabled)
                {
                    PoisonHeroBack.enabled = false;
                }
                FortressHeroBack.enabled = true;
            }
            if(temp.unit_type == "Plague")
            {
                if(FortressHeroBack.enabled)
                {
                    FortressHeroBack.enabled = false;
                }
                PoisonHeroBack.enabled = true;
            }
        }

        else
        {
            PoisonHeroBack.enabled = false;
            PoisonHero.color = Color.grey;
            FortressHeroBack.enabled = false;
            FortressHero.color = Color.grey;
            Tank.color = Color.white;
            Healer.color = Color.white;
            Warrior.color = Color.white;
            Ranger.color = Color.white;
            if (playerinfo.currentState == PlayerInformation.DraftStates.P1_Pick_1 || playerinfo.currentState == PlayerInformation.DraftStates.P1_Pick_2 ||
                playerinfo.currentState == PlayerInformation.DraftStates.P1_Pick_3)
                temp = playerinfo.AllP1Units[playerinfo.p1ScrollValue];
            else
            {
                temp = playerinfo.AllP2Units[playerinfo.p1ScrollValue];
            }
            if (temp.unit_type == "Ranger")
            {
                if (WarriorBack.enabled)
                {
                    WarriorBack.enabled = false;
                }
                if (TankBack.enabled)
                {
                    TankBack.enabled = false;
                }
                if (HealerBack.enabled)
                {
                    HealerBack.enabled = false;
                }
                RangerBack.enabled = true;
            }
            if (temp.unit_type == "Warrior")
            {
                if (RangerBack.enabled)
                {
                    RangerBack.enabled = false;
                }
                if (TankBack.enabled)
                {
                    TankBack.enabled = false;
                }
                if (HealerBack.enabled)
                {
                    HealerBack.enabled = false;
                }
                WarriorBack.enabled = true;
            }
            if (temp.unit_type == "Tank")
            {
                if (RangerBack.enabled)
                {
                    RangerBack.enabled = false;
                }
                if (WarriorBack.enabled)
                {
                    WarriorBack.enabled = false;
                }
                if (HealerBack.enabled)
                {
                    HealerBack.enabled = false;
                }
                TankBack.enabled = true;
            }
            if (temp.unit_type == "Healer")
            {
                if (Tank.enabled)
                {
                    TankBack.enabled = false;
                }
                if (WarriorBack.enabled)
                {
                    WarriorBack.enabled = false;
                }
                if (RangerBack.enabled)
                {
                    RangerBack.enabled = false;
                }
                HealerBack.enabled = true;
            }
        }



        PChoice.text = temp.unit_type;
        UnitIcon.sprite = temp.Icon;
        GameObject Icon = GameObject.Find(temp.unit_type);
        DescriptionText.text = Icon.GetComponent<IconStats>().description;
        //StatText.text = "HP: " + Icon.GetComponent<IconStats>().health.ToString() + "  ATT: " + Icon.GetComponent<IconStats>().attack
        //    + "  RANGE: " + Icon.GetComponent<IconStats>().range + "  MOBILITY: " + Icon.GetComponent<IconStats>().movement + " tiles";
        if (playerinfo.pool)
        {
            int[] unit_count;
            if (playerinfo.currentState == PlayerInformation.DraftStates.P1_Pick_1 || playerinfo.currentState == PlayerInformation.DraftStates.P1_Pick_2 ||
                playerinfo.currentState == PlayerInformation.DraftStates.P1_Pick_3)
                unit_count = CheckPool("P1");
            else
                unit_count = CheckPool("P2");

            RangerNum.text = "x" + unit_count[0].ToString();
            if (unit_count[0] == 0)
                Ranger.color = Color.grey;
            WarriorNum.text = "x" + unit_count[1].ToString();
            if (unit_count[1] == 0)
                Warrior.color = Color.grey;
            TankNum.text = "x" + unit_count[2].ToString();
            if (unit_count[2] == 0)
                Tank.color = Color.grey;
            HealerNum.text = "x" + unit_count[3].ToString();
            if (unit_count[3] == 0)
            {
                Healer.color = Color.grey;
            }

            //for (int i = 0; i < playerinfo.RemovedP1Units.Count; i++)
            //{
            //    if(playerinfo.RemovedP1Units[i].unit_type == "Ranger")
            //        Ranger.color = Color.grey;
            //    else if (playerinfo.RemovedP1Units[i].unit_type == "Warrior")
            //        Warrior.color = Color.grey;
            //    else if (playerinfo.RemovedP1Units[i].unit_type == "Tank")
            //        Tank.color = Color.grey;
            //    else if (playerinfo.RemovedP1Units[i].unit_type == "Healer")
            //        Healer.color = Color.grey;
            //}
        }

    }

    public IEnumerator Blink(Image i)
    {
        i.enabled = false;
        blinking = true;
        yield return new WaitForSeconds(0.3f);
        i.enabled = true;
        yield return new WaitForSeconds(0.3f);
        blinking = false;
    }

    public int[] CheckPool(string playerNum) //for all the units in the pool, add their count up
    {
        int[] unit_nums = {0, 0, 0,0};
        if (playerNum == "P1")
        {
            for (int j = 0; j < playerinfo.P1PoolUnits.Count; j++)
            {

                if (playerinfo.P1PoolUnits[j].unit_type == "Ranger")
                    unit_nums[0]++;
                else if (playerinfo.P1PoolUnits[j].unit_type == "Warrior")
                    unit_nums[1]++;
                else if (playerinfo.P1PoolUnits[j].unit_type == "Tank")
                    unit_nums[2]++;
                else
                    unit_nums[3]++;
            }
        }
        else
        {
            for (int j = 0; j < playerinfo.P2PoolUnits.Count; j++)
            {

                if (playerinfo.P2PoolUnits[j].unit_type == "Ranger")
                    unit_nums[0]++;
                else if (playerinfo.P2PoolUnits[j].unit_type == "Warrior")
                    unit_nums[1]++;
                else if (playerinfo.P2PoolUnits[j].unit_type == "Tank")
                    unit_nums[2]++;
                else
                    unit_nums[3]++;
            }
        }
        return unit_nums;
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
}
