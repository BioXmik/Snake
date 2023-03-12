using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class FoodScore : NetworkBehaviour
{
	public int score;
	public GameObject fishBodyPrefab;
	
	public int maxCordSpawnFood;
	public GameObject foodPrefab;
	
	public int playerID;
	
	[Server]
	void Start()
	{
		playerID = Random.Range(100000, 9999999);
	}
	
	[Server]
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if(collision.gameObject.tag=="Food")
		{
			NetworkServer.Destroy(collision.gameObject);
			score++;
			
			int newFoodPosX = Random.Range(-maxCordSpawnFood, maxCordSpawnFood);
			int newFoodPosY = Random.Range(-maxCordSpawnFood, maxCordSpawnFood);
			Vector2 newFoodSpawnPos = new Vector2 (newFoodPosX, newFoodPosY);
			
			GameObject foodObject = Instantiate(foodPrefab, newFoodSpawnPos, Quaternion.identity);
			foodObject.GetComponent<NetworkMatch>().matchId = collision.gameObject.GetComponent<NetworkMatch>().matchId;
			NetworkServer.Spawn(foodObject);
			
			GameObject fishBody = Instantiate(fishBodyPrefab, transform.position, Quaternion.identity);
			fishBody.GetComponent<BodyPart>().playerID = playerID;
			fishBody.GetComponent<NetworkMatch>().matchId = collision.gameObject.GetComponent<NetworkMatch>().matchId;
			NetworkServer.Spawn(fishBody);
			GetComponent<grow>().part = fishBody;
			GetComponent<grow>().AddPart(fishBody);
		}
	}
}
