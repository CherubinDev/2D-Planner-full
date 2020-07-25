package main

import (
	"encoding/json"
	"net/http"

	"github.com/gorilla/mux"
)

func GetPlayerHandler(w http.ResponseWriter, r *http.Request) {
	vars := mux.Vars(r)
	username := vars["username"]

	player, err := GetPlayer(username)
	if err != nil {
		w.WriteHeader(http.StatusInternalServerError)
		w.Write([]byte(err.Error()))
		return
	}

	if err := json.NewEncoder(w).Encode(player); err != nil {
		panic(err)
	}
}

func GetPlayerLocationHandler(w http.ResponseWriter, r *http.Request) {
	vars := mux.Vars(r)
	username := vars["username"]

	playerLocation, err := GetPlayerLocation(username)
	if err != nil {
		w.WriteHeader(http.StatusInternalServerError)
		w.Write([]byte(err.Error()))
		return
	}

	if err := json.NewEncoder(w).Encode(playerLocation); err != nil {
		panic(err)
	}
}

func SetPlayerLocationHandler(w http.ResponseWriter, r *http.Request) {
	vars := mux.Vars(r)
	username := vars["username"]
	location := vars["location"]

	// Save the new location
	if err := SavePlayerLocation(username, location); err != nil {
		w.WriteHeader(http.StatusInternalServerError)
		w.Write([]byte(err.Error()))
		return
	}

	// Get the updated player
	player, err := GetPlayer(username)
	if err != nil {
		w.WriteHeader(http.StatusInternalServerError)
		w.Write([]byte(err.Error()))
		return
	}

	if err := json.NewEncoder(w).Encode(player); err != nil {
		panic(err)
	}
}

func GetLevelHandler(w http.ResponseWriter, r *http.Request) {
	nonWalkable := false
	level := Level{
		BaseX: 74,
		BaseY: 46,
		Layers: []LevelLayer{
			{
				Order: 1,
				Cells: []TextureToCell{
					{
						Texture: "flower",
						Cells: []CellInfo{
							{XLocation: 3, YLocation: 1},
							{XLocation: 3, YLocation: 0},
							{XLocation: 3, YLocation: -1},
							{XLocation: 3, YLocation: -2},
							{XLocation: 3, YLocation: 3},
							{XLocation: 2, YLocation: 3},
							{XLocation: 1, YLocation: 3},
							{XLocation: 0, YLocation: 3},
							{XLocation: -1, YLocation: 3},
							{XLocation: 3, YLocation: -4},
							{XLocation: 2, YLocation: -4},
							{XLocation: 1, YLocation: -4},
							{XLocation: 0, YLocation: -4},
							{XLocation: -1, YLocation: -4},
						},
					},
				},
				Walkable: &nonWalkable,
			},
			{
				Order:    2,
				Cells:    []TextureToCell{}, // map[string][]CellInfo{},
				Walkable: &nonWalkable,
			},
		},
	}

	if err := json.NewEncoder(w).Encode(level); err != nil {
		panic(err)
	}
}
