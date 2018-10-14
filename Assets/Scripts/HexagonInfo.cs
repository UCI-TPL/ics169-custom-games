using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//HexagonInfo is a static class which holds information on how far a hexagon's
//vertices are from its' center. This is relative to a radius 10 circle
public static class HexagonInfo{
   
    public const float outerRadius = 10f;

    public const float innerRadius = outerRadius * 0.866025404f;

    public static Vector3[] corners =
    {
        new Vector3(0f, 0f, outerRadius),
        new Vector3(innerRadius,0f,0.5f  * outerRadius),
        new Vector3(innerRadius,0f,-0.5f * outerRadius),
        new Vector3(0f,0f,-outerRadius),
        new Vector3(-innerRadius,0f,-0.5f * outerRadius),
        new Vector3(-innerRadius,0f,0.5f * outerRadius),
        new Vector3(0f, 0f, outerRadius)
    };
}
