extends Node2D
class_name VisualComponent

enum ShapeKind { CIRCLE, SQUARE }

@export var shape: ShapeKind = ShapeKind.CIRCLE
@export var fill_color: Color = Style.PLAYER
## Half-size for square; radius for circle.
@export var radius: float = 22.0


func _draw() -> void:
	match shape:
		ShapeKind.CIRCLE:
			draw_circle(Vector2.ZERO, radius, fill_color)
			draw_arc(Vector2.ZERO, radius, 0.0, TAU, 64, Color(0, 0, 0, 0.45), 2.0, true)
		ShapeKind.SQUARE:
			var s: float = radius * 2.0
			var r := Rect2(-radius, -radius, s, s)
			draw_rect(r, fill_color)
			draw_rect(r, Color(0, 0, 0, 0.4), false, 2.0)


func set_fill_color(c: Color) -> void:
	fill_color = c
	queue_redraw()


var _bump_tween: Tween


## Optional punch on move / card play.
func play_bump() -> void:
	if _bump_tween:
		_bump_tween.kill()
	_bump_tween = create_tween()
	_bump_tween.set_trans(Tween.TRANS_BACK).set_ease(Tween.EASE_OUT)
	scale = Vector2.ONE
	_bump_tween.tween_property(self, "scale", Vector2(1.12, 1.12), 0.06)
	_bump_tween.tween_property(self, "scale", Vector2.ONE, 0.1)
