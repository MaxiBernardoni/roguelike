extends Control
class_name GameHUD

@onready var _panel: ColorRect = $Panel
@onready var _turn_label: Label = $Margin/VBox/TurnLabel
@onready var _energy_row: HBoxContainer = $Margin/VBox/EnergyRow
@onready var _dir_label: Label = $Margin/VBox/DirLabel
@onready var _card_panel: PanelContainer = $Margin/VBox/CardPanel
@onready var _card_name: Label = $Margin/VBox/CardPanel/VBox/CardName
@onready var _card_cost: Label = $Margin/VBox/CardPanel/VBox/CostLabel


func _ready() -> void:
	_panel.color = Style.UI_PANEL
	_card_name.text = "Dash"
	_card_cost.text = "Coste: 1"
	_dir_label.add_theme_color_override("font_color", Style.UI_MUTED)
	_turn_label.add_theme_color_override("font_color", Style.UI_TEXT)
	_card_name.add_theme_color_override("font_color", Style.UI_TEXT)
	_card_cost.add_theme_color_override("font_color", Style.UI_MUTED)
	var hint: Label = $Margin/VBox/HintLabel
	hint.add_theme_color_override("font_color", Style.UI_MUTED)
	var sb := StyleBoxFlat.new()
	sb.bg_color = Color(0.09, 0.1, 0.12, 1.0)
	sb.set_border_width_all(1)
	sb.border_color = Style.UI_BORDER
	sb.set_content_margin_all(8)
	_card_panel.add_theme_stylebox_override("panel", sb)


func set_turn_text(s: String) -> void:
	_turn_label.text = s


func set_direction_text(dir: Vector2i) -> void:
	_dir_label.text = "Dirección Dash: (%d, %d) — flechas" % [dir.x, dir.y]


func set_energy(current: int, maximum: int) -> void:
	var pips: Array = _energy_row.get_children()
	for i in pips.size():
		var r: ColorRect = pips[i] as ColorRect
		if r == null:
			continue
		var full: bool = i < current
		r.color = Style.ENERGY_FULL if full else Style.ENERGY_EMPTY


func configure_energy_pip_count(count: int) -> void:
	for c in _energy_row.get_children():
		c.queue_free()
	for i in count:
		var pip := ColorRect.new()
		pip.custom_minimum_size = Vector2(22, 14)
		pip.color = Style.ENERGY_EMPTY
		pip.size_flags_vertical = Control.SIZE_SHRINK_CENTER
		_energy_row.add_child(pip)
