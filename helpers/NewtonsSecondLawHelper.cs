using Godot;
using System;

public partial class NewtonsSecondLawHelper
{
	public static float CalculateAcceleration(float force, float mass){
		return force / mass;
	}
}
