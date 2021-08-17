extends Node
class_name BackgroundBrightness

func fade_in(duration: float):
#	$".".modulate[3] = 1
	print(@"ParallaxBackground/Mask:modulate:3".get_as_property_path())
	$Tween.interpolate_property($".", @"ParallaxBackground/Mask:modulate:3",
		1, $".".modulate[3], duration,
		Tween.TRANS_LINEAR, Tween.EASE_IN_OUT)
	$Tween.start()
	print("start")


# Declare member variables here. Examples:
# var a = 2
# var b = "text"


# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
#func _process(delta):
#	pass
