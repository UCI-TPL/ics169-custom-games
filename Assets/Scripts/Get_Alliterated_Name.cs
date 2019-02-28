using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Get_Alliterated_Name{
    Dictionary<char, List<string>> Name_Map;
    string[] names_proper;

    public Get_Alliterated_Name()
    {
        Name_Map = new Dictionary<char, List<string>>();

        TextAsset names_proper_ass = Resources.Load<TextAsset>("proper");
        names_proper = names_proper_ass.text.Split(new char[] { '\n' });

        TextAsset names_adj_ass = Resources.Load<TextAsset>("adjectives");
        string[] names_adj = names_adj_ass.text.Split(new char[] { '\n' });

        foreach(string names in names_adj)
        {
            if (Name_Map.ContainsKey(names[0]))
            {
                Name_Map[names[0]].Add(names);
            }
            else
            {
                List<string> empty = new List<string>();
                Name_Map.Add(names[0], empty);
                Name_Map[names[0]].Add(names);
            }
            
        }
    }

    public string Get_Name()
    {
        string result_name = "";

        int rand_index_proper_name = Random.Range(0, names_proper.Length - 1);
        string rand_proper_name = names_proper[rand_index_proper_name];

        List<string> adj_list = Name_Map[rand_proper_name[0]];
        string rand_adj = adj_list[Random.Range(0, adj_list.Count - 1)];

        result_name = rand_adj + " " + rand_proper_name;

        return result_name;
    }
}
