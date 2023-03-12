using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class Cords : NetworkBehaviour
{
	public Text CordsText;
	
	void Start()
	{
		if (!isLocalPlayer)
		{
			this.enabled = false;
		}
	}
	
	void Update()
	{
		CordsText.text = "X " + Mathf.Round(transform.position.x) + "    " + "Y " + Mathf.Round(transform.position.y);
	}
}
