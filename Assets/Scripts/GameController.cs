using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LiteNetLib;
using System;


public class GameController : MonoBehaviour {

	public static GameController instance;


	private GameState _currentGameState;
	[SerializeField] private GameState _startingState; //GameState_Inputting...

	[SerializeField] private Camera _camera;

	public static Action<Character> onCharacterAdded;
	public static Action<Character> onCharacterRemoved;

	private InputListener _inputListener;
	private Queue<inputData> _inputQueue;

	private List<Character> _characters;

	public GameLogic gameLogic;

	[SerializeField] private GameObject _charWaitPfb;


	private void Awake(){
		if (instance != null) {
			Destroy(gameObject);
			return;
		}
		instance = this;

		GridArea grid = new GridPolar(8, 74);
		gameLogic = new GameLogic(grid);

		_characters = new List<Character>(32);

		_inputListener = gameObject.AddComponent<InputListener>();
		_inputListener.Setup(9145, CreateCharacter, RemoveCharacter);
		_inputQueue = _inputListener.GetInputQueue();
	}

	private void Start() {
		_currentGameState = _startingState;
		_currentGameState.StartState(_characters, _inputQueue);
	}

	private void Update() {
		while (true) {
			GameState nextGameState = _currentGameState.UpdateState(_characters, _inputQueue);
			if (nextGameState == null) {
				return;
			}

			_currentGameState = nextGameState;
			_currentGameState.StartState(_characters, _inputQueue);
		}
	}

	public Character CreateCharacter(NetPeer thePeer) {
		Character character = new Character();
		_characters.Add(character);

		character.SetCharacterWaitObject(Instantiate(_charWaitPfb));
		character.SetStartingSpace(gameLogic.ReserveAStartingSpace(), gameLogic.GetGrid());

		if (onCharacterAdded != null) {
			onCharacterAdded(character);
		}
		return character;
	}

	public void RemoveCharacter(Character theCharToRemove) {
		_characters.Remove(theCharToRemove);

		if (onCharacterRemoved != null) {
			onCharacterRemoved(theCharToRemove);
		}
	}

}
