using UnityEngine;
using System.Collections;

/*
 */
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class Mob : MonoBehaviour {
	
	// data
	public float speed = 5.0f;
	public GameObject death_fx = null;
	
	// components
	private Rigidbody2D cached_body;
	private Transform cached_transform;
	private SpriteRenderer cached_renderer;
	
	// runtime
	private bool base_hit;
	
	// funcions
	private void Awake() {
		base_hit = false;
		
		cached_body = GetComponent<Rigidbody2D>();
		cached_transform = GetComponent<Transform>();
		cached_renderer = GetComponent<SpriteRenderer>();
	}
	
	private void Start() {
		cached_renderer.color = GameLogic.instance.GetRandomColor();
		cached_renderer.sprite = GameLogic.instance.GetRandomMobSprite();
	}
	
	private void Update() {
		if(cached_body == null) return;
		
		GameObject player_base = GameObject.FindWithTag("Player");
		if(player_base == null) return;
		
		Vector3 direction = (player_base.transform.position - cached_transform.position).normalized;
		cached_body.velocity = direction * speed;
	}
	
	// events
	private void OnTriggerEnter2D(Collider2D collider) {
		if(collider.gameObject.tag != "Player") return;
		
		base_hit = true;
		Destroy(gameObject);
	}
	
	private void OnDestroy() {
		if(GameLogic.instance == null) return;
		
		GameLogic.instance.OnMobDeath();
		
		if(base_hit) {
			GameLogic.instance.OnBaseHit();
		}
		
		if(death_fx != null) {
			GameObject.Instantiate(death_fx,cached_transform.position,Quaternion.identity);
		}
	}
}
