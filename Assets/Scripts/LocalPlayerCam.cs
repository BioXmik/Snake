using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalPlayerCam : MonoBehaviour
{
	void OnBecameInvisible()
    {
        enabled = false;
    }
	
	void OnBecameVisible()
	{
        enabled = true;
    }
	
    void Update()
    {
		GameObject player = GameObject.FindGameObjectWithTag("LocalPlayer");
		if (player != null)
		{
			transform.position = new Vector3 (player.transform.position.x, player.transform.position.y, player.transform.position.z - 20);
			if (GameObject.FindGameObjectWithTag("LocalPlayer").GetComponent<FoodScore>().score < 50)
			{
				Camera.main.orthographicSize = 7f;
			}
			else
			{
				Camera.main.orthographicSize = GameObject.FindGameObjectWithTag("LocalPlayer").GetComponent<FoodScore>().score / 7f;
			}
		}
	}
}
