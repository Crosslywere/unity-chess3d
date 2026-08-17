using UnityEngine;

public interface IBoard
{
    public void Remove(ChessPiece piece);

    public bool DoesPieceExist(Vector2Int position, out ChessPiece piece);

    public bool DoesPieceExist(Vector2Int position);

    public Tile TileAt(Vector2Int position);

    public bool JustMoved(ChessPiece chessPiece);
}