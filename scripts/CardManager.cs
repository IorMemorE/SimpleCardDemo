using System;
using System.Linq;
using Godot;
using Godot.Collections;

namespace CardTest.scripts;

[GlobalClass]
public partial class CardManager : Node2D
{
    private CardBase _cardBeingDragged;
    private Vector2 _screenSize;
    private bool _isHovering;
    public override void _Ready()
    {
        _screenSize = GetViewportRect().Size;        
    }
    public override void _Process(double delta)
    {
        if (_cardBeingDragged is null) {
            return;
        }
        
        var (mX,mY) = GetGlobalMousePosition();
        var (sX,sY) = _screenSize;
        _cardBeingDragged.Position = new Vector2(Math.Clamp(mX,0.0f,sX), Math.Clamp(mY,0.0f,sY));
    }
    public override void _Input(InputEvent @event)
    {
        var mEvent = @event as InputEventMouseButton;
        if (mEvent == null)
        {
            return;
        }
        
        switch (mEvent.ButtonIndex)
        {
            case MouseButton.Left:
                if (mEvent.Pressed) {
                    var card = RayCastCheckFor<CardBase>();
                    if (card != null) {
                        StartDrag(card);
                    }
                } else {
                    // GD.Print("EX");
                    EndDrag();                        
                }
                break;
            case MouseButton.Right:
                if (mEvent.Pressed)
                {
                    RayCastCheckFor<CardBase>()?.DoFlip();
                }
                break;
            case MouseButton.WheelUp:
                if (mEvent.Pressed)
                {
                    var card = RayCastCheckFor<CardBase>();
                    if (card is not null)
                    {
                        card.Rotation -= Single.Pi / 12.0f;
                    }
                }
                break;
            case MouseButton.WheelDown:
                if (mEvent.Pressed)
                {
                    var card = RayCastCheckFor<CardBase>();
                    if (card is not null)
                    {
                        card.Rotation += Single.Pi / 12.0f;
                    }
                }
                break;
            default:
                GD.Print($"{mEvent.ButtonIndex} todo!");
                break;
        }
    }

    private void StartDrag(CardBase card)
    {
        card.Scale = Vector2.One;
        _cardBeingDragged = card;
    }
    
    private void EndDrag()
    {
        if (_cardBeingDragged is null)
        {
            return;
        }
        _cardBeingDragged.Scale = Vector2.One * 1.1f;
        var slotFound = RayCastCheckFor<CardSlot>();
        // GD.Print(slotFound);
        if (slotFound is not null && !slotFound.HasCardInSlot())
        {
            _cardBeingDragged.Position = slotFound.Position;
            _cardBeingDragged.GetNode<CollisionShape2D>("Area2D/CollisionShape2D").Disabled = true;
            slotFound.Card = _cardBeingDragged;
        } 
        _cardBeingDragged = null;
    }
    
    public void DoConnectCardSignals(CardBase card)
    {
        card.Connect("Hovered", Callable.From(
            (CardBase c) => _OnHoveredOverCard(c)
        ));
        card.Connect("HoveredOff", Callable.From(
            (CardBase c) => _OnHoveredOffCard(c)
        ));
    }
    
    private void _OnHoveredOverCard(CardBase card)
    {
        if (_isHovering)
        {
            return;
        }
        _isHovering = true;
        HighlightCard(card, true);
    }
    
    private void _OnHoveredOffCard(CardBase card)
    {
        if (_cardBeingDragged is not null)
        {
            return;
        }
        HighlightCard(card, false);
        var newCardHovered = RayCastCheckFor<CardBase>();
        if (newCardHovered is not null)
        {
            HighlightCard(newCardHovered, true);
        }
        else
        {
            _isHovering = false;
        }
    }

    private static void HighlightCard(CardBase card,in bool hovered)
    {
        if (hovered)
        {
            card.Scale = Vector2.One * 1.1f;
            card.ZIndex = 2;
        }
        else
        {
            card.Scale = Vector2.One;
            card.ZIndex = 1;
        }
    }
    
    private T RayCastCheckFor<T>()
        where T : Node2D , IHasCollisionMask
    {
        var spaceState = GetWorld2D().DirectSpaceState;
        var parameters = new PhysicsPointQueryParameters2D();
        parameters.Position = GetGlobalMousePosition();
        parameters.CollideWithAreas = true;
        parameters.CollisionMask = T.CollisionMask();
        var result = spaceState.IntersectPoint(parameters);
        
        return HighestZIndexOf<T>(in result);
        // return result.Count > 0 
        //     ? result[^1]["collider"].As<Area2D>().GetParent<CardBase>() 
        //     : null;
    }
    private static T HighestZIndexOf<T>(in Array<Dictionary> items)
        where T : Node2D,IHasCollisionMask
    {
        if (items is null || items.Count == 0)
        {
            return null;
        }
        var c = items[0]["collider"].As<Area2D>()?.GetParent() as T;
        var z = c?.ZIndex ?? 0;
        foreach (var node in items.Skip(1))
        {
            var d = node["collider"].As<Area2D>()?.GetParent() as T;
            if (d is null || d.ZIndex <= z)
            {
                continue;
            }
            c = d;
            z = c.ZIndex;
        }
        
        return c;
    }
    
}
