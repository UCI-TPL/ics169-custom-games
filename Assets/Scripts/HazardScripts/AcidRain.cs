using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcidRain : EnvironmentalHazard {
    AudioSource rainSound;
    public AudioSource lightningSound;
    public int damage = 25;

    public override HazardInfo CreateHazard(Grid hexGrid)
    {
        int randRange = Random.Range(0, hexGrid.cells.Length);
        HexagonCoord rand = hexGrid.cells[randRange].coords;

        int size = Random.Range(2,5); // 0 = 1, 1 = 3, 2 = 5
        Debug.Log("hazard at (" + rand.x + "," + rand.z + ") with size: " + size);
        List<HexagonCell> frontier = new List<HexagonCell>();
        HexagonCell curr = hexGrid.Get_Cell_Index(new HexagonCoord(rand.x, rand.z));
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
            frontier[j].Create_Rain();
        }
        GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>().lightningSound.Play();
        GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>().rainSound.Play();
        return new HazardInfo(this, rand.x, rand.Y_coord, rand.z, timeOnBoard, size);
    }

    public override HazardInfo CreateHazardAt(HexagonCell cell, Grid hexGrid)
    {
        HexagonCoord coord = cell.coords;
 
        int size = Random.Range(2, 5); // 0 = 1, 1 = 3, 2 = 5
        List<HexagonCell> frontier = new List<HexagonCell>();
        //HexagonCell curr = hexGrid.Get_Cell_Index(new HexagonCoord(rand.x, rand.z));
        for (int i = 0; i < hexGrid.cells.Length; i++)
        {

            int distance = cell.coords.FindDistanceTo(hexGrid.cells[i].coords);
            if (distance <= size)
            {
                frontier.Add(hexGrid.cells[i]);
            }
        }

        for (int j = 0; j < frontier.Count; j++)
        {
            frontier[j].Create_Rain();
        }
        GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>().lightningSound.Play();
        GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>().rainSound.Play();

        return new HazardInfo(this, coord.x, coord.Y_coord, coord.z, timeOnBoard, size);
    }

    public override void RemoveHazard(Grid hexGrid, int x, int z, int size)
    {
        List<HexagonCell> frontier = new List<HexagonCell>();
        HexagonCell curr = hexGrid.Get_Cell_Index(new HexagonCoord(x, z));
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
            Destroy(frontier[j].HazardObject);
        }
        GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>().birdSound.Play();
        GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>().rainSound.Stop();
    }


    public override IEnumerator Effect(HexagonMapEditor editor, Grid hexGrid, int x, int z, int size)
    {
        List<HexagonCell> frontier = new List<HexagonCell>(); // list of nodes that the hazard has effect over
        HexagonCell curr = hexGrid.Get_Cell_Index(new HexagonCoord(x, z));
        Debug.Log(type_name + " hazard epicenter at: " + curr.coords.x + "," + curr.coords.Y_coord + "," + curr.coords.z);
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
                frontier[j].unitOnTile.current_health -= damage; // this should be changeed when we are trying to implement the fortress hero's defense

                StartUnit attacked_unit = frontier[j].unitOnTile;
                GameObject damagetext = Instantiate(attacked_unit.FloatingTextPrefab, attacked_unit.transform.position, Quaternion.identity, attacked_unit.transform);
                damagetext.GetComponent<TextMesh>().color = Color.yellow;
                damagetext.GetComponent<TextMesh>().characterSize = 0.03f + (0.06f * ((float)10 / 75f));
                damagetext.GetComponent<TextMesh>().text = damage.ToString();

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

                attacked_unit.PlayHit();
                attacked_unit.PlayBlink(editor.Unit_Hurt_Color, attacked_unit, Time.time + 1f);

                if (attacked_unit.current_health <= 0)
                {

                    attacked_unit.dead = true;
                    editor.Units_To_Delete.Add(frontier[j]);
                }
            }
        }
        yield return new WaitForSeconds(anim_time);
        Debug.Log("effect finishing");
    }
}
