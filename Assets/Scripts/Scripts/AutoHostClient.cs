using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class AutoHostClient : MonoBehaviour
{
	public Text StatusText;
	[SerializeField] NetworkManager networkManager;

	public void ConnectToServer()
	{
		StatusText.text = "Подключение...";
		networkManager.StartClient();
	}
}