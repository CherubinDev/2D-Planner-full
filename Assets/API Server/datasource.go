package main

import (
	"database/sql"

	_ "github.com/go-sql-driver/mysql"
)

var db *sql.DB

func init() {
	var err error
	db, err = sql.Open("mysql", "root:password@tcp(localhost:3306)/planner")

	// if there is an error opening the connection, handle it
	if err != nil {
		panic(err.Error())
	}
}

func GetDb() *sql.DB {
	return db
}
