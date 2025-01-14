using Godot;
using System;

public partial class PhysicsMotionHelper
{
	public static float delta = (float) 1/Engine.PhysicsTicksPerSecond;
	public static float IntegrateAcceleration(float acceleration){
		return acceleration * delta;
	}
}