using Godot;
using System;

public partial class Verlet
{
	public float friction;
	public VerletPoint[] points;
	public VerletStick[] sticks;

    public Verlet(float friction, Mesh mesh){
        this.friction = friction;
        AddVerletPointsAndSticksFromMesh(mesh);
    }

    public void AddVerletPointsAndSticksFromMesh(Mesh mesh){
		int vertexCount = mesh.mdt2.GetVertexCount();
        points = new VerletPoint[vertexCount];
        for (int i = 0; i < vertexCount; i++)
        {
            Vector3 vert = mesh.mdt.GetVertex(i);
			Vector3 vert2 = mesh.mdt2.GetVertex(i);
			VerletPoint point = new (
				vert2.X,
				vert2.Y,
				vert2.Z,
				vert.X,
				vert.Y,
				vert.Z,
				vert.X,
				vert.Y,
				vert.Z,
				mesh.mdt.GetVertexColor(i).R != 1,
                mesh.GetVertexFacesAverage(i)
			);
			points[i] = point;
        }

        int edgeCount = mesh.mdt2.GetEdgeCount();
        sticks = new VerletStick[edgeCount];
        for (int i = 0; i < edgeCount; i++)
        {
			Vector3 firstVertex = mesh.mdt.GetVertex(mesh.mdt.GetEdgeVertex(i, 0));
			Vector3 secondVertex = mesh.mdt.GetVertex(mesh.mdt.GetEdgeVertex(i, 1));
			float[] differenceBetweenPoints = DifferenceBetweenPoints(firstVertex, secondVertex);
			float distanceBetweenPoints = DistanceBetweenPoints(differenceBetweenPoints[0], differenceBetweenPoints[1], differenceBetweenPoints[2]);
			VerletStick stick = new (
				mesh.mdt.GetEdgeVertex(i, 0),
				mesh.mdt.GetEdgeVertex(i, 1),
				distanceBetweenPoints
			);
			sticks[i] = stick;
        }
	}

	public virtual void UpdatePoints(){
        if (points.Length > 0){
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
                }
		    }
        }
	}

	public void UpdateSticks(){
        if (sticks.Length > 0){
            for (int i = 0; i < sticks.Length; i++){
                VerletStick stick = sticks[i];
                VerletPoint p0 = points[stick.p0];
                VerletPoint p1 = points[stick.p1];
                float[] differenceBetweenPoints = DifferenceBetweenPoints(
                    new Vector3(p0.x, p0.y, p0.z),
                    new Vector3(p1.x, p1.y, p1.z)
                );
                float dx = differenceBetweenPoints[0];
                float dy = differenceBetweenPoints[1];
                float dz = differenceBetweenPoints[2];
                float distance = DistanceBetweenPoints(dx, dy, dz);
                float difference = stick.length - distance;
                float percent = difference / distance / 2;
                float offsetX = dx * percent;
                float offsetY = dy * percent;
                float offsetZ = dz * percent;
                if (!p0.isPinned){
                    points[stick.p0].x -= offsetX;
                    points[stick.p0].y -= offsetY;
                    points[stick.p0].z -= offsetZ;
                }

                if (!p1.isPinned){
                    points[stick.p1].x += offsetX;
                    points[stick.p1].y += offsetY;
                    points[stick.p1].z += offsetZ;
                }
            }
        }
        
    }

    public float[] DifferenceBetweenPoints(Vector3 p0, Vector3 p1){
		float dx = p1.X - p0.X;
        float dy = p1.Y - p0.Y;
        float dz = p1.Z - p0.Z;
		return new float[] { dx, dy, dz };
	}


	public float DistanceBetweenPoints(float dx, float dy, float dz){
        return (float) Math.Sqrt(dx * dx + dy * dy + dz * dz);
    }
}
