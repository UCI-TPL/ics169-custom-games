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

    private Text display;

    private Scene current_scene;

    public bool plr1Set, plr2Set;
    //___________________________________________________________________________________________
    public List<StartUnit> AllP1Units = new List<StartUnit>(); // p2 can share this list but just change their tag later
    public List<StartUnit> AllP2Units = new List<StartUnit>();

    public List<StartUnit> PoolUnits = new List<StartUnit>(); // random  pool of units
    private float p1ScrollTime;
    private float p2ScrollTime;
    public int p1ScrollValue;
    public int p2ScrollValue;

    public int p1Cost = 15;
    public int p2Cost = 15;

    public float p1PickTime;
    public float p2PickTime;

    private Text p1Text;
    private Text p2Text;

    public bool nextScene = false;
    private bool doSelect = false;

    public DraftUI draftUI;

    public enum DraftStates
    {
        Initialize,
        P1_Pick_1,
        P2_Pick_1,
        P1_Pick_2,
        P2_Pick_2,
        P1_Pick_3,
        P2_Pick_3,
        Enter_Battle
    }

    [SerializeField] public DraftStates currentState = DraftStates.Initialize;

    //____________________________________________shortcuts______________________________________
    public bool one_player;
    public bool pool = false;


    // Use this for initialization
    void Start()
    {
        display = FindObjectOfType<Text>();
        DontDestroyOnLoad(this.gameObject);
        current_scene = SceneManager.GetActiveScene();
        
    }

    // Update is called once per frame
    void Update()
    {
        current_scene = SceneManager.GetActiveScene(); // terrible coding fix later
        if (current_scene.name == "CMapping")
        {

            if (ctrsSet.Contains(1) && ctrsSet.Contains(2) && !one_player) // both players ready
                SceneManager.LoadScene(2);
            else if (ctrsSet.Contains(1) && one_player)
                SceneManager.LoadScene(2);
            else
            {
                ControllerMapping();
            }
        }
        if (current_scene.name == "SelectCharacter")
        {
            switch(currentState)
            {
                case (DraftStates.Initialize):
                    if (!doSelect)
                    {
                        doSelect = true;
                        //display = GameObject.Find("Pool Text").GetComponent<Text>();
                        if (pool)
                        {
                            RandomPool();
                        }
                        //p1Text = GameObject.Find("Player1Text").GetComponent<Text>();
                        //p2Text = GameObject.Find("Player2Text").GetComponent<Text>();
                        currentState = DraftStates.P1_Pick_1;
                    }
                    break;
                case (DraftStates.P1_Pick_1):
                    if (Player1Chosen.Count == 1)
                    {
                        draftUI.P1Pick1();
                        if (one_player)
                            currentState = DraftStates.P1_Pick_2;
                        else
                        {
                            draftUI.ChangePlayer();
                            currentState = DraftStates.P2_Pick_1;
                        }
                    }
                    if (pool)
                        CheckPool();
                    ChooseCharacter("P1");
                    DraftPick("P1");
                    break;
                case (DraftStates.P2_Pick_1):
                    if(Player2Chosen.Count == 1)
                    {
                        draftUI.P2Pick1();
                    }
                    else if (Player2Chosen.Count == 2)
                    {
                        draftUI.P2Pick2();
                        currentState = DraftStates.P1_Pick_2;
                    }
                    if (pool)
                        CheckPool();
                    ChooseCharacter("P2");
                    DraftPick("P2");
                    break;
                case (DraftStates.P1_Pick_2):
                    if(p1Cost <= 0)
                    {
                        currentState = DraftStates.P2_Pick_2;
                    }
                    if(Player1Chosen.Count == 2)
                    {
                        draftUI.P1Pick2();
                    }
                    else if (Player1Chosen.Count == 3)
                    {
                        draftUI.P1Pick3();
                        if(one_player)
                            currentState = DraftStates.P1_Pick_3;
                        else
                        {
                            currentState = DraftStates.P2_Pick_2;
                        }
                    }

                    if (pool)
                        CheckPool();
                    ChooseCharacter("P1");
                    DraftPick("P1");
                    break;
                case (DraftStates.P2_Pick_2):
                    if (p2Cost <= 0)
                    {
                        currentState = DraftStates.P1_Pick_3;
                    }
                    if (Player2Chosen.Count == 4)
                    {
                        currentState = DraftStates.P1_Pick_3;
                    }
                    if (pool)
                        CheckPool();
                    ChooseCharacter("P2");
                    DraftPick("P2");
                    break;
                case (DraftStates.P1_Pick_3):
                    if (p1Cost <= 0)
                    {
                        currentState = DraftStates.Enter_Battle;
                    }
                    if (Player1Chosen.Count == 5)
                    {
                        if(one_player)
                            currentState = DraftStates.Enter_Battle;
                        else
                        {
                            currentState = DraftStates.P2_Pick_3;
                        }
                    }
                    if (pool)
                        CheckPool();
                    ChooseCharacter("P1");
                    DraftPick("P1");
                    break;
                case (DraftStates.P2_Pick_3):
                    if (p2Cost <= 0)
                    {
                        currentState = DraftStates.Enter_Battle;
                    }
                    if (Player2Chosen.Count == 5) 
                    {
                        currentState = DraftStates.Enter_Battle;
                    }
                    if (pool)
                        CheckPool();
                    ChooseCharacter("P2");
                    DraftPick("P2");
                    break;
                case (DraftStates.Enter_Battle):
                    SceneManager.LoadScene(3);
                    break;
            }
        }
    }

    private void CheckPool()
    {
        int[] counts = new int[AllP1Units.Count];
        for (int j = 0; j < AllP1Units.Count; j++)
        {
            for (int i = 0; i < PoolUnits.Count; i++)
            {
                if(PoolUnits[i] == AllP1Units[j])
                    counts[j]++;
            }
        }
        string result = "";
        for(int k = 0; k < counts.Length; k++)
        {
            result += AllP1Units[k].name + " x" + counts[k] + "\n";
        }
        //display.text = result;
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
                display.text += "PLAYER 1 READY\n";
                plr1Set = true;
                player1 = "J2 ";
                ctrsSet.Add(2);
            }
        }
        if (!plr2Set)
        {
            if (Input.GetButtonDown("J1 A Button") && !ctrsSet.Contains(1))
            {
                display.text += "PLAYER 2 READY\n";
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

    void ChooseCharacter(string team)
    {
        float value;
        if (plr1Set && team == "P1")
        {
            value = Input.GetAxis(player1 + "Left Horizontal");
            if (value != 0.0f && p1ScrollTime <= Time.time)
            {
                p1ScrollTime = Time.time + 0.25f;
                if (value > 0.0f)
                {
                    p1ScrollValue = ChangeCharacter(p1ScrollValue, 1);
                    //p1Text.text = AllP1Units[p1ScrollValue].name;
                }
                else
                {
                    p1ScrollValue = ChangeCharacter(p1ScrollValue, -1);
                    //p1Text.text = AllP1Units[p1ScrollValue].name;
                }
            }
        }

        if (plr2Set && team == "P2")
        {
            value = Input.GetAxis(player2 + "Left Horizontal");
            if (value != 0.0f && p2ScrollTime <= Time.time)
            {
                p2ScrollTime = Time.time + 0.5f;
                if (value > 0.0f)
                {
                    p2ScrollValue = ChangeCharacter(p2ScrollValue, 1);
                    //p2Text.text = AllP1Units[p2ScrollValue].name;
                }
                else
                {
                    p2ScrollValue = ChangeCharacter(p2ScrollValue, -1);
                    //p2Text.text = AllP1Units[p2ScrollValue].name;
                }
            }
        }

    }

    private void DraftPick(string team)
    {
        if(team == "P1")
        {
            
            if (Input.GetButton(player1 + "A Button") && p1PickTime <= Time.time)
            {
                p1PickTime = Time.time + 1f;
                if(p1Cost - AllP1Units[p1ScrollValue].cost >= 0)
                {
                    Player1Chosen.Add(AllP1Units[p1ScrollValue]);
                    p1Cost -= AllP1Units[p1ScrollValue].cost;
                }
                else
                {
                    Debug.Log("unit cost too much");
                    return;
                }
                if (pool)
                {
                    PoolUnits.Remove(AllP1Units[p1ScrollValue]);
                    CheckUnits();
                }
            }
        }
        if(team == "P2")
        {
            if (Input.GetButton(player2 + "A Button") && p2PickTime <= Time.time)
            {
	            p2PickTime = Time.time + 1f;
	            if (p2Cost - AllP2Units[p2ScrollValue].cost >= 0)
	            {
	                Player2Chosen.Add(AllP2Units[p2ScrollValue]);
	                p2Cost -= AllP2Units[p2ScrollValue].cost;
	            }
	            else
	            {
	                Debug.Log("unit cost too much");
	                return;
	            }
	            if (pool)
	            {
	                PoolUnits.Remove(AllP1Units[p2ScrollValue]);
	                CheckUnits();
	            }
            }
            
        }


    }
}
