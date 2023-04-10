using System.Collections.Generic;
using UnityEngine;

public class Hexagon : MonoBehaviour
{
    public Color color;
    public List<Hexagon> neighbors = new List<Hexagon>(); // Komşuları depolamak için yeni liste

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetColor(Color newColor)
    {
        color = newColor;
        spriteRenderer.color = color;
    }

    // Tıklandığında komşuları konsola yazdırmak için yeni işlev
    private void OnMouseDown()
    {
        Debug.Log("Tıklanan Hexagon: " + transform.position);
        foreach (Hexagon neighbor in neighbors)
        {
            Debug.Log("Komşu Hexagon: " + neighbor.transform.position);
        }
    }
}
