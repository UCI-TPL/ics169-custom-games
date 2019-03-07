using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Render_Sort : MonoBehaviour {

    public void Sort_renderer(GameObject object_to_sort, HexagonCell _target_location)
    {
        Anima2D.SpriteMeshInstance Sprite_Mesh = object_to_sort.GetComponent<Anima2D.SpriteMeshInstance>();
        if (Sprite_Mesh != null)
        {
            Sprite_Mesh.sortingOrder = Sprite_Mesh.GetComponent<Mesh_Layer>()._ordered_layer
                + ((_target_location.coords.X_coord + _target_location.coords.Y_coord) * Static_Variable_Container.max_sprite_sort);
        }

        SpriteRenderer sprite_rend = object_to_sort.GetComponent<SpriteRenderer>();
        if(sprite_rend != null)
        {
            sprite_rend.sortingOrder = sprite_rend.GetComponent<Mesh_Layer>()._ordered_layer
                + ((_target_location.coords.X_coord + _target_location.coords.Y_coord) * Static_Variable_Container.max_sprite_sort);
        }

        Canvas canvas_rend = object_to_sort.GetComponent<Canvas>();
        if(canvas_rend != null)
        {
            canvas_rend.sortingOrder = canvas_rend.GetComponent<Mesh_Layer>()._ordered_layer
                + ((_target_location.coords.X_coord + _target_location.coords.Y_coord) * Static_Variable_Container.max_sprite_sort);
        }
    }

}
