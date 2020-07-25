package main

import (
	"net/http"

	"github.com/gorilla/mux"
)

type Route struct {
	Name        string
	Method      string
	Pattern     string
	QueryParam  *string
	HandlerFunc http.HandlerFunc
}

type Routes []Route

func NewRouter() *mux.Router {

	router := mux.NewRouter().StrictSlash(true)
	for _, route := range routes {
		newRoute := router.
			Methods(route.Method).
			Path(route.Pattern).
			Name(route.Name).
			Handler(route.HandlerFunc)
		if route.QueryParam != nil {
			newRoute.Queries(*route.QueryParam, "{"+*route.QueryParam+"}")
		}
	}

	return router
}

var routes = Routes{
	Route{
		Name:        "GetPlayer",
		Method:      "GET",
		Pattern:     "/player/{username}",
		HandlerFunc: GetPlayerHandler,
	},
	Route{
		Name:        "GetPlayerLocation",
		Method:      "GET",
		Pattern:     "/player/{username}/location",
		HandlerFunc: GetPlayerLocationHandler,
	},
	Route{
		Name:        "GetLevel",
		Method:      "GET",
		Pattern:     "/level",
		HandlerFunc: GetLevelHandler,
	},
	Route{
		Name:        "SetPlayerLocation",
		Method:      "GET",
		Pattern:     "/player/{username}/location/{location}",
		HandlerFunc: SetPlayerLocationHandler,
	},
}

func stringToPointer(s string) *string {
	local := s
	return &local
}
