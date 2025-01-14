using Godot;
using System;
using System.Data;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;

public partial class NetCollision
{
	public RigidBody3D ball;
	public float ballMass = 0;
	public float ballRadius = 0;
	public Area3D actionArea;
	public Godot.Plane plane = new (-1, 0, 0, 0);
	public Godot.Plane plane2 = new (0, 0, -1, 3.66f);
	public Godot.Plane plane3 = new (0, 0, 1, 3.66f);
	public Godot.Plane plane4 = new (0, 1, 0, 1.22f);
	public Godot.Plane plane5 = new (1, 0, 0, 0);
	public Godot.Plane plane6 = new (0, -1, 0, -1.22f);
	public Godot.Plane plane7 = new (0, 0, -1, -3.66f);
	public Godot.Plane plane8 = new (0, 0, 1, -3.66f);

	public Godot.Plane[] lateralPlanes = new Godot.Plane[2];
	
	public bool isColliding = false;

	public bool isNeededANewVelocity;

	public Godot.Vector3 vectorNewVelocity;


	public NetCollision( RigidBody3D ball, Area3D actionArea){
		this.ball = ball;
		ballMass = (float) ball.Get("mass");
		SphereShape3D sphereShape = (SphereShape3D) ball.GetNode<CollisionShape3D>("CollisionShape3D").Get("shape");
		ballRadius = (float) sphereShape.Get("radius");
		this.actionArea = actionArea;
	}

	public void ResolveCollision(VerletPoint point){
		if(actionArea.OverlapsBody(ball)){
			Godot.Vector3 pPosition = new (point.x, point.y, point.z);
			Godot.Vector3 pOriginalPosition = new (point.originalX, point.originalY, point.originalZ);
			bool isGoal = (bool) ball.Get("is_goal");
			
			if(isGoal){
				CollisionDetectionWhenIsGoal(pOriginalPosition);
				if(isColliding){
					CollisionResponseWhenIsGoal(point, pPosition, pOriginalPosition);
				}
			}
			else{
				String collisionArea = (String) ball.Get("collision_area");
				CollisionDetectionWhenIsNotGoal(pOriginalPosition, collisionArea);
				if(isColliding){
					CollisionResponseWhenIsNotGoal(point, pPosition, pOriginalPosition, collisionArea);
				}
			}
		}
	}


	public void CollisionDetectionWhenIsGoal(Godot.Vector3 pOriginalPosition){
		isColliding = false;

		// Diagonal planes conditions
		if(!isColliding){
			if(plane.IsPointOver(ball.GlobalPosition) && plane2.IsPointOver(ball.GlobalPosition) && plane.HasPoint(pOriginalPosition) && plane2.HasPoint(pOriginalPosition)){
				Godot.Vector3 originalPositionToCompare = new(0, pOriginalPosition.Y, 0);
				Godot.Vector3 ballPositionToCompare = new(0, 1, 0);
				CompareOriginalVerletPointDistanceToBallPosition(originalPositionToCompare, ballPositionToCompare);
			}
			else if(plane4.IsPointOver(ball.GlobalPosition) && plane2.IsPointOver(ball.GlobalPosition) && plane4.HasPoint(pOriginalPosition) && plane2.HasPoint(pOriginalPosition)){
				Godot.Vector3 originalPositionToCompare = new(pOriginalPosition.X, 0, 0);
				Godot.Vector3 ballPositionToCompare = new(1, 0, 0);
				CompareOriginalVerletPointDistanceToBallPosition(originalPositionToCompare, ballPositionToCompare);
			}

			else if(plane4.IsPointOver(ball.GlobalPosition) && plane.IsPointOver(ball.GlobalPosition) && plane4.HasPoint(pOriginalPosition) && plane.HasPoint(pOriginalPosition)){
				Godot.Vector3 originalPositionToCompare = new(0, 0, pOriginalPosition.Z);
				Godot.Vector3 ballPositionToCompare = new(0, 0, 1);
				CompareOriginalVerletPointDistanceToBallPosition(originalPositionToCompare, ballPositionToCompare);
			}

			else if(plane.IsPointOver(ball.GlobalPosition) && plane3.IsPointOver(ball.GlobalPosition) && plane.HasPoint(pOriginalPosition) && plane3.HasPoint(pOriginalPosition)){
				Godot.Vector3 originalPositionToCompare = new(0, pOriginalPosition.Y, 0);
				Godot.Vector3 ballPositionToCompare = new(0, 1, 0);
				CompareOriginalVerletPointDistanceToBallPosition(originalPositionToCompare, ballPositionToCompare);
			}

			else if(plane4.IsPointOver(ball.GlobalPosition) && plane3.IsPointOver(ball.GlobalPosition) && plane4.HasPoint(pOriginalPosition) && plane3.HasPoint(pOriginalPosition)){
				Godot.Vector3 originalPositionToCompare = new(pOriginalPosition.X, 0, 0);
				Godot.Vector3 ballPositionToCompare = new(1, 0, 0);
				CompareOriginalVerletPointDistanceToBallPosition(originalPositionToCompare, ballPositionToCompare);
			}


			// Perpendicular planes conditions

			
			if(plane.IsPointOver(ball.GlobalPosition) && plane.HasPoint(pOriginalPosition)){
				Godot.Vector3 originalPositionToCompare = new(0, pOriginalPosition.Y, pOriginalPosition.Z);
				Godot.Vector3 ballPositionToCompare = new(0, 1, 1);
				CompareOriginalVerletPointDistanceToBallPosition(originalPositionToCompare, ballPositionToCompare);
			}
			else if(plane2.IsPointOver(ball.GlobalPosition) && plane2.HasPoint(pOriginalPosition)){
				Godot.Vector3 originalPositionToCompare = new(pOriginalPosition.X, pOriginalPosition.Y, 0);
				Godot.Vector3 ballPositionToCompare = new(1, 1, 0);
				CompareOriginalVerletPointDistanceToBallPosition(originalPositionToCompare, ballPositionToCompare);
			}
			else if(plane3.IsPointOver(ball.GlobalPosition) && plane3.HasPoint(pOriginalPosition)){
				Godot.Vector3 originalPositionToCompare = new(pOriginalPosition.X, pOriginalPosition.Y, 0);
				Godot.Vector3 ballPositionToCompare = new(1, 1, 0);
				CompareOriginalVerletPointDistanceToBallPosition(originalPositionToCompare, ballPositionToCompare);
			}
			else if(plane4.IsPointOver(ball.GlobalPosition) && plane4.HasPoint(pOriginalPosition)){
				Godot.Vector3 originalPositionToCompare = new(pOriginalPosition.X, 0, pOriginalPosition.Z);
				Godot.Vector3 ballPositionToCompare = new(1, 0, 1);
				CompareOriginalVerletPointDistanceToBallPosition(originalPositionToCompare, ballPositionToCompare);
			}
		}
	}

	public void CompareOriginalVerletPointDistanceToBallPosition(Godot.Vector3 originalPositionToCompare, Godot.Vector3 ballPositionToCompare){
		Godot.Vector3[] ballPoints = (Godot.Vector3[]) ball.Get("points_from_swept_sphere");
		if(ballPoints != null){
			for (int i = 0; i < ballPoints.Length; i++){
				float originalPositionDistanceFromBall = originalPositionToCompare.DistanceSquaredTo(ballPositionToCompare * new Godot.Vector3(ballPoints[i].X, ballPoints[i].Y, ballPoints[i].Z));
				if(originalPositionDistanceFromBall <= ballRadius){
					isColliding = true;
					break;
				}
			}
		}
	}
	


	public void CollisionResponseWhenIsGoal(VerletPoint point, Godot.Vector3 pPosition,
	Godot.Vector3 pOriginalPosition){
		Godot.Vector3 vertexFacesAverage = point.faceAverage;
		Godot.Vector3 direction = pPosition.DirectionTo(ball.GlobalPosition);
		if(direction.Dot(vertexFacesAverage.Normalized()) <= 0){
			UpdatePointAfterCollision(point, direction);
			if(!isNeededANewVelocity){
				ApplyBallVelocityWhenCollidesWithNet();
				isNeededANewVelocity = true;
				if(plane3.IsPointOver(ball.GlobalPosition) || (plane.IsPointOver(ball.GlobalPosition) && plane3.IsPointOver(ball.GlobalPosition)) || (plane4.IsPointOver(ball.GlobalPosition) && plane3.IsPointOver(ball.GlobalPosition))){
					lateralPlanes[0] = plane2;
				}
				else if(plane2.IsPointOver(ball.GlobalPosition) || (plane.IsPointOver(ball.GlobalPosition) && plane2.IsPointOver(ball.GlobalPosition)) || (plane4.IsPointOver(ball.GlobalPosition) && plane2.IsPointOver(ball.GlobalPosition))){
					lateralPlanes[0] = plane3;
				}
				else{
					lateralPlanes[0] = plane2;
					lateralPlanes[1] = plane3;
				}
			}
			else{
				bool collideWithLateralPlane = false;
				if(lateralPlanes != null){
					for(int i = 0; i < lateralPlanes.Length; i++){
						if(lateralPlanes[i].IsPointOver(ball.GlobalPosition)){
							collideWithLateralPlane = true;
							break;
						}
					}
					if(!collideWithLateralPlane){
						ApplyBallDeccelerationWhenInteractsWithNet(2.5f, 1f, pOriginalPosition, pPosition, vertexFacesAverage);
					}
					else{
						ApplyBallDeccelerationWhenInteractsWithNet(15f, 15f, pOriginalPosition, pPosition, vertexFacesAverage);
					}
				}
			}
		}
	}

	public void ApplyBallVelocityWhenCollidesWithNet(){
		Godot.Vector3 ballLinearVelocity = ball.LinearVelocity;
		float ballLinearSpeed = ballLinearVelocity.Length();
		float verletPointLinearVelocity = 1f;
		float verletXPointMass = 0f;
		float verletYPointMass = 0f;
		float verletZPointMass = 0f;
		float valueToAdjustZVelocity = 0f;
		bool useMechanicHelperToAdjustZVelocity = true;
		
		// "Verlet point mass" values were chosen by me, there is no mathematical reason behind them.
		switch(ballLinearSpeed){
			// Ball linear velocity in this project is limited to 150 km/h (or 41.6667 m/s)
			case float speed when speed <= 41.6667 && speed >= 36.1111:
				verletXPointMass = 1.5f;
				verletYPointMass = 0.01f;
				verletZPointMass = 0.5f;
				valueToAdjustZVelocity = 0.25f;
				break;
			case float speed when speed < 36.1111 && speed >= 29.1667:
				verletXPointMass = 1.25f;
				verletYPointMass = 2.5f;
				verletZPointMass = 0.2f;
				valueToAdjustZVelocity = 0.1f;
				ballLinearVelocity.Y *= 5;
				break;
			case float speed when speed < 29.1667:
				verletXPointMass = 1f;
				verletYPointMass = 2f;
				verletZPointMass = 0.25f;
				valueToAdjustZVelocity = -ballLinearVelocity.Z * 0.375f;
				useMechanicHelperToAdjustZVelocity = false;
				break;
		}

		
		// I'm not sure if it should be calculated using the vector or by each individual component.
		float newXVelocity = MechanicHelper.CalculateFinalVelocityFromInelasticCollision(
			ballMass, 
			verletXPointMass,
			ballLinearVelocity.X,
			verletPointLinearVelocity
		);
		float newYVelocity = Math.Abs(MechanicHelper.CalculateFinalVelocityFromInelasticCollision(
			ballMass, 
			verletYPointMass,
			Math.Abs(ballLinearVelocity.Y),
			verletPointLinearVelocity
		));
		float newZVelocity = MechanicHelper.CalculateFinalVelocityFromInelasticCollision(
			ballMass, 
			verletZPointMass,
			-ballLinearVelocity.Z,
			verletPointLinearVelocity
		);

		if(ball.GlobalPosition.Y <= -0.5f){
			if(ballLinearVelocity.Y < 1){
				ballLinearVelocity.Y = 3.5f;
			}
			newYVelocity = Math.Abs(ballLinearVelocity.Y) * 1.5f;
			if (useMechanicHelperToAdjustZVelocity){
				verletZPointMass = valueToAdjustZVelocity;
					newZVelocity = MechanicHelper.CalculateFinalVelocityFromInelasticCollision(
					ballMass, 
					verletZPointMass,
					-ballLinearVelocity.Z,
					verletPointLinearVelocity
				);
			}
			else{
				newZVelocity = valueToAdjustZVelocity;
				newXVelocity = ballLinearVelocity.X * verletXPointMass;
			}
		}
		
		ball.LinearVelocity = new Godot.Vector3(newXVelocity, newYVelocity, newZVelocity);
	}


	public void ApplyBallDeccelerationWhenInteractsWithNet(float xElasticConstant, float zElasticConstant, 
	Godot.Vector3 pOriginalPosition, Godot.Vector3 pPosition, Godot.Vector3 vertexFacesAverage){
			float distanceToOriginal = pOriginalPosition.DistanceTo(pPosition);
			Godot.Vector3 vertexFacesAverageNormalized = vertexFacesAverage.Normalized();
			Godot.Vector3 ballLinearVelocity = ball.LinearVelocity;
			float xForce = MechanicHelper.CalculateForceFromHookesLaw(xElasticConstant, distanceToOriginal);
			float xDeceleration = NewtonsSecondLawHelper.CalculateAcceleration(xForce, ballMass);
			float xDecelerationWithDirection = xDeceleration * vertexFacesAverageNormalized.X;
			ballLinearVelocity.X -= PhysicsMotionHelper.IntegrateAcceleration(xDecelerationWithDirection);
			float zForce = MechanicHelper.CalculateForceFromHookesLaw(zElasticConstant, distanceToOriginal);
			float zDeceleration = NewtonsSecondLawHelper.CalculateAcceleration(zForce, ballMass);
			float zDecelerationWithDirection = zDeceleration * vertexFacesAverageNormalized.Z;
			ballLinearVelocity.Z -= PhysicsMotionHelper.IntegrateAcceleration(zDecelerationWithDirection);
			ball.LinearVelocity = ballLinearVelocity;
	}

	public void UpdatePointAfterCollision(VerletPoint point, Godot.Vector3 direction){
		Godot.Vector3 hitpos = ball.GlobalPosition + direction * ballRadius;
		point.x = hitpos.X;
		point.y = hitpos.Y;
		point.z = hitpos.Z;
	}


	public void CollisionDetectionWhenIsNotGoal(Godot.Vector3 pOriginalPosition, String collisionArea){
		isColliding = false;
		if(collisionArea == "front_area"){
			if(plane5.IsPointOver(ball.GlobalPosition) && plane5.HasPoint(pOriginalPosition)){
				Godot.Vector3 originalPositionToCompare = new(0, pOriginalPosition.Y, pOriginalPosition.Z);
				Godot.Vector3 ballPositionToCompare = new(0, 1, 1);
				CompareOriginalVerletPointDistanceToBallPosition(originalPositionToCompare, ballPositionToCompare);
			}
		}
		else if(collisionArea == "above_area"){
			if(plane6.IsPointOver(ball.GlobalPosition) && plane6.HasPoint(pOriginalPosition)){
				Godot.Vector3 originalPositionToCompare = new(pOriginalPosition.X, 0, pOriginalPosition.Z);
				Godot.Vector3 ballPositionToCompare = new(1, 0, 1);
				CompareOriginalVerletPointDistanceToBallPosition(originalPositionToCompare, ballPositionToCompare);
			}
		}
		else if(collisionArea == "left_area"){
			if(plane7.IsPointOver(ball.GlobalPosition) && plane7.HasPoint(pOriginalPosition)){
				Godot.Vector3 originalPositionToCompare = new(pOriginalPosition.X, pOriginalPosition.Y, 0);
				Godot.Vector3 ballPositionToCompare = new(1, 1, 0);
				CompareOriginalVerletPointDistanceToBallPosition(originalPositionToCompare, ballPositionToCompare);
			}
		}
		else if(collisionArea == "right_area"){
			if(plane8.IsPointOver(ball.GlobalPosition) && plane8.HasPoint(pOriginalPosition)){
				Godot.Vector3 originalPositionToCompare = new(pOriginalPosition.X, pOriginalPosition.Y, 0);
				Godot.Vector3 ballPositionToCompare = new(1, 1, 0);
				CompareOriginalVerletPointDistanceToBallPosition(originalPositionToCompare, ballPositionToCompare);
			}
		}
	}

	public void CollisionResponseWhenIsNotGoal(VerletPoint point, Godot.Vector3 pPosition,
	Godot.Vector3 pOriginalPosition, String collisionArea){
		Godot.Vector3 vertexFacesAverage = point.faceAverage;
		Godot.Vector3 direction = pPosition.DirectionTo(ball.GlobalPosition);

		if(direction.Dot(vertexFacesAverage.Normalized()) >= 0){
			UpdatePointAfterCollision(point, direction);
			if(!isNeededANewVelocity){
				Godot.Vector3 ballLinearVelocity = ball.LinearVelocity;
				//Custom response when ball stays above the net
				if(collisionArea == "above_area"){
					float yForce = MechanicHelper.CalculateForceFromHookesLaw(5f, pOriginalPosition.DistanceTo(pPosition));
					float yDeceleration = NewtonsSecondLawHelper.CalculateAcceleration(yForce, ballMass);
					ballLinearVelocity.Y -= PhysicsMotionHelper.IntegrateAcceleration(yDeceleration);
					if (ballLinearVelocity.Z > 0.1){
						float zForce = MechanicHelper.CalculateForceFromHookesLaw(10f, pOriginalPosition.DistanceTo(pPosition));
						float zDeceleration = NewtonsSecondLawHelper.CalculateAcceleration(zForce, ballMass);
						ballLinearVelocity.Z += PhysicsMotionHelper.IntegrateAcceleration(zDeceleration);
					}
					else{
						ballLinearVelocity.Z = 0;
					}
					
					ball.LinearVelocity = ballLinearVelocity;
					
				}
				//Default response
				else{
					ballLinearVelocity.X = MechanicHelper.CalculateFinalVelocity1FromElasticCollision(ballMass, 1f, (
						ballLinearVelocity.X < 0 ? -ballLinearVelocity.X : ballLinearVelocity.X) * 0.5f, 0
					);
					ballLinearVelocity.Y = MechanicHelper.CalculateFinalVelocity1FromElasticCollision(ballMass, 0.5f, ballLinearVelocity.Y, 0);
					ballLinearVelocity.Z = MechanicHelper.CalculateFinalVelocity1FromElasticCollision(ballMass, 0.5f, ballLinearVelocity.Z * 3, 0);
					ball.LinearVelocity = ballLinearVelocity;
					isNeededANewVelocity = true;
				}
				
			}
			// else if ((bool) ball.Get("can_change_state")){
			// 	ApplyBallDeccelerationWhenInteractsWithNet(15f, 15f, pOriginalPosition, pPosition, vertexFacesAverage);
			// }
		}
	}
}
