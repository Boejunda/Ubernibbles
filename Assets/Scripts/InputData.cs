using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct inputData {
	public Character character;
	public Direction direction;

	public inputData(Character theChar, Direction theDirection) {
		character = theChar;
		direction = theDirection;
	}
}