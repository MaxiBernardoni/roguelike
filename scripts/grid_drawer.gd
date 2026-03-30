extends Node2D
## Filled chess-board floor + subtle cell borders. Parent must have `GridManager` child.

var _grid_manager: GridManager


func _ready() -> void:
	_grid_manager = get_parent().get_node("GridManager") as GridManager
	z_index = 0
	queue_redraw()


func _draw() -> void:
	if _grid_manager == null:
		return
	var gs: Vector2i = _grid_manager.grid_size
	var cs: float = float(_grid_manager.cell_size)
	for x in range(gs.x):
		for y in range(gs.y):
			var light: bool = (x + y) % 2 == 0
			var c: Color = Style.GRID_LIGHT if light else Style.GRID_DARK
			var p: Vector2 = Vector2(x, y) * cs
			draw_rect(Rect2(p, Vector2(cs, cs)), c)
			draw_rect(Rect2(p, Vector2(cs, cs)), Style.GRID_BORDER, false, 1.0)
