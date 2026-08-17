using System.Collections.Generic;
using UnityEngine;

public class SimulatedBoard : IBoard
{
    private ChessPiece m_PieceInstance;

    private Vector2Int m_SimulatedPosition;

    private IBoard m_OriginatingBoard;

    public SimulatedBoard(IBoard board, Move move)
    {
        m_OriginatingBoard = board;
        m_PieceInstance = move.GetSelf();
        m_SimulatedPosition = move.GetMovePosition();
    }

    public bool DoesPieceExist(Vector2Int position, out ChessPiece piece)
    {
        if (m_SimulatedPosition == position)
        {
            piece = m_PieceInstance;
            return true;
        }
        else if (position == m_PieceInstance.position)
        {
            piece = null;
            return false;
        }
        else
            return m_OriginatingBoard.DoesPieceExist(position, out piece);
    }

    public bool DoesPieceExist(Vector2Int position)
    {
        if (m_SimulatedPosition == position)
            return true;
        else if (position == m_PieceInstance.position)
            return false;
        else
            return m_OriginatingBoard.DoesPieceExist(position);
    }

    public bool JustMoved(ChessPiece chessPiece)
    {
        return m_OriginatingBoard.JustMoved(chessPiece);
    }

    public void Remove(ChessPiece piece)
    {
        throw new System.UnauthorizedAccessException();
    }

    public bool TestCheck(ChessPiece checkingPiece)
    {
        return checkingPiece.GenerateMoves(this).Find(move => move is Capture capture && capture.IsCaptureKing()) == null;
    }

    public Tile TileAt(Vector2Int position)
    {
        return m_OriginatingBoard.TileAt(position);
    }
}
