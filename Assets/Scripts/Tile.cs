using UnityEngine;

public class Tile : MonoBehaviour
{
    private Vector2Int m_Position;

    private Renderer m_Renderer;

#pragma warning disable IDE1006 // Naming Styles
    public Vector2Int position
#pragma warning restore IDE1006 // Naming Styles
    {
        get => m_Position;
        set
        {
            m_Position = value;
            transform.localPosition = new(value.x - 3.5f, 0, value.y - 3.5f);
        }
    }

    void Awake()
    {
        m_Renderer = gameObject.GetComponentInChildren<MeshRenderer>();
    }

    public void DefaultStyle()
    {
        m_Renderer.material.color = (position.x + position.y) % 2 == 1 ? Color.white : Color.black;
    }

    public void StyleMove()
    {
        m_Renderer.material.color = (position.x + position.y) % 2 == 1 ? Color.lightGreen : Color.green;
    }

    public void StyleTake()
    {
        m_Renderer.material.color = (position.x + position.y) % 2 == 1 ? Color.orange : Color.orangeRed;
    }

    public void StyleSpecial()
    {
        m_Renderer.material.color = (position.x + position.y) % 2 == 1 ? Color.skyBlue : Color.blue;
    }
}
