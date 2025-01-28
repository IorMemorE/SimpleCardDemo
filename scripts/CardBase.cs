using Godot;
namespace CardTest.scripts;

[GlobalClass]
public partial class CardBase : Node2D,IHasCollisionMask
{
    public static uint CollisionMask() => 1;

    [Signal]
    public delegate void HoveredEventHandler();
    [Signal]
    public delegate void HoveredOffEventHandler();
    
    private bool _whenFront = true;
    private Sprite2D _frontFace, _backFace;
    public override void _Ready()
    {
        if (GetParent() is CardManager cm)
        {
            cm.DoConnectCardSignals(this);
        }
        
        
        _whenFront = true;
        _frontFace = GetNode<Sprite2D>("Showable/CardFront");
        _backFace = GetNode<Sprite2D>("Showable/CardBack");
    }
    
    public void DoFlip()
    {
        _whenFront = !_whenFront;
        _frontFace.Visible = _whenFront;
        _backFace.Visible = !_whenFront;
    }

    private void _OnMouseEnter()
    {
        EmitSignal("Hovered",this);
    } 

    private void _OnMouseExit()
    {
        EmitSignal("HoveredOff",this);
    }
}
