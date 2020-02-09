using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;


public class GameState_Inputting : GameState {

	private GameState _gameState_Executing;

	[SerializeField] private TMP_Text _inputText;
	[SerializeField] private TMP_Text _countDownText;
	private Transform _countDownTrans;

	private const int _inputDuration = 12;
	private bool _goToNextState;

	private int _roundNum;

	[SerializeField] private GameObject _snakePfb;


	private void Awake() {
		_gameState_Executing = GetComponent<GameState_Executing>();
		_countDownTrans = _countDownText.transform;
	}

	public override void StartState(List<Character> theCharacters, Queue<inputData> theInputQueue) {
		_goToNextState = false;

		GameLogic game = GameController.instance.gameLogic;

		theInputQueue.Clear();

		bool charsGotNewStartingSpace = false;

		_roundNum++;
		if (_roundNum % 3 == 0) {
			charsGotNewStartingSpace = true;
			//characters get new starting spaces?
			game.ResetAvailableSpace();
			for (int i = 0; i < theCharacters.Count; i++) {
				theCharacters[i].SetStartingSpace(game.ReserveAStartingSpace(), game.GetGrid());
			}
		}

		StartCoroutine(Coordinator(charsGotNewStartingSpace, theCharacters, game));

	}

	IEnumerator Coordinator(bool theCharsGotNewStartingSpaces, List<Character> theCharacters, GameLogic theGame) {

		if (theCharsGotNewStartingSpaces) {
			yield return new WaitForSeconds(2f);
		}

		for (int i = 0; i < theCharacters.Count; i++) {
			Character aChar = theCharacters[i];
			aChar.ClearInputQueue();

			Snake snake = Instantiate(_snakePfb, Vector3.zero, Quaternion.identity).GetComponent<Snake>();
			snake.AddControllingCharacter(aChar);
			theGame.RegisterCharControlsSnake(aChar, snake);
			theGame.PlaceNewSnake(snake, aChar.GetStartingSpace());
		}


		//do whatever intro animation here then...
		//

		_inputText.gameObject.SetActive(true);
		_countDownText.gameObject.SetActive(true);

		int durationRemaining = _inputDuration;
		while (durationRemaining > 0) {
			_countDownText.text = durationRemaining.ToString();
			_countDownTrans.localScale = Vector3.zero;
			_countDownTrans.DOScale(Vector3.one, 0.8f).SetEase(Ease.OutCirc);
			yield return new WaitForSeconds(1f);
			durationRemaining--;
		}

		_inputText.gameObject.SetActive(false);
		_countDownText.gameObject.SetActive(false);

		yield return new WaitForSeconds(0.4f);

		_goToNextState = true;
	}

	public override GameState UpdateState(List<Character> theCharacters, Queue<inputData> theInputQueue) {
		if (_goToNextState) {
			return _gameState_Executing;
		}

		while (theInputQueue.Count > 0) {
			inputData anInputData = theInputQueue.Dequeue();
			Character charDoingInput = anInputData.character;

			charDoingInput.QueueAnInput(anInputData.direction);

			charDoingInput.DisplayEnterInput();
		}

		return null; //don't change state
	}

}
