using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class FoodScore : NetworkBehaviour
{
	public int score;
	public GameObject partsBodyPlayer;
	public GameObject partsBodyPlayerPrefab;
	public GameObject fishBodyPrefab;
	
	public int maxCordSpawnFood;
	public GameObject foodPrefab;
	
	public int snakeID;
	
	[Server]
	void Start()
	{
		if (GetComponent<Npc>() != null)
		{
			snakeID = GetComponent<Npc>().snakeID;
		}
		
		if (GetComponent<FishControl>() != null)
		{
			snakeID = GetComponent<FishControl>().snakeID;
		}
	}
	
	[Server]
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if(collision.gameObject.tag=="Food")
		{
			if (partsBodyPlayer != null)
			{
				NetworkServer.Destroy(collision.gameObject);
				score++;
				
				int newFoodPosX = Random.Range(-maxCordSpawnFood, maxCordSpawnFood);
				int newFoodPosY = Random.Range(-maxCordSpawnFood, maxCordSpawnFood);
				Vector2 newFoodSpawnPos = new Vector2 (newFoodPosX, newFoodPosY);
				
				GameObject foodObject = Instantiate(foodPrefab, newFoodSpawnPos, Quaternion.identity);
				foodObject.GetComponent<NetworkMatch>().matchId = collision.gameObject.GetComponent<NetworkMatch>().matchId;
				NetworkServer.Spawn(foodObject);
				
				GameObject fishBody = Instantiate(fishBodyPrefab, transform.position, Quaternion.identity, partsBodyPlayer.transform);
				fishBody.GetComponent<BodyPart>().snakeID = snakeID;
				fishBody.GetComponent<NetworkMatch>().matchId = collision.gameObject.GetComponent<NetworkMatch>().matchId;
				NetworkServer.Spawn(fishBody);
				GetComponent<grow>().part = fishBody;
				GetComponent<grow>().AddPart(fishBody);
			}
			else
			{
				partsBodyPlayer = Instantiate(partsBodyPlayerPrefab);
				NetworkServer.Spawn(partsBodyPlayer);
				
				NetworkServer.Destroy(collision.gameObject);
				score++;
				
				int newFoodPosX = Random.Range(-maxCordSpawnFood, maxCordSpawnFood);
				int newFoodPosY = Random.Range(-maxCordSpawnFood, maxCordSpawnFood);
				Vector2 newFoodSpawnPos = new Vector2 (newFoodPosX, newFoodPosY);
				
				GameObject foodObject = Instantiate(foodPrefab, newFoodSpawnPos, Quaternion.identity);
				foodObject.GetComponent<NetworkMatch>().matchId = collision.gameObject.GetComponent<NetworkMatch>().matchId;
				NetworkServer.Spawn(foodObject);
				
				GameObject fishBody = Instantiate(fishBodyPrefab, transform.position, Quaternion.identity, partsBodyPlayer.transform);
				fishBody.GetComponent<BodyPart>().snakeID = snakeID;
				fishBody.GetComponent<NetworkMatch>().matchId = collision.gameObject.GetComponent<NetworkMatch>().matchId;
				NetworkServer.Spawn(fishBody);
				GetComponent<grow>().part = fishBody;
				GetComponent<grow>().AddPart(fishBody);
			}
		}
	}
}
