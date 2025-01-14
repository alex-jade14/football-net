using Godot;
using System;

public partial class NetVerlet : Verlet
{

	public NetCollision netCollision;

	public NetVerlet(float friction, NetMesh netMesh, RigidBody3D ball, Area3D actionArea) : base(friction, netMesh){
        this.netCollision = new(ball, actionArea);
	}


	public override void UpdatePoints(){
        for (int i = 0; i < points.Length; i++){
            VerletPoint point = points[i];
            if (!point.isPinned){
                float vx = (point.x - point.oldX) * friction;
                float vy = (point.y - point.oldY) * friction;
                float vz = (point.z - point.oldZ) * friction;
                points[i].oldX = point.x;
                points[i].oldY = point.y;
                points[i].oldZ = point.z;

                points[i].x += vx;
                points[i].y += vy;
                points[i].z += vz;
                
                netCollision.ResolveCollision(points[i]);
            }
		}
	}
}
