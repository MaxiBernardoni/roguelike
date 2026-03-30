extends Node
class_name CardManager

var energy: int = 0
var max_energy: int = 5
var hand: Array[Card] = []


func _ready() -> void:
	hand.append(DashCard.new())
	energy = max_energy


func try_play_card(card_index: int, context: Dictionary, turn_manager: TurnManager) -> bool:
	if turn_manager.current_state != TurnManager.State.PLAYER_TURN:
		return false
	if card_index < 0 or card_index >= hand.size():
		return false
	var card: Card = hand[card_index]
	if energy < card.energy_cost:
		return false
	if not card.execute(context):
		return false
	energy -= card.energy_cost
	return true
