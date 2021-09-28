extends Node


# Declare member variables here. Examples:
# var a = 2
# var b = "text"

var position: Vector2
var screen_size := Vector2(1024, 600)
var speed := 300

# Called when the node enters the scene tree for the first time.
func _ready():
	position = $Player.rect_global_position

var last_pointing_target := Vector2(0, 0)
var cur_pointing_target := Vector2(0, 0)

# TODO: correct rotation movement
# E.g. 1.	when up is hold while left is presses several times
# 			the rotation should be -90 while actual is -45
# 			due to up being hold giving y component a -1
func get_current_pointing_target():
	var result_target_vec := Vector2(0, 0)
	if Input.is_action_pressed("play_left"):
		result_target_vec.x -= 1
	if Input.is_action_pressed("play_right"):
		result_target_vec.x += 1
	if Input.is_action_pressed("play_up"):
		result_target_vec.y -= 1
	if Input.is_action_pressed("play_down"):
		result_target_vec.y += 1
	if result_target_vec == Vector2.ZERO:
		return last_pointing_target
	return result_target_vec

func set_rotation(target_rotation: float):
	# print($Player.rect_rotation)
	$Player.rect_rotation = Utils.Rotation.minimize_rotation_angle($Player.rect_rotation, target_rotation)
	$Tween.interpolate_property($Player, "rect_rotation",
		$Player.rect_rotation, target_rotation, 0.1,
		Tween.TRANS_LINEAR, Tween.EASE_IN_OUT)
	$Tween.start()

func _process(delta):
	last_pointing_target = cur_pointing_target
	cur_pointing_target = get_current_pointing_target()
	if last_pointing_target != cur_pointing_target:
		print("Before: %s, After: %s" % [last_pointing_target, cur_pointing_target])
		var new_rotation = Utils.Rotation.to_nearest_eight_direction(cur_pointing_target.angle())
		print("Rotation: %d" % new_rotation)
		set_rotation(new_rotation)
	position += cur_pointing_target.normalized() * speed * delta
#	position.x = clamp(position.x, 0, screen_size.x)
#	position.y = clamp(position.y, 0, screen_size.y)
	
	$Player.rect_global_position = position
#	print(position)

