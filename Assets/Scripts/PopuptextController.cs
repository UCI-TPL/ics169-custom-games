using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopuptextController : MonoBehaviour {
    private static PopupText popup;
    private static GameObject canvas;
    public static void initialize()
    {
        canvas = GameObject.Find("Canvas");
        if(!popup)
            popup = Resources.Load<PopupText>("Prefabs/PopupTextParent");
    }

    public static void CreateFloatingText(string text, Transform location)
    {
        PopupText instance = Instantiate(popup);
        instance.transform.SetParent(canvas.transform, false);
        instance.SetText(text);
    }

}
