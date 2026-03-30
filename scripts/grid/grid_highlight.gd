extends Node2D
class_name GridHighlight

var grid_manager: GridManager

## Path cells (order preserved). Last cell uses target highlight color.
var _path_cells: Array[Vector2i] = []


func _ready() -> void:
	grid_manager = get_parent().get_node("GridManager") as GridManager
	z_index = 1


func highlight_preview(path_cells: Array[Vector2i]) -> void:
	_path_cells = path_cells.duplicate()
	queue_redraw()


func clear() -> void:
	_path_cells.clear()
	queue_redraw()


func _draw() -> void:
	if grid_manager == null:
		return
	var cs: float = float(grid_manager.cell_size)
	for i in _path_cells.size():
		var cell: Vector2i = _path_cells[i]
		var is_final: bool = i == _path_cells.size() - 1
		var fill: Color = Style.HIGHLIGHT_TARGET if is_final else Style.HIGHLIGHT_PATH
		var p: Vector2 = Vector2(cell) * cs
		draw_rect(Rect2(p, Vector2(cs, cs)), fill)
		draw_rect(Rect2(p, Vector2(cs, cs)), Color(1, 1, 1, 0.12), false, 1.0)
