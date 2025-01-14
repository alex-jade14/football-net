using Godot;
using System;

public partial class NetMesh : Mesh
{
	public NetMesh(MeshInstance3D meshInstance) : base(meshInstance){
		mesh = (ArrayMesh) meshInstance.Mesh;
		mesh2 = (ArrayMesh) meshInstance.GetParent<Node3D>().GetNode<MeshInstance3D>("HiddenNet").Get("mesh");
		mdt = new MeshDataTool();
		mdt2 = new MeshDataTool();
        mdt.CreateFromSurface(mesh, 0);
		mdt2.CreateFromSurface(mesh2, 0);
	}
}
