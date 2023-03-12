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
    public void Update()
	{
		float distance = 0;
        if (Parts.Count > 1)
        {
            distance = ((Vector2)Parts[1].transform.position - (Vector2)Parts[0].transform.position).magnitude;
        }
		
		for (int n = 1; n < Parts.Count; n++)
        {
			Parts[n].transform.position = Vector2.Lerp(Parts[n].transform.position, Parts[n - 1].transform.position, distance / _gap);
			Parts[n].transform.rotation = Quaternion.Lerp(Parts[n].transform.rotation, Parts[n - 1].transform.rotation, 10 * Time.deltaTime);
        }
	}
	
	[Server]
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if(collision.gameObject.tag=="NpcBody" && GetComponent<FishControl>() != null)
		{
			for (int n = 1; n < Parts.Count; n++)
			{
				NetworkServer.Destroy(Parts[n]);
				Parts.Remove(Parts[n]);
				
				GameObject foodObject = Instantiate(foodPrefab, transform.position, Quaternion.identity);
				foodObject.GetComponent<NetworkMatch>().matchId = collision.gameObject.GetComponent<NetworkMatch>().matchId;
				NetworkServer.Spawn(foodObject);
			}
			NetworkServer.Destroy(gameObject);
		}
	}
}
