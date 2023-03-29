using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class grow : NetworkBehaviour
{
	public List<GameObject> Parts = new List<GameObject>();
	private Vector3 _start_scale;
	private float _gap = 2;
	public GameObject part;
	public GameObject foodPrefab;
	
	[Server]
	private void Awake()
    {
		Parts.Add(this.gameObject);
    }
	
	[Server]
    private void Start()
    {
        _start_scale = transform.localScale;
    }
	
	[Server]
	public void AddPart(GameObject part)
	{
		Parts.Add(this.part);
	}
	
	[Server]
    public void FixedUpdate()
	{
		float distance = 0;
        if (Parts.Count > 1)
        {
            distance = ((Vector2)Parts[1].transform.position - (Vector2)Parts[0].transform.position).magnitude;
        }
		
		for (int n = 1; n < Parts.Count; n++)
        {
			if (GetComponent<FishControl>() != null)
			{
				if(GetComponent<FishControl>().run)
				{
					Parts[n].transform.position = Vector2.Lerp(Parts[n].transform.position, Parts[n - 1].transform.position, distance / (_gap / (GetComponent<FishControl>().speed * 0.62f)));
					Parts[n].transform.rotation = Quaternion.Lerp(Parts[n].transform.rotation, Parts[n - 1].transform.rotation, 10 * Time.deltaTime);
				}
				else
				{
					Parts[n].transform.position = Vector2.Lerp(Parts[n].transform.position, Parts[n - 1].transform.position, distance / _gap);
					Parts[n].transform.rotation = Quaternion.Lerp(Parts[n].transform.rotation, Parts[n - 1].transform.rotation, 10 * Time.deltaTime);
				}
			}
			else
			{
				Parts[n].transform.position = Vector2.Lerp(Parts[n].transform.position, Parts[n - 1].transform.position, distance / _gap);
				Parts[n].transform.rotation = Quaternion.Lerp(Parts[n].transform.rotation, Parts[n - 1].transform.rotation, 10 * Time.deltaTime);
			}
        }
	}
	
	[Server]
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if(collision.gameObject.tag=="NpcBody" && GetComponent<FishControl>() != null && collision.gameObject.GetComponent<NetworkMatch>().matchId == GetComponent<NetworkMatch>().matchId)
		{
			for (int n = 1; Parts.Count > n; n++)
			{
				GameObject foodObject = Instantiate(foodPrefab, transform.position, Quaternion.identity);
				foodObject.GetComponent<NetworkMatch>().matchId = collision.gameObject.GetComponent<NetworkMatch>().matchId;
				NetworkServer.Spawn(foodObject);
			}
			GameObject partsBodyPlayer = GetComponent<FoodScore>().partsBodyPlayer;
			NetworkServer.Destroy(partsBodyPlayer);
			NetworkServer.Destroy(gameObject);
		}
		
		if(collision.gameObject.tag=="NpcBody" && GetComponent<Npc>() != null && collision.gameObject.GetComponent<BodyPart>() != null && collision.gameObject.GetComponent<BodyPart>().snakeID != GetComponent<Npc>().snakeID && collision.gameObject.GetComponent<NetworkMatch>().matchId == GetComponent<NetworkMatch>().matchId)
		{
			for (int n = 1; Parts.Count > n; n++)
			{
				GameObject foodObject = Instantiate(foodPrefab, transform.position, Quaternion.identity);
				foodObject.GetComponent<NetworkMatch>().matchId = collision.gameObject.GetComponent<NetworkMatch>().matchId;
				NetworkServer.Spawn(foodObject);
			}
			GameObject partsBodyPlayer = GetComponent<FoodScore>().partsBodyPlayer;
			NetworkServer.Destroy(partsBodyPlayer);
			NetworkServer.Destroy(gameObject);
		}
		
		if(collision.gameObject.tag=="PlayerBody" && GetComponent<Npc>() == null && collision.gameObject.GetComponent<BodyPart>().snakeID != GetComponent<FishControl>().snakeID && collision.gameObject.GetComponent<NetworkMatch>().matchId == GetComponent<NetworkMatch>().matchId)
		{
			for (int n = 1; Parts.Count > n; n++)
			{
				GameObject foodObject = Instantiate(foodPrefab, transform.position, Quaternion.identity);
				foodObject.GetComponent<NetworkMatch>().matchId = collision.gameObject.GetComponent<NetworkMatch>().matchId;
				NetworkServer.Spawn(foodObject);
			}
			GameObject partsBodyPlayer = GetComponent<FoodScore>().partsBodyPlayer;
			NetworkServer.Destroy(partsBodyPlayer);
			NetworkServer.Destroy(gameObject);
		}
		
		if(collision.gameObject.tag=="PlayerBody" && GetComponent<Npc>() != null && collision.gameObject.GetComponent<NetworkMatch>().matchId == GetComponent<NetworkMatch>().matchId)
		{
			for (int n = 1; n < Parts.Count; n++)
			{
				GameObject foodObject = Instantiate(foodPrefab, transform.position, Quaternion.identity);
				foodObject.GetComponent<NetworkMatch>().matchId = collision.gameObject.GetComponent<NetworkMatch>().matchId;
				NetworkServer.Spawn(foodObject);
			}
			GameObject partsBodyPlayer = GetComponent<FoodScore>().partsBodyPlayer;
			NetworkServer.Destroy(partsBodyPlayer);
			NetworkServer.Destroy(gameObject);
		}
	}
}
