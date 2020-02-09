using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class GameState_Victory : GameState {

	private GameState _gameState_Inputting;

	[SerializeField] private TMP_Text _resultText;
	[SerializeField] private TMP_Text _pathLengthText;

	private float _countdown;
	public const float durationWhenHaveVictor = 3.5f;
	private const float _durationWhenNoVictor = 2f;



	private void Awake() {
		_gameState_Inputting = GetComponent<GameState_Inputting>();
	}

	public override void StartState(List<Character> theCharacters, Queue<inputData> theInputQueue) {
		Debug.Log("starting victory state");

		_resultText.gameObject.SetActive(true);
		_pathLengthText.gameObject.SetActive(true);

		//figure out who, if anyone, won
		GameLogic game = GameController.instance.gameLogic;

		List<Snake> aliveSnakes = game.GetAliveSnakes();

		if (aliveSnakes.Count > 0) {
			_countdown = durationWhenHaveVictor;

			int longestPath = int.MinValue;
			for (int i = 0; i < aliveSnakes.Count; i++) {
				int aliveSnakeLength = aliveSnakes[i].GetLength();
				if (aliveSnakeLength > longestPath) {
					longestPath = aliveSnakeLength;
				}
			}

			int numWinningSnakes = 0;
			for (int i = 0; i < aliveSnakes.Count; i++) {
				if (aliveSnakes[i].GetLength() == longestPath) {
					aliveSnakes[i].DisplayAsWinner();
					numWinningSnakes++;
				}
			}

			if (numWinningSnakes > 1) {
				_resultText.text = "Winners!";
			} else {
				_resultText.text = "Winner!";
			}

			_pathLengthText.text = longestPath.ToString();

		} else {
			_countdown = _durationWhenNoVictor;
			_resultText.text = "hm.";
			_pathLengthText.gameObject.SetActive(false);
		}

	}

	public override GameState UpdateState(List<Character> theCharacters, Queue<inputData> theInputQueue) {
		_countdown -= Time.deltaTime;

		if (_countdown < 0f) {
			_resultText.gameObject.SetActive(false);
			_pathLengthText.gameObject.SetActive(false);
			GameController.instance.gameLogic.Reset();
			return _gameState_Inputting;
		}

		return null; //don't change state
	}


}
