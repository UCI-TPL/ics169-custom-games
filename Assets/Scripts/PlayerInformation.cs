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

    public GameObject display,display2;
    public Image p1controllerImg, p1controllerImg2;

    private Scene current_scene;

    public bool plr1Set, plr2Set;
    //___________________________________________________________________________________________
    public List<StartUnit> AllP1Units = new List<StartUnit>(); // p2 can share this list but just change their tag later
    public List<StartUnit> AllP2Units = new List<StartUnit>();
    public List<StartUnit> Hero1Units = new List<StartUnit>();
    public List<StartUnit> Hero2Units = new List<StartUnit>();
    public List<StartUnit> PoolUnits = new List<StartUnit>(); // random  pool of units
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
    private bool one_time;

    public DraftUI draftUI;

    public GameObject loadingScreen;
    public Slider loadingSlider;

    public GameObject ranger;

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

        loadingScreen.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        current_scene = SceneManager.GetActiveScene(); // terrible coding fix later
        if (current_scene.name == "CMapping")
        {

            if (ctrsSet.Contains(1) && (ctrsSet.Contains(2) || one_player)) // both players ready
                //SceneManager.LoadScene(2);
                LoadGame(2);
            //else if (ctrsSet.Contains(1) && one_player)
            //    //SceneManager.LoadScene(2);
            //    LoadGame(2);
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
                        if (pool)
                        {
                            RandomPool();
                        }
                        currentState = DraftStates.P1_Pick_1;
                    }
                    break;

                case (DraftStates.P1_Pick_1):
                    if (Player1Chosen.Count == 1)
                    {
                        draftUI.P1Pick1();
                        if (one_player)
                        {
                            currentState = DraftStates.P1_Pick_2;   
                        }
                        else
                        {
                            //draftUI.ChangePlayer();
                            currentState = DraftStates.P2_Pick_1;
                        }
                    }
                    if (!draftUI.blinking)
                        StartCoroutine(draftUI.Blink(draftUI.P1Choice1));
                    if (pool)
                        CheckPool();
                    ChooseCharacter("P1",true);
                    DraftPick("P1", true);
                    break;

                case (DraftStates.P2_Pick_1):
                    if(Player2Chosen.Count == 0)
                    {
                        draftUI.P2Choice1.color = draftUI.baby_blue;
                        if (!draftUI.blinking)
                            StartCoroutine(draftUI.Blink(draftUI.P2Choice1));
                        ChooseCharacter("P2", true);
                        DraftPick("P2", true);
                        break;
                    }
                    if(Player2Chosen.Count == 1)
                    {
                        draftUI.P2Pick1();
                        draftUI.P2Choice2.color = draftUI.baby_blue;
                        if (!draftUI.blinking)
                            StartCoroutine(draftUI.Blink(draftUI.P2Choice2));
                    }
                    else if (Player2Chosen.Count == 2)
                    {
                        draftUI.P2Pick2();
                        currentState = DraftStates.P1_Pick_2;
                    }
                    if (pool)
                        CheckPool();
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
                        draftUI.P1Choice2.color = draftUI.baby_blue;
                        if(!draftUI.blinking)
                            StartCoroutine(draftUI.Blink(draftUI.P1Choice2));
                    }
                    else if(Player1Chosen.Count == 2)
                    {
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
                            currentState = DraftStates.P2_Pick_2;
                        }
                    }

                    if (pool)
                        CheckPool();
                    ChooseCharacter("P1",false);
                    DraftPick("P1",false);
                    break;

                case (DraftStates.P2_Pick_2):
                    if (p2Cost <= 0)
                    {
                        currentState = DraftStates.P1_Pick_3;
                    }
                     if (Player2Chosen.Count == 2)
                    {
                        draftUI.P2Pick2();
                        draftUI.P2Choice3.color = draftUI.baby_blue;
                        if (!draftUI.blinking)
                            StartCoroutine(draftUI.Blink(draftUI.P2Choice3));
                    }
                    if (Player2Chosen.Count == 3)
                    {
                        draftUI.P2Pick3();
                        draftUI.P2Choice4.color = draftUI.baby_blue;
                        if (!draftUI.blinking)
                            StartCoroutine(draftUI.Blink(draftUI.P2Choice4));
                    }
                    if (Player2Chosen.Count == 4)
                    {
                        draftUI.P2Pick4();
                        currentState = DraftStates.P1_Pick_3;
                    }
                    if (pool)
                        CheckPool();
                    ChooseCharacter("P2",false);
                    DraftPick("P2",false);
                    break;

                case (DraftStates.P1_Pick_3):
                    //if (p1Cost <= 0)
                    //{
                    //    currentState = DraftStates.Check_Teams; //DraftStates.Enter_Battle;
                    //}
                    if(Player1Chosen.Count == 3)
                    {
                        draftUI.P1Choice4.color = draftUI.baby_blue;
                        if (!draftUI.blinking)
                            StartCoroutine(draftUI.Blink(draftUI.P1Choice4));
                    }
                    if(Player1Chosen.Count == 4)
                    {
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
                            currentState = DraftStates.P2_Pick_3;
                        }
                    }
                    if (pool)
                        CheckPool();
                    ChooseCharacter("P1",false);
                    DraftPick("P1",false);
                    break;

                case (DraftStates.P2_Pick_3):
                    //if (p2Cost <= 0)
                    //{
                    //    currentState = DraftStates.Check_Teams; //DraftStates.Enter_Battle;
                    //}
                    if(Player2Chosen.Count == 3)
                    {
                        draftUI.P2Choice4.color = draftUI.baby_blue;
                        if (!draftUI.blinking)
                            StartCoroutine(draftUI.Blink(draftUI.P2Choice4));
                    }
                    if(Player2Chosen.Count == 4)
                    {
                        draftUI.P2Pick4();
                        draftUI.P2Choice5.color = draftUI.baby_blue;
                        if (!draftUI.blinking)
                            StartCoroutine(draftUI.Blink(draftUI.P2Choice5));
                    }
                    if (Player2Chosen.Count == 5) 
                    {
                        draftUI.P2Pick5();
                        currentState = DraftStates.Check_Teams; //DraftStates.Enter_Battle;
                    }
                    if (pool)
                        CheckPool();
                    ChooseCharacter("P2",false);
                    DraftPick("P2",false);
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
        if (((Player1Chosen.Count == 5 && Player2Chosen.Count == 5) || (Player1Chosen.Count == 5 && one_player)) && one_time)
        {
            LoadGame(3);
            one_time = false;
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
        CheckUnits();
    }

    public void CheckUnits()
    {
        for(int i = 0; i < AllP1Units.Count; i++)
        {
            if(!PoolUnits.Contains(AllP1Units[i]))
            {
                AllP1Units.Remove(AllP1Units[i]);
                AllP2Units.Remove(AllP2Units[i]);
                if (p1ScrollValue == AllP1Units.Count)
                    p1ScrollValue = 0;
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

    private int ChangeCharacter(int counter, int direction, bool hero)
    {
        int newVal = counter + direction;
        int limit;
        if(hero)
            limit = Hero1Units.Count - 1;
        else
            limit = AllP1Units.Count - 1;
      
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
            if (value != 0.0f && p1ScrollTime <= Time.time)
            {
                //play move sound
                move_sound.Play();
                p1ScrollTime = Time.time + 0.25f;
                if (value > 0.0f)
                {
                    p1ScrollValue = ChangeCharacter(p1ScrollValue, 1, hero);
                    //p1Text.text = AllP1Units[p1ScrollValue].name;
                }
                else
                {
                    p1ScrollValue = ChangeCharacter(p1ScrollValue, -1, hero);
                    //p1Text.text = AllP1Units[p1ScrollValue].name;
                }
            }
        }

        if (plr2Set && team == "P2")
        {
            value = Input.GetAxis(player2 + "Left Horizontal");
            if (value != 0.0f && p2ScrollTime <= Time.time)
            {
                //play move sound
                move_sound.Play();
                p2ScrollTime = Time.time + 0.5f;
                if (value > 0.0f)
                {
                    p1ScrollValue = ChangeCharacter(p1ScrollValue, 1, hero);
                    //p2Text.text = AllP1Units[p2ScrollValue].name;
                }
                else
                {
                    p1ScrollValue = ChangeCharacter(p1ScrollValue, -1, hero);
                    //p2Text.text = AllP1Units[p2ScrollValue].name;
                }
            }
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
                    if (p1Cost - Hero1Units[p1ScrollValue].cost >= 0)
                    {
                        Player1Chosen.Add(Hero1Units[p1ScrollValue]);
                        p1Cost -= Hero1Units[p1ScrollValue].cost;
                    }
                    else
                    {
                        Debug.Log("unit cost too much");
                        return;
                    }
                }
                else
                {
                    if (p1Cost - AllP1Units[p1ScrollValue].cost >= 0)
                    {
                        Player1Chosen.Add(AllP1Units[p1ScrollValue]);
                        p1Cost -= AllP1Units[p1ScrollValue].cost;
                    }
                    else
                    {
                        Debug.Log("unit cost too much");
                        return;
                    }
                }
                if (pool && !hero)
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
                //play the picked sound or voice line???
                pick_sound.Play();
                p2PickTime = Time.time + 1f;
                if (hero)
                {
                    if (p2Cost - Hero2Units[p1ScrollValue].cost >= 0)
                    {
                        Player2Chosen.Add(Hero2Units[p1ScrollValue]);
                        p2Cost -= Hero2Units[p1ScrollValue].cost;
                    }
                    else
                    {
                        Debug.Log("unit cost too much");
                        return;
                    }
                }
                else
                {
                    if (p2Cost - AllP2Units[p1ScrollValue].cost >= 0)
                    {
                        Player2Chosen.Add(AllP2Units[p1ScrollValue]);
                        p2Cost -= AllP2Units[p1ScrollValue].cost;
                    }
                    else
                    {
                        Debug.Log("unit cost too much");
                        return;
                    }
                }
	            if (pool)
	            {
	                PoolUnits.Remove(AllP1Units[p1ScrollValue]);
	                CheckUnits();
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
