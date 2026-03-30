extends Resource
class_name Card

@export var id: String = "card"
@export var energy_cost: int = 1


func execute(context: Dictionary) -> bool:
	base_effect(context)
	modifier_effect(context)
	negative_effect(context)
	return true


func base_effect(_context: Dictionary) -> void:
	pass


func modifier_effect(_context: Dictionary) -> void:
	pass


func negative_effect(_context: Dictionary) -> void:
	pass
