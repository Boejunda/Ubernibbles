using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

//the character's actions being played out on the grid
public class Snake : MonoBehaviour {

	private Transform _trans;

	private List<Character> _controllingCharacters;

	private List<Vector2Int> _snakeSpaces; //spaces occupied by the snake

	private List<GameObject> _visualSnakePieces;
	private const string _snakeHeadPfbString = "pfb_snakeHead";
	private const string _snakeBodyPfbString = "pfb_snakeBody";

	private const string _snakeDeadPfbString = "pfb_snakeDead";

	private Direction _headingDirection;


	private void Awake() {
		_trans = transform;
		_controllingCharacters = new List<Character>(6);

		_snakeSpaces = new List<Vector2Int>(32);
		_visualSnakePieces = new List<GameObject>(32);
	}

	public void AddControllingCharacter(Character theCharacter) {
		_controllingCharacters.Add(theCharacter);
	}

	public List<Character> GetControllingCharacters() {
		return _controllingCharacters;
	}

	public void SetInitialHeadingDirection(Direction theHeadingDirection) {
		_headingDirection = theHeadingDirection;
	}

	public void AddSegment(Vector2Int theSpace, GridArea theGrid, bool isHead = false) {
		_snakeSpaces.Add(theSpace);

		// update the snake's visual appearance on the grid
		string visualPfbString = isHead ? _snakeHeadPfbString : _snakeBodyPfbString;
		GameObject snakeVisual = Instantiate(Resources.Load(visualPfbString), gameObject.transform, true) as GameObject;
		_visualSnakePieces.Add(snakeVisual);

		for (int i = 0; i < _visualSnakePieces.Count; i++) {
			_visualSnakePieces[i].transform.position = theGrid.GetPositionForSpace(_snakeSpaces[_snakeSpaces.Count - 1 - i]);
		}

		//set head orientation
		Transform headTrans = _visualSnakePieces[0].transform;
		float headRotForUpFacingDirection = theGrid.GetRotationForUpFacingDirection(GetHeadSpace());
		float rotationToHeadingDirectionFromUp = Vector2.SignedAngle(Vector2.up, GetSpaceOffsetForDirection(_headingDirection));
		headTrans.rotation = Quaternion.Euler(0f, 0f, headRotForUpFacingDirection + rotationToHeadingDirectionFromUp);

	}

	public Vector2Int GetHeadSpace() {
		return _snakeSpaces[_snakeSpaces.Count - 1];
	}

	// could modify this to change how inputs map to move direction
	public Vector2Int GetDesiredOffsetForInput(Direction theInput) {
		switch (theInput) {
			case Direction.Up:
				return GetSpaceOffsetForDirection(_headingDirection);
			case Direction.Right:
				Direction turnedRightDirection = _headingDirection == Direction.Left ? Direction.Up : (Direction)((int)_headingDirection + 1);
				_headingDirection = turnedRightDirection;
				return GetSpaceOffsetForDirection(turnedRightDirection);
			case Direction.Left:
				Direction turnedLeftDirection = _headingDirection == Direction.Up ? Direction.Left : (Direction)((int)_headingDirection - 1);
				_headingDirection = turnedLeftDirection;
				return GetSpaceOffsetForDirection(turnedLeftDirection);
			default:
				return Vector2Int.zero;
		}
	}

	private Vector2Int GetSpaceOffsetForDirection(Direction theMoveDirection) {
		switch (theMoveDirection) {
			case Direction.Up:
				return Vector2Int.up;
			case Direction.Right:
				return Vector2Int.right;
			case Direction.Down:
				return Vector2Int.down;
			case Direction.Left:
				return Vector2Int.left;
			default:
				return Vector2Int.zero;
		}
	}

	public List<Vector2Int> GetSnakeSpaces() {
		return _snakeSpaces;
	}

	public int GetLength() {
		return _snakeSpaces.Count;
	}

	public void DisplayAsWinner() {
		for (int i = 0; i < _visualSnakePieces.Count; i++) {
			_visualSnakePieces[i].transform.DOShakePosition(GameState_Victory.durationWhenHaveVictor - 0.1f, new Vector3(0.08f, 0.08f, 0f), 20);
		}
	}

	public void DemolishSnake() {
		GameObject deadSnakePfb = Resources.Load(_snakeDeadPfbString) as GameObject;

		for (int i = 0; i < _visualSnakePieces.Count; i++) {
			if (_visualSnakePieces[i] != null) {
				Destroy(Instantiate(deadSnakePfb, _visualSnakePieces[i].transform.position, Quaternion.identity), 0.5f);
			}
		}
		_visualSnakePieces.Clear();
		Destroy(gameObject);
	}

	public void DestroySnake() {
		Destroy(gameObject);
	}


}
