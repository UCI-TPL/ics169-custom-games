using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HexagonMapEditor : MonoBehaviour
{

    public PlayerInformation PlayerInfo;
    public Grid hexGrid;
    public GameObject UI_P1_Sel;
    public GameObject UI_P2_Sel;
    public GameObject UI_P1_Hov;
    public GameObject UI_P2_Hov;
    public GameObject UI_Turn;
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


    public List<StartUnit> Player1Chosen = new List<StartUnit>();
    public List<StartUnit> Player2Chosen = new List<StartUnit>();

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


    public enum TurnStates
    {
        START,
        P1_MOVE,
        P1_ATTACK,
        P2_MOVE,
        P2_ATTACK,
        CHECK,
        P1_WIN,
        P2_WIN,
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

        //UI = GetComponentInChildren<BattleUI>();
        P1_Team_portrait_UI = P1_portrait_UI.GetComponent<Team_Portrait_UI>();
        P2_Team_portrait_UI = P2_portrait_UI.GetComponent<Team_Portrait_UI>();

        BattleUI_P1 = UI_P1_Sel.GetComponent<BattleUI>();
        BattleUI_P2 = UI_P2_Sel.GetComponent<BattleUI>();

        BattleUI_P1.Hide();
        BattleUI_P2.Hide();

        BattleUI_Turn = UI_Turn.GetComponent<BattleUI>();

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
            P1Team[0].GetComponent<HeroUnit>().BuffTeam("P1");
            //P2Team[0].GetComponent<HeroUnit>().BuffTeam("P2");
        }
        MoveableUnits = new List<StartUnit>(P1Team); // put player 1's team in since they're going first
        currentState = TurnStates.P1_MOVE;
        if (MoveableUnits.Count > 0)
        {
            Snap_To_First_Unit();
            p1_unit_rotation_value = 0;
        }
        //StartCoroutine(InitializingTeams());
        P1_Team_portrait_UI.Initialize_Portraits(P1Team);
        P2_Team_portrait_UI.Initialize_Portraits(P2Team);
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(currentState);
        switch (currentState)
        {
            case (TurnStates.START):

                //currentState = TurnStates.P1_MOVE;  // go to next phase
                break;
            case (TurnStates.P1_MOVE):
                if (MoveableUnits.Count == 0) // once all units move break
                {
                    currentState = TurnStates.P1_ATTACK;
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

                BattleUI_Turn.turn.text = "PLAYER 1";
                BattleUI_Turn.turn_info_Image.GetComponent<Image>().color = P1_Color;
                MovePhase(PlayerInfo.player1);
                //MovePhase();

                break;
            case (TurnStates.P1_ATTACK):
                if (!attacking) // only call once
                {
                    attacking = true;
                    AttackPhase(P1Team); // handles all attacking for player 1
                    attacking = false;
                    currentState = TurnStates.P2_MOVE;
                    if (MoveableUnits.Count > 0)
                    {
                        Snap_To_First_Unit();
                        p2_unit_rotation_value = 0;
                    }
                    P2_Team_portrait_UI.Update_Portraits();
                }
                break;
            case (TurnStates.P2_MOVE):
                if (MoveableUnits.Count == 0)
                {
                    currentState = TurnStates.P2_ATTACK;
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


                BattleUI_Turn.turn.text = "PLAYER 2";
                BattleUI_Turn.turn_info_Image.GetComponent<Image>().color = P2_Color;
                if (PlayerInfo.one_player)
                    MovePhase(PlayerInfo.player1);
                else
                    MovePhase(PlayerInfo.player2);
                //MovePhase();

                break;
            case (TurnStates.P2_ATTACK):
                if (!attacking)
                {
                    attacking = true;
                    AttackPhase(P2Team);
                    attacking = false;
                    currentState = TurnStates.CHECK;
                    if (MoveableUnits.Count > 0)
                    {
                        Snap_To_First_Unit();
                        p1_unit_rotation_value = 0;
                    }
                    P1_Team_portrait_UI.Update_Portraits();
                }

                break;
            case (TurnStates.CHECK):
                if (P1Team.Count == 0)
                    currentState = TurnStates.P2_WIN;
                else if (P2Team.Count == 0)
                    currentState = TurnStates.P1_WIN;
                else
                {
                    currentState = TurnStates.P1_MOVE;
                }
                break;
            case (TurnStates.P1_WIN):
                Debug.Log("PLAYER 1 WINS");
                currentState = TurnStates.END;
                break;
            case (TurnStates.P2_WIN):
                Debug.Log("PLAYER 2 WINS");
                currentState = TurnStates.END;
                break;
            case (TurnStates.END):
                SceneManager.LoadScene("VictoryScene");
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
        string path_proper = Application.dataPath + "/proper.txt";
        Debug.Log(path_proper);
        Debug.Log(Application.dataPath);
        string path_adjectives = Application.dataPath + "/adjectives.txt";
        Debug.Log(path_adjectives);
        string[] names_proper = System.IO.File.ReadAllLines(path_proper);
        string[] names_adj = System.IO.File.ReadAllLines(path_adjectives);
        //Random rand_gen = new Random();
        initializing = false;
        if (player == 1)
        {
            for (int i = 0; i < team.Count; i++)
            {

                int rand_index_p = Random.Range(0, names_proper.Length - 1);
                int rand_index_a = Random.Range(0, names_adj.Length - 1);
                string rand_proper = names_proper[rand_index_p];
                string rand_adj = names_adj[rand_index_a];
                //string rand_proper = "Steve";
                //string rand_adj = "Big";
                CreateUnit(i, team[i], rand_proper, rand_adj);

            }
        }
        else
        {
            int k = 0;
            for (int j = hexGrid.cells.Length - 1; j > hexGrid.cells.Length - 1 - team.Count; j--)
            {
                int rand_index_p = Random.Range(0, names_proper.Length - 1);
                int rand_index_a = Random.Range(0, names_adj.Length - 1);
                string rand_proper = names_proper[rand_index_p];
                string rand_adj = names_adj[rand_index_a];
                //string rand_proper = "Thomas";
                //string rand_adj = "Large";
                CreateUnit(j, team[k], rand_proper, rand_adj);
                k++;
            }
        }
        //initializing = true;
    }

    public void MovePhase(string joystick) // handles input from the player to correctly move the unit
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (Input.GetButtonDown(joystick + "A Button"))
                HandleInput();
            if (Input.GetButtonDown(joystick + "B Button"))
            {
                DeselectUnit();
            }
        }

        if (Input.GetButtonDown(joystick + "X Button"))
        {
            if (MoveableUnits.Contains(SelectedUnit))
            {
                MoveableUnits.Remove(SelectedUnit);
                Anima2D.SpriteMeshInstance[] Unit_Meshes = SelectedUnit.gameObject.GetComponentsInChildren<Anima2D.SpriteMeshInstance>();
                for (int i = 0; i < Unit_Meshes.Length; i++)
                {
                    Unit_Meshes[i].color = Greyed_Unit_Color;
                    //Debug.Log("Color_Changed");
                }
            }
        }

        if (Input.GetButtonDown(joystick + "R Bumper"))
        {
            Snap_To_Next_Unit(true);
        }

        if (Input.GetButtonDown(joystick + "L Bumper"))
        {
            Snap_To_Next_Unit(false);
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
            SelectUnit(currentCell, index); // make the selected unit that unit
        }
        else if (!currentCell.occupied && isUnitSelected && !attacking) // a unit is already selected
        {
            StartCoroutine(MoveUnit(hexGrid.GetCell(SelectedUnit.transform.position), currentCell));//move that selected unit
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
        SelectedUnit = current.unitOnTile;
        unitCell = hexGrid.cells[index];
        isUnitSelected = true;

        //hexGrid.ShowPath(unitCell, SelectedUnit.mobility, hexGrid.touchedColor);
        //UI.name.text = SelectedUnit.name.ToString();
        //UI.stats.text = "HEALTH:" + (int)SelectedUnit.current_health + "\nATTACK:" + (int)SelectedUnit.current_attack;

        hexGrid.ShowPath(unitCell, SelectedUnit.mobility, SelectedUnit.attackRange, hexGrid.touchedColor, hexGrid.attackColor);

        if (SelectedUnit.CompareTag("Player 1"))
        {
            //Change stats and unit info on the UI when unit selected
            //BattleUI_P1.obj_name.text = "" + SelectedUnit.name.ToString();
            Assign_BUI_Var(BattleUI_P1);
            BattleUI_P1.Show();
            BattleUI_P2.Hide();

        }

        if (SelectedUnit.CompareTag("Player 2"))
        {
            //Change stats and unit info on the UI when unit selected
            //BattleUI_P2.obj_name.text = "" + SelectedUnit.name.ToString();
            Assign_BUI_Var(BattleUI_P2);
            BattleUI_P2.Show();
            BattleUI_P1.Hide();

        }
        //hexGrid.ShowPath(unitCell, SelectedUnit.mobility, SelectedUnit.attackRange, hexGrid.touchedColor, hexGrid.attackColor);
        //UI.obj_name.text =  "UNIT:"+ SelectedUnit.name.ToString();
        //UI.stats.text = "HEALTH:" + SelectedUnit.current_health + "\nATTACK:" + SelectedUnit.current_attack;


    }

    private void DeselectUnit() // clears all variables to the clicked position
    {
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
        // List<HexagonCell> targetable = new List<HexagonCell>();
        // foreach(HexagonCell cell in hexGrid.cells)
        // {
        //     if (unitCell.coords.FindDistanceTo(cell.coords) <= SelectedUnit.attackRange  
        //         && unitCell.coords.FindDistanceTo(cell.coords) > 0 
        //         && cell.occupied
        //         && SelectedUnit.tag != cell.unitOnTile.tag)
        //         targetable.Add(cell);
        // }
        // if (targetable.Count >= 1)
        // {
        //     StartCoroutine(SelectedUnit.Attack());
        //     int rand_index = Random.Range(0, targetable.Count);
        //     float random_val = Random.value;
        //     float damage = SelectedUnit.current_attack;
        //     if (random_val < SelectedUnit.crit)
        //         damage = SelectedUnit.current_attack * 2;
        //     int dmg_txt = (int)damage;
        //     if (targetable[rand_index].unitOnTile.FloatingTextPrefab)
        //     {
        //         GameObject damagetext = Instantiate(targetable[rand_index].unitOnTile.FloatingTextPrefab, targetable[rand_index].unitOnTile.transform.position, Quaternion.identity, transform);               
        //         damagetext.GetComponent<TextMesh>().text = dmg_txt.ToString();
        //     }
        //     StartUnit attacked_unit = targetable[rand_index].unitOnTile;
        //     targetable[rand_index].unitOnTile.current_health -= damage;
        //     attacked_unit.health_bar.GetComponent<Image>().fillAmount = attacked_unit.current_health / attacked_unit.health;

        //     if (targetable[rand_index].unitOnTile.current_attack > 10)
        //     {
        //         float percenthealth = targetable[rand_index].unitOnTile.current_health / targetable[rand_index].unitOnTile.health;
        //         targetable[rand_index].unitOnTile.current_attack *= percenthealth;
        //     }


        //    if (targetable[rand_index].unitOnTile.current_health <= 0)
        //    {
        //        int index = targetable[rand_index].coords.X_coord + targetable[rand_index].coords.Z_coord * hexGrid.width + targetable[rand_index].coords.Z_coord / 2;
        //        RemoveUnitInfo(targetable[rand_index], index);
        //    }
        //    else
        //    {
        //        yield return new WaitForSeconds(0.3f);
        //        StartCoroutine(targetable[rand_index].unitOnTile.Hit());
        //    }
        //}
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
        current.unitOnTile.dead = true;
        current.occupied = false;
        current.unitOnTile = null;

    }

    HexagonCell GetCellUnderCursor2D() // findn the cell under the cursor thats 2D
    {
        RaycastHit2D hit = Physics2D.Raycast(cursor.point.transform.position, Vector2.zero, 0f);
        if (hit)
        {
            Debug.Log(hit.transform.gameObject.GetComponent<HexagonCell>()); // debug stuff
            return hit.transform.gameObject.GetComponent<HexagonCell>();
        }
        else return null;
    }

    void CreateUnit(int index, StartUnit unit, string proper, string adjective)
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
        SelectedUnit.unit_name = "" + adjective + " " + proper;
        SelectedUnit.Unit_Stats_Panel.GetComponent<BattleUI>().Hide();
    }

    IEnumerator MoveUnit(HexagonCell _unitCell, HexagonCell _nextCell)
    {
        int distance = hexGrid.GetCell(SelectedUnit.transform.position).coords.FindDistanceTo(cursor.coords);
        int index = hexGrid.Get_Index(cursor.coords);
        unitCell = hexGrid.GetCell(SelectedUnit.transform.position);
        //int distance = unitCell.coords.FindDistanceTo(hexGrid.cells[index].coords);
        //Debug.Log("Distance From: " + unitCell.coords.ToString() + " To: " +
        //hexGrid.cells[index].coords.ToString() +
        //" = " + distance.ToString()); //for debugging distance
        if (SelectedUnit.mobility >= distance && MoveableUnits.Contains(SelectedUnit))
        {
            StartCoroutine(SelectedUnit.Moving());
            yield return new WaitForSeconds(0.25f);
            _unitCell.occupied = false;
            _unitCell.unitOnTile = null;
            SelectedUnit.transform.position = hexGrid.cells[index].transform.position;
            _unitCell = hexGrid.cells[index];
            hexGrid.cells[index].occupied = true;
            hexGrid.cells[index].unitOnTile = SelectedUnit;
            if (hexGrid.cells[index].tag == "TeamBuff" && hexGrid.cells[index].occupied)
            {
                // hexGrid.cells[index].occupied = true;
                // hexGrid.cells[index].unitOnTile = SelectedUnit;
                hexGrid.cells[index].GetComponent<TeamPowerupTiles>().discovered = true;
                if (hexGrid.cells[index].unitOnTile.tag == "Player 1")
                    hexGrid.cells[index].GetComponent<TeamPowerupTiles>().UnitsTeam = P1Team;
                if (hexGrid.cells[index].unitOnTile.tag == "Player 2")
                    hexGrid.cells[index].GetComponent<TeamPowerupTiles>().UnitsTeam = P2Team;
            }
            MoveableUnits.Remove(SelectedUnit);
            Anima2D.SpriteMeshInstance[] Unit_Meshes = SelectedUnit.gameObject.GetComponentsInChildren<Anima2D.SpriteMeshInstance>();
            for (int i = 0; i < Unit_Meshes.Length; i++)
            {
                Unit_Meshes[i].color = Greyed_Unit_Color;
                Debug.Log("Color_Changed");
            }
            DeselectUnit();
            if (MoveableUnits.Count > 0)
            {
                Snap_To_Next_Unit(true);
                Debug.Log("Snapped");
            }

        }
        else
        {
            Debug.LogError("CAN'T MOVE THATS TOO FAR FOR THE UNIT");
        }
    }

    int mod(int x, int m)
    {
        return (x % m + m) % m;
    }

    public void Show_Units_In_Range()
    {
        int range = SelectedUnit.attackRange;
        int width = hexGrid.width;
        int height = hexGrid.height;

        for (int i = 0; i < (width * height); i++)
        {
            if (hexGrid.cells[i].gameObject.tag != "Wall")
            {

                if (currentState == TurnStates.P1_MOVE && SelectedUnit.gameObject.CompareTag("Player 1"))
                {
                    if (cursor.coords.FindDistanceTo(hexGrid.cells[i].coords) <= range && hexGrid.cells[i].occupied
                        && hexGrid.cells[i].unitOnTile.gameObject.CompareTag("Player 2"))
                    {
                        hexGrid.cells[i].spriteRenderer.color = color_r;
                    }
                    else if (hexGrid.cells[i].occupied && hexGrid.cells[i].unitOnTile.gameObject.CompareTag("Player 2"))
                    {
                        hexGrid.cells[i].spriteRenderer.color = color_w;
                    }
                }

                //Should make editor change a text value so that we dont have to do a text comapare every time
                else if (currentState == TurnStates.P2_MOVE && SelectedUnit.gameObject.CompareTag("Player 2"))
                {
                    if (cursor.coords.FindDistanceTo(hexGrid.cells[i].coords) <= range && hexGrid.cells[i].occupied
                        && hexGrid.cells[i].unitOnTile.gameObject.CompareTag("Player 1"))
                    {
                        hexGrid.cells[i].spriteRenderer.color = color_r;
                    }
                    else if (hexGrid.cells[i].occupied && hexGrid.cells[i].unitOnTile.gameObject.CompareTag("Player 1"))
                    {
                        hexGrid.cells[i].spriteRenderer.color = color_w;
                    }
                }

            }

        }
    }

    //fill this out
    public bool Is_Tile_In_Move_Range()
    {
        int move_range = SelectedUnit.mobility;
        HexagonCell Selected_Unit_Cell = hexGrid.GetCell(SelectedUnit.transform.position);
        if (Selected_Unit_Cell.coords.FindDistanceTo(cursor.coords) <= move_range)
        {
            return true;
        }
        return false;
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
        }
    }

    public void Assign_BUI_Var(BattleUI _UI)
    {
        _UI.obj_name.text = SelectedUnit.unit_name;
        _UI.unit_icon.GetComponent<Image>().sprite = SelectedUnit.Icon;
        _UI.health_Bar.GetComponent<Image>().fillAmount = SelectedUnit.current_health / SelectedUnit.health;
    }

    public void Assign_Stats_Var(BattleUI _UI, StartUnit _unit)
    {
        _UI.obj_name.text = _unit.unit_name;
        _UI.obj_type.text = _unit.unit_type;
        _UI.stats_atk.text = "ATK: " + (int)_unit.current_attack;
        _UI.stats_mov.text = "MOV: " + _unit.mobility;
        _UI.stats_crit.text = "CRIT: " + (int)_unit.crit + "%";
        _UI.stats_range.text = "RNG: " + _unit.attackRange;
    }

    public void Assign_BUI_Var(BattleUI _UI, StartUnit _unit)
    {
        _UI.obj_name.text = _unit.unit_name;
        _UI.unit_icon.GetComponent<Image>().sprite = _unit.Icon;
        _UI.health_Bar.GetComponent<Image>().fillAmount = _unit.current_health / _unit.health;
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
                    }
                    break;
            }
        }

    }
}
