extends Node
class_name TurnManager

enum State { PLAYER_TURN, ENEMY_TURN }

signal player_turn_started
signal enemy_turn_started

var current_state: State = State.PLAYER_TURN
var skip_next_player_turn: bool = false

var enemy: Enemy = null
var player: Player = null


func start_player_turn() -> void:
	if skip_next_player_turn:
		skip_next_player_turn = false
		current_state = State.ENEMY_TURN
		enemy_turn_started.emit()
		process_enemy_turn()
		return
	current_state = State.PLAYER_TURN
	player_turn_started.emit()


func end_player_turn() -> void:
	if current_state != State.PLAYER_TURN:
		return
	current_state = State.ENEMY_TURN
	enemy_turn_started.emit()
	process_enemy_turn()


func process_enemy_turn() -> void:
	if enemy != null and player != null:
		enemy.take_turn(player.grid_position)
	current_state = State.PLAYER_TURN
	start_player_turn()


func queue_skip_next_player_turn() -> void:
	skip_next_player_turn = true
