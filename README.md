# Live Football World Cup Scoreboard

## Project Description
This is a simple library that simulates a live football World Cup scoreboard. It can start a new match (a match is added to scoreboard), update scores, finish match (a match is removed from scoreboard), 
and provide a list of all in progress matches ordered by total score and recency. 
The scoreboard uses an in-memory collection to track ongoing matches, and each match has unique identifier by which can be retreived from the collection.

## Features
 - StartMatch: Starts a new match with initial score 0 - 0.
 - UpdateScore: Updates the score of specific ongoing match.
 - FinishMatch: Removes specific match from the scoreboard.
 - GetMatches: Retrieves all ongoing matches, ordered by total score and recency.


## Usage Instructions
To use this library you need to interact with the IScoreboard interface. This interface should be injected into your class through Dependency Injection (DI).
IScoreboard _scoreboard = scoreboard;

//Start new matches - parameters home team, away team, match start time
//Returns unique id of the match (example: "01ASB2XFCZJY7WHZ2FNRTMQJCT")
Ulid firstMatchId = _scoreboard.StartMatch("Mexico", "Canada", DateTime.UtcNow);
Ulid secondMatchId = _scoreboard.StartMatch("Spain", "Brazil", DateTime.UtcNow);

//Update match scores - parameters match id, home team score, away team score
_scoreboard.UpdateScore(firstMatchId, 1, 2);
_scoreboard.UpdateScore(secondMatchId, 3, 1);

//Finish a match - parameter match id
_scoreboard.FinishMatch(firstMatchId);

//Get summary of ongoing matches
List<Match> activeMatches = _scoreboard.GetMatches();
