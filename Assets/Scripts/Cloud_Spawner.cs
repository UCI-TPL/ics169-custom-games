using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cloud_Spawner : MonoBehaviour {
    public int min_y = -30;
    public int max_y = 30;
    public int min_x = -30;
    public int max_x = 30;
    public int gap_length = 5;
    public int half_cloud_size = 5;
    public float min_size = 5f;
    public float max_size = 10f;
    public float Animation_Offset_Time = 10f;
    public List<GameObject> Clouds;

	// Use this for initialization
	void Start () {

        int cloud = 0;
        for (int i = min_y; i <= max_y; i += gap_length)
        {
            cloud = Random.Range(0, Clouds.Count);
            GameObject cloud_obj = Clouds[cloud];
            GameObject new_cloud = GameObject.Instantiate(cloud_obj, this.transform);
            float size = Random.Range(min_size, max_size);
            new_cloud.transform.position = new Vector3(min_x - (size * half_cloud_size * 1.2f), i, 0);
            new_cloud.transform.localScale = new Vector3(size,size,size);
            new_cloud.GetComponentInChildren<Animator>().Play(0,-1, Random.Range(0f, Animation_Offset_Time));
        }

        for (int i = min_y; i <= max_y; i += gap_length)
        {
            cloud = Random.Range(0, Clouds.Count);
            GameObject cloud_obj = Clouds[cloud];
            GameObject new_cloud = GameObject.Instantiate(cloud_obj, this.transform);
            float size = Random.Range(min_size, max_size);
            new_cloud.transform.position = new Vector3(max_x + (size * half_cloud_size * 1.2f), i, 0);
            new_cloud.transform.localScale = new Vector3(size, size, size);
            new_cloud.GetComponentInChildren<Animator>().Play(0, -1, Random.Range(0f, Animation_Offset_Time));
        }

        for (int i = min_x; i <= max_x; i += (gap_length * 2))
        {
            cloud = Random.Range(0, Clouds.Count);
            GameObject cloud_obj = Clouds[cloud];
            GameObject new_cloud = GameObject.Instantiate(cloud_obj, this.transform);
            float size = Random.Range(min_size, max_size);
            new_cloud.transform.position = new Vector3(i, min_y - (size * half_cloud_size * 0.8f), 0);
            new_cloud.transform.localScale = new Vector3(size, size, size);
            new_cloud.GetComponentInChildren<Animator>().Play(0, -1, Random.Range(0f, Animation_Offset_Time));
        }

        for (int i = min_x; i <= max_x; i += (gap_length * 2))
        {
            cloud = Random.Range(0, Clouds.Count);
            GameObject cloud_obj = Clouds[cloud];
            GameObject new_cloud = GameObject.Instantiate(cloud_obj, this.transform);
            float size = Random.Range(min_size, max_size);
            new_cloud.transform.position = new Vector3(i, max_y + (size * half_cloud_size * 0.8f), 0);
            new_cloud.transform.localScale = new Vector3(size, size, size);
            new_cloud.GetComponentInChildren<Animator>().Play(0, -1, Random.Range(0f, Animation_Offset_Time));
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void spawn_clouds(int min_pos, int max_pos, int x_pos, int y_pos)
    {
        for (int i = min_pos; i <= max_pos; i += gap_length)
        {
            int _cloud = Random.Range(0, Clouds.Count);
            GameObject cloud_obj = Clouds[_cloud];
            GameObject new_cloud = GameObject.Instantiate(cloud_obj, this.transform);
            new_cloud.transform.position = new Vector3(x_pos, i, 0);
            float size = Random.Range(min_size, max_size);
            new_cloud.transform.localScale = new Vector3(size, size, size);
            new_cloud.GetComponentInChildren<Animator>().Play(0, -1, Random.Range(0f, Animation_Offset_Time));
        }
    }
}
