using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class LocalPlayerCam : NetworkBehaviour
{
	private Camera mainCamera;
	
	private void Awake()
	{
		mainCamera = Camera.main;
	}
	
	void Update()
	{
		if (!isLocalPlayer) return;
		CameraMovement();
	}
	
	private void CameraMovement()
	{
		mainCamera.transform.localPosition = new Vector3(transform.position.x, transform.position.y, -1f);
		transform.position = Vector2.MoveTowards(transform.position, mainCamera.transform.localPosition, Time.deltaTime);
	}
}
