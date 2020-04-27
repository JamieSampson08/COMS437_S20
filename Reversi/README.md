# Reversi

### Tools: 
- Unity
- Rider (IDE)
- Mixamo (Characters & Animations)
- Unity Asset: BehaviorBricks
- Unity Asset: TextMesh Pro
- Unity Asset: Jammo Character
- GameBoard & Piece Assets: Jim Lathrop

### TurnBased: C# Terminal Game
- colored terminal game
- error handling for incorrect inputs (difficulty, turn position, moves)
- 'W' is always the computer (thus, player = 'B') 
- shows all the possible moves as "?"
- lists the possible moves for the player
- AI search ahead x many times, where x = difficulty level

### Reversi: C# Unity Project
- behaviors generated using BehaviorsBricks
- 3 different behaviors (animations) for three different scenarios
    1. Player loses > 3 pieces: Robot "scared" 
    2. Player gains > 3 pieces: Robot "fist pump"
    3. Computer's Turn: Michelle "thinking"
- incorporate the logic constructed in "TurnBased" for gameplay
- show a list of possible moves
- skipping turns and invalid moves notifications
- scoreboard and whose turn is it
- Menu, Credits, GameOver, HowToPlay scenes
- options to choose what level of difficulty and who takes the first turn
