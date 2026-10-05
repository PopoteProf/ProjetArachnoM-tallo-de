using UnityEngine;

public class DebugCell : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;

    public void ChangeColor(Color color) {
        spriteRenderer.color = color;
    } 
}