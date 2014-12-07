using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class GameLogic : MonoSingleton<GameLogic> {
	
	// data
	public GameObject mob_prefab = null;
	public Sprite[] mob_sprites = null;
	public float spawn_interval = 1.0f;
	public float spawn_radius = 10.0f;
	
	public Color[] colors = new Color[] {
		Color.red,
		Color.green,
		Color.blue,
		Color.yellow,
	};
	
	// runtime
	private float current_time;
	private Transform base_transform;
	
	// getters
	public Color GetStepColor(int time,int steps) {
		if(colors.Length == 0) return Color.black;
		
		int step = (int)Mathf.Floor((time * colors.Length) / (float)steps);
		return colors[step % colors.Length];
	}
	
	public Color GetRandomColor() {
		if(colors.Length == 0) return Color.black;
		
		return colors[Random.Range(0,colors.Length)];
	}
	
	public Sprite GetRandomMobSprite() {
		if(mob_sprites.Length == 0) return null;
		
		return mob_sprites[Random.Range(0,mob_sprites.Length)];
	}
	
	// interface
	public GameObject SpawnMob() {
		if(mob_prefab == null) return null;
		
		float angle = Random.Range(0.0f,360.0f) * Mathf.Deg2Rad;
		
		GameObject mob = GameObject.Instantiate(mob_prefab) as GameObject;
		mob.transform.position = base_transform.position + new Vector3(Mathf.Cos(angle),Mathf.Sin(angle),0.0f) * spawn_radius;
		
		return mob;
	}
	
	public void KillMobs(Sprite sprite,Color color) {
		GameObject[] mobs = GameObject.FindGameObjectsWithTag("Mob");
		
		for(int i = mobs.Length - 1; i >= 0; i--) {
			SpriteRenderer mob_renderer = mobs[i].GetComponent<SpriteRenderer>();
			if(mob_renderer == null) continue;
			if(mob_renderer.color != color) continue;
			if(mob_renderer.sprite != sprite) continue;
			
			Destroy(mobs[i]);
		}
	}
	
	// functions
	private void Awake() {
		current_time = 0.0f;
	}
	
	private void Start() {
		GameObject base_object = GameObject.FindWithTag("Player");
		if(base_object) base_transform = base_object.transform;
	}
	
	private void Update() {
		if(base_transform == null) return;
		
		current_time += Time.deltaTime;
		
		while(current_time > spawn_interval) {
			current_time -= spawn_interval;
			SpawnMob();
		}
	}
}
