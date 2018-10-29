﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class HexagonMapEditor : MonoBehaviour {

    public PlayerInformation PlayerInfo;
    public Grid hexGrid;
    public BattleUI UI;
    public Cursor cursor;

    public StartUnit unit1Prefab;
    public StartUnit unit2Prefab;

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

    [SerializeField] private TurnStates currentState;
    private void Awake()
    {
        PlayerInfo = FindObjectOfType<PlayerInformation>();
    }
    // Use this for initialization
    void Start () {
        initializing = true;
        UI = GetComponentInChildren<BattleUI>();
        if (initializing) // stop loop if already doing it
        {
            //initializing = false;
            InitialPhase(PlayerInfo.Player1Chosen,1);
            if(PlayerInfo.one_player)
                InitialPhase(Player2Chosen,2);
            else
                InitialPhase(PlayerInfo.Player2Chosen, 2);
            FindTeam("Player 1"); // find the units for player 1's team
            FindTeam("Player 2"); // "             " for player 2's team
        }
        MoveableUnits = new List<StartUnit>(P1Team); // put player 1's team in since they're going first
        currentState = TurnStates.P1_MOVE;
        //StartCoroutine(InitializingTeams());

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
                }
                UI.turn.text = "TURN:PLAYER 1";
                MovePhase(PlayerInfo.player1);
                break;
            case (TurnStates.P1_ATTACK):
                if (!attacking) // only call once
                {
                    attacking = true;
                    AttackPhase(P1Team); // handles all attacking for player 1
                    attacking = false;
                    currentState = TurnStates.P2_MOVE;
                }
                break;
            case (TurnStates.P2_MOVE):
                if (MoveableUnits.Count == 0)
                {
                    currentState = TurnStates.P2_ATTACK;
                    MoveableUnits = new List<StartUnit>(P1Team);
                }
                UI.turn.text = "TURN:PLAYER 2";
                if(PlayerInfo.one_player)
                    MovePhase(PlayerInfo.player1);
                else
                    MovePhase(PlayerInfo.player2);
                break;
            case (TurnStates.P2_ATTACK):
                if (!attacking)
                {
                    attacking = true;
                    AttackPhase(P2Team);
                    attacking = false;
                    currentState = TurnStates.CHECK;
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
        foreach(GameObject g in l)
        {
            if (team_name == "Player 1")
                P1Team.Add(g.GetComponent<StartUnit>());
            else
                P2Team.Add(g.GetComponent<StartUnit>());
        }
    }

    void InitialPhase(List<StartUnit> team, int player) // creates random units on the grid it sometimes repeats the units on tiles but not important cause will change later
    {
        initializing = false;
        if(player == 1)
        {
            for(int i = 0; i < team.Count;i++)
            {
                CreateUnit(i, team[i]);
               
            }
        }
        else
        {
            int k = 0;
            for(int j = hexGrid.cells.Length-1; j > hexGrid.cells.Length-1-team.Count; j--)
            {
                CreateUnit(j, team[k]);
                k++;
            }
        }
        //initializing = true;
    }

    public void MovePhase(string joystick) // handles input from the player to correctly move the unit
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (Input.GetButtonDown(joystick + "X Button"))
                HandleInput();
            if (Input.GetButtonDown(joystick + "B Button"))
            {
                DeselectUnit();
            }
        }

        if(Input.GetButtonDown(joystick + "A Button"))
        {
            if(MoveableUnits.Contains(SelectedUnit))
            {
                MoveableUnits.Remove(SelectedUnit);
            }
        }
    }

    public void AttackPhase(List<StartUnit> attackingTeam) // handles input from the player to correctly attack
    {

        foreach(StartUnit unit in attackingTeam)
        {
            SelectedUnit = unit;
            unitCell = hexGrid.GetCell(unit.transform.position);
            isUnitSelected = true;
            StartCoroutine(AttackUnit());
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
        else if(!currentCell.occupied && isUnitSelected && !attacking) // a unit is already selected
        {
            StartCoroutine(MoveUnit(index));//move that selected unit
        }

    }

    void HandleAttack() // similar to HandleInput() but for the attack phase only
    {
        HexagonCell currentCell = GetCellUnderCursor2D();
        int index = currentCell.coords.X_coord + currentCell.coords.Z_coord * hexGrid.width + currentCell.coords.Z_coord / 2;
        if (currentCell.occupied && attacking) // if attacking and cell is occupied
        {
            if (!whileAttacking)
                StartCoroutine(AttackUnit());
        }
    }

    void SelectUnit(HexagonCell current, int index) // sets variables to the clicked position's unit
    {
        SelectedUnit = current.unitOnTile;
        unitCell = hexGrid.cells[index];
        isUnitSelected = true;

        //hexGrid.ShowPath(unitCell, SelectedUnit.mobility, hexGrid.touchedColor);
        //UI.name.text = SelectedUnit.name.ToString();
        //UI.stats.text = "HEALTH:" + (int)SelectedUnit.current_health + "\nATTACK:" + (int)SelectedUnit.current_attack;

        hexGrid.ShowPath(unitCell, SelectedUnit.mobility, SelectedUnit.attackRange, hexGrid.touchedColor, hexGrid.attackColor);
        UI.obj_name.text =  "UNIT:"+ SelectedUnit.name.ToString();
        UI.stats.text = "HEALTH:" + (int)SelectedUnit.current_health + "\nATTACK:" + (int)SelectedUnit.current_attack;

        //hexGrid.ShowPath(unitCell, SelectedUnit.mobility, SelectedUnit.attackRange, hexGrid.touchedColor, hexGrid.attackColor);
        //UI.obj_name.text =  "UNIT:"+ SelectedUnit.name.ToString();
        //UI.stats.text = "HEALTH:" + SelectedUnit.current_health + "\nATTACK:" + SelectedUnit.current_attack;


    }

    void DeselectUnit() // clears all variables to the clicked position
    {
        SelectedUnit = null;
        unitCell = null;
        isUnitSelected = false;
        hexGrid.ClearPath();
        UI.obj_name.text = "UNIT:";
        UI.stats.text = "HEALTH:\nATTACK:";
    }

    IEnumerator AttackUnit()
    {
        whileAttacking = true;
        List<HexagonCell> targetable = new List<HexagonCell>();
        foreach(HexagonCell cell in hexGrid.cells)
        {
            if (unitCell.coords.FindDistanceTo(cell.coords) <= SelectedUnit.attackRange  
                && unitCell.coords.FindDistanceTo(cell.coords) > 0 
                && cell.occupied
                && SelectedUnit.tag != cell.unitOnTile.tag)
                targetable.Add(cell);
        }
        if (targetable.Count >= 1)
        {
            StartCoroutine(SelectedUnit.Attack());
            int rand_index = Random.Range(0, targetable.Count);
            float random_val = Random.value;
            float damage = SelectedUnit.current_attack;
            if (random_val < SelectedUnit.crit)
                damage = SelectedUnit.current_attack * 2;
            int dmg_txt = (int)damage;
            if (targetable[rand_index].unitOnTile.FloatingTextPrefab)
            {
                GameObject damagetext = Instantiate(targetable[rand_index].unitOnTile.FloatingTextPrefab, targetable[rand_index].unitOnTile.transform.position, Quaternion.identity, transform);               
                damagetext.GetComponent<TextMesh>().text = dmg_txt.ToString();
            }
            targetable[rand_index].unitOnTile.current_health -= damage;

            if (targetable[rand_index].unitOnTile.current_attack > 10)
            {
                float percenthealth = targetable[rand_index].unitOnTile.current_health / targetable[rand_index].unitOnTile.health;
                targetable[rand_index].unitOnTile.current_attack *= percenthealth;
            }


            if (targetable[rand_index].unitOnTile.current_health <= 0)
            {
                int index = targetable[rand_index].coords.X_coord + targetable[rand_index].coords.Z_coord * hexGrid.width + targetable[rand_index].coords.Z_coord / 2;
                RemoveUnitInfo(targetable[rand_index], index);
            }
            else
            {
                yield return new WaitForSeconds(0.3f);
                StartCoroutine(targetable[rand_index].unitOnTile.Hit());
            }
       }
        whileAttacking = false;

    }

    void RemoveUnitInfo(HexagonCell current, int index)  // when a unit dies use this function to remove it from the grid
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
    
    void CreateUnit(int index, StartUnit unit)
    {
        SelectedUnit = Instantiate(unit);
        isUnitSelected = true;
        unitCell = hexGrid.cells[index];
        SelectedUnit.transform.position = hexGrid.cells[index].transform.position;
        unitCell.occupied = true;
        unitCell.unitOnTile = SelectedUnit;
    }

    IEnumerator MoveUnit(int index)
    {

        int distance = unitCell.coords.FindDistanceTo(hexGrid.cells[index].coords);
        //Debug.Log("Distance From: " + unitCell.coords.ToString() + " To: " +
        //hexGrid.cells[index].coords.ToString() +
        //" = " + distance.ToString()); //for debugging distance
        if (SelectedUnit.mobility >= distance && MoveableUnits.Contains(SelectedUnit))
        {
            StartCoroutine(SelectedUnit.Moving());
            yield return new WaitForSeconds(0.25f);
            unitCell.occupied = false;
            unitCell.unitOnTile = null;
            SelectedUnit.transform.position = hexGrid.cells[index].transform.position;
            unitCell = hexGrid.cells[index];
            hexGrid.cells[index].occupied = true;
            hexGrid.cells[index].unitOnTile = SelectedUnit;
            MoveableUnits.Remove(SelectedUnit);
        }
        else
        {
            Debug.LogError("CAN'T MOVE THATS TOO FAR FOR THE UNIT");
        }
    }
}
