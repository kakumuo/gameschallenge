using Godot;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public partial class PlayerController : CharacterBody2D
{
	[Export]
	private bool isPlayer1 = false;
	[Export]
	private float moveSpeed = 300f; 
	private Dictionary<string, Key> _p1InputMap = new Dictionary<string, Key>();
	private Dictionary<string, Key> _p2InputMap = new Dictionary<string, Key>();
	private Dictionary<string, Key> _targetInputMap;
	private int _maxY = 10; 
	public override void _Ready()
	{
		GD.Print("Added to scene");
		this._p1InputMap.Add("up", Key.W);
		this._p1InputMap.Add("down", Key.S);

		this._p2InputMap.Add("up", Key.Up);
		this._p2InputMap.Add("down", Key.Down);

		this._targetInputMap = this.isPlayer1 ? this._p1InputMap : this._p2InputMap;
	}

	public override void _Process(double delta)
	{
		int dir = 0;

		if (Input.IsKeyPressed(_targetInputMap["up"])) dir += -1; 
		if (Input.IsKeyPressed(_targetInputMap["down"])) dir += 1;

		Vector2 curPos = Position;
		curPos.Y += dir * moveSpeed *  ((float) delta);
		curPos.Y = Math.Clamp(curPos.Y, 105, 550); 
		Position = curPos; 

		base._Process(delta);
	}
}
