using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Npc : NetworkBehaviour
{
	public float speed;
	public GameObject[] foods;
	public GameObject targetFood;
	
	[Server]
    void Start()
    {
        newTargetPos();
		Update1s();
    }
	
	[Server]
	void Update1s()
	{
		if (targetFood == null)
		{
			newTargetPos();
		}
		Invoke("Update1s", 1f);
	}
	
	[Server]
	void Update()
	{
		if (targetFood != null)
		{
			var step =  speed * Time.deltaTime;
			transform.position = Vector3.MoveTowards(transform.position, targetFood.transform.position, step);
		}
	}
	
	[Server]
	public void newTargetPos()
	{
		foods = GameObject.FindGameObjectsWithTag("Food");
		if (foods != null)
		{
			targetFood = foods[Random.Range(0, foods.Length)];
			Vector3 diference = targetFood.transform.position - transform.position;
			float rotateZ = Mathf.Atan2(diference.y, diference.x) * Mathf.Rad2Deg;
			transform.rotation = Quaternion.Euler(0f, 0f, rotateZ - 90);
		}
	}

    [Server]
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if(collision.gameObject.tag=="Food")
		{
			newTargetPos();
		}
	}
}
