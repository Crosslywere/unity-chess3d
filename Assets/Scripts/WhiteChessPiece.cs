using UnityEngine;

public class WhiteChessPiece : ChessPiece
{
    public override void DefaultStyle()
    {
        meshRenderer.material.color = Color.beige;
        selected = false;
    }

    public override void StyleCapture() => meshRenderer.material.color = new(1, 0.25f, 0.25f);

    public override void StyleSpecial() => meshRenderer.material.color = Color.skyBlue;
    public override void StyleSelected()
    {
        meshRenderer.material.color = Color.purple;
        selected = true;
    }
}