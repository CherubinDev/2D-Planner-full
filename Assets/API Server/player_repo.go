package main

import (
	"database/sql"

	_ "github.com/go-sql-driver/mysql"
)

type playerRecord struct {
	ID       uint64 `db:"id"`
	Username string `db:"username"`
	CreateAt string `db:"created_at"`
}

type PlayerRepo interface {
	GetOrCreatePlayer(username string) (*PlayerModel, bool, error)
}

type playerRepoImpl struct {
	db *sql.DB
}

var _ PlayerRepo = (*playerRepoImpl)(nil)

func (p *playerRepoImpl) GetOrCreatePlayer(username string) (*PlayerModel, bool, error) {
	// Start transaction
	tx, err := p.db.Begin()
	if err != nil {
		return nil, false, err
	}

	created := false
	// Look up the player
	row := tx.QueryRow(
		"SELECT id, username, created_at FROM player WHERE username = ?",
		username,
	)
	playerRec := &playerRecord{}
	if err := row.Scan(
		&playerRec.ID,
		&playerRec.Username,
		&playerRec.CreateAt,
	); err == sql.ErrNoRows {
		// Doesn't exist, create the player
		res, err := tx.Exec(
			"INSERT INTO player (username, created_at) VALUES (?, NOW())",
			username,
		)
		if err != nil {
			return nil, false, err
		}
		newId, err := res.LastInsertId()
		if err != nil {
			return nil, false, err
		}
		// Propogate the changes
		playerRec.ID = uint64(newId)
		playerRec.Username = username
		created = true
	} else if err != nil {
		return nil, false, err
	}

	return playerRec.toPlayerModel(), created, tx.Commit()
}

func (pr *playerRecord) toPlayerModel() *PlayerModel {
	return &PlayerModel{
		ID:       pr.ID,
		Username: pr.Username,
	}
}
