using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//HexagonMesh is a what turns cells into their hexagon shapes

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HexagonMesh : MonoBehaviour {

    Mesh hexMesh;
    List<Vector3> vertices;
    List<int> triangles;

	// Use this for initialization
	void Awake () {

        GetComponent<MeshFilter>().mesh = hexMesh = new Mesh();
        hexMesh.name = "Hex Mesh";
        vertices = new List<Vector3>();
        triangles = new List<int>();
	}

    public void Triangulate(HexagonCell[] cells) // Triangulate clears all of the initial data and triangulates every cell on the board again
    {
        hexMesh.Clear();
        vertices.Clear();
        triangles.Clear();
        for(int i = 0; i < cells.Length; i++)
        {
            Triangulate(cells[i]);
        }

        hexMesh.vertices = vertices.ToArray();
        hexMesh.triangles = triangles.ToArray();
        hexMesh.RecalculateNormals();
    }

    void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3)// AddTriangle takes three points and makes them the vertices of a triangle
    {
        int vertexIndex = vertices.Count;
        vertices.Add(v1);
        vertices.Add(v2);
        vertices.Add(v3);
        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 2);
    }

    void Triangulate(HexagonCell cell)//Triangulate finds the center point of the cell and creates the hexagon based off of that point
    {
        Vector3 center = cell.transform.localPosition;
        Quaternion rotation = Quaternion.Euler(90f, 0f,0f);
        center = rotation * center;
        for (int i = 0; i < 6; i++)
        {
            AddTriangle(
              center,
              center + HexagonInfo.corners[i],
              center + HexagonInfo.corners[i + 1]);
        }
    }

}
