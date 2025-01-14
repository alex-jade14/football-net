using Godot;
using System;

public partial class new_script : MeshInstance3D
{
    private MeshDataTool mdt;

	private MeshDataTool mdt2;
    private (double originalX, double originalY, double originalZ, double x, double y, double z, double oldX, double oldY, double oldZ, bool pinned)[] points;
    private (int p0, int p1, double length)[] sticks;
    private double friction = 0.9;
   
    private ArrayMesh mesh;

	private ArrayMesh mesh2;


	private Camera3D camera;
 	private RigidBody3D ball;

	private double extendFromDirection = 0.4456f;
	private Vector3 initialVelocity = new Vector3(-15f, 3f, -5f);
	private Area3D area3D;

	private Plane plane = new Plane(-1, 0, 0, 0);
	private Plane plane2 = new Plane(0, 0, -1, 3.66f);
	private Plane plane3 = new Plane(0, 0, 1, 3.66f);
	private Plane plane4 = new Plane(0, 1, 0, 1.22f);

	private Plane plane5 = new Plane(1, 0, 0, 0);
	private Plane plane6 = new Plane(0, -1, 0, -1.22f);
	private Plane plane7 = new Plane(0, 0, -1, -3.66f);
	private Plane plane8 = new Plane(0, 0, 1, -3.66f);

	private bool reduceVelocity;
	private bool newVelocity;
	private Vector3 vectorNewVelocity;
	private Plane[] wallPlanes = new Plane[2];
	private bool ballCanChangeState;

	private bool canStopZMovementInAboveArea;


    public override void _Ready()
    {
        mesh = (ArrayMesh) this.Mesh;
		mesh2 = (ArrayMesh) GetParent<Node3D>().GetNode<MeshInstance3D>("MeshInstance3D3").Get("mesh");
		ball = GetParent<Node3D>().GetNode<RigidBody3D>("RigidBody3D");
		ballCanChangeState = (bool) ball.Get("can_change_state");
		camera = GetParent<Node3D>().GetNode<Camera3D>("Camera3D");
		area3D = GetParent<Node3D>().GetNode<Area3D>("Area3D");
        mdt = new MeshDataTool();
		mdt2 = new MeshDataTool();
        mdt.CreateFromSurface(mesh, 0);
		mdt2.CreateFromSurface(mesh2, 0);
        int vertexCount = mdt2.GetVertexCount();
        points = new (double, double, double, double, double, double, double, double, double, bool)[vertexCount];
        for (int i = 0; i < vertexCount; i++)
        {
            var vert = mdt.GetVertex(i);
			var vert2 = mdt2.GetVertex(i);
			
			points[i] = (
				vert2.X,
				vert2.Y,
				vert2.Z,
				vert.X,
				vert.Y,
				vert.Z,
				vert.X,
				vert.Y,
				vert.Z,
				mdt.GetVertexColor(i).R != 1
			);
        }
        int edgeCount = mdt2.GetEdgeCount();
        sticks = new (int, int, double)[edgeCount];
        for (int i = 0; i < edgeCount; i++)
        {
			Vector3 firstVertex = mdt.GetVertex(mdt.GetEdgeVertex(i, 0));
			Vector3 secondVertex = mdt.GetVertex(mdt.GetEdgeVertex(i, 1));
			
            sticks[i] = (mdt.GetEdgeVertex(i, 0), mdt.GetEdgeVertex(i, 1), distance(firstVertex, secondVertex));
        }
        
    }

    public override void _PhysicsProcess(double delta)
    {
		if(Engine.GetFramesPerSecond() >= 60 && !reduceVelocity){
			ball.ApplyCentralImpulse(initialVelocity);
			reduceVelocity = true;
		}
        UpdatePoints(delta);
        UpdateSticks();
        UpdateMesh();
		// var ball_position = ball.GlobalPosition;
		// var ball_rotation = ball.GlobalRotation;  
		// if(Input.IsActionPressed("ui_left")){
		// 	ball_position.X-= 0.1f;
		// }
		// else if(Input.IsActionPressed("ui_right")){
		// 	ball_position.X+= 0.1f;
		// }
		// if(Input.IsActionPressed("ui_up")){
		// 	ball_position.Z -= 0.1f;
		// }
		// else if(Input.IsActionPressed("ui_down")){
		// 	ball_position.Z += 0.1f;
		// }
		// if(Input.IsActionPressed("up")){
		// 	ball_position.Y += 0.1f;
		// }
		// else if(Input.IsActionPressed("down")){
		// 	ball_position.Y -= 0.1f;
		// }
		// if(Input.IsActionPressed("right")){
		// 	ball_position.Y += 0.1f;
		// }
		// else if(Input.IsActionPressed("left")){
		// 	ball_position.Y -= 0.1f;
		// }

		// ball.GlobalPosition = ball_position;
		// ball.GlobalRotation = ball_rotation;

		var camera_position = camera.GlobalPosition;
		var camera_rotation = camera.GlobalRotation;  
		if(Input.IsActionPressed("ui_left")){
			camera_position.X-= 0.1f;
		}
		else if(Input.IsActionPressed("ui_right")){
			camera_position.X+= 0.1f;
		}
		if(Input.IsActionPressed("ui_up")){
			camera_position.Z -= 0.1f;
		}
		else if(Input.IsActionPressed("ui_down")){
			camera_position.Z += 0.1f;
		}
		if(Input.IsActionPressed("up")){
			camera_position.Y += 0.1f;
		}
		else if(Input.IsActionPressed("down")){
			camera_position.Y -= 0.1f;
		}
		if(Input.IsActionPressed("right")){
			camera_rotation.Y += 0.1f;
		}
		else if(Input.IsActionPressed("left")){
			camera_rotation.Y -= 0.1f;
		}

		camera.GlobalPosition = camera_position;
		camera.GlobalRotation = camera_rotation;
    }

    private void UpdatePoints(double delta)
    {
        for (int i = 0; i < points.Length; i++)
        {
            var point = points[i];
            if (!point.pinned)
            {
                var vx = (point.x - point.oldX) * friction;
                var vy = (point.y - point.oldY) * friction;
                var vz = (point.z - point.oldZ) * friction;
                points[i].oldX = point.x;
                points[i].oldY = point.y;
                points[i].oldZ = point.z;

                points[i].x += vx;
                points[i].y += vy;
                points[i].z += vz;
				
				if(area3D.OverlapsBody(ball)){
					var pPosition = new Vector3((float) point.x, (float) point.y, (float) point.z);
					var enter = false;
					var pOriginalPosition = new Vector3((float) point.originalX, (float) point.originalY, (float) point.originalZ);
					if((bool) ball.Get("is_goal")){
						
						if(!enter && plane.IsPointOver(ball.GlobalPosition) && plane2.IsPointOver(ball.GlobalPosition) && plane.HasPoint(pOriginalPosition) && plane2.HasPoint(pOriginalPosition)){
							var ball_points = (Vector3[]) ball.Get("points");
							if(ball_points != null){
								for (int j = 0; j < ball_points.Length; j++){
									var aux_distance = new Vector3(0, pOriginalPosition.Y, 0).DistanceSquaredTo(new Vector3(0, ball_points[j].Y, 0));
									if(aux_distance <= 2.228f){
										enter = true;
										extendFromDirection = 1.228f;
										break;
									}
								}
							}
						}

						else if(!enter && plane4.IsPointOver(ball.GlobalPosition) && plane2.IsPointOver(ball.GlobalPosition) && plane4.HasPoint(pOriginalPosition) && plane2.HasPoint(pOriginalPosition)){
							var ball_points = (Vector3[]) ball.Get("points");
							if(ball_points != null){
								for (int j = 0; j < ball_points.Length; j++){
									var aux_distance = new Vector3(pOriginalPosition.X, 0, 0).DistanceSquaredTo(new Vector3(ball_points[j].X, 0, 0));
									if(aux_distance <= 2.228f){
										enter = true;
										extendFromDirection = 1.228f;
										break;
									}
								}
							}
						}

						else if(!enter && plane4.IsPointOver(ball.GlobalPosition) && plane.IsPointOver(ball.GlobalPosition) && plane4.HasPoint(pOriginalPosition) && plane.HasPoint(pOriginalPosition)){
							var ball_points = (Vector3[]) ball.Get("points");
							if(ball_points != null){
								for (int j = 0; j < ball_points.Length; j++){
									var aux_distance = new Vector3(0, 0, pOriginalPosition.Z).DistanceSquaredTo(new Vector3(0, 0, ball_points[j].Z));
									if(aux_distance <= 2.228f){
										enter = true;
										extendFromDirection = 1.228f;
										break;
									}
								}
							}
						}

						else if(!enter && plane.IsPointOver(ball.GlobalPosition) && plane3.IsPointOver(ball.GlobalPosition) && plane.HasPoint(pOriginalPosition) && plane3.HasPoint(pOriginalPosition)){
							var ball_points = (Vector3[]) ball.Get("points");
							if(ball_points != null){
								for (int j = 0; j < ball_points.Length; j++){
									var aux_distance = new Vector3(0, pOriginalPosition.Y, 0).DistanceSquaredTo(new Vector3(0, ball_points[j].Y, 0));
									if(aux_distance <= 2.228f){
										enter = true;
										extendFromDirection = 1.228f;
										break;
									}
								}
							}
						}

						else if(!enter && plane4.IsPointOver(ball.GlobalPosition) && plane3.IsPointOver(ball.GlobalPosition) && plane4.HasPoint(pOriginalPosition) && plane3.HasPoint(pOriginalPosition)){
							var ball_points = (Vector3[]) ball.Get("points");
							if(ball_points != null){
								for (int j = 0; j < ball_points.Length; j++){
									var aux_distance = new Vector3(pOriginalPosition.X, 0, 0).DistanceSquaredTo(new Vector3(ball_points[j].X, 0, 0));
									if(aux_distance <= 2.228f){
										enter = true;
										extendFromDirection = 1.228f;
										break;
									}
								}
							}
						}
						
						if(!enter && plane.IsPointOver(ball.GlobalPosition) && plane.HasPoint(pOriginalPosition)){
							var ball_points = (Vector3[]) ball.Get("points");
							if(ball_points != null){
								for (int j = 0; j < ball_points.Length; j++){
									var aux_distance = new Vector3(0, pOriginalPosition.Y, pOriginalPosition.Z).DistanceSquaredTo(new Vector3(0, ball_points[j].Y, ball_points[j].Z));
									if(aux_distance <= 0.4456f){
										enter = true;
										extendFromDirection = 0.4456f;
										break;
									}
								}
							}
						}
						else if(!enter && plane2.IsPointOver(ball.GlobalPosition) && plane2.HasPoint(pOriginalPosition)){
							var ball_points = (Vector3[]) ball.Get("points");
							if(ball_points != null){
								for (int j = 0; j < ball_points.Length; j++){
									var aux_distance = new Vector3(pOriginalPosition.X, pOriginalPosition.Y, 0).DistanceSquaredTo(new Vector3(ball_points[j].X, ball_points[j].Y, 0));
									if(aux_distance <= 0.4456f){
										enter = true;
										extendFromDirection = 0.4456f;
										break;
									}
								}
							}
						}
						else if(!enter && plane3.IsPointOver(ball.GlobalPosition) && plane3.HasPoint(pOriginalPosition)){
							var ball_points = (Vector3[]) ball.Get("points");
							if(ball_points != null){
								for (int j = 0; j < ball_points.Length; j++){
									var aux_distance = new Vector3(pOriginalPosition.X, pOriginalPosition.Y, 0).DistanceSquaredTo(new Vector3(ball_points[j].X, ball_points[j].Y, 0));
									if(aux_distance <= 0.4456f){
										enter = true;
										extendFromDirection = 0.4456f;
										break;
									}
								}
							}
						}
						else if(!enter && plane4.IsPointOver(ball.GlobalPosition) && plane4.HasPoint(pOriginalPosition)){
							var ball_points = (Vector3[]) ball.Get("points");
							if(ball_points != null){
								for (int j = 0; j < ball_points.Length; j++){
									var aux_distance = new Vector3(pOriginalPosition.X, 0, pOriginalPosition.Z).DistanceSquaredTo(new Vector3(ball_points[j].X, 0, ball_points[j].Z));
									if(aux_distance <= 0.4456f){
										enter = true;
										extendFromDirection = 0.4456f;
										break;
									}
								}
							}
						}
						
						if(enter){
							
							var vertexFaces = mdt.GetVertexFaces(i);
							Vector3 vertexFacesAverage = new Vector3(0,0,0);

							for (int j = 0; j < vertexFaces.Length; j++){
								vertexFacesAverage += mdt.GetFaceNormal(vertexFaces[j]);
							}
							var direction = (ball.GlobalPosition - pPosition).Normalized();
							var condition = direction.Dot(vertexFacesAverage.Normalized()) <= 0;
							

							if(condition){
								var hitpos = ball.GlobalPosition + direction * (float) extendFromDirection;
								points[i].x = hitpos.X;
								points[i].y = hitpos.Y;
								points[i].z = hitpos.Z;
								if(!newVelocity){
									var ballLinearVelocity = ball.LinearVelocity;
									var newXVelocity = (0.454f * ballLinearVelocity.X + 3.5 * vx)/(0.454f + 3.5);
									var newYVelocity = (0.454f * ballLinearVelocity.Y + 0.5f * vy)/(0.454f + 0.5f);
									var newZVelocity = (0.454f * (-ballLinearVelocity.Z) + 0.6f * vz)/(0.454f + 0.6f);
									if(ballLinearVelocity.Length() > 45){
										newXVelocity = (0.454f * ballLinearVelocity.X + 3.5 * vx)/(0.454f + 3.5);
										newYVelocity = (0.454f * ballLinearVelocity.Y + 0.5f * vy)/(0.454f + 0.5f);
										newZVelocity = (0.454f * (-ballLinearVelocity.Z) + 0.6f * vz)/(0.454f + 0.6f);
										if(ball.GlobalPosition.Y <= -0.5f){
											if(ballLinearVelocity.Y < 1){
												ballLinearVelocity.Y = 2f;
											}
											newZVelocity = (0.454f * (-ballLinearVelocity.Z) + 0.3f * vz)/(0.454f + 0.3f);
											newYVelocity = Math.Abs(ballLinearVelocity.Y) * 1.25f;
										}
									}
									else if(ballLinearVelocity.Length() <= 45 && ballLinearVelocity.Length() >= 35){
										newXVelocity = (0.454f * ballLinearVelocity.X + 1.5f * vx)/(0.454f + 1.5f);
										newYVelocity = (0.454f * Math.Abs(ballLinearVelocity.Y) + 0.01f * vy)/(0.454f + 0.01f);
										newZVelocity = (0.454f * (-ballLinearVelocity.Z) + 0.5f * vz)/(0.454f + 0.5f);
										if(ball.LinearVelocity.Y >= 5){
											newYVelocity = (0.454f * Math.Abs(ballLinearVelocity.Y) + 1 * vy)/(0.454f + 1);
										}
										if(ball.GlobalPosition.Y <= -0.5f){
											
											if(ballLinearVelocity.Y < 1){
												ballLinearVelocity.Y = 3f;
											}
											newZVelocity = (0.454f * (-ballLinearVelocity.Z) + 0.25f * vz)/(0.454f + 0.25f);
											newYVelocity = Math.Abs(ballLinearVelocity.Y) * 1.5f;
										}

									}
									else if(ballLinearVelocity.Length() < 35 && ballLinearVelocity.Length() >= 30){

										newXVelocity = (0.454f * ballLinearVelocity.X + 1.25 * vx)/(0.454f + 1.25);
										newYVelocity = Math.Abs((0.454f * (Math.Abs(ballLinearVelocity.Y) * 5f) + 2.5f)/(0.454f + 2.5f));
										newZVelocity = (0.454f * (-ballLinearVelocity.Z) + 0.25f * vz)/(0.454f + 0.25f);
										if(ball.GlobalPosition.Y <= -0.5f){
											if(ballLinearVelocity.Y < 1){
												ballLinearVelocity.Y = 3.5f;
											}								
											newZVelocity = (0.454f * (-ballLinearVelocity.Z) + 0.1f * vz)/(0.454f + 0.1f);
											newYVelocity = Math.Abs(ballLinearVelocity.Y) * 1.5f;
										}
									}
									else if(ballLinearVelocity.Length() < 30){
										newXVelocity = ballLinearVelocity.X * 0.25f;
										newYVelocity = Math.Abs(ballLinearVelocity.Y) * 0.75f;
										newZVelocity = -ballLinearVelocity.Z * 0.75f;
										if(ball.GlobalPosition.Y <= -0.5f){
											if(ballLinearVelocity.Y < 1){
												ballLinearVelocity.Y = 3.5f;
											}
											newYVelocity = Math.Abs(ballLinearVelocity.Y) * 1.5f;
											newZVelocity = -ballLinearVelocity.Z * 0.375f;
										}
										
									}
									
									vectorNewVelocity = new Vector3((float) newXVelocity, (float) newYVelocity, (float) newZVelocity);
									ball.LinearVelocity = vectorNewVelocity;
									newVelocity = true;
									if(plane3.IsPointOver(ball.GlobalPosition) || (plane.IsPointOver(ball.GlobalPosition) && plane3.IsPointOver(ball.GlobalPosition)) || (plane4.IsPointOver(ball.GlobalPosition) && plane3.IsPointOver(ball.GlobalPosition))){
										wallPlanes[0] = plane2;
									}
									else if(plane2.IsPointOver(ball.GlobalPosition) || (plane.IsPointOver(ball.GlobalPosition) && plane2.IsPointOver(ball.GlobalPosition)) || (plane4.IsPointOver(ball.GlobalPosition) && plane2.IsPointOver(ball.GlobalPosition))){
										wallPlanes[0] = plane3;
									}
									else{
										wallPlanes[0] = plane2;
										wallPlanes[1] = plane3;
									}
								}
								else{
									var collideWithWallPlane = false;
									if(wallPlanes != null){
										for(int j = 0; j < wallPlanes.Length; j++){
											if(wallPlanes[j].IsPointOver(ball.GlobalPosition)){
												collideWithWallPlane = true;
												extendFromDirection = 0.4456f;
												break;
											}
										}
										if(!collideWithWallPlane){
											var distanceToOriginal = pOriginalPosition.DistanceTo(pPosition);
											var vertexFacesAverageNormalized = vertexFacesAverage.Normalized();
											var xForce = -2.5f * distanceToOriginal;
											var xDecceleration = xForce / 0.454f;
											var ballLinearVelocity = ball.LinearVelocity;
											ballLinearVelocity.X -= (xDecceleration * vertexFacesAverageNormalized).X * (float) delta;
											var zForce = -1f * distanceToOriginal;
											var zDecceleration = zForce / 0.454f;
											ballLinearVelocity.Z -= (zDecceleration * vertexFacesAverageNormalized).Z * (float) delta;
											ball.LinearVelocity = ballLinearVelocity;
										}
										else{
											var distanceToOriginal = pOriginalPosition.DistanceTo(pPosition);
											var vertexFacesAverageNormalized = vertexFacesAverage.Normalized();
											var xForce = -15f * distanceToOriginal;
											var xDecceleration = xForce / 0.454f;
											var ballLinearVelocity = ball.LinearVelocity;
											var zForce = -15f * distanceToOriginal;
											if(ball.GlobalPosition.X <= -1){
												xForce = -2f * distanceToOriginal;
												zForce = -5f * distanceToOriginal;
											}
											ballLinearVelocity.X -= (xDecceleration * vertexFacesAverageNormalized).X * (float) delta;
											var zDecceleration = zForce / 0.454f;
											ballLinearVelocity.Z -= (zDecceleration * vertexFacesAverageNormalized).Z * (float) delta;
											ball.LinearVelocity = ballLinearVelocity;
										}
									}
								}
							}
						}
					}
					else{
					var collisionArea = (String) ball.Get("collision_area");
					if(collisionArea == "front_area"){
						if(plane5.IsPointOver(ball.GlobalPosition) && plane5.HasPoint(pOriginalPosition)){
							var ball_points = (Vector3[]) ball.Get("points");
							if(ball_points != null){
								for (int j = 0; j < ball_points.Length; j++){
									var aux_distance = new Vector3(0, pOriginalPosition.Y, pOriginalPosition.Z).DistanceSquaredTo(new Vector3(0, ball_points[j].Y, ball_points[j].Z));
									if(aux_distance <= 0.1114f){
										enter = true;
										break;
									}
								}
							}
						}
					}
					else if(collisionArea == "above_area"){
						if(plane6.IsPointOver(ball.GlobalPosition) && plane6.HasPoint(pOriginalPosition)){
							var ball_points = (Vector3[]) ball.Get("points");
							if(ball_points != null){
								for (int j = 0; j < ball_points.Length; j++){
									var aux_distance = new Vector3(pOriginalPosition.X, 0, pOriginalPosition.Z).DistanceSquaredTo(new Vector3(ball_points[j].X, 0, ball_points[j].Z));
									if(aux_distance <= 0.1114f){
										enter = true;
										break;
									}
								}
							}
						}
					}
					else if(collisionArea == "left_area"){
						if(plane7.IsPointOver(ball.GlobalPosition) && plane7.HasPoint(pOriginalPosition)){
							var ball_points = (Vector3[]) ball.Get("points");
							if(ball_points != null){
								for (int j = 0; j < ball_points.Length; j++){
									var aux_distance = new Vector3(pOriginalPosition.X, pOriginalPosition.Y, 0).DistanceSquaredTo(new Vector3(ball_points[j].X, ball_points[j].Y, 0));
									if(aux_distance <= 0.1114f){
										enter = true;
										break;
									}
								}
							}
						}
					}
					else if(collisionArea == "right_area"){
						if(plane8.IsPointOver(ball.GlobalPosition) && plane8.HasPoint(pOriginalPosition)){
							var ball_points = (Vector3[]) ball.Get("points");
							if(ball_points != null){
								for (int j = 0; j < ball_points.Length; j++){
									var aux_distance = new Vector3(pOriginalPosition.X, pOriginalPosition.Y, 0).DistanceSquaredTo(new Vector3(ball_points[j].X, ball_points[j].Y, 0));
									if(aux_distance <= 0.1114f){
										enter = true;
										break;
									}
								}
							}
						}
					}

					if(enter){
						var vertexFaces = mdt.GetVertexFaces(i);
						Vector3 vertexFacesAverage = new Vector3(0,0,0);

						for (int j = 0; j < vertexFaces.Length; j++){
							vertexFacesAverage += mdt.GetFaceNormal(vertexFaces[j]);
						}
						var direction = (ball.GlobalPosition - pPosition).Normalized();
						var condition = direction.Dot(vertexFacesAverage.Normalized()) >= 0;

						if(condition){
							var hitpos = ball.GlobalPosition + direction * 0.4456f;
							points[i].x = hitpos.X;
							points[i].y = hitpos.Y;
							points[i].z = hitpos.Z;
							if(!newVelocity){
								var ballLinearVelocity = ball.LinearVelocity;
								if(collisionArea == "above_area"){
									var force = -0.25f * pOriginalPosition.DistanceTo(pPosition);
									var decceleration = force/0.454f;
									ballLinearVelocity.Y -= decceleration;
									ball.LinearVelocity = ballLinearVelocity;
								}
								else{
									ballLinearVelocity.X = ((0.454f - 1f)/(0.454f + 1f)) * ballLinearVelocity.Y;
									ballLinearVelocity.Y = ((0.454f - 0.5f)/(0.454f + 0.5f)) * ballLinearVelocity.Y;
									ballLinearVelocity.Z = ((0.454f - 0.5f)/(0.454f + 0.5f)) * ballLinearVelocity.Z;
									ball.LinearVelocity = ballLinearVelocity;
									newVelocity = true;
								}
								
							}
							else{
								var distanceToOriginal = pOriginalPosition.DistanceTo(pPosition);
								var vertexFacesAverageNormalized = vertexFacesAverage.Normalized();
								var xForce = -15f * distanceToOriginal;
								var xDecceleration = xForce / 0.454f;
								var ballLinearVelocity = ball.LinearVelocity;
								ballLinearVelocity.X += (xDecceleration * vertexFacesAverageNormalized).X * (float) delta;
								var zForce = -15f * distanceToOriginal;
								var zDecceleration = zForce / 0.454f;
								ballLinearVelocity.Z += (zDecceleration * vertexFacesAverageNormalized).Z * (float) delta;
								ball.LinearVelocity = ballLinearVelocity;
							}
						}

					}
				}
				}
				
            }
        }
    }

    private void UpdateSticks()
    {
        for (int i = 0; i < sticks.Length; i++)
        {
            var stick = sticks[i];
            var p0 = points[stick.p0];
            var p1 = points[stick.p1];

            var dx = p1.x - p0.x;
            var dy = p1.y - p0.y;
            var dz = p1.z - p0.z;
            var distance = Math.Sqrt(dx * dx + dy * dy + dz * dz);
            var difference = stick.length - distance;
            var percent = difference / distance / 2;
            var offsetX = dx * percent;
            var offsetY = dy * percent;
            var offsetZ = dz * percent;
            if (!p0.pinned)
            {
                points[stick.p0].x -= offsetX;
                points[stick.p0].y -= offsetY;
                points[stick.p0].z -= offsetZ;
            }

            if (!p1.pinned)
            {
                points[stick.p1].x += offsetX;
                points[stick.p1].y += offsetY;
                points[stick.p1].z += offsetZ;
            }
        }
    }

    private void UpdateMesh()
    {
        for (int i = 0; i < points.Length; i++)
        {
            var point = points[i];
            var vectorPosition = new Vector3((float) point.x, (float) point.y, (float) point.z);
            
			mdt.SetVertex(i, vectorPosition);
			mdt.SetVertexTangent(i, mdt.GetVertexTangent(i));
			// meshInvertFace.mdt.setVertex(i, vector_position);
			// meshInvertFace.mdt.SetVertexTangent(i, meshInvertFace.mdt.GetVertexTangent(i));
        }

        mesh.ClearSurfaces();
        mdt.CommitToSurface(mesh);
		// meshInvertFace.updateMesh();
    }

    private double distance(Vector3 p0, Vector3 p1)
    {
        var dx = p1.X - p0.X;
        var dy = p1.Y - p0.Y;
        var dz = p1.Z - p0.Z;
        return Math.Sqrt(dx * dx + dy * dy + dz * dz);
    }
}
