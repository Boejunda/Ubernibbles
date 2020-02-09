using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSquare : GridArea {

	private float _gridSpaceSize = 1f;
	private float _borderSize = 0.08f;


	public GridSquare(int theNumRows, int theNumColumns, float theSpaceSize = 1f, float theBorderSize = 0.08f) {

		_numRows = theNumRows;
		_numColumns = theNumColumns;

		_gridSpaceSize = theSpaceSize;
		_borderSize = theBorderSize;

		_spotObjects = new GameObject[_numRows * _numColumns];

		Transform gridTrans = new GameObject("grid").transform;


		Vector2 spacesToOutmost = new Vector2((_numColumns - 1f) / 2f, (_numRows - 1f) / 2f);
		Vector2 offsetToOutmost = spacesToOutmost * _gridSpaceSize;
		Vector2 borderOffsset = spacesToOutmost * _borderSize;
		offsetToOutmost += borderOffsset;
		Vector2 upperLeftSpaceCenter = new Vector2(-offsetToOutmost.x, offsetToOutmost.y);


		Vector2 currentCenterPos = upperLeftSpaceCenter;
		int currentIndex = 0;

		for (int column = 0; column < _numColumns; column++) {
			for (int row = 0; row < _numRows; row++) {
				GameObject aSpotObject = CreateSpotObject(column, row);

				aSpotObject.transform.SetParent(gridTrans, true);

				aSpotObject.transform.localPosition = currentCenterPos;

				currentCenterPos.y -= (_gridSpaceSize + _borderSize);
				_spotObjects[currentIndex] = aSpotObject;
				currentIndex++;
			}
			currentCenterPos.x += (_gridSpaceSize + _borderSize);
			currentCenterPos.y = upperLeftSpaceCenter.y;
		}
	}

	private GameObject CreateSpotObject(int forColumn, int forRow) {
		GameObject aSpotObject = new GameObject(string.Format("spot ({0},{1})", forColumn, forRow));
		MeshFilter aMeshFilter = aSpotObject.AddComponent<MeshFilter>();
		aSpotObject.AddComponent<MeshRenderer>();

		Mesh spotMesh = new Mesh();
		Vector3[] vertices = new Vector3[4];

		vertices[0] = new Vector3(-0.5f * _gridSpaceSize, -0.5f * _gridSpaceSize, 0f);
		vertices[1] = new Vector3(0.5f * _gridSpaceSize, -0.5f * _gridSpaceSize, 0f);
		vertices[2] = new Vector3(0.5f * _gridSpaceSize, 0.5f * _gridSpaceSize, 0f);
		vertices[3] = new Vector3(-0.5f * _gridSpaceSize, 0.5f * _gridSpaceSize, 0f);

		int[] tris = new int[] { 0, 3, 1, 1, 3, 2 };

		spotMesh.vertices = vertices;
		spotMesh.triangles = tris;

		spotMesh.RecalculateNormals();
		spotMesh.RecalculateBounds();

		aMeshFilter.mesh = spotMesh;

		return aSpotObject;
	}

	public override Vector2 GetPositionForSpace(Vector2Int theSpace) {
		GameObject spotObjectForSpace = _spotObjects[GetIndexForSpace(theSpace)];
		return spotObjectForSpace.transform.position;
	}

	public override int GetIndexForSpace(Vector2Int theSpace) {
		return theSpace.x + (theSpace.y * _numColumns);
	}

	public override Vector2 GetSizeOfGrid() {
		float gridWidth = _numColumns * _gridSpaceSize + (_numColumns - 1) * _borderSize;
		float gridHeight = _numRows * _gridSpaceSize + (_numRows - 1) * _borderSize;
		return new Vector2(gridWidth, gridHeight);
	}

	public override int GetNumberOfSpaces() {
		return _numColumns * _numRows;
	}

	public override List<Vector2Int> GetListOfStartingSpaces() {
		List<Vector2Int> startingSpaces = new List<Vector2Int>(2 * (_numRows - 1) + 2 * (_numColumns - 1));

		//populate the list with the perimeter spaces
		for (int i = 0; i < _numRows; i++) {

			if (i == 0 || i == _numRows - 1) {
				for (int j = 0; j < _numColumns; j++) {
					startingSpaces.Add(new Vector2Int(j, i));
				}
			} else {
				startingSpaces.Add(new Vector2Int(0, i));
				startingSpaces.Add(new Vector2Int(_numColumns - 1, i));
			}
		}
		
		return startingSpaces;
	}

	public override Vector2Int? GetSpaceAfterMove(Vector2Int theCurrentSpace, Vector2Int theMoveOffset) {
		throw new System.NotImplementedException();
	}

	public override float GetRotationForUpFacingDirection(Vector2Int currentSpace) {
		return 90f;
	}

	public override Vector2 GetWaitPositionForSpace(Vector2Int space) {
		return Vector2.zero;
	}

}
