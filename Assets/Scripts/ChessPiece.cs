using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public abstract class ChessPiece : MonoBehaviour
{
    private ChessPieceType m_Type;

    private MeshRenderer m_MeshRenderer;

    private uint m_MoveCount = 0;

    private float m_Epsilon = 0f;

    private bool m_IsMoving = false;

    private bool m_IsDying = false;

    private bool m_IsSelected = false;

    private int m_TurnMoved = -2;

    protected Renderer meshRenderer => m_MeshRenderer;

    private Vector3 m_FromPosition;

    public ChessPieceType type
    {
        get => m_Type;
        set
        {
            m_Type = value;
            var filter = gameObject.GetComponent<MeshFilter>();
            filter.mesh = value.mesh;
            var collider = gameObject.GetComponent<MeshCollider>();
            collider.sharedMesh = value.mesh;
        }
    }

    public int turnMoved => m_TurnMoved;

    public bool selected { get => m_IsSelected; set => m_IsSelected = value; }

    public Vector2Int position{ get => GetComponentInParent<Tile>().position; }

    public bool moved => m_MoveCount > 0;

    public uint moveCount => m_MoveCount;

    public static readonly float ANIMATION_TIME = 1f;

    public abstract void DefaultStyle();

    public abstract void StyleCapture();

    public abstract void StyleSpecial();

    public abstract void StyleSelected();

    public List<Move> GenerateMoves(IBoard board) => m_Type.GenerateMoves(this, board);

    public void Die()
    {
        m_IsDying = true;
        m_Epsilon = 0f;
        Destroy(GetComponent<MeshCollider>());
    }

    public void MoveTo(Tile target, int turn, float offset = 0f)
    {
        m_TurnMoved = turn;
        m_MoveCount++;
        m_IsMoving = true;
        m_Epsilon = -offset;
        gameObject.transform.parent = target.transform;
        m_FromPosition = transform.localPosition;
    }

    public override string ToString()
    {
        return $"{type}";
    }

    void Awake()
    {
        m_MeshRenderer = gameObject.GetComponent<MeshRenderer>();
        gameObject.AddComponent<MeshCollider>();
        transform.localPosition = Vector3.zero;
    }

    void Update()
    {
        if (m_IsDying)
        {
            m_Epsilon += Time.deltaTime;
            var t = Mathf.Clamp01(m_Epsilon / ANIMATION_TIME);
            gameObject.transform.localScale = Vector3.Lerp(new(0.8f, 0.8f, 0.8f), Vector3.zero, Mathf.SmoothStep(0, 1, t * t * (3 - 2 * t)));
            gameObject.transform.localEulerAngles = Vector3.down * 180f * t;
            if (t >= 1)
            {
                m_IsDying = false;
                gameObject.transform.localScale = Vector3.zero;
                gameObject.GetComponentInParent<IBoard>().Remove(this);
                Destroy(gameObject);
            }
        }
        if (m_IsMoving)
        {
            m_Epsilon += Time.deltaTime;
            var t = Mathf.Clamp01(m_Epsilon / ANIMATION_TIME);
            var pos = gameObject.transform.localPosition = Vector3.Lerp(m_FromPosition, Vector3.zero, Mathf.SmoothStep(0, 1, t * t * (3 - 2 * t)));
            gameObject.transform.localPosition = new(pos.x, Mathf.Sin(Mathf.PI * t), pos.z);
            if (t >= 1)
            {
                m_IsMoving = false;
                gameObject.transform.localPosition = Vector3.zero;
            }
        }
        if (m_IsSelected)
            gameObject.transform.localEulerAngles += Vector3.down * 90f * Time.deltaTime;
    }

    public string GetPosString()
    {
        var tile = GetComponentInParent<Tile>();
        if (tile != null)
            return tile.GetPosString();
        return "";
    }
}
