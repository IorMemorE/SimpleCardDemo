using Godot;
using System;
namespace CardTest.scripts;
[GlobalClass]
public partial class CardSlot : Node2D,IHasCollisionMask
{
    public CardBase Card;

    public static uint CollisionMask() => 2;

    public bool HasCardInSlot()
    {
        return Card != null;
    }
}
