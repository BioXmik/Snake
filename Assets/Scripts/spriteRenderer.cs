using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spriteRenderer : MonoBehaviour
{
	private SpriteRenderer _sprite_renderer;
	private GameObject _player;
	
	void Start()
	{
		_sprite_renderer = GetComponent<SpriteRenderer>();
		_player = GameObject.FindGameObjectWithTag("LocalPlayer");
	}
	
	private void Update()
    {
        if(_player != null)
        {
            if (Vector2.Distance(_player.transform.position, this.transform.position) < 20)
            {
                _sprite_renderer.enabled = true;
            }
            else
            {
                _sprite_renderer.enabled = false;
            }
        }        
    }
}
