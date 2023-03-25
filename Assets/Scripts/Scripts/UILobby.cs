using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace MirrorBasics {

    public class UILobby : MonoBehaviour {

        public static UILobby instance;

        [Header ("Host Join")]
        [SerializeField] InputField joinMatchInput;
        [SerializeField] List<Selectable> lobbySelectables = new List<Selectable> ();
        [SerializeField] Canvas lobbyCanvas;
        [SerializeField] Canvas searchCanvas;
        bool searching = false;

        [Header ("Lobby")]
        [SerializeField] Transform UIPlayerParent;
        [SerializeField] GameObject UIPlayerPrefab;
        [SerializeField] Text matchIDText;
        [SerializeField] GameObject beginGameButton;

        GameObject localPlayerLobbyUI;
		
		public InputField countCrystals;
		public InputField countPlayers;

        void Start () {
            instance = this;
        }

        public void SetStartButtonActive (bool active) {
            beginGameButton.SetActive (active);
        }

        public void HostPublic () {
			if (countCrystals.text != "")
			{
				lobbySelectables.ForEach (x => x.interactable = false);
				
				Player.localPlayer.HostGame (true, int.Parse(countCrystals.text), int.Parse(countPlayers.text));
			}
        }

        public void HostPrivate () {
			if (countCrystals.text != "" && int.Parse(countCrystals.text) <= 400 && int.Parse(countPlayers.text) <= 12)
			{
				lobbySelectables.ForEach (x => x.interactable = false);
				
				Player.localPlayer.HostGame (false, int.Parse(countCrystals.text), int.Parse(countPlayers.text));
			}
        }

        public void HostSuccess (bool success, string matchID) {
            if (success) {
                lobbyCanvas.enabled = true;

                if (localPlayerLobbyUI != null) Destroy (localPlayerLobbyUI);
                localPlayerLobbyUI = SpawnPlayerUIPrefab (Player.localPlayer);
                matchIDText.text = matchID;
				BeginGame();
            } else {
                lobbySelectables.ForEach (x => x.interactable = true);
            }
        }

        public void Join () {
            lobbySelectables.ForEach (x => x.interactable = false);

            Player.localPlayer.JoinGame (joinMatchInput.text.ToUpper ());
        }

        public void JoinSuccess (bool success, string matchID) {
            if (success) {
                lobbyCanvas.enabled = true;

                if (localPlayerLobbyUI != null) Destroy (localPlayerLobbyUI);
                localPlayerLobbyUI = SpawnPlayerUIPrefab (Player.localPlayer);
                matchIDText.text = matchID;
				GameObject.FindGameObjectWithTag("RoomUI").SetActive(false);
            } else {
                lobbySelectables.ForEach (x => x.interactable = true);
            }
        }

        public void DisconnectGame () {
            if (localPlayerLobbyUI != null) Destroy (localPlayerLobbyUI);
            Player.localPlayer.DisconnectGame ();

            lobbyCanvas.enabled = false;
            lobbySelectables.ForEach (x => x.interactable = true);
        }

        public GameObject SpawnPlayerUIPrefab (Player player) {
            GameObject newUIPlayer = Instantiate (UIPlayerPrefab, UIPlayerParent);
            newUIPlayer.GetComponent<UIPlayer> ().SetPlayer (player);
            newUIPlayer.transform.SetSiblingIndex (player.playerIndex - 1);

            return newUIPlayer;
        }

        public void BeginGame () {
            Player.localPlayer.BeginGame ();
        }

        public void SearchGame () {
            StartCoroutine (Searching ());
        }

        public void CancelSearchGame () {
            searching = false;
        }

        public void SearchGameSuccess (bool success, string matchID) {
            if (success) {
                searchCanvas.enabled = false;
                searching = false;
                JoinSuccess (success, matchID);
            }
        }

        IEnumerator Searching () {
            searchCanvas.enabled = true;
            searching = true;

            float searchInterval = 1;
            float currentTime = 1;

            while (searching) {
                if (currentTime > 0) {
                    currentTime -= Time.deltaTime;
                } else {
                    currentTime = searchInterval;
                    Player.localPlayer.SearchGame ();
                }
                yield return null;
            }
            searchCanvas.enabled = false;
        }

    }
}