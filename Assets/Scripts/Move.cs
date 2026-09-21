using System;
using UnityEngine;

[Serializable]
public class Move
{
    protected ChessPiece self { get; }

    protected Tile tile { get; }

    public Move(ChessPiece self, Tile tile)
    {
        this.self = self;
        this.tile = tile;
    }

    public virtual void Apply(int turn) => self.MoveTo(tile, turn);

    public virtual void Show() => tile.StyleMove();

    public virtual void Hide() => tile.DefaultStyle();

    public virtual bool IsTile(Tile tile) => this.tile.position == tile.position;

    public ChessPiece GetSelf() => self;

    public Vector2Int GetMovePosition() => tile.position;

    public virtual SimulatedBoard SimulateApplied(ChessBoard board)
    {
        return new(board, this);
    }

    public virtual string ApplyMessage()
    {
        return "mov:" + self.GetPosString() + "->" + tile.GetPosString();
    }
}

[Serializable]
public class Capture : Move
{
    protected ChessPiece target { get; }

    public Capture(ChessPiece self, Tile tile, ChessPiece target) : base(self, tile)
    {
        this.target = target;
    }

    public override void Apply(int turn)
    {
        target.Die();
        self.MoveTo(tile, turn, ChessPiece.ANIMATION_TIME / 2);
    }

    public override void Show()
    {
        tile.StyleTake();
        target.StyleCapture();
    }

    public override void Hide()
    {
        base.Hide();
        target.DefaultStyle();
    }

    public virtual bool IsCaptureKing()
    {
        return target.type.pieceType == ChessPieceType.Type.King;
    }

    public virtual bool Captures(ChessPiece target)
    {
        return this.target.position == target.position;
    }

    public override string ApplyMessage()
    {
        return "cap:" + target.GetPosString() + "|" + base.ApplyMessage();
    }
}

[Serializable]
public class Castling : Move
{
    protected ChessPiece castle { get; }

    protected Tile castleTile { get; }
    
    public Castling(ChessPiece self, Tile tile, ChessPiece castle, Tile castleTile) : base(self, tile)
    {
        this.castle = castle;
        this.castleTile = castleTile;
    }

    public override void Apply(int turn)
    {
        self.MoveTo(tile, turn);
        castle.MoveTo(castleTile, turn, ChessPiece.ANIMATION_TIME / 2);
    }

    public override void Show()
    {
        tile.StyleSpecial();
        castle.StyleSpecial();
    }

    public override void Hide()
    {
        base.Hide();
        castle.DefaultStyle();
        castleTile.DefaultStyle();
    }

    public override bool IsTile(Tile tile) => this.tile.position == tile.position || castleTile.position == tile.position || castle.position == tile.position;

    public override string ApplyMessage()
    {
        return "cas-mov:" + castle.GetPosString() + "->" + castleTile.GetPosString() + "-" + self.GetPosString() + "->" + tile.GetPosString();
    }
}

public class EnPassant : Capture
{
    public EnPassant(ChessPiece self, Tile tile, ChessPiece pawn) : base(self, tile, pawn) {}

    public override bool IsTile(Tile tile) => this.tile.position == tile.position || tile.position == target.position;
}
