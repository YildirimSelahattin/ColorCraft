using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ColorPalette", menuName = "ScriptableObjects/ColorPalette", order = 1)]
public class ColorPalette : ScriptableObject
{
    public List<Color> colors;

    public Color GetColor(int index)
    {
        return colors[index % colors.Count];
    }
}