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
	pass
	# $AspectRatioContainer/Play.connect("pressed", self, "_on_Play_pressed")

func _on_Play_pressed():
	play()
	
#func _on_CreateMap_pressed():
#	var MapScript = load("res://Scripts/Map.cs")
#	var PathScript = load("res://Scripts/Path.cs")
#	var path = PathScript.new()
#	path.SetDirection([1, 1, 1, 0])
#	var Map = MapScript.new()
#	Map.CreateMap("Test", "TestName2")
#	# print(Map.map_set.ToString())
#	Map.map_set.Maps[0].AddNote(1000, 0, path._direction)
#
#	print("Created Map")

# Called every frame. 'delta' is the elapsed time since the previous frame.
#func _process(delta):
#	pass

