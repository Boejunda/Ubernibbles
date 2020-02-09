using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameState : MonoBehaviour {

	public abstract void StartState(List<Character> theCharacters, Queue<inputData> theInputQueue);
	public abstract GameState UpdateState(List<Character> theCharacters, Queue<inputData> theInputQueue);

}
