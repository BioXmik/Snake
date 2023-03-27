using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random=UnityEngine.Random;

namespace MirrorBasics {

    [RequireComponent (typeof (NetworkMatch))]
    public class Player : NetworkBehaviour {

        public static Player localPlayer;
        [SyncVar] public string matchID;
        [SyncVar] public int playerIndex;

        NetworkMatch networkMatch;

        [SyncVar] public Match currentMatch;

        [SerializeField] GameObject playerLobbyUI;

        Guid netIDGuid;
		
		public int maxCountFood;
		public int maxCordSpawn;
		public GameObject foodPrefab;
		
		public int maxCountNpc;
		public GameObject npcPrefab;

        void Awake () {
            networkMatch = GetComponent<NetworkMatch> ();
        }

        public override void OnStartServer () {
            netIDGuid = netId.ToString ().ToGuid ();
            networkMatch.matchId = netIDGuid;
        }

        public override void OnStartClient () {
            if (isLocalPlayer) {
                localPlayer = this;
            } else {
                Debug.Log ($"Spawning other player UI Prefab");
                playerLobbyUI = UILobby.instance.SpawnPlayerUIPrefab (this);
            }
        }

        public override void OnStopClient () {
            Debug.Log ($"Client Stopped");
            ClientDisconnect ();
        }

        public override void OnStopServer () {
            Debug.Log ($"Client Stopped on Server");
            ServerDisconnect ();
        }

        /* 
            HOST MATCH
        */

        public void HostGame (bool publicMatch, int countCristals, int countPlayers)
		{
            string matchID = MatchMaker.GetRandomMatchID ();
            CmdHostGame (matchID, publicMatch, countCristals, countPlayers);
        }

        [Command]
        void CmdHostGame (string _matchID, bool publicMatch, int countCristals, int countPlayers) {
            matchID = _matchID;
            if (MatchMaker.instance.HostGame (_matchID, this, publicMatch, out playerIndex)) {
                Debug.Log ($"<color=green>Game hosted successfully</color>");
                networkMatch.matchId = _matchID.ToGuid ();
				GetComponent<NetworkMatch>().matchId = _matchID.ToGuid ();
				
				for (int i = 0; i < countCristals; i++)
				{
					int newFoodPosX = Random.Range(-maxCordSpawn, maxCordSpawn);
					int newFoodPosY = Random.Range(-maxCordSpawn, maxCordSpawn);
					Vector2 newFoodSpawnPos = new Vector2 (newFoodPosX, newFoodPosY);
					
					GameObject foodObject = Instantiate(foodPrefab, newFoodSpawnPos, Quaternion.identity);
					foodObject.GetComponent<NetworkMatch>().matchId = _matchID.ToGuid ();
					NetworkServer.Spawn(foodObject);
				}
				
				for (int i = 0; i < maxCountNpc; i++)
				{
					int newFoodPosX = Random.Range(-maxCordSpawn, maxCordSpawn);
					int newFoodPosY = Random.Range(-maxCordSpawn, maxCordSpawn);
					Vector2 newNpcSpawnPos = new Vector2 (newFoodPosX, newFoodPosY);
					
					GameObject npcObject = Instantiate(npcPrefab, newNpcSpawnPos, Quaternion.identity);
					npcObject.GetComponent<NetworkMatch>().matchId = _matchID.ToGuid ();
					NetworkServer.Spawn(npcObject, connectionToClient);
				}
				
                TargetHostGame (true, _matchID, countPlayers);
            } else {
                Debug.Log ($"<color=red>Game hosted failed</color>");
                TargetHostGame (false, _matchID, countPlayers);
            }
        }

        [TargetRpc]
        void TargetHostGame (bool success, string _matchID, int _playerIndex) {
            playerIndex = _playerIndex;
            matchID = _matchID;
            Debug.Log ($"MatchID: {matchID} == {_matchID}");
            UILobby.instance.HostSuccess (success, _matchID);
        }

        /* 
            JOIN MATCH
        */

        public void JoinGame (string _inputID) {
            CmdJoinGame (_inputID);
        }

        [Command]
        void CmdJoinGame (string _matchID) {
            matchID = _matchID;
            if (MatchMaker.instance.JoinGame (_matchID, this, out playerIndex)) {
                Debug.Log ($"<color=green>Game Joined successfully</color>");
                networkMatch.matchId = _matchID.ToGuid ();
                TargetJoinGame (true, _matchID, playerIndex);

                //Host
                if (isServer && playerLobbyUI != null) {
                    playerLobbyUI.SetActive (true);
                }
            } else {
                Debug.Log ($"<color=red>Game Joined failed</color>");
                TargetJoinGame (false, _matchID, playerIndex);
            }
        }

        [TargetRpc]
        void TargetJoinGame (bool success, string _matchID, int _playerIndex) {
            playerIndex = _playerIndex;
            matchID = _matchID;
            Debug.Log ($"MatchID: {matchID} == {_matchID}");
            UILobby.instance.JoinSuccess (success, _matchID);
        }

        /* 
            DISCONNECT
        */

        public void DisconnectGame () {
            CmdDisconnectGame ();
        }

        [Command]
        void CmdDisconnectGame () {
            ServerDisconnect ();
        }

        void ServerDisconnect () {
            MatchMaker.instance.PlayerDisconnected (this, matchID);
            RpcDisconnectGame ();
            networkMatch.matchId = netIDGuid;
        }

        [ClientRpc]
        void RpcDisconnectGame () {
            ClientDisconnect ();
        }

        void ClientDisconnect () {
            if (playerLobbyUI != null) {
                if (!isServer) {
                    Destroy (playerLobbyUI);
                } else {
                    playerLobbyUI.SetActive (false);
                }
            }
        }

        /* 
            SEARCH MATCH
        */

        public void SearchGame () {
            CmdSearchGame ();
        }

        [Command]
        void CmdSearchGame () {
            if (MatchMaker.instance.SearchGame (this, out playerIndex, out matchID)) {
                Debug.Log ($"<color=green>Game Found Successfully</color>");
                networkMatch.matchId = matchID.ToGuid ();
                TargetSearchGame (true, matchID, playerIndex);

                //Host
                if (isServer && playerLobbyUI != null) {
                    playerLobbyUI.SetActive (true);
                }
            } else {
                Debug.Log ($"<color=red>Game Search Failed</color>");
                TargetSearchGame (false, matchID, playerIndex);
            }
        }

        [TargetRpc]
        void TargetSearchGame (bool success, string _matchID, int _playerIndex) {
            playerIndex = _playerIndex;
            matchID = _matchID;
            Debug.Log ($"MatchID: {matchID} == {_matchID} | {success}");
            UILobby.instance.SearchGameSuccess (success, _matchID);
        }

        /* 
            MATCH PLAYERS
        */

        [Server]
        public void PlayerCountUpdated (int playerCount) {
            TargetPlayerCountUpdated (playerCount);
        }

        [TargetRpc]
        void TargetPlayerCountUpdated (int playerCount) {
            if (playerCount > 1) {
                UILobby.instance.SetStartButtonActive(true);
            } else {
                UILobby.instance.SetStartButtonActive(false);
            }
        }

        /* 
            BEGIN MATCH
        */

        public void BeginGame () {
            CmdBeginGame ();
        }

        [Command]
        void CmdBeginGame () {
            MatchMaker.instance.BeginGame (matchID);
            Debug.Log ($"<color=red>Game Beginning</color>");
        }

        public void StartGame () { //Server
            TargetBeginGame ();
        }

        [TargetRpc]
        void TargetBeginGame () {
			GameObject.FindGameObjectWithTag("LocalPlayer").GetComponent<FishControl>().SetMathText(matchID);
            Debug.Log ($"MatchID: {matchID} | Beginning");
            //Additively load game scene
			GameObject.FindGameObjectWithTag("RoomUI").SetActive(false);
            //SceneManager.LoadScene (2, LoadSceneMode.Additive);
        }

    }
}