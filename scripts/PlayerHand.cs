using Godot;
using System;
using System.Diagnostics;
using System.Linq;
using Godot.Collections;
namespace CardTest.scripts;

[GlobalClass]
public partial class PlayerHand : Node2D
{
    const string CardScenePath = "res://scenes/card_base.tscn";
    private PackedScene _cardScene = (PackedScene)ResourceLoader.Load(CardScenePath);
    private const int DefaultHandCnt = 2;
    private Array<CardBase> _cardInHand = [];
    private float _centerScreenX;

    private const int CardWidth = 200;
    private const int HandY = 600;

    public override void _Ready()
    {
        _centerScreenX = GetViewportRect().Size.X / 2;
        for (var i = 0; i < DefaultHandCnt; ++i)
        {
            var card = _cardScene.Instantiate() as CardBase;
            Debug.Assert(null != card);
            card.Name = $"Card_{i}";
            AddToHand(card);
            GetNode<CardManager>("../CardManager").AddChild(card);
        }
    }

    private void AddToHand(CardBase card)
    {
        _cardInHand.Add(card);
        UpdateHandPositions();
    }

    private void UpdateHandPositions()
    {
        for (var i = 0; i < _cardInHand.Count; ++i)
        {
            var card = _cardInHand[i];
            var newPosition = CalculateCardPosition(i);
            card.Position = newPosition;
        }
    }
    
    private Vector2 CalculateCardPosition(int i)
    {
        var handSize = _cardInHand.Count;
        var totalWidth = (handSize - 1) * CardWidth;
        var xOffset = _centerScreenX + i * CardWidth - totalWidth / 2.0f;
        return new Vector2(xOffset,HandY);
    }
}
