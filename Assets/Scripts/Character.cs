using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


// represents a person playing the game
//  -- collects their input, references the "wait" object -- the circle outside the perimeter of the grid, and could potentially store stats about how well they're doing

public class Character {

	public Queue<Direction> _inputQueue;

	private Vector2Int _startingSpace;

	private GameObject _characterWaitObject;

	public Character() {
		_inputQueue = new Queue<Direction>(16);
	}
	
	public void QueueAnInput(Direction theInputDirection) {
		_inputQueue.Enqueue(theInputDirection);
	}

	public void SetStartingSpace(Vector2Int forSpace, GridArea theGrid) {
		_startingSpace = forSpace;
		_characterWaitObject.transform.DOMove(theGrid.GetWaitPositionForSpace(_startingSpace), 1.4f).SetEase(Ease.InOutCubic);
	}

	public Vector2Int GetStartingSpace() {
		return _startingSpace;
	}

	public void SetCharacterWaitObject(GameObject theCharWaitObject) {
		_characterWaitObject = theCharWaitObject;
	}

	public GameObject GetCharWaitObject() {
		return _characterWaitObject;
	}

	public Direction? GetNextInput() {
		if (_inputQueue.Count > 0) {
			return _inputQueue.Dequeue();
		} else {
			return null;
		}
	}

	public void DisplayEnterInput() {
		_characterWaitObject.transform.DOKill(true);
		_characterWaitObject.transform.DOPunchScale(Vector3.one * 1.6f, 0.3f).SetEase(Ease.OutCubic);
	}

	public void ClearInputQueue() {
		_inputQueue.Clear();
	}

}
