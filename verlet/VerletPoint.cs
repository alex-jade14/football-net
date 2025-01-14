using Godot;
using System;

public partial class VerletPoint
{
	public float originalX;
	public float originalY;
	public float originalZ;
	public float x;
	public float y;
	public float z;
	public float oldX;
	public float oldY;
	public float oldZ;
	public bool isPinned;

	public Godot.Vector3 faceAverage;

	public VerletPoint(float originalX, float originalY, float originalZ,
	float x, float y, float z, float oldX, float oldY, float oldZ, bool isPinned, 
	Godot.Vector3 faceAverage){
		this.originalX = originalX;
		this.originalY = originalY;
		this.originalZ = originalZ;
		this.x = x;
		this.y = y;
		this.z = z;
		this.oldX = oldX;
		this.oldY = oldY;
		this.oldZ = oldZ;
		this.isPinned = isPinned;
		this.faceAverage = faceAverage;
	}
}
