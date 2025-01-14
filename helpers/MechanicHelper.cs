using Godot;
using System;

public partial class MechanicHelper
{
	public static float CalculateFinalVelocityFromInelasticCollision(float mass1, float mass2, float initialVelocity1, float initialVelocity2){
		return (mass1 * initialVelocity1 + mass2 * initialVelocity2) / (mass1 + mass2);
	}

	public static float CalculateFinalVelocity1FromElasticCollision(float mass1, float mass2, float initialVelocity1, float initialVelocity2){
		return (initialVelocity1 * (mass1 - mass2)  + 2 * mass2 * initialVelocity2) / (mass1 + mass2);
	}

	public static float CalculateForceFromHookesLaw(float k, float x){
		return -k * x;
	}
}
