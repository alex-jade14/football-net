extends RigidBody3D

var is_goal: bool
var points_from_swept_sphere: Array[Vector3]
var collision_area: String
var last_position: Vector3
var space: PhysicsDirectSpaceState3D
var parameters: PhysicsPointQueryParameters3D
var can_change_state: bool
var radius: float


func _ready() -> void:
	initialize_items()


func _physics_process(delta: float) -> void:
	if (not last_position == global_position):
		calculate_points_for_swept_sphere()
		last_position = global_position
	

func initialize_items() -> void:
	last_position = global_position
	can_change_state = true
	space = PhysicsServer3D.space_get_direct_state(get_world_3d().get_space())
	parameters = PhysicsPointQueryParameters3D.new()
	radius = get_node("CollisionShape3D").get_shape().get_radius()

# Topic: Continous Collision Detection (CDD)
func calculate_points_for_swept_sphere() -> void:
	points_from_swept_sphere = []
	if($"../Area3D".overlaps_body(self)):
		var displacement: float = global_position.distance_to(last_position)
		var spheres_between: int = ceil(displacement/radius)
		var direction: Vector3 = global_position.direction_to(last_position)
		var point_position: Vector3  = last_position
		for i in range(spheres_between):
			point_position = point_position + direction * radius
			points_from_swept_sphere.append(point_position)
			verify_goal(point_position)
		points_from_swept_sphere.reverse()
		point_position = last_position
		points_from_swept_sphere.append(last_position)
		verify_goal(point_position)
		for i in range(spheres_between):
			point_position = point_position - direction * radius
			points_from_swept_sphere.append(point_position)
			verify_goal(point_position)

func verify_goal(point: Vector3) -> void:
	if(can_change_state):
		parameters.position = point
		parameters.collide_with_areas = true
		parameters.collide_with_bodies = true
		parameters.collision_mask = 2
		var result := space.intersect_point(parameters)
		if(result):
			for i in range(result.size()):
				var name_from_collider = result[i].collider.get_name()
				match name_from_collider:
					"GoalDetector":
						is_goal = true
						can_change_state = false
						break
					"AboveDetector":
						collision_area = "above_area"
						can_change_state = false
						break
					"LeftDetector":
						collision_area = "left_area"
						can_change_state = false
						break
					"FrontDetector":
						collision_area = "front_area"
						can_change_state = false
						break
					"RightDetector":
						collision_area = "right_area"
						can_change_state = false
						break
