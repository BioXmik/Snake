using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class FishControl : NetworkBehaviour
{
	public float speed;
	public bool run;
	public float runEnergy;
	Quaternion RotatePlayer;
	private Camera cam;
	public Slider runSlider;
	public GameObject Canvas;
	public Text mathIdText;
	
	[Client]
    void Start()
    {
		if (isLocalPlayer)
		{
			cam = Camera.main;
			gameObject.tag = "LocalPlayer";
		}
		else
		{
			Canvas.SetActive(false);
		}
		Update01s();
    }
	
	public void SetMathText(string mathID)
	{
		mathIdText.text = mathID;
	}

    void Update()
    {
		if (isServer)
		{
			if (run == true && runEnergy > 0)
			{
				transform.Translate(Vector3.up * speed * 2 * Time.deltaTime, Space.Self);
				runEnergy = runEnergy - 0.1f;
			}
			else
			{
				transform.Translate(Vector3.up * speed * Time.deltaTime, Space.Self);
				if (runEnergy < 100)
				{
					runEnergy = runEnergy + 0.1f;
				}
			}
		}
		
		if (run == true)
		{
			runSlider.value = runSlider.value - 0.1f;
		}
		else
		{
			runSlider.value = runSlider.value + 0.1f;
		}
    }
	
	[Client]
	public void Update01s()
	{
		Vector3 diference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
		float rotateZ = Mathf.Atan2(diference.y, diference.x) * Mathf.Rad2Deg;
		CmdSetRotate(rotateZ);
		Invoke("Update01s", 0.05f);
	}
	
	[Command]
	public void CmdSetRotate(float rotateZ)
	{
		transform.rotation = Quaternion.Euler(0f, 0f, rotateZ - 90);
	}
	
	[Command]
	public void CmdSetRun(bool newRunBool)
	{
		run = newRunBool;
		RpcSyncRunEnergy(GetComponent<NetworkIdentity>().connectionToClient, runEnergy);
	}
	
	[TargetRpc]
    public void RpcSyncRunEnergy(NetworkConnection target, float newRunEnergy)
    {
		runSlider.value = newRunEnergy;
    }
}
