extends Node
class Rotation:
	static func minimize_rotation_angle(cur_rotation: float, target_rotation: float) -> float:
		var minimized_cur_rotation: float = cur_rotation
		var rotation_diff = target_rotation - cur_rotation
		if abs(rotation_diff) > 180:
			minimized_cur_rotation += -360 if rotation_diff < 0 else 360
		return minimized_cur_rotation
	static func to_nearest_eight_direction(angle: float) -> int:
		return (round((rad2deg(angle) + 90) / 5) * 5) as int
