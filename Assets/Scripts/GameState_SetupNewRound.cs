using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;


public class GameState_SetupNewRound : GameState {

	private GameState _gameState_Inputting;

	private bool _goToNextState;

	private int _roundNum;

	[SerializeField] private GameObject _snakePfb;

	[SerializeField] private TMP_Text _newRoundText;


	private void Awake() {
		_gameState_Inputting = GetComponent<GameState_Inputting>();
	}

	public override void StartState(List<Character> theCharacters, Queue<inputData> theInputQueue) {
		_goToNextState = false;

		GameLogic gameLogic = GameController.instance.gameLogic;

		bool charsGotNewStartingSpace = false;

		_roundNum++;
		if (_roundNum % 3 == 0) {
			charsGotNewStartingSpace = true;
			//characters get new starting spaces?
			gameLogic.ResetAvailableSpace();
			for (int i = 0; i < theCharacters.Count; i++) {
				theCharacters[i].SetStartingSpace(gameLogic.ReserveAStartingSpace(), gameLogic.GetGrid());
			}
		}

		StartCoroutine(Coordinator(charsGotNewStartingSpace, theCharacters, gameLogic));

	}

	IEnumerator Coordinator(bool theCharsGotNewStartingSpaces, List<Character> theCharacters, GameLogic theGame) {

		_newRoundText.gameObject.SetActive(true);

		if (theCharsGotNewStartingSpaces) {
			yield return new WaitForSeconds(2f);
		}

		yield return new WaitForSeconds(0.7f);


		for (int i = 0; i < theCharacters.Count; i++) {
			Character aChar = theCharacters[i];
			aChar.ClearInputQueue();

			Snake snake = Instantiate(_snakePfb, Vector3.zero, Quaternion.identity).GetComponent<Snake>();
			snake.AddControllingCharacter(aChar);
			theGame.RegisterCharControlsSnake(aChar, snake);
			theGame.PlaceNewSnake(snake, aChar.GetStartingSpace());
		}

		_newRoundText.gameObject.SetActive(false);

		_goToNextState = true;
	}

	public override GameState UpdateState(List<Character> theCharacters, Queue<inputData> theInputQueue) {
		if (_goToNextState) {
			return _gameState_Inputting;
		}

		return null; //don't change state
	}

}