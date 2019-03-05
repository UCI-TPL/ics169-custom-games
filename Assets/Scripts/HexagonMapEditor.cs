using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HexagonMapEditor : MonoBehaviour
{
    public Camera Main_Cam;
    public PlayerInformation PlayerInfo;
    public Grid hexGrid;
    public GameObject UI_P1_Sel;
    public GameObject UI_P2_Sel;
    public GameObject UI_P1_Hov;
    public GameObject UI_P2_Hov;
    public GameObject UI_Turn;
    public GameObject P1_Cooldown;
    public GameObject P2_Cooldown;
    private Text P1_Cooldown_Text;
    private Text P2_Cooldown_Text;
    public BattleUI BattleUI_P1;
    public BattleUI BattleUI_P2;
    public BattleUI BattleUI_Turn;
    public Color32 P1_Color;
    public Color32 P2_Color;
    public Color32 color_r;
    public Color32 color_w;
    public Color32 Greyed_Unit_Color;
    public Cursor cursor;
    private int p1_unit_rotation_value;
    private int p2_unit_rotation_value;
    [SerializeField]
    public List<Image> P1_Unit_Icons = new List<Image>();
    [SerializeField]
    public List<Image> P2_Unit_Icons = new List<Image>();
    public GameObject P1_portrait_UI;
    public GameObject P2_portrait_UI;
    public Team_Portrait_UI P1_Team_portrait_UI;
    public Team_Portrait_UI P2_Team_portrait_UI;
    public Color32 Unit_Hurt_Color;
    public GameObject WinCanvas;
    public Text winText;
    public GameObject FirstObject;




    private List<StartUnit> Player1Chosen = new List<StartUnit>();
    public List<StartUnit> Player2Chosen = new List<StartUnit>();

    public List<int> P1start = new List<int>() { 104, 105, 106, 124, 125, 126, 145, 146 };
    public List<int> P2start = new List<int>() { 233, 234, 252, 253, 254, 272, 273, 274 };

    public List<StartUnit> P1Team = new List<StartUnit>(); // list of player 1 team
    public List<StartUnit> P2Team = new List<StartUnit>(); // list of player 2 team
    public List<StartUnit> MoveableUnits; // mulitpurpose list to hold units with actions

    public StartUnit SelectedUnit; // the current unit that is selected
    public bool isUnitSelected = false; // boolean to tell if a unit is selected
    public HexagonCell unitCell; // cell the selected unit is on

    public bool initializing = true;

    public bool attacking = false;
    public bool whileAttacking = false;

    public bool moveInProgress = false;

    public bool onePlayer;
    public bool cur_attacking;
    public int attack_count;
    public bool wasP1Turn = true;
    public bool debuffed = false;

    /***************************************************************************/
    public AudioSource select_sound, turn_change_sound, cycle_unit_sound;


    /***************************************************************************/
    private bool first_turn = true;
    public bool move_confirmed = false;
    public string move_string = "no";
    public GameObject Confirm_Window;
    public GameObject Dynamic_Controls_list_obj;
    private UI_List_Manager Dynamic_Controls_list;

    /***********************ENVIRONMENTAL VARIABLES*****************************/
    public EnvironmentalHazard[] hazardList; // list of all the types of hazards possible
    //create a list of current hazards on the board (figure out what to put in the list (time left on board, type of hazard, size)
    [SerializeField]
    public List<EnvironmentalHazard.HazardInfo> hazardsOnGrid = new List<EnvironmentalHazard.HazardInfo>();
    bool environmentExecuting = false;
    public bool incoming = false;
    public int incoming_in = int.MaxValue; // the counter for how long until the hazard comes to the map
    int whichHazard; // keep track of which hazard, int gives you the index in hazardList
    HexagonCoord hazardSpot; // where the hazard incoming will be
    int size; // how big the hazard is
    public bool hazardsExecuting = false;
    public bool hazardsFinished = false;
    public int hazardCount = 0;
    /***********************STATUS EFFECTS*****************************/
    public List<EnvironmentalHazard.HazardInfo> P1StatusOnGrid = new List<EnvironmentalHazard.HazardInfo>();
    public List<EnvironmentalHazard.HazardInfo> P2StatusOnGrid = new List<EnvironmentalHazard.HazardInfo>();
    public bool statusJustStarted = true;
    public bool statusExecuting = false;
    public bool statusFinished = false;
    public int statusCount = 0;

    public bool allow_cursor_control;

    public List<HexagonCell> Units_To_Delete = new List<HexagonCell>();

    public int max_sprites_per_unit;


    public enum TurnStates
    {
        ENVIRONMENT,
        P1_MOVE,
        P1_ATTACK,
        P2_MOVE,
        P2_ATTACK,
        CHECK,
        P1_WIN,
        P2_WIN,
        P1_STATUS_EFFECT,
        P2_STATUS_EFFECT,
        END
    }

    [SerializeField] public TurnStates currentState;
    private void Awake()
    {
        PlayerInfo = FindObjectOfType<PlayerInformation>();
    }
    // Use this for initialization
    void Start()
    {
        initializing = true;
        cur_attacking = false;
        attacking = false;
        allow_cursor_control = true;
        attack_count = 0;
        //UI = GetComponentInChildren<BattleUI>();
        P1_Team_portrait_UI = P1_portrait_UI.GetComponent<Team_Portrait_UI>();
        P2_Team_portrait_UI = P2_portrait_UI.GetComponent<Team_Portrait_UI>();

        BattleUI_P1 = UI_P1_Sel.GetComponent<BattleUI>();
        BattleUI_P2 = UI_P2_Sel.GetComponent<BattleUI>();

        BattleUI_P1.Hide();
        BattleUI_P2.Hide();

        BattleUI_Turn = UI_Turn.GetComponent<BattleUI>();

        Dynamic_Controls_list = Dynamic_Controls_list_obj.GetComponent<UI_List_Manager>();

        P1_Cooldown_Text = P1_Cooldown.GetComponentInChildren<Text>();
        P2_Cooldown_Text = P2_Cooldown.GetComponentInChildren<Text>();

        if (initializing) // stop loop if already doing it
        {
            //initializing = false;
            InitialPhase(PlayerInfo.Player1Chosen, 1);
            if (PlayerInfo.one_player)
                InitialPhase(Player2Chosen, 2);
            else
                InitialPhase(PlayerInfo.Player2Chosen, 2);
            FindTeam("Player 1"); // find the units for player 1's team
            FindTeam("Player 2"); // "             " for player 2's team
            if (P1Team[0].GetComponent<HeroUnit>().myType == HeroUnit.BuffType.OneTime || P1Team[0].GetComponent<HeroUnit>().myType == HeroUnit.BuffType.EveryTurn)
            {
                HexagonCell myCell = hexGrid.GetCell(P1Team[0].transform.position);
                P1Team[0].GetComponent<HeroUnit>().BuffTeam("P1", myCell);
            }
            if (P2Team[0].GetComponent<HeroUnit>().myType == HeroUnit.BuffType.OneTime || P2Team[0].GetComponent<HeroUnit>().myType == HeroUnit.BuffType.EveryTurn)
            {
                HexagonCell myCell = hexGrid.GetCell(P2Team[0].transform.position);
                P2Team[0].GetComponent<HeroUnit>().BuffTeam("P2", myCell);
            }
        }
        MoveableUnits = new List<StartUnit>(P1Team); // put player 1's team in since they're going first
        currentState = TurnStates.P1_MOVE;
        if (MoveableUnits.Count > 0)
        {
            Snap_To_First_Unit();
            p1_unit_rotation_value = 0;
            //Snap_To_Next_Unit(true);
        }
        //StartCoroutine(InitializingTeams());
        P1_Team_portrait_UI.Initialize_Portraits(P1Team);
        P2_Team_portrait_UI.Initialize_Portraits(P2Team);
        
    }

    // Update is called once per frame
    void Update()
    {

        switch (currentState)
        {
            case (TurnStates.ENVIRONMENT):
                if (!environmentExecuting) // do once
                {
                    Units_To_Delete.Clear();
                    allow_cursor_control = false;
                    environmentExecuting = true;
                    if (incoming) //a environmental hazard is coming already
                    {
                        incoming_in -= 1;

                        List<HexagonCell> tilesToEffect = new List<HexagonCell>();
                        HexagonCell curr = hexGrid.Get_Cell_Index(new HexagonCoord(hazardSpot.x, hazardSpot.z));
                        for (int i = 0; i < hexGrid.cells.Length; i++)
                        {
                            int distance = curr.coords.FindDistanceTo(hexGrid.cells[i].coords);
                            if (distance <= size)
                            {
                                tilesToEffect.Add(hexGrid.cells[i]);
                            }
                        }
                        for (int j = 0; j < tilesToEffect.Count; j++)
                        {
                            // HERE IS WHERE WE CHANGE THE EFFECT FOR THE HAZARD EFFECT COUNTER ON THE MAP
                            // ex) tilesToEffect[j].stopWatch[incoming_in].enabled = true;
                            //     tilesToEffect[j].stopWatch[incoming_in+1].enabled = false;
                        }


                        if (incoming_in == 0) // time to create hazard
                        {
                            // REMOVE THE COUNTER EFFECT
                            for (int j = 0; j < tilesToEffect.Count; j++)
                            {
                                // HERE IS WHERE WE CHANGE THE EFFECT FOR THE HAZARD EFFECT COUNTER ON THE MAP
                                // ex) tilesToEffect[j].stopWatch[incoming_in].enabled = false;
                            }

                            hazardsOnGrid.Add(hazardList[whichHazard].CreateHazard(size, hazardSpot, hexGrid));
                            incoming_in = int.MaxValue;
                            incoming = false;
                            
                        }
                    }
                    else // no environmental hazard coming
                    {
                        int chance = Random.Range(0, 1); // 1/10  chance to create hazard
                        if (chance == 0)
                        {
                            int hazard = Random.Range(0, 2); // 2 hazards right now *** Random.Range(inclusive, exclusive)
                            incoming = true;
                            incoming_in = hazardList[hazard].timeToCome; // how long before it lands on the board
                            whichHazard = hazard; // decides type of hazard that is coming
                            int randRange = Random.Range(0, hexGrid.cells.Length);
                            size = Random.Range(1, 3);

                            hazardSpot = hexGrid.cells[randRange].coords;
                            List<HexagonCell> tilesToEffect = new List<HexagonCell>();
                            HexagonCell curr = hexGrid.Get_Cell_Index(new HexagonCoord(hazardSpot.x, hazardSpot.z));
                            for (int i = 0; i < hexGrid.cells.Length; i++)
                            {
                                int distance = curr.coords.FindDistanceTo(hexGrid.cells[i].coords);
                                if (distance <= size)
                                {
                                    tilesToEffect.Add(hexGrid.cells[i]);
                                }
                            }
                            for (int j = 0; j < tilesToEffect.Count; j++)
                            {
                                // USE tilesToEffect LIST TO EDIT 
                                // ex) tilesToEffect[j].stopWatch[incoming_in].enabled = true;
                            }
                        }
                    }
                }
                if (!hazardsExecuting && environmentExecuting) // hazard exectution 
                {
                    hazardsExecuting = true;
                    if (hazardCount == hazardsOnGrid.Count) // when hazards are done
                    {
                        for(int i = 0; i < hazardsOnGrid.Count; i++)
                        {
                            if (hazardsOnGrid[i].timeLeft <= 0)
                            {
                                EnvironmentalHazard.HazardInfo h = hazardsOnGrid[i];
                                //Debug.Log("hazard x:" + h.x + " y:" + h.y + " z:" + h.z);
                                //Debug.Log("going into remove hazard: "  + h.placedWeatherVane);
                                h.type.RemoveHazard(hexGrid, h.x, h.z, h.size, h.placedWeatherVane);
                                hazardsOnGrid.Remove(hazardsOnGrid[i]);
                            }
                        }
                        foreach (HexagonCell units_cell in Units_To_Delete)
                        {
                            int index = units_cell.coords.X_coord + units_cell.coords.Z_coord * hexGrid.width + units_cell.coords.Z_coord / 2;
                            RemoveUnitInfo(units_cell, index);
                        }
                        hazardsFinished = true;
                    }
                    if (hazardCount < hazardsOnGrid.Count) //for every hazard
                    {
                        EnvironmentalHazard.HazardInfo h = hazardsOnGrid[hazardCount];

                        hazardsOnGrid[hazardCount] = new EnvironmentalHazard.HazardInfo(h.type, h.x, h.y, h.z, h.timeLeft-1, h.size, h.placedWeatherVane);
                        //Debug.Log("hazard time left: " + h.timeLeft--.ToString());
                        //Debug.Log("x: " + h.x + " z: " + h.z);
                        //Debug.Log("placed weather vane:" + h.placedWeatherVane);
                        StartCoroutine(Snap_To_Hazard(h.x, h.z, h.type.anim_time));
                        StartCoroutine(HandleHazards(hazardCount));
                        
                    }
                }
                if (hazardsFinished) // when hazarrds are done
                {
                    BattleUI_Turn.turn.text = "PLAYER 1";
                    BattleUI_Turn.turn_info_Image.GetComponent<Image>().color = P1_Color;
                    StartCoroutine(turn_animation_starter());
                    environmentExecuting = false;
                    hazardsExecuting = false;
                    hazardsFinished = false;
                    hazardCount = 0;
                    allow_cursor_control = true;
                    Snap_To_First_Unit();
                    currentState = TurnStates.P1_MOVE;
                }
                
                break;
            case (TurnStates.P1_MOVE):
                if (first_turn)
                {
                    first_turn = false;
                    if (MoveableUnits.Count > 0)
                    {
                        Snap_To_First_Unit();
                        p1_unit_rotation_value = 0;
                    }
                }
                if (P1Team[0].GetComponent<HeroUnit>().myType == HeroUnit.BuffType.EveryTurn && !debuffed)
                {
                    HexagonCell myCell = hexGrid.GetCell(P1Team[0].transform.position);
                    P1Team[0].GetComponent<HeroUnit>().DebufTeam("P1", myCell);
                    debuffed = true;

                }
                if (MoveableUnits.Count == 0) // once all units move break
                {
                    if (P1Team[0].GetComponent<HeroUnit>().myType == HeroUnit.BuffType.EveryTurn)
                    {
                        HexagonCell myCell = hexGrid.GetCell(P1Team[0].transform.position);
                        P1Team[0].GetComponent<HeroUnit>().BuffTeam("P1", myCell);
                    }
                    currentState = TurnStates.P1_ATTACK;
                    debuffed = false;
                    allow_cursor_control = false;
                    MoveableUnits = new List<StartUnit>(P2Team);
                    //Turn Units Back To Normal Color
                    for (int i = 0; i < MoveableUnits.Count; i++)
                    {
                        Anima2D.SpriteMeshInstance[] Unit_Sprites = MoveableUnits[i].gameObject.GetComponentsInChildren<Anima2D.SpriteMeshInstance>();
                        for (int k = 0; k < Unit_Sprites.Length; k++)
                        {
                            Unit_Sprites[k].color = Color.white;
                        }
                    }

                }
                //UI.turn.text = "TURN:PLAYER 1";

                
                MovePhase(PlayerInfo.player1);
                //MovePhase();

                break;
            case (TurnStates.P1_ATTACK):
                if (!attacking) // only call once
                {

                    //sets all units on team to currently attacking
                    if (!cur_attacking)
                    {
                        Units_To_Delete.Clear();
                        foreach (StartUnit unit in P1Team) {
                            unit.currently_attacking = true;
                        }
                        cur_attacking = true;
                        //get the ball rolling with attack_count

                        StartCoroutine(P1Team[attack_count].BasicAttack(hexGrid, hexGrid.GetCell(P1Team[attack_count].transform.position)));
                    }

                    //calls attack on each next unit after previous has attacked
                    if(attack_count < P1Team.Count - 1)
                    {
                        if(P1Team[attack_count].currently_attacking == false)
                        {
                            
                            //next unit attacks because prev has finished
                            attack_count += 1;
                            StartCoroutine(P1Team[attack_count].BasicAttack(hexGrid, hexGrid.GetCell(P1Team[attack_count].transform.position)));
                        }
                        //else
                        //{
                        //    //unit is still attacking so do nothing I guess
                        //}
                    }
                    if (P1Team[P1Team.Count - 1] == null || P1Team[P1Team.Count - 1].currently_attacking == false) 
                    {
                        
                        //the final unit has finished attacking or there were no units to attack in the first place
                        attacking = true;
                    }

                    
                }
                else
                {
                    foreach(HexagonCell units_cell in Units_To_Delete)
                    {
                        int index = units_cell.coords.X_coord + units_cell.coords.Z_coord * hexGrid.width + units_cell.coords.Z_coord / 2;
                        RemoveUnitInfo(units_cell, index);
                    }

                    cur_attacking = false;
                    attack_count = 0;
                    attacking = false;

                    //currentState = TurnStates.P2_MOVE;
					BattleUI_Turn.turn.text = "PLAYER 2";
                    BattleUI_Turn.turn_info_Image.GetComponent<Image>().color = P2_Color;
                    StartCoroutine(turn_animation_starter());
                    currentState = TurnStates.P1_STATUS_EFFECT;
                    allow_cursor_control = true;
                    if (MoveableUnits.Count > 0)
                    {
                        Snap_To_First_Unit();
                        p2_unit_rotation_value = 0;
                    }
                    P2_Team_portrait_UI.Update_Portraits();
                    P1_Team_portrait_UI.Update_Portraits();
                    foreach (StartUnit _unit in P1Team)
                    {
                        if (_unit != null)
                        {
                            Assign_Stats_Var(_unit.gameObject.GetComponentInChildren<BattleUI>(), _unit);
                        }
                    }
                    foreach (StartUnit _unit in P2Team)
                    {
                        if (_unit != null)
                        {
                            Assign_Stats_Var(_unit.gameObject.GetComponentInChildren<BattleUI>(), _unit);
                        }
                    }
                }
                break;

            case (TurnStates.P1_STATUS_EFFECT):
                if(statusJustStarted)
                {
                    statusJustStarted = false;
                    Units_To_Delete.Clear();
                }
                if (!statusExecuting) // status effect exectution 
                {
                    statusExecuting = true;
                    if (statusCount == P1StatusOnGrid.Count) // when status effects are done
                    {
                        for (int i = 0; i < P1StatusOnGrid.Count; i++)
                        {
                            if (P1StatusOnGrid[i].timeLeft <= 0)
                            {
                                EnvironmentalHazard.HazardInfo h = P1StatusOnGrid[i];
                                h.type.RemoveHazard(hexGrid, h.x, h.z, h.size, h.placedWeatherVane);
                                P1StatusOnGrid.Remove(P1StatusOnGrid[i]);
                            }
                        }

                        foreach (HexagonCell units_cell in Units_To_Delete)
                        {
                            int index = units_cell.coords.X_coord + units_cell.coords.Z_coord * hexGrid.width + units_cell.coords.Z_coord / 2;
                            RemoveUnitInfo(units_cell, index);
                        }

                        statusFinished = true;
                    }
                    if (statusCount < P1StatusOnGrid.Count) //for every status
                    {
                        EnvironmentalHazard.HazardInfo h = P1StatusOnGrid[statusCount];

                        P1StatusOnGrid[statusCount] = new EnvironmentalHazard.HazardInfo(h.type, h.x, h.y, h.z, h.timeLeft - 1, h.size);
                        //Debug.Log("hazard time left: " + h.timeLeft--.ToString());
                        //Debug.Log("x: " + h.x + " z: " + h.z);
                        StartCoroutine(Snap_To_Hazard(h.x, h.z, h.type.anim_time));
                        StartCoroutine(HandleStatus(statusCount,1));

                    }
                }
                if (statusFinished) // when hazarrds are done
                {
                    //BattleUI_Turn.turn.text = "PLAYER 1";
                    //BattleUI_Turn.turn_info_Image.GetComponent<Image>().color = P1_Color;
                    statusJustStarted = true;
                    wasP1Turn = true;
                    statusExecuting = false;
                    statusFinished = false;
                    statusCount = 0;
                    allow_cursor_control = true;
                    //Snap_To_First_Unit();
                    currentState = TurnStates.CHECK;
                }
                break;

            case (TurnStates.P2_MOVE):
                if (P2Team[0].GetComponent<HeroUnit>().myType == HeroUnit.BuffType.EveryTurn && !debuffed)
                {
                    HexagonCell myCell = hexGrid.GetCell(P2Team[0].transform.position);
                    P2Team[0].GetComponent<HeroUnit>().DebufTeam("P2", myCell);
                    debuffed = true;

                }
                if (MoveableUnits.Count == 0)
                {
                    if (P2Team[0].GetComponent<HeroUnit>().myType == HeroUnit.BuffType.EveryTurn)
                    {
                        HexagonCell myCell = hexGrid.GetCell(P2Team[0].transform.position);
                        P2Team[0].GetComponent<HeroUnit>().BuffTeam("P2", myCell);
                    }
                    currentState = TurnStates.P2_ATTACK;
                    debuffed = false;
                    allow_cursor_control = false;
                    MoveableUnits = new List<StartUnit>(P1Team);
                    for (int i = 0; i < MoveableUnits.Count; i++)
                    {
                        Anima2D.SpriteMeshInstance[] Unit_Sprites = MoveableUnits[i].gameObject.GetComponentsInChildren<Anima2D.SpriteMeshInstance>();
                        for (int k = 0; k < Unit_Sprites.Length; k++)
                        {
                            Unit_Sprites[k].color = Color.white;
                        }
                    }

                }

                //UI.turn.text = "TURN:PLAYER 2";


                
                if (PlayerInfo.one_player)
                    MovePhase(PlayerInfo.player1);
                else
                    MovePhase(PlayerInfo.player2);
                //MovePhase();

                break;
            case (TurnStates.P2_ATTACK):
                if (!attacking)
                {

                    //sets all units on team to currently attacking
                    if (!cur_attacking)
                    {
                        Units_To_Delete.Clear();
                        foreach (StartUnit unit in P2Team)
                        {
                            unit.currently_attacking = true;
                        }
                        cur_attacking = true;
                        //get the ball rolling with attack_count
                        
                        StartCoroutine(P2Team[attack_count].BasicAttack(hexGrid, hexGrid.GetCell(P2Team[attack_count].transform.position)));
                    }

                    //calls attack on each next unit after previous has attacked
                    if (attack_count < P2Team.Count - 1)
                    {
                        if (P2Team[attack_count].currently_attacking == false)
                        {
                            
                            //next unit attacks because prev has finished
                            attack_count += 1;
                            
                            StartCoroutine(P2Team[attack_count].BasicAttack(hexGrid, hexGrid.GetCell(P2Team[attack_count].transform.position)));
                        }
                        else
                        {
                            //unit is still attacking so do nothing I guess
                        }
                    }

                    if (P2Team[P2Team.Count - 1] == null || P2Team[P2Team.Count - 1].currently_attacking == false) // where the index out of range is being thrown
                    {
                        
                        //the final unit has finished attacking or there were no units to attack in the first place
                        attacking = true;
                    }
                }
                else
                {
                    foreach (HexagonCell units_cell in Units_To_Delete)
                    {
                        int index = units_cell.coords.X_coord + units_cell.coords.Z_coord * hexGrid.width + units_cell.coords.Z_coord / 2;
                        RemoveUnitInfo(units_cell, index);
                    }

                    cur_attacking = false;
                    attack_count = 0;
                    attacking = false;
                    currentState = TurnStates.P2_STATUS_EFFECT;
                    allow_cursor_control = true;
                    if (MoveableUnits.Count > 0)
                    {
                        Snap_To_First_Unit();
                        p1_unit_rotation_value = 0;
                    }
                    P1_Team_portrait_UI.Update_Portraits();
                    P2_Team_portrait_UI.Update_Portraits();
                    foreach (StartUnit _unit in P1Team)
                    {
                        if (_unit != null)
                        {
                            Assign_Stats_Var(_unit.gameObject.GetComponentInChildren<BattleUI>(), _unit);
                        }
                    }
                    foreach (StartUnit _unit in P2Team)
                    {
                        if (_unit != null)
                        {
                            Assign_Stats_Var(_unit.gameObject.GetComponentInChildren<BattleUI>(), _unit);
                        }
                    }
                }
                break;

            case (TurnStates.P2_STATUS_EFFECT):
                if (statusJustStarted)
                {
                    statusJustStarted = false;
                    Units_To_Delete.Clear();
                }
                if (!statusExecuting) // status effect exectution 
                {
                    statusExecuting = true;
                    if (statusCount == P2StatusOnGrid.Count) // when status effects are done
                    {
                        for (int i = 0; i < P2StatusOnGrid.Count; i++)
                        {
                            if (P2StatusOnGrid[i].timeLeft <= 0)
                            {
                                EnvironmentalHazard.HazardInfo h = P2StatusOnGrid[i];
                                h.type.RemoveHazard(hexGrid, h.x, h.z, h.size, h.placedWeatherVane);
                                P2StatusOnGrid.Remove(P2StatusOnGrid[i]);
                            }
                        }

                        foreach (HexagonCell units_cell in Units_To_Delete)
                        {
                            int index = units_cell.coords.X_coord + units_cell.coords.Z_coord * hexGrid.width + units_cell.coords.Z_coord / 2;
                            RemoveUnitInfo(units_cell, index);
                        }

                        statusFinished = true;
                    }
                    if (statusCount < P2StatusOnGrid.Count) //for every status
                    {
                        EnvironmentalHazard.HazardInfo h = P2StatusOnGrid[statusCount];

                        P2StatusOnGrid[statusCount] = new EnvironmentalHazard.HazardInfo(h.type, h.x, h.y, h.z, h.timeLeft - 1, h.size);
                        //Debug.Log("hazard time left: " + h.timeLeft--.ToString());
                        //Debug.Log("x: " + h.x + " z: " + h.z);
                        StartCoroutine(Snap_To_Hazard(h.x, h.z, h.type.anim_time));
                        StartCoroutine(HandleStatus(statusCount, 1));

                    }
                }
                if (statusFinished) // when hazarrds are done
                {
                    //BattleUI_Turn.turn.text = "PLAYER 1";
                    //BattleUI_Turn.turn_info_Image.GetComponent<Image>().color = P1_Color;

                    statusJustStarted = true;

                    //wasP1Turn = false; //commenting this out cause it was breaking the game i think
                    statusExecuting = false;
                    statusFinished = false;
                    statusCount = 0;
                    allow_cursor_control = true;
                    //Snap_To_First_Unit();
                    currentState = TurnStates.CHECK;
                }
                break;
            case (TurnStates.CHECK):
                if (P1Team.Count == 0 || P1Team[0].GetComponent<HeroUnit>() == null)
                    currentState = TurnStates.P2_WIN;
                else if (P2Team.Count == 0 || P2Team[0].GetComponent<HeroUnit>() == null)

                    currentState = TurnStates.P1_WIN;
                else if(wasP1Turn)
                {
                    wasP1Turn = false;
                    Snap_To_First_Unit();
                    currentState = TurnStates.P2_MOVE;
                }
                else
                {
                    currentState = TurnStates.ENVIRONMENT;
                }
                break;
            case (TurnStates.P1_WIN):
                Debug.Log("PLAYER 1 WINS");
                allow_cursor_control = false;
                winText.text = "Player 1 Wins";
                winText.color = Color.blue;
                currentState = TurnStates.END;
                break;
            case (TurnStates.P2_WIN):
                Debug.Log("PLAYER 2 WINS");
                allow_cursor_control = false;
                winText.text = "Player 2 Wins";
                winText.color = Color.red;
                currentState = TurnStates.END;
                break;
            case (TurnStates.END):
                WinCanvas.SetActive(true);
                GameObject.Find("EventSystem").GetComponent<EventSystem>().SetSelectedGameObject(FirstObject);
                //SceneManager.LoadScene("VictoryScene");
                break;
        }
    }

    public void FindTeam(string team_name) // places teams in the correct list for later use
    {
        GameObject[] l = GameObject.FindGameObjectsWithTag(team_name);
        foreach (GameObject g in l)
        {
            if (team_name == "Player 1")
                P1Team.Add(g.GetComponent<StartUnit>());
            else
                P2Team.Add(g.GetComponent<StartUnit>());
        }
    }

    void InitialPhase(List<StartUnit> team, int player) // creates random units on the grid it sometimes repeats the units on tiles but not important cause will change later
    {
        //create unit names here need to put in proper file

        //application.datapath returns a different place in build vs in editor
        //place text in root directory where executable is located when creating the actual build for this to work as is
        //string path_proper = Application.dataPath + "/proper.txt";
        //Debug.Log(path_proper);
        //Debug.Log(Application.dataPath);
        //string path_adjectives = Application.dataPath + "/adjectives.txt";
        //Debug.Log(path_adjectives);

        //TextAsset names_proper_ass = Resources.Load<TextAsset>("proper");
        //string[] names_proper = names_proper_ass.text.Split(new char[] { '\n' });

        //string[] names_proper = System.IO.File.ReadAllLines(path_proper);
        //string[] names_adj = System.IO.File.ReadAllLines(path_adjectives);

        //TextAsset names_adj_ass = Resources.Load<TextAsset>("adjectives");
        //string[] names_adj = names_adj_ass.text.Split(new char[] { '\n' });

        Get_Alliterated_Name name_generator = new Get_Alliterated_Name();

        //Random rand_gen = new Random();
        initializing = false;
        //if (player == 1)
        //{
          for (int i = 0; i < team.Count; i++)
          {
            //int rand_index_p = Random.Range(0, names_proper.Length - 1);
            //int rand_index_a = Random.Range(0, names_adj.Length - 1);
            //string rand_proper = names_proper[rand_index_p];
            //string rand_adj = names_adj[rand_index_a];

            //string rand_proper = "Steve";
            //string rand_adj = "Big";
            

            if(player == 1)
                CreateUnit(P1start[i], team[i], name_generator.Get_Name());
            if (player == 2)
                CreateUnit(P2start[i], team[i], name_generator.Get_Name());

          }
        //}
        //else
        //{
        //    int k = 0;
        //    for (int j = 0; j < team.Count; j++)
        //    {
        //        int rand_index_p = Random.Range(0, names_proper.Length - 1);
        //        int rand_index_a = Random.Range(0, names_adj.Length - 1);
        //        string rand_proper = names_proper[rand_index_p];
        //        string rand_adj = names_adj[rand_index_a];
        //        //string rand_proper = "Thomas";
        //        //string rand_adj = "Large";
        //        CreateUnit(P2starting_pos[j], team[j], rand_proper, rand_adj);
        //        k++;
        //    }
        //}
        //initializing = true;
    }

    public IEnumerator HandleHazards(int count)
    {

        EnvironmentalHazard.HazardInfo h_info = hazardsOnGrid[count];
        StartCoroutine(h_info.type.Effect(this, hexGrid, h_info.x, h_info.z, h_info.size));
        yield return new WaitForSeconds(h_info.type.anim_time);
        hazardsExecuting = false;
        hazardCount++;
        h_info.timeLeft -= 1;
        
        
        //hazardsFinished = true;
    }
    public IEnumerator HandleStatus(int count, int playerNum)
    {
        EnvironmentalHazard.HazardInfo h_info;
        if (playerNum == 1)
            h_info = P1StatusOnGrid[count];
        else
            h_info = P2StatusOnGrid[count];
        StartCoroutine(h_info.type.Effect(this, hexGrid, h_info.x, h_info.z, h_info.size));
        yield return new WaitForSeconds(h_info.type.anim_time);
        statusExecuting = false;
        statusCount++;
        h_info.timeLeft -= 1;
    }

    public void MovePhase(string joystick) // handles input from the player to correctly move the unit
    {
        if(allow_cursor_control == true)
        {

            //int index = hexGrid.Get_Index(cursor.coords);

            //if (unitCell != hexGrid.cells[index])
            //{
            //    Stack<HexagonCell> path = hexGrid.FindPath(unitCell, hexGrid.cells[index]);
            //    Debug.Log(path.Count);
            //}
            //BUG: Possible bug occuring here where you can't hit a or b on occassion;S


            //if (!EventSystem.current.IsPointerOverGameObject())
            //{
                if (Input.GetButtonDown(joystick + "A Button"))
                {
                    HandleInput();
                    Dynamic_Controls_list.update_current_controls();
                }
                    
                if (Input.GetButtonDown(joystick + "B Button"))
                {
                    select_sound.Play();
                    DeselectUnit();
                    Dynamic_Controls_list.update_current_controls();
                }
           //}


            if (Input.GetButtonDown(joystick + "R Bumper"))
            {
                Snap_To_Next_Unit(true);
                Dynamic_Controls_list.update_current_controls();
            }

            if (Input.GetButtonDown(joystick + "L Bumper"))
            {
                Snap_To_Next_Unit(false);
                Dynamic_Controls_list.update_current_controls();
            }
        }
    }

    public void AttackPhase(List<StartUnit> attackingTeam) // handles input from the player to correctly attack
    {
        
        foreach (StartUnit unit in attackingTeam)
        {
                    unitCell = hexGrid.GetCell(unit.transform.position);
                    isUnitSelected = true;
                    StartCoroutine(AttackUnit(unit));
        }
    }

    void HandleInput() // get the current cell and depending on what phase, move, or select a unit
    {
        HexagonCell currentCell = GetCellUnderCursor2D();
        int index = currentCell.coords.X_coord + currentCell.coords.Z_coord * hexGrid.width + currentCell.coords.Z_coord / 2;
        if (currentCell.occupied) // if clicked and there is a unit there
        {
            if(currentCell.unitOnTile == SelectedUnit)
            {
                if (MoveableUnits.Contains(SelectedUnit))
                {
                    //moving to same tile
                    StartCoroutine(MoveUnit(hexGrid.GetCell(SelectedUnit.transform.position), currentCell));
                    select_sound.Play();

                    
                }
            }
            else
            {
                if (MoveableUnits.Contains(currentCell.unitOnTile))
                {
                    SelectUnit(currentCell, index); // make the selected unit that unit
                    select_sound.Play(); //Play Unit Selected Sound
                }
                
            }
        }
        else if (!currentCell.occupied && isUnitSelected && !attacking) // a unit is already selected
        {
            //prompt user to see if they actually want to move
            Debug.Log("we're using show path here");
            StartCoroutine(MoveUnit(hexGrid.GetCell(SelectedUnit.transform.position), currentCell));//move that selected unit
            //Play Movement Selected Sound
            select_sound.Play();
        }

    }

    //void HandleAttack() // similar to HandleInput() but for the attack phase only
    //{
    //    HexagonCell currentCell = GetCellUnderCursor2D();
    //    int index = currentCell.coords.X_coord + currentCell.coords.Z_coord * hexGrid.width + currentCell.coords.Z_coord / 2;
    //    if (currentCell.occupied && attacking) // if attacking and cell is occupied
    //    {
    //        if (!whileAttacking)
    //            StartCoroutine(AttackUnit());
    //    }
    //}

    void SelectUnit(HexagonCell current, int index) // sets variables to the clicked position's unit
    {
        hexGrid.ClearPath();
        if (SelectedUnit != null)
        {
            //Debug.Log("unitCell is assigned");
            SelectedUnit.Hide_Arrow_Select();
        }
        
        SelectedUnit = current.unitOnTile;
        SelectedUnit.Show_Arrow_Select();
        //current.Show_Selected_Icon();
        //StartCoroutine(SelectedUnit.Blink(Color.grey, SelectedUnit, Time.time + 0.6f));
        unitCell = hexGrid.cells[index];
        isUnitSelected = true;

        //hexGrid.ShowPath(unitCell, SelectedUnit.mobility, hexGrid.touchedColor);
        //UI.name.text = SelectedUnit.name.ToString();
        //UI.stats.text = "HEALTH:" + (int)SelectedUnit.current_health + "\nATTACK:" + (int)SelectedUnit.current_attack;

        hexGrid.ShowPath(unitCell, SelectedUnit.current_mobility, SelectedUnit.attackRange, hexGrid.touchedColor, hexGrid.attackColor);

        if (SelectedUnit.CompareTag("Player 1"))
        {
            //Change stats and unit info on the UI when unit selected
            //BattleUI_P1.obj_name.text = "" + SelectedUnit.name.ToString();
            Assign_BUI_Var(BattleUI_P1);
            BattleUI_P1.Show();
            BattleUI_P2.Hide();
            if (SelectedUnit.GetComponent<HeroUnit>() != null)
            {
                if(SelectedUnit.GetComponent<HeroUnit>().myType == HeroUnit.BuffType.BasicAttack)
                {
                    P1_Cooldown.SetActive(true);
                    if (SelectedUnit.GetComponent<PoisonHero>() != null)
                    {
                        P1_Cooldown_Text.text = SelectedUnit.GetComponent<PoisonHero>().specialAttackCounter.ToString();
                    }
                    if (SelectedUnit.GetComponent<KidnapperHero>() != null)
                    {
                        P1_Cooldown_Text.text = SelectedUnit.GetComponent<KidnapperHero>().specialAttackCounter.ToString();
                    }
                    if (SelectedUnit.GetComponent<WeatherHero>() != null)
                    {
                        P1_Cooldown_Text.text = SelectedUnit.GetComponent<WeatherHero>().specialAttackCounter.ToString();
                    }
                }
                else
                {
                    P1_Cooldown.SetActive(false);
                }
            }
            else
            {
                P1_Cooldown.SetActive(false);
            }
            
        }

        if (SelectedUnit.CompareTag("Player 2"))
        {
            //Change stats and unit info on the UI when unit selected
            //BattleUI_P2.obj_name.text = "" + SelectedUnit.name.ToString();
            Assign_BUI_Var(BattleUI_P2);
            BattleUI_P2.Show();
            BattleUI_P1.Hide();
            if (SelectedUnit.GetComponent<HeroUnit>() != null)
            {
                if (SelectedUnit.GetComponent<HeroUnit>().myType == HeroUnit.BuffType.BasicAttack)
                {
                    P2_Cooldown.SetActive(true);
                    if(SelectedUnit.GetComponent<PoisonHero>() != null)
                    {
                        P2_Cooldown_Text.text = SelectedUnit.GetComponent<PoisonHero>().specialAttackCounter.ToString();
                    }
                    if(SelectedUnit.GetComponent<KidnapperHero>() != null)
                    {
                        P2_Cooldown_Text.text = SelectedUnit.GetComponent<KidnapperHero>().specialAttackCounter.ToString();
                    }
                    if (SelectedUnit.GetComponent<WeatherHero>() != null)
                    {
                        P2_Cooldown_Text.text = SelectedUnit.GetComponent<WeatherHero>().specialAttackCounter.ToString();
                    }
                }
                else
                {
                    P2_Cooldown.SetActive(false);
                }
            }
            else
            {
                P2_Cooldown.SetActive(false);
            }
        }
        //hexGrid.ShowPath(unitCell, SelectedUnit.mobility, SelectedUnit.attackRange, hexGrid.touchedColor, hexGrid.attackColor);
        //UI.obj_name.text =  "UNIT:"+ SelectedUnit.name.ToString();
        //UI.stats.text = "HEALTH:" + SelectedUnit.current_health + "\nATTACK:" + SelectedUnit.current_attack;


    }

    private void DeselectUnit() // clears all variables to the clicked position
    {
        SelectedUnit.Hide_Arrow_Select();
        SelectedUnit = null;
        unitCell = null;
        isUnitSelected = false;
        hexGrid.ClearPath();
        BattleUI_P1.Hide();
        BattleUI_P2.Hide();
        //BattleUI_P1.obj_name.text = "UNIT:";
        //BattleUI_P1.stats.text = "HEALTH:\nATTACK:";
    }

    IEnumerator AttackUnit(StartUnit select_unit) // port this whole function to start unit class
    {
        whileAttacking = true;
        StartCoroutine(select_unit.BasicAttack(hexGrid, unitCell));

        yield return new WaitForSeconds(0.5f);
        whileAttacking = false;

    }

    public void RemoveUnitInfo(HexagonCell current, int index)  // when a unit dies use this function to remove it from the grid
    {
        if (current.unitOnTile.tag == "Player 1")
            P1Team.Remove(current.unitOnTile);
        else
            P2Team.Remove(current.unitOnTile);
        if (MoveableUnits.Contains(current.unitOnTile))
            MoveableUnits.Remove(current.unitOnTile);
        current.unitOnTile.removed = true;
        current.occupied = false;
        current.unitOnTile = null;
        
    }

    HexagonCell GetCellUnderCursor2D() // findn the cell under the cursor thats 2D
    {
        RaycastHit2D hit = Physics2D.Raycast(cursor.point.transform.position, Vector2.zero, 0f);
        if (hit)
        {
            //Debug.Log(hit.transform.gameObject.GetComponent<HexagonCell>()); // debug stuff
            return hit.transform.gameObject.GetComponent<HexagonCell>();
        }
        else return null;
    }

    void CreateUnit(int index, StartUnit unit, string name)
    {   
        //if(unit == null)
        //{
        //    return;
        //}
        SelectedUnit = Instantiate(unit);
        isUnitSelected = true;
        unitCell = hexGrid.cells[index];
        SelectedUnit.transform.position = hexGrid.cells[index].transform.position;
        unitCell.occupied = true;
        unitCell.unitOnTile = SelectedUnit;
        SelectedUnit.unit_name = name;
        SelectedUnit.Unit_Stats_Panel.GetComponent<BattleUI>().Hide();
        Anima2D.SpriteMeshInstance[] Unit_Meshes = SelectedUnit.gameObject.GetComponentsInChildren<Anima2D.SpriteMeshInstance>();
        for (int i = 0; i < Unit_Meshes.Length; i++)
        {
            //puts each unit in a section of the sorting layer according to the tile they are on.
            Unit_Meshes[i].sortingOrder = Unit_Meshes[i].GetComponent<Mesh_Layer>()._ordered_layer
                + ((hexGrid.cells[index].coords.X_coord + hexGrid.cells[index].coords.Y_coord) * max_sprites_per_unit);
            //Debug.Log("Color_Changed");
        }
        SpriteRenderer[] sprites = this.gameObject.GetComponentsInChildren<SpriteRenderer>();
        foreach(SpriteRenderer sprite_rend in sprites)
        {
            sprite_rend.sortingOrder = sprite_rend.GetComponent<Mesh_Layer>()._ordered_layer
                + ((hexGrid.cells[index].coords.X_coord + hexGrid.cells[index].coords.Y_coord) * max_sprites_per_unit);
        }
    }

    IEnumerator MoveUnit(HexagonCell _unitCell, HexagonCell _nextCell)
    {
        allow_cursor_control = false;

        if(hexGrid.GetCell(SelectedUnit.transform.position) == hexGrid.GetCell(cursor.transform.position))
        {
            //moving to same tile...
            
            Confirm_Window.GetComponent<Confirm_Window>().Activate_Conf_Win();
            while (move_confirmed == false)
            {
                yield return null; //the big wait for user input on conf window
            }
            move_confirmed = false;

            if (move_string.Equals("yes"))
            {
                //basically just staying in the same spot
                MoveableUnits.Remove(SelectedUnit);
                Anima2D.SpriteMeshInstance[] Unit_Meshes = SelectedUnit.gameObject.GetComponentsInChildren<Anima2D.SpriteMeshInstance>();
                for (int i = 0; i < Unit_Meshes.Length; i++)
                {
                    Unit_Meshes[i].color = Greyed_Unit_Color;
                    //Debug.Log("Color_Changed");
                }
                DeselectUnit();
                if (MoveableUnits.Count > 0)
                {
                    Snap_To_Next_Unit(true);
                    //Debug.Log("Snapped");
                }
            }
            else if (move_string.Equals("no"))
            {
                Debug.Log("---------- Thats A No On The Move Jimbo ----------");
            }
            allow_cursor_control = true;
            yield break;
        }
        //int distance = hexGrid.GetCell(SelectedUnit.transform.position).coords.FindDistanceTo(cursor.coords);
        int index = hexGrid.Get_Index(cursor.coords);
        int distance = hexGrid.FindPath(_unitCell, hexGrid.cells[index]).Count;
        unitCell = hexGrid.GetCell(SelectedUnit.transform.position);
        //int distance = unitCell.coords.FindDistanceTo(hexGrid.cells[index].coords);
        //Debug.Log("Distance From: " + unitCell.coords.ToString() + " To: " +
        //hexGrid.cells[index].coords.ToString() +
        //" = " + distance.ToString()); //for debugging distance
        if (SelectedUnit.current_mobility >= distance && MoveableUnits.Contains(SelectedUnit))
        {
            //ask for confirmation
            //move_confirmed = true;
            //while (move_confirmed)
            //{
            //    //wait for move to confirm I guess??
            //}
            Confirm_Window.GetComponent<Confirm_Window>().Activate_Conf_Win();
            while(move_confirmed == false)
            {
                yield return null; //the big wait for user input on conf window
            }
            move_confirmed = false;

            if (move_string.Equals("yes"))
            {
                StartCoroutine(SelectedUnit.HopToPlace(hexGrid, unitCell, index, distance));
                //StartCoroutine(SelectedUnit.Moving());
                yield return new WaitForSeconds(((float)distance * 0.6f) + 0.1f);
                _unitCell.occupied = false;
                _unitCell.unitOnTile = null;
                //SelectedUnit.transform.position = hexGrid.cells[index].transform.position;
                _unitCell = hexGrid.cells[index];
                hexGrid.cells[index].occupied = true;
                hexGrid.cells[index].unitOnTile = SelectedUnit;

                if (hexGrid.cells[index].tag == "TeamBuff" && hexGrid.cells[index].occupied)
                {
                    // hexGrid.cells[index].occupied = true;
                    // hexGrid.cells[index].unitOnTile = SelectedUnit;
                    hexGrid.cells[index].GetComponent<TeamPowerupTiles>().discovered = true;
                    //if (hexGrid.cells[index].unitOnTile.tag == "Player 1")
                    //    hexGrid.cells[index].GetComponent<TeamPowerupTiles>().UnitsTeam = P1Team;
                    //if (hexGrid.cells[index].unitOnTile.tag == "Player 2")
                    //    hexGrid.cells[index].GetComponent<TeamPowerupTiles>().UnitsTeam = P2Team;
                }

                MoveableUnits.Remove(SelectedUnit);
                Anima2D.SpriteMeshInstance[] Unit_Meshes = SelectedUnit.gameObject.GetComponentsInChildren<Anima2D.SpriteMeshInstance>();
                for (int i = 0; i < Unit_Meshes.Length; i++)
                {
                    Unit_Meshes[i].color = Greyed_Unit_Color;
                    //Debug.Log("Color_Changed");
                }
                DeselectUnit();
                if (MoveableUnits.Count > 0)
                {
                    Snap_To_Next_Unit(true);
                    //Debug.Log("Snapped");
                }
            }
            else if (move_string.Equals("no"))
            {
                Debug.Log("---------- Thats A No On The Move Jimbo ----------");
            }
            

        }
        else
        {
            Debug.LogError("CAN'T MOVE THATS TOO FAR FOR THE UNIT");
        }
        allow_cursor_control = true;
    }

    int mod(int x, int m)
    {
        return (x % m + m) % m;
    }

    public void Show_Units_In_Range()
    {
        //needs to be rewritten using neighbours to improve efficiency
        int range = SelectedUnit.attackRange;
        int width = hexGrid.width;
        int height = hexGrid.height;

        for (int i = 0; i < (width * height); i++)
        {
            if (hexGrid.cells[i].gameObject.tag != "Wall")
            {

                if (currentState == TurnStates.P1_MOVE && SelectedUnit.gameObject.CompareTag("Player 1"))
                {
                    if (hexGrid.cells[i].occupied)
                    {
                        //hexGrid.cells[i].spriteRenderer.color = color_w;
                        hexGrid.cells[i].Hide_Cross_Icon();
                        hexGrid.cells[i].Hide_Selected_Icon();
                    }

                    if (cursor.coords.FindDistanceTo(hexGrid.cells[i].coords) <= range && hexGrid.cells[i].occupied
                        && (hexGrid.cells[i].unitOnTile.gameObject.CompareTag("Player 2") || hexGrid.cells[i].unitOnTile.gameObject.CompareTag("TeamBuff")))
                    {
                        //hexGrid.cells[i].spriteRenderer.color = color_r;
                        //in range create blinking crosshair
                        if(SelectedUnit.unit_type != "Healer")
                        {
                            hexGrid.cells[i].Show_Cross_Icon();
                        }
                    }
                    else if (cursor.coords.FindDistanceTo(hexGrid.cells[i].coords) <= range && hexGrid.cells[i].occupied && hexGrid.cells[i].unitOnTile.gameObject.CompareTag("Player 1")
                        && hexGrid.cells[i].unitOnTile.current_health < hexGrid.cells[i].unitOnTile.health && hexGrid.cells[i].unitOnTile != SelectedUnit)
                    {
                        if (SelectedUnit.unit_type == "Healer")
                        {
                            //Show healable ICON
                            hexGrid.cells[i].Show_Selected_Icon();
                        }
                    }
                    
                    
                }

                //Should make editor change a text value so that we dont have to do a text comapare every time
                else if (currentState == TurnStates.P2_MOVE && SelectedUnit.gameObject.CompareTag("Player 2"))
                {
                    if (hexGrid.cells[i].occupied)
                    {
                        //hexGrid.cells[i].spriteRenderer.color = color_w;
                        hexGrid.cells[i].Hide_Cross_Icon();
                        hexGrid.cells[i].Hide_Selected_Icon();
                    }

                    if (cursor.coords.FindDistanceTo(hexGrid.cells[i].coords) <= range && hexGrid.cells[i].occupied
                        && (hexGrid.cells[i].unitOnTile.gameObject.CompareTag("Player 1") || hexGrid.cells[i].unitOnTile.gameObject.CompareTag("TeamBuff")))
                    {
                        //hexGrid.cells[i].spriteRenderer.color = color_r;
                        //in range
                        if (SelectedUnit.unit_type != "Healer")
                        {
                            hexGrid.cells[i].Show_Cross_Icon();
                        }
                    }
                    else if (cursor.coords.FindDistanceTo(hexGrid.cells[i].coords) <= range && hexGrid.cells[i].occupied && hexGrid.cells[i].unitOnTile.gameObject.CompareTag("Player 2")
                        && hexGrid.cells[i].unitOnTile.current_health < hexGrid.cells[i].unitOnTile.health && hexGrid.cells[i].unitOnTile != SelectedUnit)
                    {
                        if (SelectedUnit.unit_type == "Healer")
                        {
                            //Show healable ICON
                            hexGrid.cells[i].Show_Selected_Icon();
                        }
                    }
                    
                    
                }

            }

        }
    }

    //fill this out
    public bool Is_Tile_In_Move_Range()
    {
        int move_range = SelectedUnit.current_mobility;
        HexagonCell Selected_Unit_Cell = hexGrid.GetCell(SelectedUnit.transform.position);
        if (Selected_Unit_Cell.coords.FindDistanceTo(cursor.coords) <= move_range)
        {
            return true;
        }
        return false;
    }

    

    public void Assign_BUI_Var(BattleUI _UI)
    {
        _UI.obj_name.text = SelectedUnit.unit_name;
        _UI.obj_stats.text = ((int)Mathf.CeilToInt(SelectedUnit.current_health)).ToString()+ "/" + SelectedUnit.health.ToString();
        _UI.unit_icon.GetComponent<Image>().sprite = SelectedUnit.Icon;
        _UI.health_Bar.GetComponent<Image>().fillAmount = SelectedUnit.current_health / SelectedUnit.health;
        Show_Current_Buffs(_UI);
    }

    public void Show_Current_Buffs(BattleUI _UI)
    {
        bool no_buffs = true;

        if(SelectedUnit.attack_buff == true)
        {
            _UI.attack_buff.SetActive(true);
            no_buffs = false;
        }
        else
        {
            _UI.attack_buff.SetActive(false);
        }

        if (SelectedUnit.health_buff == true)
        {
            _UI.health_buff.SetActive(true);
            no_buffs = false;
        }
        else
        {
            _UI.health_buff.SetActive(false);
        }

        if(SelectedUnit.move_buff == true)
        {
            _UI.mobility_buff.SetActive(true);
            no_buffs = false;
        }
        else
        {
            _UI.mobility_buff.SetActive(false);
        }

        if(SelectedUnit.crit_buff == true)
        {
            _UI.critical_buff.SetActive(true);
            no_buffs = false;
        }
        else
        {
            _UI.critical_buff.SetActive(false);
        }

        if (no_buffs)
        {
            _UI.buff_background.SetActive(false);
        }
        else
        {
            _UI.buff_background.SetActive(true);
        }
    }

    public void Assign_Stats_Var(BattleUI _UI, StartUnit _unit)
    {
        _UI.obj_name.text = _unit.unit_name;
        _UI.obj_type.text = _unit.unit_type;
        _UI.stats_atk.text = "ATK: " + (int)_unit.current_attack;
        _UI.stats_mov.text = "MOV: " + _unit.current_mobility;
        _UI.stats_crit.text = "CRIT: " + (int)_unit.crit + "%";
        _UI.stats_range.text = "RNG: " + _unit.attackRange;
    }

    public void Assign_BUI_Var(BattleUI _UI, StartUnit _unit)
    {
        _UI.obj_name.text = _unit.unit_name;
        _UI.unit_icon.GetComponent<Image>().sprite = _unit.Icon;
        _UI.health_Bar.GetComponent<Image>().fillAmount = _unit.current_health / _unit.health;
    }


    public void re_sort_unit_position(StartUnit _unit, HexagonCell _target_location)
    {
        Anima2D.SpriteMeshInstance[] Unit_Meshes = _unit.gameObject.GetComponentsInChildren<Anima2D.SpriteMeshInstance>();
        for (int i = 0; i < Unit_Meshes.Length; i++)
        {
            //puts each unit in a section of the sorting layer according to the tile they are on.
            Unit_Meshes[i].sortingOrder = Unit_Meshes[i].GetComponent<Mesh_Layer>()._ordered_layer
                + ((_target_location.coords.X_coord + _target_location.coords.Y_coord) * max_sprites_per_unit);
            //Debug.Log("Color_Changed");
        }
        SpriteRenderer[] sprites = _unit.gameObject.GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer sprite_rend in sprites)
        {
            sprite_rend.sortingOrder = sprite_rend.GetComponent<Mesh_Layer>()._ordered_layer
                + ((_target_location.coords.X_coord + _target_location.coords.Y_coord) * max_sprites_per_unit);
        }
    }

    public void printState()
    {
        string state_string = "";
        state_string += "Current State: \n";
        state_string += currentState;
        state_string += "\n P1 Team: \n";
        foreach(StartUnit _unit in P1Team){
            state_string += _unit.unit_name + " | ";
        }
        state_string += "\n P2 Team: \n";
        foreach (StartUnit _unit in P2Team)
        {
            state_string += _unit.unit_name + " | ";
        }
        state_string += "\n Units To Delete: \n";
        foreach (HexagonCell _unit_cell in Units_To_Delete)
        {
            state_string += _unit_cell.unitOnTile.unit_name + " | ";
        }
    }

    IEnumerator turn_animation_starter()
    {
        turn_change_sound.Play();
        BattleUI_Turn.turn_info_Image.GetComponent<Animator>().SetBool("Transition", true);
        yield return new WaitForSeconds(1f);
        BattleUI_Turn.turn_info_Image.GetComponent<Animator>().SetBool("Transition", false);
    }

    public void Snap_To_First_Unit()
    {
        StartUnit unit_to_select;
        HexagonCell currentCell = hexGrid.Get_Cell_Index(cursor.coords);
        int unit_to_select_index = 0;
        int index = 0;

        unit_to_select = MoveableUnits[unit_to_select_index];
        //get coordinates of the tile under the selected unit and give them to the cursor
        //cursor.coords = HexagonCoord.FromPosition(unit_to_select.transform.position);
        //move the cursor to the position of the newly selected tile/unit
        //cursor.transform.position = hexGrid.Get_Cell_Index(cursor.coords).gameObject.transform.position;
        HexagonCoord _new_coord = HexagonCoord.FromPosition(unit_to_select.transform.position);
        cursor.Assign_Position(hexGrid.Get_Cell_Index(_new_coord).gameObject.transform.position, _new_coord);
        currentCell = hexGrid.Get_Cell_Index(cursor.coords);
        index = cursor.coords.X_coord + cursor.coords.Z_coord * hexGrid.width + cursor.coords.Z_coord / 2;
        if (currentCell.occupied) // if clicked and there is a unit there
        {
            SelectUnit(currentCell, index); // make the selected unit that unit
            Show_Units_In_Range();
            SelectedUnit.Unit_Stats_Panel.SetActive(true);
        }
    }

    public IEnumerator Snap_To_Hazard(int x, int z, float anim_time)
    {
        HexagonCoord newCoord = new HexagonCoord(x, z);
        cursor.Assign_Position(hexGrid.Get_Cell_Index(newCoord).gameObject.transform.position, newCoord);
        yield return new WaitForSeconds(anim_time);

    }

    public void Snap_To_Next_Unit(bool back_forward)
    {
        //add a parameter to determine whether or not to increment unit_to_select_index
        //add a parameter to determine whether or not you want to go to a specific unit index
        int unit_to_select_index = 0;
        StartUnit unit_to_select;
        HexagonCell currentCell = hexGrid.Get_Cell_Index(cursor.coords);
        int index = 0;
        HexagonCoord _new_coord;
        //play swap to next unit sound
        cycle_unit_sound.Play();

        if (back_forward)
        {
            switch (currentState)
            {
                case (TurnStates.P1_MOVE):
                    unit_to_select_index = mod(p1_unit_rotation_value, MoveableUnits.Count);
                    p1_unit_rotation_value = unit_to_select_index;
                    p1_unit_rotation_value++;
                    //select indexed unit from moveable units
                    unit_to_select = MoveableUnits[unit_to_select_index];
                    //get coordinates of the tile under the selected unit and give them to the cursor
                    //cursor.coords = HexagonCoord.FromPosition(unit_to_select.transform.position);
                    //move the cursor to the position of the newly selected tile/unit
                    //cursor.transform.position = hexGrid.Get_Cell_Index(cursor.coords).gameObject.transform.position;
                    _new_coord = HexagonCoord.FromPosition(unit_to_select.transform.position);
                    cursor.Assign_Position(hexGrid.Get_Cell_Index(_new_coord).gameObject.transform.position, _new_coord);
                    currentCell = hexGrid.Get_Cell_Index(cursor.coords);
                    index = cursor.coords.X_coord + cursor.coords.Z_coord * hexGrid.width + cursor.coords.Z_coord / 2;
                    if (currentCell.occupied) // if clicked and there is a unit there
                    {
                        SelectUnit(currentCell, index); // make the selected unit that unit
                        Show_Units_In_Range();
                        SelectedUnit.Unit_Stats_Panel.SetActive(true);
                    }
                    break;
                case (TurnStates.P2_MOVE):
                    unit_to_select_index = mod(p2_unit_rotation_value, MoveableUnits.Count);
                    p2_unit_rotation_value = unit_to_select_index;
                    p2_unit_rotation_value++;
                    //select indexed unit from moveable units
                    unit_to_select = MoveableUnits[unit_to_select_index];
                    //get coordinates of the tile under the selected unit and give them to the cursor
                    //cursor.coords = HexagonCoord.FromPosition(unit_to_select.transform.position);
                    //move the cursor to the position of the newly selected tile/unit
                    //cursor.transform.position = hexGrid.Get_Cell_Index(cursor.coords).gameObject.transform.position;
                    _new_coord = HexagonCoord.FromPosition(unit_to_select.transform.position);
                    cursor.Assign_Position(hexGrid.Get_Cell_Index(_new_coord).gameObject.transform.position, _new_coord);
                    currentCell = hexGrid.Get_Cell_Index(cursor.coords);
                    index = cursor.coords.X_coord + cursor.coords.Z_coord * hexGrid.width + cursor.coords.Z_coord / 2;
                    if (currentCell.occupied) // if clicked and there is a unit there
                    {
                        SelectUnit(currentCell, index); // make the selected unit that unit
                        Show_Units_In_Range();
                        SelectedUnit.Unit_Stats_Panel.SetActive(true);
                    }
                    break;
            }
        }
        else
        {
            switch (currentState)
            {
                case (TurnStates.P1_MOVE):
                    unit_to_select_index = mod(p1_unit_rotation_value, MoveableUnits.Count);
                    p1_unit_rotation_value = unit_to_select_index;
                    p1_unit_rotation_value--;
                    //select indexed unit from moveable units
                    unit_to_select = MoveableUnits[unit_to_select_index];
                    //get coordinates of the tile under the selected unit and give them to the cursor
                    //cursor.coords = HexagonCoord.FromPosition(unit_to_select.transform.position);
                    //move the cursor to the position of the newly selected tile/unit
                    //cursor.transform.position = hexGrid.Get_Cell_Index(cursor.coords).gameObject.transform.position;
                    _new_coord = HexagonCoord.FromPosition(unit_to_select.transform.position);
                    cursor.Assign_Position(hexGrid.Get_Cell_Index(_new_coord).gameObject.transform.position, _new_coord);
                    currentCell = hexGrid.Get_Cell_Index(cursor.coords);
                    index = cursor.coords.X_coord + cursor.coords.Z_coord * hexGrid.width + cursor.coords.Z_coord / 2;
                    if (currentCell.occupied) // if clicked and there is a unit there
                    {
                        SelectUnit(currentCell, index); // make the selected unit that unit
                        Show_Units_In_Range();
                        SelectedUnit.Unit_Stats_Panel.SetActive(true);
                    }
                    break;
                case (TurnStates.P2_MOVE):
                    unit_to_select_index = mod(p2_unit_rotation_value, MoveableUnits.Count);
                    p2_unit_rotation_value = unit_to_select_index;
                    p2_unit_rotation_value--;
                    //select indexed unit from moveable units
                    unit_to_select = MoveableUnits[unit_to_select_index];
                    //get coordinates of the tile under the selected unit and give them to the cursor
                    //cursor.coords = HexagonCoord.FromPosition(unit_to_select.transform.position);
                    //move the cursor to the position of the newly selected tile/unit
                    //cursor.transform.position = hexGrid.Get_Cell_Index(cursor.coords).gameObject.transform.position;
                    _new_coord = HexagonCoord.FromPosition(unit_to_select.transform.position);
                    cursor.Assign_Position(hexGrid.Get_Cell_Index(_new_coord).gameObject.transform.position, _new_coord);
                    currentCell = hexGrid.Get_Cell_Index(cursor.coords);
                    index = cursor.coords.X_coord + cursor.coords.Z_coord * hexGrid.width + cursor.coords.Z_coord / 2;
                    if (currentCell.occupied) // if clicked and there is a unit there
                    {
                        SelectUnit(currentCell, index); // make the selected unit that unit
                        Show_Units_In_Range();
                        SelectedUnit.Unit_Stats_Panel.SetActive(true);
                    }
                    break;
            }
        }

    }
}
