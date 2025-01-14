extends RigidBody3D

var last_position: Vector3
var points: Array[Vector3]
var space: PhysicsDirectSpaceState3D
var is_goal: bool
var can_change_state: bool
var collision_area: String
var parameters 
func _ready():
	last_position = global_position
	can_change_state = true
	space = PhysicsServer3D.space_get_direct_state(get_world_3d().get_space())
	print(space)
	parameters = PhysicsPointQueryParameters3D.new()

func _physics_process(delta):
		if (not last_position == global_position):
			points = []
			if($"../Area3D".overlaps_body(self)):
				var displacement = global_position.distance_to(last_position)
				var spheres_between = ceil(displacement/0.1114)
				var direction = global_position.direction_to(last_position)
				var aux_position = last_position
				for i in range(spheres_between):
					aux_position = aux_position + direction * (0.1114)
					points.append(aux_position)
					verify_goal(aux_position)
					#point(aux_position)
				points.reverse()
				aux_position = last_position
				points.append(last_position)
				verify_goal(aux_position)
				#point(aux_position)
				for i in range(spheres_between):
					aux_position = aux_position - direction * (0.1114)
					points.append(aux_position)
					verify_goal(aux_position)
					#point(aux_position)
			last_position = global_position



func line(pos1: Vector3, pos2: Vector3, color = Color.WHITE_SMOKE, persist_ms = 0):
	var mesh_instance := MeshInstance3D.new()
	var immediate_mesh := ImmediateMesh.new()
	var material := ORMMaterial3D.new()

	mesh_instance.mesh = immediate_mesh
	mesh_instance.cast_shadow = GeometryInstance3D.SHADOW_CASTING_SETTING_OFF

	immediate_mesh.surface_begin(Mesh.PRIMITIVE_LINES, material)
	immediate_mesh.surface_add_vertex(pos1)
	immediate_mesh.surface_add_vertex(pos2)
	immediate_mesh.surface_end()

	material.shading_mode = BaseMaterial3D.SHADING_MODE_UNSHADED
	material.albedo_color = color

	return await final_cleanup(mesh_instance, persist_ms)

func cilynder(pos: Vector3, radius = 0.05, height = 0.5, color = Color.WHITE_SMOKE, persist_ms = 0):
	var mesh_instance := MeshInstance3D.new()
	var cylinder_mesh := CylinderMesh.new()
	var material := ORMMaterial3D.new()

	mesh_instance.mesh = cylinder_mesh
	mesh_instance.cast_shadow = GeometryInstance3D.SHADOW_CASTING_SETTING_OFF
	mesh_instance.position = pos
	mesh_instance.rotation.z = deg_to_rad(90)

	cylinder_mesh.height = height
	cylinder_mesh.bottom_radius = radius
	cylinder_mesh.top_radius = radius
	cylinder_mesh.material = material

	material.shading_mode = BaseMaterial3D.SHADING_MODE_UNSHADED
	material.albedo_color = color

	return await final_cleanup(mesh_instance, persist_ms)
	

func point(pos: Vector3, radius = 0.05, color = Color.WHITE_SMOKE, persist_ms = 0):
	var mesh_instance := MeshInstance3D.new()
	var sphere_mesh := SphereMesh.new()
	var material := ORMMaterial3D.new()

	mesh_instance.mesh = sphere_mesh
	mesh_instance.cast_shadow = GeometryInstance3D.SHADOW_CASTING_SETTING_OFF
	mesh_instance.position = pos

	sphere_mesh.radius = radius
	sphere_mesh.height = radius*2
	sphere_mesh.material = material

	material.shading_mode = BaseMaterial3D.SHADING_MODE_UNSHADED
	material.albedo_color = color

	return await final_cleanup(mesh_instance, persist_ms)

func square(pos: Vector3, size: Vector2, color = Color.WHITE_SMOKE, persist_ms = 0):
	var mesh_instance := MeshInstance3D.new()
	var box_mesh := BoxMesh.new()
	var material := ORMMaterial3D.new()

	mesh_instance.mesh = box_mesh
	mesh_instance.cast_shadow = GeometryInstance3D.SHADOW_CASTING_SETTING_OFF
	mesh_instance.position = pos

	box_mesh.size = Vector3(size.x, size.y, 1)
	box_mesh.material = material

	material.shading_mode = BaseMaterial3D.SHADING_MODE_UNSHADED
	material.albedo_color = color

	return await final_cleanup(mesh_instance, persist_ms)

## 1 -> Lasts ONLY for current physics frame
## >1 -> Lasts X time duration.
## <1 -> Stays indefinitely
func final_cleanup(mesh_instance: MeshInstance3D, persist_ms: float):
	get_tree().get_root().add_child(mesh_instance)
	if persist_ms == 1:
		await get_tree().physics_frame
		mesh_instance.queue_free()
	elif persist_ms > 0:
		await get_tree().create_timer(persist_ms).timeout
		mesh_instance.queue_free()
	else:
		return mesh_instance


func _on_area_3d_4_body_entered(body):
	if(can_change_state):
		collision_area = "front_area"
		can_change_state = false
	elif(collision_area == "front_area"):
		can_change_state = true


func verify_goal(point):
	if(can_change_state):
		parameters.position= point
		parameters.collide_with_areas = true
		parameters.collide_with_bodies = true
		parameters.collision_mask = 2

		var result = space.intersect_point(parameters)
		if(result):
			for i in range(result.size()):
				var name_from_collider = result[i].collider.get_name()
				if(name_from_collider == "StaticBody3D5"):
					is_goal = true
					can_change_state = false
					break
				elif(name_from_collider == "StaticBody3D6"):
					collision_area = "above_area"
					can_change_state = false
					break
				elif(name_from_collider == "StaticBody3D7"):
					collision_area = "left_area"
					can_change_state = false
					break
				elif(name_from_collider == "StaticBody3D8"):
					collision_area = "front_area"
					can_change_state = false
					break
				elif(name_from_collider == "StaticBody3D9"):
					collision_area = "right_area"
					can_change_state = false
					break

func _on_area_3d_5_body_entered(body):
	if(can_change_state):
		collision_area = "right_area"
		can_change_state = false
	elif(collision_area == "right_area"):
		can_change_state = true


func _on_area_3d_3_body_entered(body):
	if(can_change_state):
		collision_area = "left_area"
		can_change_state = false
	elif(collision_area == "left_area"):
		can_change_state = true


func _on_area_3d_2_body_entered(body):
	if(can_change_state):
		collision_area = "above_area"
		can_change_state = false
	elif(collision_area == "above_area"):
		can_change_state = true
