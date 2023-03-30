using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class FishControl : NetworkBehaviour
{
	public float speed;
	
	[SyncVar]
	public bool run;
	
	[SyncVar]
	public float rotateZ;
	
	public float runEnergy;
	Quaternion RotatePlayer;
	private Camera cam;
	public Slider runSlider;
	public GameObject Canvas;
	public Text mathIdText;
	
	public int snakeID;
	
    void Start()
    {
		if (isServer)
		{
			snakeID = Random.Range(100000, 9999999);
		}
	
		if (isLocalPlayer)
		{
			cam = Camera.main;
			gameObject.tag = "LocalPlayer";
		}
		else
		{
			Canvas.SetActive(false);
		}
    }
	
	public void SetMathText(string mathID)
	{
		mathIdText.text = mathID;
	}

    void FixedUpdate()
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
			
			transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, rotateZ -90), speed / 10);
		}
		
		if (run == true && isLocalPlayer)
		{
			runSlider.value = runSlider.value - 0.1f;
		}
		else
		{
			runSlider.value = runSlider.value + 0.1f;
		}
    }
	
	void Update()
	{
		if (isLocalPlayer)
		{
			Vector3 diference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
			rotateZ = Mathf.Atan2(diference.y, diference.x) * Mathf.Rad2Deg;
		}
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
