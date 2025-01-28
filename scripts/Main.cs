using Godot;

public partial class Main : Node
{
    public void _on_button_pressed()
    {
        GetNode<Button>("Button").Hide();
    }
    [Signal]
    public delegate void HitEventHandler(Variant _0);
}
