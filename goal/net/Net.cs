using Godot;
using System;

public partial class Net : MeshInstance3D
{
	public NetMesh netMesh;
    public NetVerlet netVerlet;

    public override void _Ready(){
        netMesh = new NetMesh(this);
        netVerlet = new NetVerlet(
            0.99f,
            netMesh,
            GetParent<Node3D>().GetNode<RigidBody3D>("Ball"),
            GetParent<Node3D>().GetNode<Area3D>("Area3D")
        );
    }

    public override void _PhysicsProcess(double delta){
        netVerlet.UpdatePoints();
        netVerlet.UpdateSticks();
        netMesh.UpdateMeshWithVerletPoints(netVerlet.points);
    }
}
