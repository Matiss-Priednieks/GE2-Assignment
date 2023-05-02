extends Spatial

export var polar_bear_scene: PackedScene
export var spawn_area_size = 10.0
export var num_polar_bears = 1

func _ready():
	randomize()
	call_deferred("_spawn_polar_bears")

func _spawn_polar_bears():
	for _i in range(num_polar_bears):
		spawn_polar_bear()

func spawn_polar_bear():
	print("Spawning polar bear")
	if polar_bear_scene:
		var polar_bear_instance = polar_bear_scene.instance()
		var random_pos = Vector3(rand_range(-spawn_area_size, spawn_area_size),
								 5,  # Adjust this value if necessary
								 rand_range(-spawn_area_size, spawn_area_size))
		polar_bear_instance.global_transform.origin = transform.origin + random_pos
		get_tree().get_root().add_child(polar_bear_instance)
	else:
		print("Error: Polar bear scene not set")

