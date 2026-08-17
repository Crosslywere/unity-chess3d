using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Create Chess Piece Type")]
public class ChessPieceType : ScriptableObject
{
    public enum Type
    {
        Bishop,
        King,
        Knight,
        Pawn,
        Queen,
        Rook,
    }

    private delegate List<Move> MoveGenerator(ChessPiece self, IBoard board);

    [SerializeField, Tooltip("Access via (mesh) in code.")]
    private Mesh m_MeshInstance;

#pragma warning disable IDE1006 // Naming Styles
    public Mesh mesh => m_MeshInstance;
#pragma warning restore IDE1006 // Naming Styles

    [SerializeField]
    private Type m_PieceType;

#pragma warning disable IDE1006 // Naming Styles
    public Type pieceType => m_PieceType;
#pragma warning restore IDE1006 // Naming Styles

    private MoveGenerator[] m_MovesGenerators = { BishopMoves, KingMoves, KnightMoves, PawnMoves, QueenMoves, RookMoves };

    public List<Move> GenerateMoves(ChessPiece self, IBoard board)
    {
        return m_MovesGenerators[(int)m_PieceType](self, board);
    }

    static List<Move> PawnMoves(ChessPiece self, IBoard board)
    {
        var moveDir = self is WhiteChessPiece ? Vector2Int.up : Vector2Int.down;
        List<Move> moves = new();
        var position = self.position;
        ChessPiece target;
        if (!board.DoesPieceExist(position + moveDir))
        {
            moves.Add(new Move(self, board.TileAt(position + moveDir)));
            if (!self.moved && !board.DoesPieceExist(position + (moveDir * 2)))
            {
                moves.Add(new Move(self, board.TileAt(position + (moveDir * 2))));
            }
        }
        if (board.DoesPieceExist(position + moveDir + Vector2Int.right, out target))
        {
            if ((self is WhiteChessPiece && target is not WhiteChessPiece) || (self is BlackChessPiece && target is not BlackChessPiece))
                moves.Add(new Capture(self, board.TileAt(position + moveDir + Vector2Int.right), target));
        }
        if (board.DoesPieceExist(position + moveDir + Vector2Int.left, out target))
        {
            if ((self is WhiteChessPiece && target is not WhiteChessPiece) || (self is BlackChessPiece && target is not BlackChessPiece))
                moves.Add(new Capture(self, board.TileAt(position + moveDir + Vector2Int.left), target));
        }
        if (self is WhiteChessPiece && position.y == 4)
        {
            if (board.DoesPieceExist(position + Vector2Int.left, out target) && target is BlackChessPiece && target.type.pieceType == Type.Pawn && board.JustMoved(target))
            {
                moves.Add(new EnPassant(self, board.TileAt(position + Vector2Int.left + moveDir), target));
            }
            if (board.DoesPieceExist(position + Vector2Int.right, out target) && target is BlackChessPiece && target.type.pieceType == Type.Pawn && board.JustMoved(target))
            {
                moves.Add(new EnPassant(self, board.TileAt(position + Vector2Int.right + moveDir), target));
            }
        }
        if (self is BlackChessPiece && position.y == 3)
        {
            if (board.DoesPieceExist(position + Vector2Int.left, out target) && target is WhiteChessPiece && target.type.pieceType == Type.Pawn && board.JustMoved(target))
            {
                moves.Add(new EnPassant(self, board.TileAt(position + Vector2Int.left + moveDir), target));
            }
            if (board.DoesPieceExist(position + Vector2Int.right, out target) && target is WhiteChessPiece && target.type.pieceType == Type.Pawn && board.JustMoved(target))
            {
                moves.Add(new EnPassant(self, board.TileAt(position + Vector2Int.right + moveDir), target));
            }
        }
        return moves;
    }

    static List<Move> RookMoves(ChessPiece self, IBoard board)
    {
        List<Move> moves = new();
        Tile tile;
        for (int i = 1; i < 8; i++)
        {
            var position = self.position + Vector2Int.up * i;
            if ((tile = board.TileAt(position)) != null)
            {
                if (board.DoesPieceExist(position, out ChessPiece piece))
                {
                    if ((self is WhiteChessPiece && piece is not WhiteChessPiece) || (self is BlackChessPiece && piece is not BlackChessPiece))
                    {
                        moves.Add(new Capture(self, tile, piece));
                    }
                    break;
                }
                else
                {
                    moves.Add(new Move(self, tile));
                }
            }
        }
        for (int i = 1; i < 8; i++)
        {
            var position = self.position + Vector2Int.down * i;
            if ((tile = board.TileAt(position)) != null)
            {
                if (board.DoesPieceExist(position, out ChessPiece piece))
                {
                    if ((self is WhiteChessPiece && piece is not WhiteChessPiece) || (self is BlackChessPiece && piece is not BlackChessPiece))
                    {
                        moves.Add(new Capture(self, tile, piece));
                    }
                    break;
                }
                else
                {
                    moves.Add(new Move(self, tile));
                }
            }
        }
        for (int i = 1; i < 8; i++)
        {
            var position = self.position + Vector2Int.left * i;
            if ((tile = board.TileAt(position)) != null)
            {
                if (board.DoesPieceExist(position, out ChessPiece piece))
                {
                    if ((self is WhiteChessPiece && piece is not WhiteChessPiece) || (self is BlackChessPiece && piece is not BlackChessPiece))
                    {
                        moves.Add(new Capture(self, tile, piece));
                    }
                    break;
                }
                else
                {
                    moves.Add(new Move(self, tile));
                }
            }
        }
        for (int i = 1; i < 8; i++)
        {
            var position = self.position + Vector2Int.right * i;
            if ((tile = board.TileAt(position)) != null)
            {
                if (board.DoesPieceExist(position, out ChessPiece piece))
                {
                    if ((self is WhiteChessPiece && piece is not WhiteChessPiece) || (self is BlackChessPiece && piece is not BlackChessPiece))
                    {
                        moves.Add(new Capture(self, tile, piece));
                    }
                    break;
                }
                else
                {
                    moves.Add(new Move(self, tile));
                }
            }
        }
        return moves;
    }

    static List<Move> KnightMoves(ChessPiece self, IBoard board)
    {
        List<Move> moves = new();
        Vector2Int position = self.position;
        ChessPiece target;
        Tile tile;
        if ((tile = board.TileAt(position + Vector2Int.up + Vector2Int.up + Vector2Int.right)) != null)
        {
            if (board.DoesPieceExist(tile.position, out target))
            {
                if ((self is WhiteChessPiece && target is not WhiteChessPiece) || (self is BlackChessPiece && target is not BlackChessPiece))
                    moves.Add(new Capture(self, tile, target));
            }
            else
                moves.Add(new Move(self, tile));
        }
        if ((tile = board.TileAt(position + Vector2Int.up + Vector2Int.right + Vector2Int.right)) != null)
        {
            if (board.DoesPieceExist(tile.position, out target))
            {
                if ((self is WhiteChessPiece && target is not WhiteChessPiece) || (self is BlackChessPiece && target is not BlackChessPiece))
                    moves.Add(new Capture(self, tile, target));
            }
            else
                moves.Add(new Move(self, tile));
        }
        if ((tile = board.TileAt(position + Vector2Int.down + Vector2Int.right + Vector2Int.right)) != null)
        {
            if (board.DoesPieceExist(tile.position, out target))
            {
                if ((self is WhiteChessPiece && target is not WhiteChessPiece) || (self is BlackChessPiece && target is not BlackChessPiece))
                    moves.Add(new Capture(self, tile, target));
            }
            else
                moves.Add(new Move(self, tile));
        }
        if ((tile = board.TileAt(position + Vector2Int.down + Vector2Int.down + Vector2Int.right)) != null)
        {
            if (board.DoesPieceExist(tile.position, out target))
            {
                if ((self is WhiteChessPiece && target is not WhiteChessPiece) || (self is BlackChessPiece && target is not BlackChessPiece))
                    moves.Add(new Capture(self, tile, target));
            }
            else
                moves.Add(new Move(self, tile));
        }
        if ((tile = board.TileAt(position + Vector2Int.down + Vector2Int.down + Vector2Int.left)) != null)
        {
            if (board.DoesPieceExist(tile.position, out target))
            {
                if ((self is WhiteChessPiece && target is not WhiteChessPiece) || (self is BlackChessPiece && target is not BlackChessPiece))
                    moves.Add(new Capture(self, tile, target));
            }
            else
                moves.Add(new Move(self, tile));
        }
        if ((tile = board.TileAt(position + Vector2Int.down + Vector2Int.left + Vector2Int.left)) != null)
        {
            if (board.DoesPieceExist(tile.position, out target))
            {
                if ((self is WhiteChessPiece && target is not WhiteChessPiece) || (self is BlackChessPiece && target is not BlackChessPiece))
                    moves.Add(new Capture(self, tile, target));
            }
            else
                moves.Add(new Move(self, tile));
        }
        if ((tile = board.TileAt(position + Vector2Int.up + Vector2Int.left + Vector2Int.left)) != null)
        {
            if (board.DoesPieceExist(tile.position, out target))
            {
                if ((self is WhiteChessPiece && target is not WhiteChessPiece) || (self is BlackChessPiece && target is not BlackChessPiece))
                    moves.Add(new Capture(self, tile, target));
            }
            else
                moves.Add(new Move(self, tile));
        }
        if ((tile = board.TileAt(position + Vector2Int.up + Vector2Int.up + Vector2Int.left)) != null)
        {
            if (board.DoesPieceExist(tile.position, out target))
            {
                if ((self is WhiteChessPiece && target is not WhiteChessPiece) || (self is BlackChessPiece && target is not BlackChessPiece))
                    moves.Add(new Capture(self, tile, target));
            }
            else
                moves.Add(new Move(self, tile));
        }
        return moves;
    }

    static List<Move> BishopMoves(ChessPiece self, IBoard board)
    {
        List<Move> moves = new();
        Tile tile;
        for (int i = 1; i < 8; i++)
        {
            var position = self.position + (Vector2Int.up + Vector2Int.right) * i;
            if ((tile = board.TileAt(position)) != null)
            {
                if (board.DoesPieceExist(position, out ChessPiece piece))
                {
                    if ((self is WhiteChessPiece && piece is not WhiteChessPiece) || (self is BlackChessPiece && piece is not BlackChessPiece))
                    {
                        moves.Add(new Capture(self, tile, piece));
                    }
                    break;
                }
                else
                {
                    moves.Add(new Move(self, tile));
                }
            }
        }
        for (int i = 1; i < 8; i++)
        {
            var position = self.position + (Vector2Int.up + Vector2Int.left) * i;
            if ((tile = board.TileAt(position)) != null)
            {
                if (board.DoesPieceExist(position, out ChessPiece piece))
                {
                    if ((self is WhiteChessPiece && piece is not WhiteChessPiece) || (self is BlackChessPiece && piece is not BlackChessPiece))
                    {
                        moves.Add(new Capture(self, tile, piece));
                    }
                    break;
                }
                else
                {
                    moves.Add(new Move(self, tile));
                }
            }
        }
        for (int i = 1; i < 8; i++)
        {
            var position = self.position + (Vector2Int.down + Vector2Int.right) * i;
            if ((tile = board.TileAt(position)) != null)
            {
                if (board.DoesPieceExist(position, out ChessPiece piece))
                {
                    if ((self is WhiteChessPiece && piece is not WhiteChessPiece) || (self is BlackChessPiece && piece is not BlackChessPiece))
                    {
                        moves.Add(new Capture(self, tile, piece));
                    }
                    break;
                }
                else
                {
                    moves.Add(new Move(self, tile));
                }
            }
        }
        for (int i = 1; i < 8; i++)
        {
            var position = self.position + (Vector2Int.down + Vector2Int.left) * i;
            if ((tile = board.TileAt(position)) != null)
            {
                if (board.DoesPieceExist(position, out ChessPiece piece))
                {
                    if ((self is WhiteChessPiece && piece is not WhiteChessPiece) || (self is BlackChessPiece && piece is not BlackChessPiece))
                    {
                        moves.Add(new Capture(self, tile, piece));
                    }
                    break;
                }
                else
                {
                    moves.Add(new Move(self, tile));
                }
            }
        }
        return moves;
    }

    static List<Move> QueenMoves(ChessPiece self, IBoard board)
    {
        List<Move> moves = new();
        moves.AddRange(RookMoves(self, board));
        moves.AddRange(BishopMoves(self, board));
        return moves;
    }

    static List<Move> KingMoves(ChessPiece self, IBoard board)
    {
        List<Move> moves = new();
        Vector2Int position = self.position;
        ChessPiece target;
        Tile tile;
        if ((tile = board.TileAt(position + Vector2Int.up)) != null)
        {
            if (board.DoesPieceExist(tile.position, out target))
            {
                if ((self is WhiteChessPiece && target is not WhiteChessPiece) || (self is BlackChessPiece && target is not BlackChessPiece))
                    moves.Add(new Capture(self, tile, target));
            }
            else
                moves.Add(new Move(self, tile));
        }
        if ((tile = board.TileAt(position + Vector2Int.up + Vector2Int.right)) != null)
        {
            if (board.DoesPieceExist(tile.position, out target))
            {
                if ((self is WhiteChessPiece && target is not WhiteChessPiece) || (self is BlackChessPiece && target is not BlackChessPiece))
                    moves.Add(new Capture(self, tile, target));
            }
            else
                moves.Add(new Move(self, tile));
        }
        if ((tile = board.TileAt(position + Vector2Int.right)) != null)
        {
            if (board.DoesPieceExist(tile.position, out target))
            {
                if ((self is WhiteChessPiece && target is not WhiteChessPiece) || (self is BlackChessPiece && target is not BlackChessPiece))
                    moves.Add(new Capture(self, tile, target));
            }
            else
                moves.Add(new Move(self, tile));
        }
        if ((tile = board.TileAt(position + Vector2Int.down + Vector2Int.right)) != null)
        {
            if (board.DoesPieceExist(tile.position, out target))
            {
                if ((self is WhiteChessPiece && target is not WhiteChessPiece) || (self is BlackChessPiece && target is not BlackChessPiece))
                    moves.Add(new Capture(self, tile, target));
            }
            else
                moves.Add(new Move(self, tile));
        }
        if ((tile = board.TileAt(position + Vector2Int.down)) != null)
        {
            if (board.DoesPieceExist(tile.position, out target))
            {
                if ((self is WhiteChessPiece && target is not WhiteChessPiece) || (self is BlackChessPiece && target is not BlackChessPiece))
                    moves.Add(new Capture(self, tile, target));
            }
            else
                moves.Add(new Move(self, tile));
        }
        if ((tile = board.TileAt(position + Vector2Int.down + Vector2Int.left)) != null)
        {
            if (board.DoesPieceExist(tile.position, out target))
            {
                if ((self is WhiteChessPiece && target is not WhiteChessPiece) || (self is BlackChessPiece && target is not BlackChessPiece))
                    moves.Add(new Capture(self, tile, target));
            }
            else
                moves.Add(new Move(self, tile));
        }
        if ((tile = board.TileAt(position + Vector2Int.left)) != null)
        {
            if (board.DoesPieceExist(tile.position, out target))
            {
                if ((self is WhiteChessPiece && target is not WhiteChessPiece) || (self is BlackChessPiece && target is not BlackChessPiece))
                    moves.Add(new Capture(self, tile, target));
            }
            else
                moves.Add(new Move(self, tile));
        }
        if ((tile = board.TileAt(position + Vector2Int.up + Vector2Int.left)) != null)
        {
            if (board.DoesPieceExist(tile.position, out target))
            {
                if ((self is WhiteChessPiece && target is not WhiteChessPiece) || (self is BlackChessPiece && target is not BlackChessPiece))
                    moves.Add(new Capture(self, tile, target));
            }
            else
                moves.Add(new Move(self, tile));
        }
        if (!self.moved)
        {
            ChessPiece castle;
            if (!board.DoesPieceExist(new(3, position.y)) && !board.DoesPieceExist(new(2, position.y)) && !board.DoesPieceExist(new(1, position.y))
                && board.DoesPieceExist(new(0, position.y), out castle))
            {
                if (!castle.moved)
                    moves.Add(new Castling(self, board.TileAt(new(2, position.y)), castle, board.TileAt(new(3, position.y))));
            }
            if (!board.DoesPieceExist(new(5, position.y)) && !board.DoesPieceExist(new(6, position.y)) && board.DoesPieceExist(new(7, position.y), out castle))
            {
                if (!castle.moved)
                    moves.Add(new Castling(self, board.TileAt(new(6, position.y)), castle, board.TileAt(new(5, position.y))));
            }
        }
        return moves;
    }
}
