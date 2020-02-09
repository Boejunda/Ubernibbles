using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridPolar : GridArea {

	private Transform _gridTrans;

	public const float radius = 7.1f;
	public const float borderSize = 0.07f;
	public const float borderSizeHalf = borderSize * 0.5f;


	public GridPolar(int theNumRows, int theNumColumns) {

		_numRows = theNumRows;
		_numColumns = theNumColumns;
		_spotObjects = new GameObject[_numRows * _numColumns];

		_gridTrans = new GameObject("grid").transform;

		float radianWidthOfColumn = (2f * Mathf.PI) / theNumColumns;


		int currentIndex = 0;
		float currentRadianOffset = 0f;
		float currentRadius = radius;

		for (int row = 0; row < _numRows; row++) {
			float rowHeightAdjust = 0f;
			for (int column = 0; column < _numColumns; column++) {
				GameObject aSpotObject = new GameObject(string.Format("spot ({0},{1})", column, row));
				aSpotObject.transform.SetParent(_gridTrans, true);
				MeshFilter aMeshFilter = aSpotObject.AddComponent<MeshFilter>();
				aSpotObject.AddComponent<MeshRenderer>();

				Mesh spotMesh = new Mesh();

				Vector3 farVector = new Vector3(0f, currentRadius, 0f);
				Vector3 vertex0 = Quaternion.Euler(0f, 0f, currentRadianOffset * Mathf.Rad2Deg) * farVector;
				Vector3 vertex1 = Quaternion.Euler(0f, 0f, (currentRadianOffset + radianWidthOfColumn) * Mathf.Rad2Deg) * farVector;

				rowHeightAdjust = Vector3.Distance(vertex0, vertex1);

				Vector3 nearVector = new Vector3(0f, currentRadius - rowHeightAdjust + borderSize, 0f);
				Vector3 vertex2 = Quaternion.Euler(0f, 0f, (currentRadianOffset + radianWidthOfColumn) * Mathf.Rad2Deg) * nearVector; 
				Vector3 vertex3 = Quaternion.Euler(0f, 0f, currentRadianOffset * Mathf.Rad2Deg) * nearVector;

				Vector3 from3To0 = (vertex0 - vertex3).normalized;
				Vector3 offsetSide1ForBorder = Vector3.Cross(from3To0, Vector3.back) * borderSizeHalf;
				vertex0 += offsetSide1ForBorder;
				vertex3 += offsetSide1ForBorder;

				Vector3 from2To1 = (vertex1 - vertex2).normalized;
				Vector3 offsetSide2ForBorder = Vector3.Cross(from2To1, Vector3.forward) * borderSizeHalf;
				vertex1 += offsetSide2ForBorder;
				vertex2 += offsetSide2ForBorder;


				Vector3 center = (vertex0 + vertex1 + vertex2 + vertex3) * 0.25f;
				vertex0 -= center;
				vertex1 -= center;
				vertex2 -= center;
				vertex3 -= center;

				aSpotObject.transform.localPosition = center;

				Vector3[] vertices = new Vector3[] { vertex0, vertex1, vertex2, vertex3 };

				int[] tris = new int[] {0, 3, 1, 3, 2, 1};

				spotMesh.vertices = vertices;
				spotMesh.triangles = tris;

				spotMesh.RecalculateNormals();
				spotMesh.RecalculateBounds();
				

				aMeshFilter.mesh = spotMesh;

				_spotObjects[currentIndex] = aSpotObject;

				currentRadianOffset += radianWidthOfColumn;

				currentIndex++;
			}
			currentRadianOffset = 0f;
			currentRadius -= rowHeightAdjust;
		}
	}


	public override Vector2 GetPositionForSpace(Vector2Int theSpace) {
		GameObject spotObjectForSpace = _spotObjects[GetIndexForSpace(theSpace)];
		return spotObjectForSpace.transform.position;
	}


	public override int GetIndexForSpace(Vector2Int theSpace) {
		return theSpace.x + theSpace.y * _numColumns;
	}

	public override Vector2 GetSizeOfGrid() {
		float gridWidth = radius * 2f;
		float gridHeight = radius * 2f;
		return new Vector2(gridWidth, gridHeight);
	}


	public override int GetNumberOfSpaces() {
		return _numColumns * _numRows;
	}


	public override List<Vector2Int> GetListOfStartingSpaces() {
		List<Vector2Int> startingSpaces = new List<Vector2Int>(_numColumns);
		for (int i = 0; i < _numColumns; i++) {
			startingSpaces.Add(new Vector2Int(i, 0));
		}

		return startingSpaces;
	}


	public override Vector2Int? GetSpaceAfterMove(Vector2Int theCurrentSpace, Vector2Int theMoveOffset) {
		Vector2Int updatedSpace = new Vector2Int((theCurrentSpace.x + theMoveOffset.x + _numColumns) % _numColumns, theCurrentSpace.y + theMoveOffset.y);
		// x wraps so only have to worry about y
		if (updatedSpace.y < 0 || updatedSpace.y >= _numRows) {
			return null;
		}
		return updatedSpace;
	}


	public override float GetRotationForUpFacingDirection(Vector2Int currentSpace) {
		//the "up" direction will *always* be towards the center of the polar grid
		Vector2 currentSpacePosToGridCenter = (Vector2)_gridTrans.position - GetPositionForSpace(currentSpace);
		float angleToFaceGridCenter = Mathf.Atan2(currentSpacePosToGridCenter.y, currentSpacePosToGridCenter.x) * Mathf.Rad2Deg;
		return angleToFaceGridCenter;
	}

	public override Vector2 GetWaitPositionForSpace(Vector2Int space) {
		Vector2 posForSpace = GetPositionForSpace(space);

		Vector2 centerToPosForSpace = posForSpace - Vector2.zero;
		Vector2 normalizedFromCenter = centerToPosForSpace.normalized;
		float radiusForWait = radius + 0.2f;
		return normalizedFromCenter * radiusForWait;
	}

}
