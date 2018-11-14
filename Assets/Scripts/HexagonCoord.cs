using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//HexagonCoord simply stores the data of the x,y and z coordinates for any HexagonCell object
[System.Serializable]
public struct HexagonCoord{

    [SerializeField]
    public int x, z;

    public int X_coord { get { return x; } }
    public int Z_coord { get { return z; } }
    public int Y_coord { get { return -X_coord - Z_coord; } }

    
    public HexagonCoord(int x , int z)// constructor
    {
        this.x = x;
        this.z = z;
    }

    public static HexagonCoord FromOffsetCoordinates(int x, int z) // creates a HexagonCoord object with correct offset
    {
        return new HexagonCoord(x - z / 2, z);
    }

  

    public override string ToString() // to use for visualization of coordinates
    {
        return "(" + X_coord.ToString() + ", " + Y_coord + ", " + Z_coord + ")";
    }

    public string ToStringSeparateLines() // to use for visualization of coordinates
    {
        return X_coord.ToString() + "\n" + Y_coord.ToString() + "\n" + Z_coord.ToString();
    }

    public static HexagonCoord FromPosition(Vector3 position)
    {
        float x = position.x / (HexagonInfo.innerRadius * 2f);
        float y = -x;

        float offset = position.y / (HexagonInfo.outerRadius * 3f);
        x -= offset;
        y -= offset;

        int iX = Mathf.RoundToInt(x);
        int iY = Mathf.RoundToInt(y);
        int iZ = Mathf.RoundToInt(-x - y);
        return new HexagonCoord(iX, iZ);
    }

    public int FindDistanceTo(HexagonCoord other)
    {
        return
            ((X_coord < other.X_coord ? other.X_coord - X_coord : X_coord - other.X_coord) +
            (Y_coord < other.Y_coord ? other.Y_coord - Y_coord : Y_coord - other.Y_coord) +
            (Z_coord < other.Z_coord ? other.Z_coord - Z_coord : Z_coord - other.Z_coord)) / 2;
    }

    public int DistanceTo(HexagonCoord other)
    {
        return x < other.x ? other.x - x : x - other.x;
    }

}
