using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct HexagonCoord{

    [SerializeField]
    private int x, z;

    public int X_coord { get { return x; } }
    public int Z_coord { get { return z; } }
    public int Y_coord { get { return -X_coord - Z_coord; } }

    public HexagonCoord(int x , int z)
    {
        this.x = x;
        this.z = z;
    }

    public static HexagonCoord FromOffsetCoordinates(int x, int z)
    {
        return new HexagonCoord(x - z / 2, z);
    }

    public override string ToString()
    {
        return "(" + X_coord.ToString() + ", " + Z_coord + ")";
    }

    public string ToStringSeparateLines()
    {
        return X_coord.ToString() + "\n" + Z_coord.ToString();
    }

}
