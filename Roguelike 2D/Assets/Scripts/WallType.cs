using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WallType
{
    public GameObject
        s_left, s_right, s_top, s_bottom,
        d_top_left, d_top_right, d_top_bottom, d_left_right, d_left_bottom, d_right_bottom,
        t_top_left_right, t_top_left_bottom, t_top_right_Bottom, t_left_right_bottom,
        q_all;

    public enum Door
    {
        left=8, 
        top=4, 
        bottom=2, 
        right = 1
    };
}
