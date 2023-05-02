extends KinematicBody

enum State {WANDERING, RUNNING}

export var wander_radius = 10
export var speed = 2
export var floor_max_angle = 45.0
export var rotation_speed = 5.0

var _random = RandomNumberGenerator.new()
var _target_position = Vector3()

# Reference to the AnimationPlayer node
onready var animation_player = $AnimationPlayer

var state = State.WANDERING
var polar_bears = []

func _ready():
	_random.randomize()
	set_new_target_position()
	polar_bears = get_tree().get_nodes_in_group("polar_bear")

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
		State.RUNNING:
			run(delta)

func wander(delta):
	check_for_polar_bears()

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

func check_for_polar_bears():
	for polar_bear in polar_bears:
		if polar_bear.global_transform.origin.distance_to(global_transform.origin) < wander_radius:
			_target_position = polar_bear.global_transform.origin
			state = State.RUNNING
			animation_player.stop()
			break

func run(delta):
	var direction = (global_transform.origin - _target_position).normalized()
	var velocity = direction * (speed * 1.5)

	var snap_vector = Vector3.DOWN * 0.1
	move_and_slide_with_snap(velocity, snap_vector, Vector3.UP, true, 4, deg2rad(floor_max_angle), false)

	rotate_toward_target(delta)

	if animation_player.current_animation != "run_loop":
		animation_player.play("run_loop")

	if global_transform.origin.distance_to(_target_position) > wander_radius * 2:
		state = State.WANDERING

func rotate_toward_target(delta):
	var target_direction = (_target_position - global_transform.origin).normalized()
	var target_rotation = atan2(target_direction.x, target_direction.z)

	var current_rotation = global_transform.basis.get_euler().y
	var new_rotation = lerp_angle(current_rotation, target_rotation, rotation_speed * delta)

	# Update the penguin's rotation
	rotation.y = new_rotation
