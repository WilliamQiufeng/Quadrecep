extends Node


# Declare member variables here. Examples:
# var a = 2
# var b = "text"


# Called when the node enters the scene tree for the first time.
func _ready():
	$ParallaxBackground/AudioStreamPlayer.play()
	$ParallaxBackground/VideoPlayer.free()
	$ParallaxBackground/Background.visible = true
	$ParallaxBackground/Mask.fade_in(1)
	var my_csharp_script = load("res://Scripts/Map.cs")
	var my_csharp_node = my_csharp_script.new()
	my_csharp_node.CreateMap("Test", "TestName")


# Called every frame. 'delta' is the elapsed time since the previous frame.
#func _process(delta):
#	pass
