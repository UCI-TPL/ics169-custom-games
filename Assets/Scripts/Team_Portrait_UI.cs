using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Team_Portrait_UI : MonoBehaviour {
    public Color32 _default = Color.white;
    public Color32 _dead;

    [SerializeField]
    public List<Portrait_Script> unit_portraits = new List<Portrait_Script>();

    public Dictionary<Portrait_Script, StartUnit> Unit_Port_Map = new Dictionary<Portrait_Script, StartUnit>();

    public int cur_num_units;
    public int max_team_size = 7;

    void Awake()
    {
        unit_portraits.AddRange(gameObject.GetComponentsInChildren<Portrait_Script>());
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Hide_Unused()
    {
        for(int i = 0; i < max_team_size; i++)
        {
            if (!unit_portraits[i].In_Use)
            {
                unit_portraits[i].gameObject.SetActive(false);
            }     
        }
    }

    public void Initialize_Portraits(List<StartUnit> Unit_List)
    {
        cur_num_units = Unit_List.Count;
        int k = cur_num_units - 1;
        for(int i = 0; i < Unit_List.Count; i++)
        {
            unit_portraits[i].Change_portrait(Unit_List[k].Icon);
            unit_portraits[i].In_Use = true;
            Unit_Port_Map.Add(unit_portraits[i], Unit_List[k]);
            k--;
        }

        Hide_Unused();
    }

    public void Update_Portraits()
    {
        foreach (KeyValuePair<Portrait_Script, StartUnit> entry in Unit_Port_Map)
        {
            if(entry.Value.dead)
            {
                //grey out portrait
                grey_portrait(entry.Key);
            }
        }
    }

    public void grey_portrait(Portrait_Script port_script)
    {
        port_script.Portrait.color = _dead;
        port_script.Portrait_BG.color = _dead;
    }
}
