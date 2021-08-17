extends Node

var next_scene = preload("res://Scenes/Play.tscn")

func play():
	$AudioStreamPlayer.stop()
	get_tree().change_scene_to(next_scene)

func _process(delta):
	if Input.is_action_pressed("ui_accept"):
		play()

# Called when the node enters the scene tree for the first time.
func _ready():
	get_node("AspectRatioContainer/CenterContainer/Play").connect("pressed", self, "_on_Play_pressed")

func _on_Play_pressed():
	play()

# Called every frame. 'delta' is the elapsed time since the previous frame.
#func _process(delta):
#	pass

