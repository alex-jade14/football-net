using Godot;
using System;

public partial class VerletStick
{
	public int p0;
	public int p1;
	public float length;

	public VerletStick(int p0, int p1, float length){
		this.p0 = p0;
		this.p1 = p1;
		this.length = length;
	}
}
