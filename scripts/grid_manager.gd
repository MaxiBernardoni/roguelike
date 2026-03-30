extends Node
class_name GridManager

## Grid size in cells (width x height).
var grid_size: Vector2i = Vector2i(8, 8)
## Pixel size of one cell edge.
var cell_size: int = 64

## Vector2i -> occupant Node (or null for free). Only stores occupied cells.
var _occupied: Dictionary = {}


func is_in_bounds(cell: Vector2i) -> bool:
	return (
		cell.x >= 0
		and cell.y >= 0
		and cell.x < grid_size.x
		and cell.y < grid_size.y
	)


func grid_to_world(cell: Vector2i) -> Vector2:
	return Vector2(cell) * float(cell_size)


func world_to_grid(pos: Vector2) -> Vector2i:
	var sx := pos.x / float(cell_size)
	var sy := pos.y / float(cell_size)
	return Vector2i(int(floor(sx)), int(floor(sy)))


func is_occupied(cell: Vector2i) -> bool:
	return _occupied.has(cell)


func get_occupant(cell: Vector2i) -> Node:
	return _occupied.get(cell, null)


func set_occupied(cell: Vector2i, entity: Node) -> void:
	if entity == null:
		_occupied.erase(cell)
	else:
		_occupied[cell] = entity


func clear_cell(cell: Vector2i) -> void:
	_occupied.erase(cell)
