using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLogic {

	private GridArea _grid;
	private Dictionary<Character, Snake> _charToSnake;
	private List<Snake> _aliveSnakes;

	private List<Snake>[] _gridState;

	private List<Vector2Int> _availableStartingSpaces;
	private List<Vector2Int> _usedStartingSpaces;

	private List<Snake> _snakesToDemolish;


	public GameLogic(GridArea theGrid) {
		_grid = theGrid;
		_charToSnake = new Dictionary<Character, Snake>(32);
		_aliveSnakes = new List<Snake>(32);

		_gridState = new List<Snake>[_grid.GetNumberOfSpaces()];
		for (int i = 0; i < _gridState.Length; i++) {
			_gridState[i] = new List<Snake>(4);
		}

		_availableStartingSpaces = _grid.GetListOfStartingSpaces();
		_usedStartingSpaces = new List<Vector2Int>(32);

		_snakesToDemolish = new List<Snake>();
	}

	public Vector2Int ReserveAStartingSpace() {
		int randomIndex = Random.Range(0, _availableStartingSpaces.Count);
		Vector2Int startingSpace = _availableStartingSpaces[randomIndex];
		_availableStartingSpaces.RemoveAt(randomIndex);
		_usedStartingSpaces.Add(startingSpace);

		return startingSpace;
	}

	public void ResetAvailableSpace() {
		for (int i = 0; i < _usedStartingSpaces.Count; i++) {
			_availableStartingSpaces.Add(_usedStartingSpaces[i]);
		}
		_usedStartingSpaces.Clear();
	}

	public void PlaceNewSnake(Snake theSnake, Vector2Int theStartingSpace) {
		_aliveSnakes.Add(theSnake);

		theSnake.SetInitialHeadingDirection(Direction.Up);
		theSnake.AddSegment(theStartingSpace, _grid, true);
		//update the grid state..
		int gridStateIndex = _grid.GetIndexForSpace(theStartingSpace);
		_gridState[gridStateIndex].Add(theSnake);
	}

	//returns if moved game forward
	public bool ExecuteInput(Character theChar, Direction theInput) {
		bool movedGameForward = false;

		Snake snake = null;
		if (_charToSnake.TryGetValue(theChar, out snake)) {
			movedGameForward = true;
			Vector2Int desiredGridOffset = snake.GetDesiredOffsetForInput(theInput);
			Vector2Int? nextSpace = _grid.GetSpaceAfterMove(snake.GetHeadSpace(), desiredGridOffset);
			if (nextSpace.HasValue){

				snake.AddSegment(nextSpace.Value, _grid);
				
				//update the grid state...
				int gridStateIndex = _grid.GetIndexForSpace(nextSpace.Value);
				_gridState[gridStateIndex].Add(snake);

			} else {
				//snake tried moving off the grid...
				_snakesToDemolish.Add(snake);
			}
		}

		return movedGameForward;
	}

	//returns if any snakes were demolished
	public bool EvaluateGridState() {
		//look through the gridState for overlaps
		for (int i = 0; i < _gridState.Length; i++) {
			List<Snake> snakesAtThisSpace = _gridState[i];
			if (snakesAtThisSpace.Count > 1) {
				for (int j = 0; j < snakesAtThisSpace.Count; j++) {
					Snake snake = snakesAtThisSpace[j];
					if (_grid.GetIndexForSpace(snake.GetHeadSpace()) == i && _snakesToDemolish.Contains(snake) == false) {
						//if the snake's head is at this space (overlapping something else)
						_snakesToDemolish.Add(snake);
					}
				}
			}
		}

		bool snakeWasDemolished = false;

		for (int i = 0; i < _snakesToDemolish.Count; i++) {
			snakeWasDemolished = true;
			Snake snake = _snakesToDemolish[i];
			_aliveSnakes.Remove(snake);

			//clear the gridState of this snake
			List<Vector2Int> snakeSpaces = snake.GetSnakeSpaces();
			for (int j = 0; j < snakeSpaces.Count; j++) {
				int stateIndexWithSnake = _grid.GetIndexForSpace(snakeSpaces[j]);
				_gridState[stateIndexWithSnake].Remove(snake);
			}

			List<Character> controllingCharacters = snake.GetControllingCharacters();
			for (int j = 0; j < controllingCharacters.Count; j++) {
				_charToSnake.Remove(controllingCharacters[j]);
				controllingCharacters[j].ClearInputQueue();
			}

			snake.DemolishSnake();
		}

		_snakesToDemolish.Clear();

		return snakeWasDemolished;
	}

	public List<Snake> GetAliveSnakes() {
		return _aliveSnakes;
	}

	public void Reset() {
		_charToSnake.Clear();

		for (int i = 0; i < _aliveSnakes.Count; i++) {
			_aliveSnakes[i].DestroySnake();
		}
		_aliveSnakes.Clear();

		for (int i = 0; i < _gridState.Length; i++) {
			_gridState[i].Clear();
		}
	}

	public GridArea GetGrid() {
		return _grid;
	}

	public void CharacterLeftMatch(Character theChar) {
		Snake snake = null;
		if (_charToSnake.TryGetValue(theChar, out snake)) {
			_snakesToDemolish.Add(snake);
		}
	}


	public void RegisterCharControlsSnake(Character theChar, Snake theSnake) {
		_charToSnake.Add(theChar, theSnake);
	}

}
