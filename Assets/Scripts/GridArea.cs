using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class GridArea {

	protected int _numRows;
	protected int _numColumns;

	protected GameObject[] _spotObjects;
	protected Character[] _spotStates;

	public abstract Vector2 GetSizeOfGrid();

	public abstract Vector2 GetPositionForSpace(Vector2Int theSpace);

	public abstract int GetNumberOfSpaces();

	public abstract List<Vector2Int> GetListOfStartingSpaces();

	public abstract int GetIndexForSpace(Vector2Int theSpace);

	public abstract Vector2Int? GetSpaceAfterMove(Vector2Int theCurrentSpace, Vector2Int theMoveOffset);

	public abstract float GetRotationForUpFacingDirection(Vector2Int currentSpace); // if the head is oriented "up" on this grid space, what would its angle (in degrees) be?

	public abstract Vector2 GetWaitPositionForSpace(Vector2Int space);
}
