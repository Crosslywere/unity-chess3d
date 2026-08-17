using UnityEngine;

public class BlackChessPiece : ChessPiece
{
    public override void DefaultStyle()
    {
        meshRenderer.material.color = new(0.274509804f, 0.274509804f, 0.274509804f);
        selected = false;
    }

    public override void StyleCapture() => meshRenderer.material.color = Color.red;

    public override void StyleSpecial() => meshRenderer.material.color = Color.blue;

    public override void StyleSelected()
    {
        meshRenderer.material.color = Color.rebeccaPurple;
        selected = true;
    }
}