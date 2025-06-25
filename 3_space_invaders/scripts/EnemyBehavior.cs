using Godot;
using System;

public partial class EnemyBehavior : Node
{

    public void Kill()
    {
        EmitSignal(SignalName.OnEnemyKilled); 
        Free();
    }

    [Signal] public delegate void OnEnemyKilledEventHandler(); 
}
