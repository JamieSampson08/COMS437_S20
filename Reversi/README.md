Reversi (WIP)
	- TurnBased : C# AI

Tools: 
- Unity
- Rider (IDE)

Implmented:
- currently only works for depth of 1
- handles incorrect inputs 
- handles invalid moves
- 'W' is always the computer (thus, player = 'B') 
    - planning on making these customizable in the future since it's just a const var change
- shows all the possible moves as "?"
- lists the possible moves
- shows board coordinates to enter moves

Incomplete:
- the correct move isn't being returned even though it correctly min/maxs values
- alpha beta purning (decided it wasn't a good idea to try to implement this before I got ^ fixed first)
- remove debugging lines (too many to fix upon this submission, but will for final)
- uncomment out the more sophisticated evalutation logic (easier to debug with staright up # of pieces on the board)

