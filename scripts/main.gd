extends Node2D

@onready var grid_manager: GridManager = $Grid/GridManager
@onready var grid_highlight: GridHighlight = $Grid/GridHighlight
@onready var player: Player = $Player
@onready var enemy: Enemy = $Enemy
@onready var turn_manager: TurnManager = $TurnManager
@onready var card_manager: CardManager = $CardManager
@onready var hud: GameHUD = $CanvasLayer/GameHUD

var _dash_direction: Vector2i = Vector2i.RIGHT


func _ready() -> void:
	player.setup(Vector2i(1, 1), grid_manager)
	enemy.setup(Vector2i(6, 6), grid_manager)
	player.z_index = 2
	enemy.z_index = 3
	turn_manager.player = player
	turn_manager.enemy = enemy
	turn_manager.player_turn_started.connect(_on_player_turn_started)
	turn_manager.enemy_turn_started.connect(_on_enemy_phase)
	turn_manager.start_player_turn()
	hud.configure_energy_pip_count(card_manager.max_energy)
	_refresh_hud()


func _on_player_turn_started() -> void:
	card_manager.energy = card_manager.max_energy
	_refresh_hud()


func _on_enemy_phase() -> void:
	_refresh_hud()


func _unhandled_input(event: InputEvent) -> void:
	if turn_manager.current_state != TurnManager.State.PLAYER_TURN:
		return
	if event is InputEventKey and event.pressed:
		match event.keycode:
			KEY_UP:
				_dash_direction = Vector2i.UP
				_refresh_hud()
				get_viewport().set_input_as_handled()
			KEY_DOWN:
				_dash_direction = Vector2i.DOWN
				_refresh_hud()
				get_viewport().set_input_as_handled()
			KEY_LEFT:
				_dash_direction = Vector2i.LEFT
				_refresh_hud()
				get_viewport().set_input_as_handled()
			KEY_RIGHT:
				_dash_direction = Vector2i.RIGHT
				_refresh_hud()
				get_viewport().set_input_as_handled()
			KEY_1:
				_try_play_card(0)
				get_viewport().set_input_as_handled()
			KEY_SPACE:
				turn_manager.end_player_turn()
				_refresh_hud()
				get_viewport().set_input_as_handled()


func _try_play_card(index: int) -> void:
	var ctx := {
		"player": player,
		"grid_manager": grid_manager,
		"direction": _dash_direction,
		"turn_manager": turn_manager,
	}
	card_manager.try_play_card(index, ctx, turn_manager)
	_refresh_hud()


func _compute_dash_path() -> Array[Vector2i]:
	var path: Array[Vector2i] = []
	var dir := _dash_direction
	var cur := player.grid_position
	var gm := grid_manager
	for i in 2:
		var next: Vector2i = cur + dir
		if not gm.is_in_bounds(next):
			break
		if gm.is_occupied(next):
			var occ: Node = gm.get_occupant(next)
			if occ != player:
				break
		path.append(next)
		cur = next
	return path


func _update_dash_preview() -> void:
	if grid_highlight == null:
		return
	if turn_manager.current_state != TurnManager.State.PLAYER_TURN:
		grid_highlight.clear()
		return
	grid_highlight.highlight_preview(_compute_dash_path())


func _refresh_hud() -> void:
	var state_str := "JUGADOR" if turn_manager.current_state == TurnManager.State.PLAYER_TURN else "ENEMIGO"
	hud.set_turn_text("Turno: %s" % state_str)
	hud.set_energy(card_manager.energy, card_manager.max_energy)
	hud.set_direction_text(_dash_direction)
	_update_dash_preview()
