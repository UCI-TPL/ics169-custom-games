using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerInformation : MonoBehaviour
{

    public List<StartUnit> Player1Chosen = new List<StartUnit>();
    public List<StartUnit> Player2Chosen = new List<StartUnit>();

    public List<int> ctrsSet = new List<int>(); // list of controllers that were mapped
    public string player1; // player 1 string to map to the correct input
    public string player2; // player 2 string to map to the correct input

    public Text display;

    public Scene current_scene;

    public bool plr1Set, plr2Set;
    //___________________________________________________________________________________________
    public List<StartUnit> AllP1Units = new List<StartUnit>(); // p2 can share this list but just change their tag later
    public List<StartUnit> AllP2Units = new List<StartUnit>();

    public List<StartUnit> PoolUnits = new List<StartUnit>(); // random  pool of units
    public float p1ScrollTime;
    public float p2ScrollTime;
    public int p1ScrollValue;
    public int p2ScrollValue;

    public float p1PickTime;

    public bool pool = false;


    // Use this for initialization
    void Start()
    {
        display = FindObjectOfType<Text>();
        DontDestroyOnLoad(this.gameObject);
        current_scene = SceneManager.GetActiveScene();
        if(pool)
        {
            RandomPool();
        }
    }

    // Update is called once per frame
    void Update()
    {
        current_scene = SceneManager.GetActiveScene(); // terrible coding fix later
        if (current_scene.name == "CMapping")
        {
            //if (ctrsSet.Contains(1) && ctrsSet.Contains(2)) // both players ready
            if (ctrsSet.Contains(1))
                SceneManager.LoadScene(2);
            else
            {
                ControllerMapping();
            }
        }
        if (current_scene.name == "SelectCharacter")
        {
            if (Player1Chosen.Count == 3)
                SceneManager.LoadScene(3);
            ChooseCharacter();
            DraftPick();
        }
    }

    public void RandomPool()
    {
        for(int i = 0; i < 10; i++)
        {
            int rand_index = Random.Range(0, AllP1Units.Count);
            PoolUnits.Add(AllP1Units[rand_index]);
        }
    }

    public void CheckUnits()
    {
        for(int i = 0; i < AllP1Units.Count-1; i++)
        {
            if(!PoolUnits.Contains(AllP1Units[i]))
            {
                AllP1Units.Remove(AllP1Units[i]);
            }
                
        }
    }


    void ControllerMapping()
    {
        if (!plr1Set)
        {
            if (Input.GetButtonDown("J1 A Button") && !ctrsSet.Contains(1))
            {
                display.text += "PLAYER 1 READY\n";
                plr1Set = true;
                player1 = "J1 ";
                ctrsSet.Add(1);
            }
            else if (Input.GetButtonDown("J2 A Button") && !ctrsSet.Contains(2))
            {
                display.text += "PLAYER 2 READY\n";
                plr1Set = true;
                player1 = "J2 ";
                ctrsSet.Add(2);
            }
        }
        if (!plr2Set)
        {
            if (Input.GetButtonDown("J1 A Button") && !ctrsSet.Contains(1))
            {
                display.text += "PLAYER 1 READY\n";
                plr2Set = true;
                player2 = "J1 ";
                ctrsSet.Add(1);
            }
            else if (Input.GetButtonDown("J2 A Button") && !ctrsSet.Contains(2))
            {
                display.text += "PLAYER 2 READY\n";
                plr2Set = true;
                player2 = "J2 ";
                ctrsSet.Add(2);
            }
        }
    }

    private int ChangeCharacter(int counter, int direction)
    {
        int newVal = counter + direction;
        int limit = AllP1Units.Count - 1;
        if (newVal < 0)
        {
            newVal = limit;
        }
        else if (newVal > limit)
        {
            newVal = 0;
        }
        return newVal;
    }

    void ChooseCharacter()
    {
        float value;
        if (plr1Set)
        {
            value = Input.GetAxis(player1 + "Left Horizontal");
            Debug.Log(value);
            if (value != 0.0f && p1ScrollTime <= Time.time)
            {
                p1ScrollTime = Time.time + 0.25f;
                if (value > 0.0f)
                {
                    p1ScrollValue = ChangeCharacter(p1ScrollValue, 1);
                }
                else
                {
                    p1ScrollValue = ChangeCharacter(p1ScrollValue, -1);
                }
            }
        }

        if (plr2Set)
        {
            value = Input.GetAxis(player1 + "Left Horizontal");
            if (value != 0.0f && p2ScrollTime <= Time.time)
            {
                p2ScrollTime = Time.time + 0.5f;
                if (value > 0.0f)
                {
                    p2ScrollValue = ChangeCharacter(p2ScrollValue, 1);
                }
                else
                {
                    p2ScrollValue = ChangeCharacter(p2ScrollValue, -1);
                }
            }
        }

    }

    private void DraftPick()
    {
        if (Player1Chosen.Count < 3)
        {
            
            if (Input.GetButton(player1 + "A Button") && p1PickTime <= Time.time)
            {
                p1PickTime = Time.time + 1f;
                Player1Chosen.Add(AllP1Units[p1ScrollValue]);
                if (pool)
                {
                    PoolUnits.Remove(AllP1Units[p1ScrollValue]);
                    CheckUnits();
                }
            }
        }
        //if (Player2Chosen.Count < 3)
        //{
        //    if (Input.GetButton(player2 + "A Button"))
        //    {
        //        Player2Chosen.Add(AllP2Units[p2ScrollValue]);
        //    }
        //}


    }
}
