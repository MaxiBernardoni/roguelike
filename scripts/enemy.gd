extends Node2D
class_name Enemy

var grid_position: Vector2i = Vector2i(-1, -1)
var grid_manager: GridManager
var _placed: bool = false
var _suppress_tween: bool = false
var _move_tween: Tween

@onready var _visual: VisualComponent = $VisualComponent


func setup(initial_cell: Vector2i, gm: GridManager) -> void:
	grid_manager = gm
	if _visual:
		_visual.position = Vector2(gm.cell_size, gm.cell_size) * 0.5
		_visual.shape = VisualComponent.ShapeKind.SQUARE
		_visual.fill_color = Style.ENEMY
	_suppress_tween = true
	move_to(initial_cell)
	_suppress_tween = false


func move_to(cell: Vector2i) -> void:
	if grid_manager == null:
		return
	if _placed and grid_manager.is_in_bounds(grid_position):
		if grid_manager.get_occupant(grid_position) == self:
			grid_manager.clear_cell(grid_position)
	grid_position = cell
	grid_manager.set_occupied(cell, self)
	var target_world: Vector2 = grid_manager.grid_to_world(cell)
	if _move_tween:
		_move_tween.kill()
	if _suppress_tween or not _placed:
		position = target_world
	else:
		if _visual:
			_visual.play_bump()
		_move_tween = create_tween()
		_move_tween.set_trans(Tween.TRANS_SINE).set_ease(Tween.EASE_OUT)
		_move_tween.tween_property(self, "position", target_world, 0.12)
	_placed = true


func take_turn(player_position: Vector2i) -> void:
	if grid_manager == null:
		return
	var delta := player_position - grid_position
	var step := Vector2i.ZERO
	if absi(delta.x) >= absi(delta.y):
		if delta.x != 0:
			step.x = signi(delta.x)
	else:
		if delta.y != 0:
			step.y = signi(delta.y)
	if step == Vector2i.ZERO:
		return
	var target := grid_position + step
	if not grid_manager.is_in_bounds(target):
		return
	if grid_manager.is_occupied(target):
		var occ: Node = grid_manager.get_occupant(target)
		if occ != self:
			return
	move_to(target)
