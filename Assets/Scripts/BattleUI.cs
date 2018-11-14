using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUI : MonoBehaviour
{

    public Text obj_name;
    public Text stats_atk;
    public Text stats_range;
    public Text stats_crit;
    public Text stats_mov;
    public Text stats;
    public Text turn;
    public GameObject health_Bar;
    public Slider health_slider;
    public Text health_text;
    public Image unit_icon;
    public Image unit_info_Image;
    public Image turn_info_Image;
    public Text stats_2;
    public Text obj_type;

    private Renderer[] cur_renderers;

    // Use this for initialization
    void Awake()
    {
        cur_renderers = GetComponentsInChildren<Renderer>(true);

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Hide()
    {
        //Debug.Log("Attempt_To_Hide");
        this.gameObject.SetActive(false);
        //SetRendererEnabled(false);
    }

    public void Show()
    {
        //Debug.Log("Attempt_To_Show");
        this.gameObject.SetActive(true);
        //SetRendererEnabled(true);
    }

    private void SetRendererEnabled(bool enableRenderer)
    {
        for (int x = 0; x < cur_renderers.Length; x++)
            cur_renderers[x].enabled = enableRenderer;
    }
}
