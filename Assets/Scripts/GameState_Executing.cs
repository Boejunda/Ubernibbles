using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class GameState_Executing : GameState {

	private GameState _gameState_Victory;

	[SerializeField] private TMP_Text _executingText;
	private Transform _executingTextTrans;

	private const float _initialTickDuration = 0.4f;
	private float _durationTillNextTick;
	private float _timer;


	private void Awake() {
		_gameState_Victory = GetComponent<GameState_Victory>();
		_executingTextTrans = _executingText.GetComponent<Transform>();

		_executingText.gameObject.SetActive(false);
	}

	public override void StartState(List<Character> theCharacters, Queue<inputData> theInputQueue) {
		_executingText.gameObject.SetActive(true);
		_executingTextTrans.localScale = Vector3.zero;
		_executingTextTrans.DOScale(1f, 1.2f).SetEase(Ease.OutQuart);


		_durationTillNextTick = _initialTickDuration;
		_timer = 0f;
	}

	public override GameState UpdateState(List<Character> theCharacters, Queue<inputData> theInputQueue) {

		_timer += Time.deltaTime;

		GameLogic game = GameController.instance.gameLogic;

		bool gameStillInProgress = true;

		while (_timer > _durationTillNextTick) {
			//execute 1 step of the move queue in a character
			gameStillInProgress = false;

			for (int i = 0; i < theCharacters.Count; i++) {
				Direction? nextInput = theCharacters[i].GetNextInput();
				if (nextInput.HasValue) {
					bool movedGameForward = game.ExecuteInput(theCharacters[i], nextInput.Value);
					if (movedGameForward) {
						gameStillInProgress = true;
					}
				}
			}

			bool shouldPauseToShowDemolishedSnake = game.EvaluateGridState();
			if (shouldPauseToShowDemolishedSnake) {
				_timer -= 1f;
			}

			_timer -= _durationTillNextTick;
			_durationTillNextTick = Mathf.Max(0.1f, _durationTillNextTick - 0.01f); //speed up the ticks over the first # of moves
		}

		//could maybe use the inputQueue to shine/shake the corresponding snake (another way for the player to figure out who they are...)

		if (gameStillInProgress) {
			return null;
		} else {
			_executingText.gameObject.SetActive(false);
			return _gameState_Victory;
		}
	}


}
