using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PartsBody : NetworkBehaviour
{
    void Update()
    {
        if (transform.childCount == 0)
		{
			NetworkServer.Destroy(gameObject);
		}
    }
}
