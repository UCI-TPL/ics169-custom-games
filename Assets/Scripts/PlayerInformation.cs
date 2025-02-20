﻿using System.Collections;
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

    public GameObject display,display2;
    public Image p1controllerImg, p1controllerImg2;

    private Scene current_scene;

    public bool plr1Set, plr2Set;
    //___________________________________________________________________________________________
    public List<StartUnit> AllP1Units = new List<StartUnit>(); // p2 can share this list but just change their tag later
    public List<StartUnit> AllP2Units = new List<StartUnit>();
    public List<StartUnit> Hero1Units = new List<StartUnit>();
    public List<StartUnit> Hero2Units = new List<StartUnit>();
    public List<StartUnit> P1PoolUnits = new List<StartUnit>(); // random  pool of units
    public List<StartUnit> P2PoolUnits = new List<StartUnit>();
    public List<StartUnit> RemovedP1Units = new List<StartUnit>(); // list of units that will get put back in after because they cant be chosen by that player
    public List<StartUnit> RemovedP2Units = new List<StartUnit>(); 
    public bool removed = false;
    private float p1ScrollTime;
    private float p2ScrollTime;
    public int p1ScrollValue;
    public int p2ScrollValue;

    public int p1Cost = 2000;
    public int p2Cost = 2000;

    public float p1PickTime;
    public float p2PickTime;

    private Text p1Text;
    private Text p2Text;

    public bool nextScene = false;
    private bool doSelect = false;
    public bool one_time, loadOne;

    public DraftUI draftUI;

    public GameObject loadingScreen;
    public Slider loadingSlider;

    public GameObject ranger;

    public bool reset;
    public bool reRandom;

    public Color alpha1, alpha2;
    /********************************************************************************/
    //sound stuff
    public AudioSource move_sound, pick_sound;

    public enum DraftStates
    {
        Initialize,
        P1_Pick_1,
        P2_Pick_1,
        P1_Pick_2,
        P2_Pick_2,
        P1_Pick_3,
        P2_Pick_3,
        Check_Teams,
        //Enter_Battle
    }

    [SerializeField] public DraftStates currentState = DraftStates.Initialize;

    //____________________________________________shortcuts______________________________________
    public bool one_player;
    public bool pool = false;

    private void Awake()
    {
        loadingScreen = GameObject.Find("LoadingScreen");
        loadingSlider = loadingScreen.GetComponentInChildren<Slider>();
    }

    // Use this for initialization
    void Start()
    {
        one_time = true;
        //display = FindObjectOfType<Text>();
        DontDestroyOnLoad(this.gameObject);
        current_scene = SceneManager.GetActiveScene();
        reRandom = false;
        loadingScreen.SetActive(false);
        loadOne = true;
        alpha1 = draftUI.P1Side.color;
        alpha2 = draftUI.P2side.color;
    }

    // Update is called once per frame
    void Update()
    {
        current_scene = SceneManager.GetActiveScene(); // terrible coding fix later
        if (current_scene.name == "CMapping")
        {

            if (ctrsSet.Contains(1) && (ctrsSet.Contains(2) || one_player) && loadOne)
            {// both players ready
                LoadGame(1);
                loadOne = false;
            }
            else
            {
                ControllerMapping();
            }
        }
        if (reset)
        {
            if (RemovedP1Units.Count != 0)
            {
                AddUnitsBack("P1");
            }
            Debug.Log("Removed P1Units");
            if (RemovedP2Units.Count != 0)
            {
                AddUnitsBack("P2");
                Debug.Log("P2 Removed");
            }
            Debug.Log("Removed P2Units");
            Player1Chosen.Clear();
            Player2Chosen.Clear();
            P1PoolUnits.Clear();
            P2PoolUnits.Clear();
            one_time = true;
            loadOne = true;
            reset = false;
            Debug.Log("REMOVED ALL");
        }
        if (current_scene.name == "SelectCharacter")
        {
            switch (currentState)
            {
                case (DraftStates.Initialize):
                    if (!doSelect)
                    {
                        doSelect = true;
                        if (pool)
                        {
                            RandomPool();
                        }
                        draftUI.ChoiceText.text = "Player 1 Select Your Warlord";
                        currentState = DraftStates.P1_Pick_1;
                    }
                    break;

                case (DraftStates.P1_Pick_1):
                    draftUI.P2side.GetComponent<Image>().color = new Color(draftUI.P2side.GetComponent<Image>().color.r, draftUI.P2side.GetComponent<Image>().color.g, draftUI.P2side.GetComponent<Image>().color.b, .5f);
                    if (reRandom)
                    {
                        RandomPool();
                        reRandom = false;
                    }
                    if (Player1Chosen.Count == 1)
                    {
                        draftUI.P1Side.GetComponent<Image>().color = new Color(draftUI.P1Side.GetComponent<Image>().color.r, draftUI.P1Side.GetComponent<Image>().color.g, draftUI.P1Side.GetComponent<Image>().color.b, 1f);
                        //draftUI.P2side.GetComponent<Image>().color = new Color(draftUI.P2side.GetComponent<Image>().color.r, draftUI.P2side.GetComponent<Image>().color.g, draftUI.P2side.GetComponent<Image>().color.b, .5f);
                        draftUI.P1Pick1();
                        if (one_player)
                        {
                            currentState = DraftStates.P1_Pick_2;   
                        }
                        else
                        {
                            //draftUI.ChangePlayer();
                            draftUI.P1Side.GetComponent<Image>().color = new Color(draftUI.P1Side.GetComponent<Image>().color.r, draftUI.P1Side.GetComponent<Image>().color.g, draftUI.P1Side.GetComponent<Image>().color.b, .5f);
                            currentState = DraftStates.P2_Pick_1;
                        }
                    }
                    if (!draftUI.blinking)
                        StartCoroutine(draftUI.Blink(draftUI.P1Choice1));
                    //if (pool)
                    //    CheckPool();
                    ChooseCharacter("P1",true);
                    DraftPick("P1", true);
                    break;

                case (DraftStates.P2_Pick_1):
                    if(Player2Chosen.Count == 0)
                    {
                        draftUI.P2side.GetComponent<Image>().color = new Color(draftUI.P2side.GetComponent<Image>().color.r, draftUI.P2side.GetComponent<Image>().color.g, draftUI.P2side.GetComponent<Image>().color.b, 1f);
                        draftUI.P2Choice1.color = draftUI.pink;
                        if (!draftUI.blinking)
                            StartCoroutine(draftUI.Blink(draftUI.P2Choice1));
                        ChooseCharacter("P2", true);
                        DraftPick("P2", true);
                        draftUI.ChoiceText.text = "Player 2 Select Your Warlord";
                        break;
                    }
                    if(Player2Chosen.Count == 1)
                    {
                        draftUI.P2side.GetComponent<Image>().color = new Color(draftUI.P2side.GetComponent<Image>().color.r, draftUI.P2side.GetComponent<Image>().color.g, draftUI.P2side.GetComponent<Image>().color.b, 1f);
                        draftUI.P2Pick1();
                        draftUI.P2Choice2.color = draftUI.pink;
                        draftUI.ChoiceText.text = "Player 2 Select Your Unit";
                        if (!draftUI.blinking)
                            StartCoroutine(draftUI.Blink(draftUI.P2Choice2));
                    }
                    else if (Player2Chosen.Count == 2)
                    {
                        draftUI.P2side.GetComponent<Image>().color = new Color(draftUI.P2side.GetComponent<Image>().color.r, draftUI.P2side.GetComponent<Image>().color.g, draftUI.P2side.GetComponent<Image>().color.b, .5f);
                        draftUI.P2Pick2();
                        currentState = DraftStates.P1_Pick_2;
                    }
                    //if (pool)
                    //    CheckPool();
                    ChooseCharacter("P2",false);
                    DraftPick("P2",false);
                    
                    break;

                case (DraftStates.P1_Pick_2):
                    if(p1Cost <= 0)
                    {
                        currentState = DraftStates.P2_Pick_2;
                    }
                    if(Player1Chosen.Count == 1)
                    {
                        draftUI.P1Side.GetComponent<Image>().color = new Color(draftUI.P1Side.GetComponent<Image>().color.r, draftUI.P1Side.GetComponent<Image>().color.g, draftUI.P1Side.GetComponent<Image>().color.b, 1f);
                        draftUI.P1Choice2.color = draftUI.baby_blue;
                        if(!draftUI.blinking)
                            StartCoroutine(draftUI.Blink(draftUI.P1Choice2));
                    }
                    else if(Player1Chosen.Count == 2)
                    {
                        draftUI.P1Side.GetComponent<Image>().color = new Color(draftUI.P1Side.GetComponent<Image>().color.r, draftUI.P1Side.GetComponent<Image>().color.g, draftUI.P1Side.GetComponent<Image>().color.b, 1f);
                        draftUI.P1Pick2();
                        draftUI.P1Choice3.color = draftUI.baby_blue;
                        if (!draftUI.blinking)
                            StartCoroutine(draftUI.Blink(draftUI.P1Choice3));
                    }
                    else if (Player1Chosen.Count == 3)
                    {
                        draftUI.P1Pick3();
                        if(one_player)
                            currentState = DraftStates.P1_Pick_3;
                        else
                        {
                            draftUI.P1Side.GetComponent<Image>().color = new Color(draftUI.P1Side.GetComponent<Image>().color.r, draftUI.P1Side.GetComponent<Image>().color.g, draftUI.P1Side.GetComponent<Image>().color.b, .5f);
                            currentState = DraftStates.P2_Pick_2;
                        }
                    }

                    //if (pool)
                    //    CheckPool();
                    ChooseCharacter("P1",false);
                    DraftPick("P1",false);
                    draftUI.ChoiceText.text = "Player 1 Select Your Unit";
                    break;

                case (DraftStates.P2_Pick_2):
                    if (p2Cost <= 0)
                    {
                        currentState = DraftStates.P1_Pick_3;
                    }
                     if (Player2Chosen.Count == 2)
                    {
                        draftUI.P2side.GetComponent<Image>().color = new Color(draftUI.P2side.GetComponent<Image>().color.r, draftUI.P2side.GetComponent<Image>().color.g, draftUI.P2side.GetComponent<Image>().color.b, 1f);
                        draftUI.P2Pick2();
                        draftUI.P2Choice3.color = draftUI.pink;
                        if (!draftUI.blinking)
                            StartCoroutine(draftUI.Blink(draftUI.P2Choice3));
                    }
                    if (Player2Chosen.Count == 3)
                    {
                        draftUI.P2side.GetComponent<Image>().color = new Color(draftUI.P2side.GetComponent<Image>().color.r, draftUI.P2side.GetComponent<Image>().color.g, draftUI.P2side.GetComponent<Image>().color.b, 1f);
                        draftUI.P2Pick3();
                        draftUI.P2Choice4.color = draftUI.pink;
                        if (!draftUI.blinking)
                            StartCoroutine(draftUI.Blink(draftUI.P2Choice4));
                    }
                    if (Player2Chosen.Count == 4)
                    {
                        draftUI.P2side.GetComponent<Image>().color = new Color(draftUI.P2side.GetComponent<Image>().color.r, draftUI.P2side.GetComponent<Image>().color.g, draftUI.P2side.GetComponent<Image>().color.b, .5f);
                        draftUI.P2Pick4();
                        currentState = DraftStates.P1_Pick_3;
                    }
                    //if (pool)
                    //    CheckPool();
                    ChooseCharacter("P2",false);
                    DraftPick("P2",false);
                    draftUI.ChoiceText.text = "Player 2 Select Your Unit";
                    break;

                case (DraftStates.P1_Pick_3):
                    //if (p1Cost <= 0)
                    //{
                    //    currentState = DraftStates.Check_Teams; //DraftStates.Enter_Battle;
                    //}
                    if(Player1Chosen.Count == 3)
                    {
                        draftUI.P1Side.GetComponent<Image>().color = new Color(draftUI.P1Side.GetComponent<Image>().color.r, draftUI.P1Side.GetComponent<Image>().color.g, draftUI.P1Side.GetComponent<Image>().color.b, 1f);
                        draftUI.P1Choice4.color = draftUI.baby_blue;
                        if (!draftUI.blinking)
                            StartCoroutine(draftUI.Blink(draftUI.P1Choice4));
                    }
                    if(Player1Chosen.Count == 4)
                    {
                        draftUI.P1Side.GetComponent<Image>().color = new Color(draftUI.P1Side.GetComponent<Image>().color.r, draftUI.P1Side.GetComponent<Image>().color.g, draftUI.P1Side.GetComponent<Image>().color.b, 1f);
                        draftUI.P1Pick4();
                        draftUI.P1Choice5.color = draftUI.baby_blue;
                        if (!draftUI.blinking)
                            StartCoroutine(draftUI.Blink(draftUI.P1Choice5));
                    }
                    if (Player1Chosen.Count == 5)
                    {
                        draftUI.P1Pick5();
                        if (one_player)
                           currentState = DraftStates.Check_Teams;// DraftStates.Enter_Battle;
                        else
                        {
                            draftUI.P1Side.GetComponent<Image>().color = new Color(draftUI.P1Side.GetComponent<Image>().color.r, draftUI.P1Side.GetComponent<Image>().color.g, draftUI.P1Side.GetComponent<Image>().color.b, .5f);
                            currentState = DraftStates.P2_Pick_3;
                        }
                    }
                    //if (pool)
                    //    CheckPool();
                    ChooseCharacter("P1",false);
                    DraftPick("P1",false);
                    draftUI.ChoiceText.text = "Player 1 Select Your Unit";
                    break;

                case (DraftStates.P2_Pick_3):
                    //if (p2Cost <= 0)
                    //{
                    //    currentState = DraftStates.Check_Teams; //DraftStates.Enter_Battle;
                    //}
                    if(Player2Chosen.Count == 3)
                    {
                        alpha2.a = 1f;
                        draftUI.P2side.GetComponent<Image>().color = new Color(draftUI.P2side.GetComponent<Image>().color.r, draftUI.P2side.GetComponent<Image>().color.g, draftUI.P2side.GetComponent<Image>().color.b, 1f);
                        draftUI.P2Choice4.color = draftUI.pink;
                        if (!draftUI.blinking)
                            StartCoroutine(draftUI.Blink(draftUI.P2Choice4));
                    }
                    if(Player2Chosen.Count == 4)
                    {
                        alpha2.a = 1f;
                        draftUI.P2side.GetComponent<Image>().color = new Color(draftUI.P2side.GetComponent<Image>().color.r, draftUI.P2side.GetComponent<Image>().color.g, draftUI.P2side.GetComponent<Image>().color.b, 1f);
                        draftUI.P2Pick4();
                        draftUI.P2Choice5.color = draftUI.pink;
                        if (!draftUI.blinking)
                            StartCoroutine(draftUI.Blink(draftUI.P2Choice5));
                    }
                    if (Player2Chosen.Count == 5) 
                    {
                        draftUI.P2Pick5();
                        currentState = DraftStates.Check_Teams; //DraftStates.Enter_Battle;
                    }
                    //if (pool)
                    //    CheckPool();
                    ChooseCharacter("P2",false);
                    DraftPick("P2",false);
                    draftUI.ChoiceText.text = "Player 2 Select Your Unit";
                    break;

                case (DraftStates.Check_Teams):
                   // if (Player1Chosen.Count == 5 && (Player2Chosen.Count == 5 || one_player))
                   //     currentState = DraftStates.Enter_Battle;
                   // else if (Player1Chosen.Count == 5 && Player2Chosen.Count == 5 && !one_player)
                   //     currentState = DraftStates.Enter_Battle;
                    break;

                //case (DraftStates.Enter_Battle):
                    //SceneManager.LoadScene(3);
                //    LoadGame(3);
                //    break;
            }
        }

        if ( ((Player1Chosen.Count == 5 && Player2Chosen.Count == 5) || (Player1Chosen.Count == 5 && one_player)) && one_time)
        {
            LoadGame(3);
            one_time = false;
            reRandom = true;
        }
    }

    private void CheckPool(string playerNum) // for all the units, for all the units in the pool, add their count
    {
        if (playerNum == "P1")
        {
            int[] counts = new int[AllP1Units.Count];
            for (int j = 0; j < AllP1Units.Count; j++)
            {
                for (int i = 0; i < P1PoolUnits.Count; i++)
                {
                    if (P1PoolUnits[i] == AllP1Units[j])
                        counts[j]++;
                }
            }
            string result = "";
            for (int k = 0; k < counts.Length; k++)
            {
                result += AllP1Units[k].name + " x" + counts[k] + "\n";
            }
            //display.text = result;
        }
        else
        {
            int[] counts = new int[AllP2Units.Count];
            for (int j = 0; j < AllP2Units.Count; j++)
            {
                for (int i = 0; i < P2PoolUnits.Count; i++)
                {
                    if (P2PoolUnits[i] == AllP2Units[j])
                        counts[j]++;
                }
            }
            string result = "";
            for (int k = 0; k < counts.Length; k++)
            {
                result += AllP2Units[k].name + " x" + counts[k] + "\n";
            }
        }
    }

    public void RandomPool()
    {
        for(int j = 0; j < 4; j++)
        {
            P1PoolUnits.Add(AllP1Units[j]);
            P2PoolUnits.Add(AllP2Units[j]);
        }
        for(int i = 0; i < 6; i++)
        {
            int rand_index = Random.Range(0, AllP1Units.Count);
            P1PoolUnits.Add(AllP1Units[rand_index]);
            P2PoolUnits.Add(AllP2Units[rand_index]);
        }
        CheckUnits("P1");
        CheckUnits("P2");
    }

    public void CheckUnits(string playerNum) // for all the units, if there are no more of them, remove them from the list
    {
        if (playerNum == "P1")
        {
            for (int i = 0; i < AllP1Units.Count; i++)
            {
                if (!P1PoolUnits.Contains(AllP1Units[i]))
                {
                    AllP1Units.Remove(AllP1Units[i]);
                    //RemovedP1Units.Add(AllP1Units[i]);
                    if (p1ScrollValue == AllP1Units.Count)
                        p1ScrollValue = 0;
                }

            }
        }
        else if (playerNum == "P2")
        {
            for (int i = 0; i < AllP2Units.Count; i++)
            {
                if (!P2PoolUnits.Contains(AllP2Units[i]))
                {
                    AllP2Units.Remove(AllP2Units[i]);
                    //RemovedP2Units.Add(AllP2Units[i]);
                    if (p1ScrollValue == AllP2Units.Count)
                        p1ScrollValue = 0;
                }

            }
        }
    }


    void ControllerMapping()
    {
        if (!plr1Set)
        {
            if (Input.GetButtonDown("J1 A Button") && !ctrsSet.Contains(1))
            {
                display.SetActive(true);
                //display.text += "PLAYER 1 READY\n";
                Color c = p1controllerImg.color;
                c.a = 1f;
                p1controllerImg.color = c; 
                plr1Set = true;
                player1 = "J1 ";
                ctrsSet.Add(1);
            }
            else if (Input.GetButtonDown("J2 A Button") && !ctrsSet.Contains(2))
            {
                display.SetActive(true);
                //display.text += "PLAYER 1 READY\n";
                Color c = p1controllerImg.color;
                c.a = 1f;
                p1controllerImg.color = c;
                plr1Set = true;
                player1 = "J2 ";
                ctrsSet.Add(2);
            }
        }
        if (!plr2Set)
        {
            if (Input.GetButtonDown("J1 A Button") && !ctrsSet.Contains(1))
            {
                display2.SetActive(true);
                //display2.text += "PLAYER 2 READY\n";
                Color c = p1controllerImg2.color;
                c.a = 1f;
                p1controllerImg2.color = c;
                plr2Set = true;
                player2 = "J1 ";
                ctrsSet.Add(1);
            }
            else if (Input.GetButtonDown("J2 A Button") && !ctrsSet.Contains(2))
            {
                display2.SetActive(true);
                //display2.text += "PLAYER 2 READY\n";
                Color c = p1controllerImg2.color;
                c.a = 1f;
                p1controllerImg2.color = c;
                plr2Set = true;
                player2 = "J2 ";
                ctrsSet.Add(2);
            }
        }
    }

    private int ChangeCharacter(int counter, int direction, bool hero, string playerNum)
    {
        int newVal = counter + direction;
        int limit;
        if (hero)
            limit = Hero1Units.Count - 1;
        else if (playerNum == "P1")
            limit = AllP1Units.Count - 1;
        else
            limit = AllP2Units.Count - 1;
      
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

    void ChooseCharacter(string team, bool hero)
    {
        float value;
        if (plr1Set && team == "P1")
        {
            value = Input.GetAxis(player1 + "Left Horizontal");
            if (value == 0.0f)
                value = Input.GetAxis(player1 + "Horizontal D-Pad");
            if (value != 0.0f && p1ScrollTime <= Time.time)
            {
                //play move sound
                move_sound.Play();
                p1ScrollTime = Time.time + 0.25f;
                if (value > 0.0f)
                {
                    p1ScrollValue = ChangeCharacter(p1ScrollValue, 1, hero, "P1"  );
                    //p1Text.text = AllP1Units[p1ScrollValue].name;
                }
                else
                {
                    p1ScrollValue = ChangeCharacter(p1ScrollValue, -1, hero, "P1");
                    //p1Text.text = AllP1Units[p1ScrollValue].name;
                }
            }
        }

        if (plr2Set && team == "P2")
        {
            value = Input.GetAxis(player2 + "Left Horizontal");
            if (value == 0.0f)
                value = Input.GetAxis(player2 + "Horizontal D-Pad");
            if (value != 0.0f && p2ScrollTime <= Time.time)
            {
                //play move sound
                move_sound.Play();
                p2ScrollTime = Time.time + 0.5f;
                if (value > 0.0f)
                {
                    p1ScrollValue = ChangeCharacter(p1ScrollValue, 1, hero, "P2");
                    //p2Text.text = AllP1Units[p2ScrollValue].name;
                }
                else
                {
                    p1ScrollValue = ChangeCharacter(p1ScrollValue, -1, hero, "P2");
                    //p2Text.text = AllP1Units[p2ScrollValue].name;
                }
            }
        }

    }

    

    public void AddUnitsBack(string playerNum) // couuld be cause not adding back the units properly?
    {
        Debug.Log("added units back");
        if (playerNum == "P1")
        {
            for (int i = 0; i < RemovedP1Units.Count; i++)
            {
                ////--------------   Adding Heroes   -------------//

                //if (RemovedP1Units[i].unit_type == "Fortress")
                //    Hero1Units.Insert(0, RemovedP1Units[i]);
                //else if (RemovedP1Units[1].unit_type == "Poison")
                //    Hero1Units.Insert(1, RemovedP1Units[i]);
                //else if (RemovedP1Units[i].unit_type == "Wraith")
                //    Hero1Units.Insert(2, RemovedP1Units[i]);

                //--------------   Adding Units   --------------//
                if (RemovedP1Units[i].unit_type == "Ranger" && !AllP1Units.Contains(RemovedP1Units[i]))
                    AllP1Units.Insert(0, RemovedP1Units[i]);
                else if (RemovedP1Units[i].unit_type == "Warrior" && !AllP1Units.Contains(RemovedP1Units[i]))
                    AllP1Units.Insert(1, RemovedP1Units[i]);
                else if (RemovedP1Units[i].unit_type == "Tank" && !AllP1Units.Contains(RemovedP1Units[i]))
                    AllP1Units.Insert(2, RemovedP1Units[i]);
                else if (RemovedP1Units[i].unit_type == "Healer" && !AllP1Units.Contains(RemovedP1Units[i]))
                    AllP1Units.Insert(3, RemovedP1Units[i]);
                //RemovedP1Units.Remove(RemovedP1Units[i]);
            }
            RemovedP1Units.Clear();
        }
        else
        {
            for (int j = 0; j < RemovedP2Units.Count; j++)
            {
                //-------------   Adding Heroes   -------------//

                //if (RemovedP2Units[j].unit_type == "Fortress")
                //    Hero2Units.Insert(0, RemovedP2Units[j]);
                //else if (RemovedP2Units[j].unit_type == "Poison")
                //    Hero2Units.Insert(1, RemovedP2Units[j]);
                //else if (RemovedP2Units[j].unit_type == "Wraith")
                //    Hero2Units.Insert(2, RemovedP2Units[j]);

                //-------------   Adding Units    -------------//

                if (RemovedP2Units[j].unit_type == "Ranger" && !AllP2Units.Contains(RemovedP2Units[j]))
                    AllP2Units.Insert(0, RemovedP2Units[j]);
                else if (RemovedP2Units[j].unit_type == "Warrior" && !AllP2Units.Contains(RemovedP2Units[j]))
                    AllP2Units.Insert(1, RemovedP2Units[j]);
                else if (RemovedP2Units[j].unit_type == "Tank" && !AllP2Units.Contains(RemovedP2Units[j]))
                    AllP2Units.Insert(2, RemovedP2Units[j]);
                else if (RemovedP2Units[j].unit_type == "Healer" && !AllP2Units.Contains(RemovedP2Units[j]))
                    AllP2Units.Insert(3, RemovedP2Units[j]);
                //RemovedP2Units.Remove(RemovedP2Units[j]);
            }
            RemovedP2Units.Clear();
        }
    }

    private void DraftPick(string team, bool hero)
    {
        if(team == "P1")
        {

            if (Input.GetButton(player1 + "A Button") && p1PickTime <= Time.time)
            {
                //play the picked sound or voice line???
                pick_sound.Play();
                p1PickTime = Time.time + 1f;
                if(hero)
                {
                    Player1Chosen.Add(Hero1Units[p1ScrollValue]);
                    RemovedP1Units.Add(Hero1Units[p1ScrollValue]);
                }
                else
                {
                    Player1Chosen.Add(AllP1Units[p1ScrollValue]);

                }
                if (pool && !hero)
                {
                    P1PoolUnits.Remove(AllP1Units[p1ScrollValue]);
                    if (!RemovedP1Units.Contains(AllP1Units[p1ScrollValue]))
                    {
                        RemovedP1Units.Add(AllP1Units[p1ScrollValue]);
                    }
                    CheckUnits("P1");
                }

            }
            
        }
        if(team == "P2")
        {

            if (Input.GetButton(player2 + "A Button") && p2PickTime <= Time.time)
            {
                //play the picked sound or voice line???
                pick_sound.Play();
                p2PickTime = Time.time + 1f;
                if (hero)
                {
                    Player2Chosen.Add(Hero2Units[p1ScrollValue]);
                    RemovedP2Units.Add(Hero2Units[p1ScrollValue]);

                }
                else
                {
                
                   Player2Chosen.Add(AllP2Units[p1ScrollValue]);

                }
                if (pool && !hero)
                {
                    P2PoolUnits.Remove(AllP2Units[p1ScrollValue]);
                    if (!RemovedP2Units.Contains(AllP2Units[p1ScrollValue]))
                    {
                        RemovedP2Units.Add(AllP2Units[p1ScrollValue]);
                    }
                    CheckUnits("P2");
                }

            }


        }


    }

    void LoadGame(int sceneIndex)
    {
        Debug.Log("NewScene");
        StartCoroutine(UsingLoadingBar(sceneIndex));
    }

    IEnumerator UsingLoadingBar(int sceneIndex)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);//prepares the upcoming scene in the background
        loadingScreen.SetActive(true);
        while (!operation.isDone)
        {
            loadingSlider.value = Mathf.Clamp01(operation.progress / .9f);//creates the percentage for the loading bar
            yield return null;
        }
    }
}
