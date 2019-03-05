using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : EnvironmentalHazard {
    public AudioSource launchSound;
    public AudioSource explosionSound;
    public int damage = 40;

    public override HazardInfo CreateHazard(int size, HexagonCoord coord, Grid hexGrid)
    {
        //int randRange = Random.Range(0, hexGrid.cells.Length);
        //HexagonCoord rand = hexGrid.cells[randRange].coords;

        //int size = Random.Range(2, 4); // 0 = 1, 1 = 3, 2 = 5
        Debug.Log("hazard at (" + coord.x + "," + coord.z + ") with size: " + size);
        
        return new HazardInfo(this, coord.x, coord.Y_coord, coord.z, timeOnBoard, size);
    }

    public override void RemoveHazard(Grid hexGrid, int x, int z, int size, bool weatherVane) // weatherVane is true when placed by tempest hero
    {
        HexagonCell curr = hexGrid.Get_Cell_Index(new HexagonCoord(x, z));

        Destroy(curr.Missile_Obj);

        if (weatherVane) // if placed by a tempest hero
        {
            Destroy(curr.Weather_Vane_Obj);
        }
    }

    public override IEnumerator Effect(HexagonMapEditor editor, Grid hexGrid, int x, int z, int size)
    {
        List<HexagonCell> frontier = new List<HexagonCell>(); // list of nodes that the hazard has effect over
        HexagonCell curr = hexGrid.Get_Cell_Index(new HexagonCoord(x, z));
        Debug.Log(type_name + " hazard epicenter at: " + curr.coords.x + "," + curr.coords.Y_coord + "," + curr.coords.z);
        curr.CreateMissile();
        GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>().lightningSound.Play(); // play launch sound instead
        yield return new WaitForSeconds(anim_time-1);
        GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>().lightningSound.Play(); // play explosion sound instead
        for (int i = 0; i < hexGrid.cells.Length; i++)
        {

            int distance = curr.coords.FindDistanceTo(hexGrid.cells[i].coords);
            if (distance <= size)
            {
                frontier.Add(hexGrid.cells[i]);
            }
        }
        for (int j = 0; j < frontier.Count; j++)
        {
            if (frontier[j].occupied)
            {
                frontier[j].unitOnTile.current_health -= damage - frontier[j].unitOnTile.defense; // this should be changeed when we are trying to implement the fortress hero's defense

                StartUnit attacked_unit = frontier[j].unitOnTile;
                GameObject damagetext = Instantiate(attacked_unit.FloatingTextPrefab, attacked_unit.transform.position, Quaternion.identity, attacked_unit.transform);
                damagetext.GetComponent<TextMesh>().color = Color.yellow;
                damagetext.GetComponent<TextMesh>().characterSize = 0.03f + (0.06f * ((float)10 / 75f));
                damagetext.GetComponent<TextMesh>().text = (damage - frontier[j].unitOnTile.defense).ToString();

                if (Mathf.Sign(damagetext.transform.parent.localScale.x) == -1 && Mathf.Sign(damagetext.transform.localScale.x) == 1)
                {
                    damagetext.gameObject.transform.localScale = new Vector3(damagetext.transform.localScale.x * -1, damagetext.transform.localScale.y,
                        damagetext.transform.localScale.z);

                    //damagetext.GetComponent<TextMesh>().color = Color.green;
                    //Debug.Log("BackWards Text");
                }
                else
                {
                    if (Mathf.Sign(damagetext.transform.parent.localScale.x) == 1 && Mathf.Sign(damagetext.transform.localScale.x) == -1)
                    {
                        damagetext.gameObject.transform.localScale = new Vector3(damagetext.transform.localScale.x * -1, damagetext.transform.localScale.y,
                            damagetext.transform.localScale.z);
                    }
                }

                attacked_unit.TakeDamage(attacked_unit, (damage - frontier[j].unitOnTile.defense));
                attacked_unit.PlayHit();
                attacked_unit.PlayBlink(editor.Unit_Hurt_Color, attacked_unit, Time.time + 1f);

                if (attacked_unit.current_health <= 0)
                {

                    attacked_unit.dead = true;
                    editor.Units_To_Delete.Add(frontier[j]);
                }
            }
        }
        yield return new WaitForSeconds(1f);
        Debug.Log("effect finishing");
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
