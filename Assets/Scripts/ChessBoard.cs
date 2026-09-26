using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ChessBoard : MonoBehaviour, IBoard
{
    [Header("Board Tracked Stuff")]
    public Tile m_TilePrefab;

    public List<ChessPieceType> m_ChessPieceTypes;

    public WhiteChessPiece m_WhiteChessPiecePrefab;

    public BlackChessPiece m_BlackChessPiecePrefab;

    private List<WhiteChessPiece> m_WhiteChessPieces = new();

    private List<BlackChessPiece> m_BlackChessPieces = new();

    private Tile[] m_Tiles;

    private List<Move> m_PossibleMoves = new();

    private ChessPiece m_CurrentSelection = null;

    private int m_Turn = 0;

    private bool m_Checked = false;

    private ChessPiece m_CheckingPiece;

    [Header("Controls")]
    public InputActionReference m_PressActionRef;

    public InputActionReference m_PressPositionRef;

    public InputActionReference m_PressPositionDeltaRef;

    public InputActionReference m_QuitEventKey;

    [Header("Promotion")]
    public List<Button> m_PromotionButtons;

    private bool m_Promotion;

    [Header("Miscellanious")]
    private Vector3 m_LastRotation;

    private bool m_Selectable = true;

    private bool m_SwitchingSides;

    private float m_Epsilon;

    public TextMeshProUGUI m_CheckedText;

    void OnEnable()
    {
        m_PressActionRef.action.Enable();
        m_PressPositionRef.action.Enable();
        m_PressPositionDeltaRef.action.Enable();
        m_QuitEventKey.action.Enable();
    }

    void OnDisable()
    {
        m_PressActionRef.action.Disable();
        m_PressPositionRef.action.Disable();
        m_PressPositionDeltaRef.action.Disable();
        m_QuitEventKey.action.Disable();
    }

    /// <summary>
    /// Instantiates an 8x8 grd of tiles
    /// </summary>
    /// <returns>The array of 64 tiles instantiated</returns>
    Tile[] SpawnTiles()
    {
        Tile[] tiles = new Tile[64];
        for (int i = 0; i < 64; i++)
        {
            tiles[i] = Instantiate(m_TilePrefab, transform, false);
            tiles[i].position = new(i % 8, i / 8);
            tiles[i].DefaultStyle();
        }
        return tiles;
    }

    /// <summary>
    /// Creates the chess pieces in all the appropriate positions. Should be called after SpawnTiles
    /// </summary>
    void SpawnPieces()
    {
        if (m_WhiteChessPieces == null)
            m_WhiteChessPieces = new();
        m_WhiteChessPieces.Clear();
        if (m_BlackChessPieces == null)
            m_BlackChessPieces = new();
        m_BlackChessPieces.Clear();
        for (int i = 0; i < 16; i++)
        {
            if (i / 8 == 1)
            {
                var type = m_ChessPieceTypes.FirstOrDefault(pt => pt.pieceType == ChessPieceType.Type.Pawn);
                m_WhiteChessPieces.Add(Instantiate(m_WhiteChessPiecePrefab, m_Tiles[i].transform));
                m_WhiteChessPieces.Last().type = type;
                m_BlackChessPieces.Add(Instantiate(m_BlackChessPiecePrefab, m_Tiles[i + 40].transform));
                m_BlackChessPieces.Last().type = type;
            }
            else
            {
                int x = i % 8;
                switch (x)
                {
                    case 0:
                    case 7:
                        var type = m_ChessPieceTypes.Find(pt => pt.pieceType == ChessPieceType.Type.Rook);
                        m_WhiteChessPieces.Add(Instantiate(m_WhiteChessPiecePrefab, m_Tiles[i].transform));
                        m_WhiteChessPieces.Last().type = type;
                        m_BlackChessPieces.Add(Instantiate(m_BlackChessPiecePrefab, m_Tiles[i + 56].transform));
                        m_BlackChessPieces.Last().type = type;
                        break;
                    case 1:
                    case 6:
                        type = m_ChessPieceTypes.Find(pt => pt.pieceType == ChessPieceType.Type.Knight);
                        m_WhiteChessPieces.Add(Instantiate(m_WhiteChessPiecePrefab, m_Tiles[i].transform));
                        m_WhiteChessPieces.Last().type = type;
                        m_BlackChessPieces.Add(Instantiate(m_BlackChessPiecePrefab, m_Tiles[i + 56].transform));
                        m_BlackChessPieces.Last().type = type;
                        break;
                    case 2:
                    case 5:
                        type = m_ChessPieceTypes.Find(pt => pt.pieceType == ChessPieceType.Type.Bishop);
                        m_WhiteChessPieces.Add(Instantiate(m_WhiteChessPiecePrefab, m_Tiles[i].transform));
                        m_WhiteChessPieces.Last().type = type;
                        m_BlackChessPieces.Add(Instantiate(m_BlackChessPiecePrefab, m_Tiles[i + 56].transform));
                        m_BlackChessPieces.Last().type = type;
                        break;
                    case 3:
                        type = m_ChessPieceTypes.Find(pt => pt.pieceType == ChessPieceType.Type.Queen);
                        m_WhiteChessPieces.Add(Instantiate(m_WhiteChessPiecePrefab, m_Tiles[i].transform));
                        m_WhiteChessPieces.Last().type = type;
                        m_BlackChessPieces.Add(Instantiate(m_BlackChessPiecePrefab, m_Tiles[i + 56].transform));
                        m_BlackChessPieces.Last().type = type;
                        break;
                    case 4:
                        type = m_ChessPieceTypes.Find(pt => pt.pieceType == ChessPieceType.Type.King);
                        m_WhiteChessPieces.Add(Instantiate(m_WhiteChessPiecePrefab, m_Tiles[i].transform));
                        m_WhiteChessPieces.Last().type = type;
                        m_BlackChessPieces.Add(Instantiate(m_BlackChessPiecePrefab, m_Tiles[i + 56].transform));
                        m_BlackChessPieces.Last().type = type;
                        break;
                }
            }
        }
        m_WhiteChessPieces.ForEach(piece => piece.DefaultStyle());
        m_BlackChessPieces.ForEach(piece => piece.DefaultStyle());
    }

    /// <summary>
    /// Removes the chess piece from the list of chess pieces
    /// </summary>
    /// <param name="piece">The chess piece to remove</param>
    public void Remove(ChessPiece piece)
    {
        if (piece is WhiteChessPiece wp)
            m_WhiteChessPieces.Remove(wp);
        if (piece is BlackChessPiece bp)
            m_BlackChessPieces.Remove(bp);
    }

    /// <summary>
    /// Checks if there is a chess piece existing at the position
    /// </summary>
    /// <param name="position">A Vector2Int representing the position to check</param>
    /// <param name="piece">The piece at that position</param>
    /// <returns>true if a piece exists at the board position, false otherwise</returns>
    public bool DoesPieceExist(Vector2Int position, out ChessPiece piece)
    {
        piece = null;
        foreach (var cp in m_WhiteChessPieces)
        {
            if (cp.position == position)
            {
                piece = cp;
                return true;
            }
        }
        foreach (var cp in m_BlackChessPieces)
        {
            if (cp.position == position)
            {
                piece = cp;
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Checks if there is a chess piece existing at the position
    /// </summary>
    /// <param name="position">A Vector2Int representing the position to check</param>
    /// <returns>true if a piece exists at the board position, false otherwise</returns>m_CheckedText
    public bool DoesPieceExist(Vector2Int position)
    {
        foreach (var cp in m_WhiteChessPieces)
        {
            if (cp.position == position)
                return true;
        }
        foreach (var cp in m_BlackChessPieces)
        {
            if (cp.position == position)
                return true;
        }
        return false;
    }

    /// <summary>
    /// Gets the tile at a Vector2Int position
    /// </summary>
    /// <param name="position">The position of the tile</param>
    /// <returns>The tile at that position</returns>m_CheckedText
    public Tile TileAt(Vector2Int position)
    {
        if (position.x >= 0 && position.x < 8 && position.y >= 0 && position.y < 8)
            return m_Tiles[position.x + (position.y * 8)];

        return null;
    }

    /// <summary>
    /// Checks if the chess piece just moved
    /// </summary>
    /// <param name="chessPiece">The chess piece to check if has just moved</param>
    /// <returns></returns>
    public bool JustMoved(ChessPiece chessPiece) => chessPiece.turnMoved == m_Turn - 1;

    /// <summary>
    /// Selects the chess piece clicked on/at the board position and colors the possible moves.
    /// </summary>
    /// <param name="hitInfo">The Physics.RayCast out RaycastHit result</param>
    public void SelectChessPiece(RaycastHit hitInfo)
    {
        var tile = hitInfo.collider.GetComponentInParent<Tile>();
        if (tile != null && DoesPieceExist(tile.position, out ChessPiece piece))
        {
            if ((m_Turn % 2 == 0 && piece is WhiteChessPiece) || (m_Turn % 2 == 1 && piece is BlackChessPiece))
            {
                piece.StyleSelected();
                if (m_Checked)
                {
                    m_PossibleMoves = new();
                    var generatedMoves = piece.GenerateMoves(this);
                    foreach (var move in generatedMoves)
                    {
                        if (move is Capture capture && capture.Captures(m_CheckingPiece))
                            m_PossibleMoves.Add(move);
                        else if (move.SimulateApplied(this).DoesNotCheck(m_CheckingPiece))
                            m_PossibleMoves.Add(move);
                    }
                }
                else
                    m_PossibleMoves = piece.GenerateMoves(this);
                m_CurrentSelection = piece;
                m_PossibleMoves.ForEach(move => move.Show());
            }
        }
    }

    /// <summary>
    /// Moves the currently selected chess piece to clicked position and then deselects
    /// </summary>
    /// <param name="hitInfo">The Physics.RayCast out RaycastHit result</param>
    public void MoveAndDeselect(RaycastHit hitInfo)
    {
        var tile = hitInfo.collider.GetComponentInParent<Tile>();
        if (tile != null)
        {
            var m = m_PossibleMoves.Find(move => move.IsTile(tile));
            if (m != null)
            {
                m.Apply(m_Turn);
                m_Turn ++;
            }
        }
        Deselect();
    }

    /// <summary>
    /// Deselects the currently selected chess piece
    /// </summary>
    void Deselect()
    {
        m_Promotion = (m_CurrentSelection is WhiteChessPiece wp && wp.type.pieceType == ChessPieceType.Type.Pawn && wp.position.y == 7) ||
                        (m_CurrentSelection is BlackChessPiece bp && bp.type.pieceType == ChessPieceType.Type.Pawn && bp.position.y == 0);
        if (m_CurrentSelection != null && !m_Promotion)
        {
            if (JustMoved(m_CurrentSelection))
            {
                m_SwitchingSides = true;
                m_LastRotation = transform.localEulerAngles;
                m_Epsilon = 0;
                m_Checked = m_CurrentSelection.GenerateMoves(this).Find(move => move is Capture capture && capture.IsCaptureKing()) != null;
                m_CheckingPiece = m_Checked ? m_CurrentSelection : null;
                m_CheckedText.color = m_Turn % 2 == 1 ? Color.white : Color.black;
            }
            m_CheckedText.gameObject.SetActive(m_Checked);
            m_CurrentSelection.DefaultStyle();
            m_CurrentSelection = null;
            m_PossibleMoves.ForEach(move => move.Hide());
            m_PossibleMoves.Clear();
            m_PromotionButtons.ForEach(button => button.gameObject.SetActive(false));
        }
        if (m_Promotion)
            m_PromotionButtons.ForEach(button => button.gameObject.SetActive(true));
    }

#region Promotion Code

    public void PromoteToRook()
    {
        if (m_CurrentSelection != null)
        {
            m_CurrentSelection.type = m_ChessPieceTypes.Find(type => type.pieceType == ChessPieceType.Type.Rook);
            m_Promotion = false;
            Deselect();
        }
    }

    public void PromoteToKnight()
    {
        if (m_CurrentSelection != null)
        {
            m_CurrentSelection.type = m_ChessPieceTypes.Find(type => type.pieceType == ChessPieceType.Type.Knight);
            m_Promotion = false;
            Deselect();
        }
    }

    public void PromoteToBishop()
    {
        if (m_CurrentSelection != null)
        {
            m_CurrentSelection.type = m_ChessPieceTypes.Find(type => type.pieceType == ChessPieceType.Type.Bishop);
            m_Promotion = false;
            Deselect();
        }
    }

    public void PromoteToQueen()
    {
        if (m_CurrentSelection != null)
        {
            m_CurrentSelection.type = m_ChessPieceTypes.Find(type => type.pieceType == ChessPieceType.Type.Queen);
            m_Promotion = false;
            Deselect();
        }
    }

#endregion

    public void GameOver()
    {
        
    }

    void Start()
    {
        m_Tiles = SpawnTiles();
        SpawnPieces();
        m_PromotionButtons.ForEach(button => button.gameObject.SetActive(false));
    }

    void Update()
    {
        if (m_QuitEventKey.action.WasPressedThisFrame())
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
        if (!m_Promotion)
        {
            var pos = m_PressPositionRef.action.ReadValue<Vector2>();
            if (m_PressActionRef.action.WasReleasedThisFrame())
            {
                if (m_Selectable)
                {
                    var ray = Camera.main.ScreenPointToRay(pos);
                    if (Physics.Raycast(ray, out RaycastHit hit))
                    {
                        if (m_CurrentSelection == null)
                            SelectChessPiece(hit);
                        else
                            MoveAndDeselect(hit);
                    }
                    else
                        Deselect();
                }
                m_Selectable = true;
            }
            else if (m_PressActionRef.action.IsPressed())
            {
                float delta = m_PressPositionDeltaRef.action.ReadValue<Vector2>().x;
                gameObject.transform.localEulerAngles += Vector3.down * 0.1f * delta;
                if (m_Selectable)
                    m_Selectable = Mathf.Abs(delta) <= 1f;
            }
        }
        if (m_SwitchingSides)
        {
            m_Epsilon += Time.deltaTime;
            float t = Mathf.Clamp01(m_Epsilon / 2f);
            Camera.main.backgroundColor = Color.Lerp(m_Turn % 2 == 0 ? Color.black : Color.white, m_Turn % 2 == 0 ? Color.white : Color.black, t);
            transform.localEulerAngles = Vector3.Lerp(m_LastRotation, m_Turn % 2 == 0 ? Vector3.zero : Vector3.up * 180, t * t * (3 - 2 * t));
            if (t >= 1)
            {
                m_SwitchingSides = false;
                Camera.main.backgroundColor = m_Turn % 2 == 0 ? Color.white : Color.black;
                transform.localEulerAngles = m_Turn % 2 == 0 ? Vector3.zero : Vector3.up * 180;
            }
        }
    }
}
