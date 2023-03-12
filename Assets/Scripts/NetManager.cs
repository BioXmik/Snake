using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetManager : NetworkManager
{
	public int lobbyOnline;
	
	public override void OnServerConnect(NetworkConnectionToClient conn)
	{
		lobbyOnline++;
	}
	
	public override void OnServerDisconnect(NetworkConnectionToClient conn)
	{
		lobbyOnline--;
	}
}
