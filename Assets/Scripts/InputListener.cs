using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LiteNetLib;
using System;

public class InputListener : MonoBehaviour {

	public int portNum;

	private NetManager _netManager;

	private Func<NetPeer, Character> makeCharacter;
	private Action<Character> removeCharacter;

	Dictionary<NetPeer, Character> _peerToCharacter;

	private Queue<inputData> _inputQueue;

	public void Setup(int thePort, Func<NetPeer, Character> theMakeCharacter, Action<Character> theRemoveCharacter) {

		makeCharacter = theMakeCharacter;
		removeCharacter = theRemoveCharacter;

		_peerToCharacter = new Dictionary<NetPeer, Character>(32);
		_inputQueue = new Queue<inputData>(32);

		EventBasedNetListener listener = new EventBasedNetListener();
		listener.ConnectionRequestEvent += Listener_ConnectionRequestEvent;
		listener.PeerConnectedEvent += Listener_PeerConnectedEvent;
		listener.PeerDisconnectedEvent += Listener_PeerDisconnectedEvent;
		listener.NetworkReceiveEvent += Listener_NetworkReceiveEvent;

		_netManager = new NetManager(listener);
		_netManager.Start(9145);
	}

	public void Update() {
		_netManager.PollEvents();
	}

	public Queue<inputData> GetInputQueue() {
		return _inputQueue;
	}

	private void Listener_ConnectionRequestEvent(ConnectionRequest request) {
		Debug.Log("got connection request");
		request.AcceptIfKey("hello");
	}

	private void Listener_PeerConnectedEvent(NetPeer peer) {
		Debug.Log("peer connected");
		_peerToCharacter.Add(peer, makeCharacter(peer));
	}

	private void Listener_PeerDisconnectedEvent(NetPeer peer, DisconnectInfo disconnectInfo) {
		Debug.LogFormat("peer disconnected. reason: {0}", disconnectInfo.Reason.ToString());
		removeCharacter(_peerToCharacter[peer]);
		_peerToCharacter.Remove(peer);
	}

	private void Listener_NetworkReceiveEvent(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod) {
		byte packetType = reader.GetByte();
		switch (packetType) {
			case PacketType.INPUT:
				int inputDirInt = reader.GetInt();
				if (Enum.IsDefined(typeof(Direction), inputDirInt)) {
					_inputQueue.Enqueue(new inputData(_peerToCharacter[peer], (Direction)inputDirInt));
				}
				break;
			default:
				Debug.LogWarning("received *odd* message");
				break;
		}
	}


}
