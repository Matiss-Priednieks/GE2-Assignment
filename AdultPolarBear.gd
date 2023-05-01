extends KinematicBody

enum State {WANDERING, CHARGING, ATTACKING}

export var wander_radius = 15
export var charge_radius = 10
export var attack_radius = 2
export var speed = 3
export var floor_max_angle = 45.0
export var rotation_speed = 5.0

var _random = RandomNumberGenerator.new()
var _target_position = Vector3()

# Reference to the AnimationPlayer node
onready var animation_player = $AnimationPlayer

var state = State.WANDERING
var penguins = []

func _ready():
	_random.randomize()
	set_new_target_position()
	penguins = get_tree().get_nodes_in_group("penguin")

func set_new_target_position():
	var random_angle = _random.randf_range(0, 2 * PI)
	var random_distance = _random.randf_range(0, wander_radius)
	var random_x_offset = cos(random_angle) * random_distance
	var random_z_offset = sin(random_angle) * random_distance

	_target_position = global_transform.origin + Vector3(random_x_offset, 0, random_z_offset)

func _physics_process(delta):
	match state:
		State.WANDERING:
			wander(delta)
		State.CHARGING:
			charge(delta)
		State.ATTACKING:
			attack(delta)

func wander(delta):
	check_for_penguins()

	var direction = (_target_position - global_transform.origin).normalized()
	var velocity = direction * speed

	var snap_vector = Vector3.DOWN * 0.1
	move_and_slide_with_snap(velocity, snap_vector, Vector3.UP, true, 4, deg2rad(floor_max_angle), false)

	if (global_transform.origin.distance_to(_target_position) < 0.5):
		set_new_target_position()
		animation_player.stop()
	else:
		rotate_toward_target(delta)
		if not animation_player.is_playing() and state == State.WANDERING:
			animation_player.play("walk_loop")

func check_for_penguins():
	for penguin in penguins:
		if penguin.global_transform.origin.distance_to(global_transform.origin) < charge_radius:
			_target_position = penguin.global_transform.origin
			state = State.CHARGING
			animation_player.stop()
			break

func charge(delta):
	var direction = (_target_position - global_transform.origin).normalized()
	var velocity = direction * (speed * 2)  # Double the speed for charging

	var snap_vector = Vector3.DOWN * 0.1
	move_and_slide_with_snap(velocity, snap_vector, Vector3.UP, true, 4, deg2rad(floor_max_angle), false)

	rotate_toward_target(delta)

	if animation_player.current_animation != "running_loop":
		animation_player.play("running_loop")

	var penguin_distance = global_transform.origin.distance_to(_target_position)
	if penguin_distance < attack_radius:
		state = State.ATTACKING
	elif penguin_distance < 0.5:
		state = State.WANDERING

func attack(delta):
	rotate_toward_target(delta)

	if animation_player.current_animation != "attack":
		animation_player.play("attack")
		animation_player.connect("animation_finished", self, "_on_attack_animation_finished")

		var closest_penguin = get_closest_penguin()
		if closest_penguin:
			closest_penguin.queue_free()
			penguins.erase(closest_penguin)

func get_closest_penguin():
	var closest_penguin = null
	var closest_distance = 1e9

	for penguin in penguins:
		var distance = penguin.global_transform.origin.distance_to(global_transform.origin)
		if distance < closest_distance:
			closest_distance = distance
			closest_penguin = penguin

	return closest_penguin

func _on_attack_animation_finished(animation_name: String):
	if animation_name == "attack":
		state = State.WANDERING 
		set_new_target_position() 
		animation_player.disconnect("animation_finished", self, "_on_attack_animation_finished")


func rotate_toward_target(delta):
	var target_direction = (_target_position - global_transform.origin).normalized()
	var target_rotation = atan2(target_direction.x, target_direction.z)

	var current_rotation = global_transform.basis.get_euler().y
	var new_rotation = lerp_angle(current_rotation, target_rotation, rotation_speed * delta)

	# Update the polar bear's rotation
	rotation.y = new_rotation
