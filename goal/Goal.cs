using Godot;
using System;

public partial class Goal : Node3D
{
	public bool reduceVelocity = false;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		if(Engine.GetFramesPerSecond() >= 60 && !reduceVelocity){
			GetNode<RigidBody3D>("Ball").ApplyCentralImpulse(new Vector3(-15f, 3f, -5.3f));
			reduceVelocity = true;
		}
	}
}
