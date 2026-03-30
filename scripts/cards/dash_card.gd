extends Card
class_name DashCard

var _moved_this_play: bool = false


func _init() -> void:
	id = "dash"
	energy_cost = 1


func execute(context: Dictionary) -> bool:
	_moved_this_play = false
	base_effect(context)
	modifier_effect(context)
	if _moved_this_play:
		negative_effect(context)
	return _moved_this_play


func base_effect(context: Dictionary) -> void:
	var p: Player = context.get("player")
	var gm: GridManager = context.get("grid_manager")
	var dir: Vector2i = context.get("direction", Vector2i.ZERO)
	if dir == Vector2i.ZERO or p == null or gm == null:
		return
	for _i in 2:
		var next_cell: Vector2i = p.grid_position + dir
		if not gm.is_in_bounds(next_cell):
			break
		if gm.is_occupied(next_cell):
			var occ: Node = gm.get_occupant(next_cell)
			if occ != p:
				break
		p.move_to(next_cell)
		_moved_this_play = true


func modifier_effect(_context: Dictionary) -> void:
	pass


func negative_effect(context: Dictionary) -> void:
	var tm: TurnManager = context.get("turn_manager")
	if tm:
		tm.queue_skip_next_player_turn()
