package main

type Level struct {
	BaseX  uint16       `json:"base_x"`
	BaseY  uint16       `json:"base_y"`
	Layers []LevelLayer `json:"layers"`
}

type LevelLayer struct {
	Order uint8 `json:"order"`
	//Cells map[string][]CellInfo `json:"cells"`
	Cells    []TextureToCell `json:"cells"`
	Walkable *bool           `json:"walkable"`
}

type TextureToCell struct {
	Texture string     `json:"texture"`
	Cells   []CellInfo `json:"cells"`
}

type CellInfo struct {
	XLocation int8  `json:"x_location"`
	YLocation int8  `json:"y_location"`
	Walkable  *bool `json:"walkable"`
}
