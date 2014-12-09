using UnityEngine;
using System.Collections;

/*
 */
public class Mob : MonoBehaviour {
	
	// data
	public GameObject death_fx = null;
	public GameObject spawn_fx = null;
	public float smoothness = 1.0f;
	public Color base_color;
	
	// components
	private Rigidbody cached_body;
	private Transform cached_transform;
	private SpriteRenderer cached_renderer;
	private ParticleSystem cached_particles;
	
	// runtime
	private bool base_hit;
	
	// funcions
	private void Awake() {
		base_hit = false;
		
		cached_body = GetComponent<Rigidbody>();
		cached_transform = GetComponent<Transform>();
		cached_renderer = GetComponent<SpriteRenderer>();
		cached_particles = GetComponentInChildren<ParticleSystem>();
	}
	
	private void Start() {
		base_color = GameLogic.instance.GetRandomMobColor();
		
		cached_renderer.color = base_color.WithA(0.0f);
		cached_renderer.sprite = GameLogic.instance.GetRandomMobSprite();
		
		cached_particles.startColor = base_color;
		
		if(spawn_fx != null) {
			GameObject.Instantiate(spawn_fx,cached_transform.position,Quaternion.identity);
		}
	}
	
	private void Update() {
		
		cached_renderer.color = Color.Lerp(cached_renderer.color,base_color,Time.deltaTime * smoothness);
		
		if(cached_body == null) return;
		cached_body.velocity = cached_body.velocity.WithXZ(0.0f,0.0f);
		
		if(GameLogic.instance.game_over) return;
		
		GameObject player_base = GameObject.FindWithTag("Player");
		if(player_base == null) return;
		
		SphereCollider player_collider = player_base.GetComponentInChildren<SphereCollider>();
		if(player_collider == null) return;
		
		int tempo = Sequencer.instance.tempo;
		int max_moves = GameLogic.instance.max_mob_moves;
		float radius = GameLogic.instance.spawn_radius;
		
		float speed = ((radius - player_collider.radius) / max_moves) * (60.0f / tempo);
		
		Vector3 velocity = (player_base.transform.position - cached_transform.position).normalized * speed;
		cached_body.velocity = velocity.WithY(cached_body.velocity.y);
	}
	
	// events
	private void OnTriggerEnter(Collider collider) {
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
