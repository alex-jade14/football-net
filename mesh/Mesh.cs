using Godot;
using System;

public partial class Mesh
{
    public MeshDataTool mdt;
	public MeshDataTool mdt2;

	public ArrayMesh mesh;

	public ArrayMesh mesh2;

    public Mesh(MeshInstance3D meshInstance){
		mesh = (ArrayMesh) meshInstance.Mesh;
		mesh2 = (ArrayMesh) meshInstance.GetParent<Node3D>().GetNode<MeshInstance3D>("HiddenNet").Get("mesh");
		mdt = new MeshDataTool();
		mdt2 = new MeshDataTool();
        mdt.CreateFromSurface(mesh, 0);
		mdt2.CreateFromSurface(mesh2, 0);
	}


	public void UpdateMeshWithVerletPoints(VerletPoint[] points)
    {
        for (int i = 0; i < points.Length; i++)
        {
            VerletPoint point = points[i];
            Vector3 vectorPosition = new (point.x, point.y, point.z);
            
			mdt.SetVertex(i, vectorPosition);
			mdt.SetVertexTangent(i, mdt.GetVertexTangent(i));
        }

        mesh.ClearSurfaces();
        mdt.CommitToSurface(mesh);
    }

	public Godot.Vector3 GetVertexFacesAverage(int position){
		int[] vertexFaces = mdt.GetVertexFaces(position);
		Vector3 vertexFacesAverage = new(0,0,0);

		for (int i = 0; i < vertexFaces.Length; i++){
			vertexFacesAverage += mdt.GetFaceNormal(vertexFaces[i]);
		}

		return vertexFacesAverage;
	}
}