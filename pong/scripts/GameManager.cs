using Godot;
using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters;

public partial class GameManager : Node2D
{

    int p1Points, p2Points;
    Label p1Label, p2Label;
    Area2D p1Goal, p2Goal;
    Marker2D ballResetPoint;
    BallBehavior ballObj;
    public override void _Ready()
    {
        this.p1Label = this.GetNode<Label>("p1Score");
        this.p2Label = this.GetNode<Label>("p2Score");

        this.p1Goal = this.GetNode<Area2D>("goalp1");
        this.p2Goal = this.GetNode<Area2D>("goalp2");

        this.ballResetPoint = this.GetNode<Marker2D>("ballResetPoint");
        this.ballObj = this.GetNode<CharacterBody2D>("ball") as BallBehavior;

        this.p1Goal.BodyEntered += createHandleBodyEnter(isP1: true);
        this.p2Goal.BodyEntered += createHandleBodyEnter(isP1: false); 

        this.resetGame();
    }

    private Area2D.BodyEnteredEventHandler createHandleBodyEnter(bool isP1) {
        return (Node2D body) =>
        {
            if (isP1) p1Points += 1;
            else p2Points += 1;
            resetGame();
        }; 
    }

    private void resetGame()
    {
        this.p1Label.Text = p1Points.ToString();
        this.p2Label.Text = p2Points.ToString();
        this.ballObj.Position = this.ballResetPoint.Position;
        this.ballObj.LaunchBall();
    }
}