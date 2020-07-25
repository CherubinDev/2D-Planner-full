package main

import (
	"fmt"
	"strconv"
	"strings"
)

const (
	LOCATION_DELIMITER = ","
)

var (
	DEFAULT_LOCATION = &PlayerLocation{XCoordinate: 0, YCoordinate: 0}
)

type Player struct {
	Username   string            `json:"username"`
	Attributes []PlayerAttribute `json:"attributes"`
}

type PlayerLocation struct {
	XCoordinate int64 `json:"x"`
	YCoordinate int64 `json:"y"`
}

func (pl *PlayerLocation) toAttributeModel() *PlayerAttributeModel {
	return &PlayerAttributeModel{
		Name: LOCATION_ATTR_NAME,
		Value: strings.Join([]string{
			strconv.FormatInt(pl.XCoordinate, 10),
			strconv.FormatInt(pl.YCoordinate, 10),
		}, LOCATION_DELIMITER),
	}
}

type PlayerAttribute struct {
	Name  string `json:"name"`
	Value string `json:"value"`
}

type PlayerModel struct {
	ID         uint64
	Username   string
	Attributes []*PlayerAttributeModel
}

func (pm *PlayerModel) toPlayer() *Player {
	return &Player{
		Username:   pm.Username,
		Attributes: PlayerAttributeModels(pm.Attributes).toPlayerAttributes(),
	}
}

var (
	playerRepo          PlayerRepo
	playerAttributeRepo PlayerAttributeRepo
)

func init() {
	playerRepo = &playerRepoImpl{db: GetDb()}
	playerAttributeRepo = &playerAttributeRepoImpl{db: GetDb()}
}

func GetPlayer(username string) (*Player, error) {
	// Look up Player
	model, created, err := playerRepo.GetOrCreatePlayer(username)
	if err != nil {
		return nil, err
	}
	if !created {
		// If not created, look up attributes
		playerAttrs, err := playerAttributeRepo.GetPlayerAttributes(model.ID)
		if err != nil {
			return nil, err
		}
		model.Attributes = playerAttrs
	}

	return model.toPlayer(), nil
}

func GetPlayerLocation(username string) (*PlayerLocation, error) {
	// Look up Player
	model, _, err := playerRepo.GetOrCreatePlayer(username)
	if err != nil {
		return nil, err
	}
	attr, err := playerAttributeRepo.GetPlayerAttribute(model.ID, LOCATION_ATTR_NAME)
	if err != nil {
		return nil, err
	}
	if attr == nil {
		return DEFAULT_LOCATION, nil
	}
	location, err := parseLocation(attr.Value)
	if err != nil {
		return nil, err
	}

	return location, nil
}

func SavePlayerLocation(username string, value string) error {
	location, err := parseLocation(value)
	if err != nil {
		return err
	}

	// Look up Player
	model, _, err := playerRepo.GetOrCreatePlayer(username)
	if err != nil {
		return err
	}

	// Save the location
	if err := playerAttributeRepo.SavePlayerAttribute(
		model.ID,
		location.toAttributeModel(),
	); err != nil {
		return err
	}
	return nil
}

func parseLocation(value string) (*PlayerLocation, error) {
	if !strings.Contains(value, LOCATION_DELIMITER) {
		return nil, fmt.Errorf("value doesn't contain `%s` delimiter", LOCATION_DELIMITER)
	}
	splitVals := strings.Split(value, LOCATION_DELIMITER)
	if len(splitVals) != 2 {
		return nil, fmt.Errorf("value contains incorrect number of values")
	}
	location := &PlayerLocation{}
	var err error
	if location.XCoordinate, err = strconv.ParseInt(splitVals[0], 10, 64); err != nil {
		return nil, fmt.Errorf("invalid x coordinate with value `%s`", splitVals[0])
	}
	if location.YCoordinate, err = strconv.ParseInt(splitVals[1], 10, 64); err != nil {
		return nil, fmt.Errorf("invalid y coordinate with value `%s`", splitVals[1])
	}
	return location, nil
}
