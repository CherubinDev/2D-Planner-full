package main

import (
	"database/sql"
	"fmt"
)

const (
	LOCATION_ATTR_NAME = "location"
)

type playerAttributeRecord struct {
	ID       uint64 `db:"id"`
	PlayerID uint64 `db:"player_id"`
	Name     string `db:"name"`
	Value    string `db:"value"`
	CreateAt string `db:"created_at"`
}

func (p *playerAttributeRecord) toModel() *PlayerAttributeModel {
	return &PlayerAttributeModel{
		Name:  p.Name,
		Value: p.Value,
	}
}

type PlayerAttributeModel struct {
	Name  string
	Value string
}

func (p *PlayerAttributeModel) toAttribute() PlayerAttribute {
	return PlayerAttribute{
		Name:  p.Name,
		Value: p.Value,
	}
}

type PlayerAttributeModels []*PlayerAttributeModel

func (p PlayerAttributeModels) toPlayerAttributes() []PlayerAttribute {
	attrs := make([]PlayerAttribute, len(p))
	for idx, attribute := range p {
		attrs[idx] = attribute.toAttribute()
	}
	return attrs
}

type PlayerAttributeRepo interface {
	GetPlayerAttributes(playerId uint64) ([]*PlayerAttributeModel, error)
	GetPlayerAttribute(playerId uint64, name string) (*PlayerAttributeModel, error)
	SavePlayerAttribute(playerId uint64, attr *PlayerAttributeModel) error
}

type playerAttributeRepoImpl struct {
	db *sql.DB
}

var _ PlayerAttributeRepo = (*playerAttributeRepoImpl)(nil)

func (p *playerAttributeRepoImpl) GetPlayerAttributes(playerId uint64) ([]*PlayerAttributeModel, error) {
	rows, err := p.db.Query(
		"SELECT name, value FROM player_attribute WHERE player_id = ?",
		playerId,
	)
	if err != nil {
		return nil, err
	}
	var attrRecords []*playerAttributeRecord
	for rows.Next() {
		rec := &playerAttributeRecord{}
		if err := rows.Scan(&rec.Name, &rec.Value); err != nil {
			return nil, err
		}
		attrRecords = append(attrRecords, rec)
	}

	attributes := make([]*PlayerAttributeModel, len(attrRecords))
	for idx, attrRecord := range attrRecords {
		attributes[idx] = attrRecord.toModel()
	}
	return attributes, nil
}

func (p *playerAttributeRepoImpl) GetPlayerAttribute(playerId uint64, name string) (*PlayerAttributeModel, error) {
	row := p.db.QueryRow(
		"SELECT name, value FROM player_attribute WHERE player_id = ? and name = ?",
		playerId, name,
	)
	record := &playerAttributeRecord{}
	if err := row.Scan(
		&record.Name,
		&record.Value,
	); err == sql.ErrNoRows {
		return nil, nil
	} else if err != nil {
		return nil, err
	}
	return record.toModel(), nil
}

func (p *playerAttributeRepoImpl) SavePlayerAttribute(playerId uint64, attr *PlayerAttributeModel) error {
	if attr == nil {
		return nil
	}
	const query = "INSERT INTO player_attribute (player_id, `name`, `value`, created_at) VALUES (?, ?, ?, NOW())" +
		"ON DUPLICATE KEY UPDATE `value` = ?"
	res, err := p.db.Exec(query, playerId, attr.Name, attr.Value, attr.Value)
	if err != nil {
		return err
	}
	if rows, err := res.RowsAffected(); err != nil {
		return err
	} else if rows < 0 && rows > 2 {
		return fmt.Errorf("expected to affect one or two rows, but affected %d", rows)
	}
	return nil
}
