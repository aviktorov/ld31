﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

internal class Note {
	public int id;
	public int lifetime;
	public bool ghost;
}

/*
 */
public class GameLogic : MonoSingleton<GameLogic> {
	
	// data
	public GameObject mob_prefab = null;
	public GameObject lightning_prefab = null;
	public Sprite[] mob_sprites = null;
	public float spawn_interval = 1.0f;
	public float spawn_radius = 10.0f;
	public int max_active_notes = 3;
	public int max_ghost_note_lifetime = 2;
	public int max_mob_moves = 10;
	public bool game_over = false;
	
	public Color[] colors;
	public Color[] mob_colors = new Color[] {
		Color.red,
		Color.green,
		Color.blue,
		Color.yellow,
	};
	
	// runtime
	private float current_time;
	private Transform base_transform;
	private Transform lightning_transform;
	private Dictionary<int,Note> notes;
	private List<Note> active_notes;
	
	// getters
	public Color GetStepColor(int row,int step,int steps) {
		if(colors.Length == 0) return Color.black;
		
		int adjusted_step = (int)Mathf.Floor((step * colors.Length) / (float)steps);
		int adjusted_row = row / mob_sprites.Length;
		
		float mob_factor = ((adjusted_row % 2) == 0) ? 0.9f : 1.0f;
		
		return colors[adjusted_step % colors.Length] * mob_factor;
	}
	
	public Color GetStepMobColor(int time,int steps) {
		if(mob_colors.Length == 0) return Color.black;
		
		int step = (int)Mathf.Floor((time * mob_colors.Length) / (float)steps);
		return mob_colors[step % mob_colors.Length];
	}
	
	public Color GetRandomMobColor() {
		if(mob_colors.Length == 0) return Color.black;
		
		return mob_colors[Random.Range(0,mob_colors.Length)];
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
		mob.transform.position = base_transform.position + new Vector3(Mathf.Cos(angle),0.0f,Mathf.Sin(angle)) * spawn_radius;
		
		SphereCollider sphere = mob.GetComponent<SphereCollider>();
		if(sphere) mob.transform.position += Vector3.up * sphere.radius;
		
		return mob;
	}
	
	public void KillMobs(Sprite sprite,Color color) {
		GameObject[] mobs = GameObject.FindGameObjectsWithTag("Mob");
		
		for(int i = mobs.Length - 1; i >= 0; i--) {
			SpriteRenderer mob_renderer = mobs[i].GetComponent<SpriteRenderer>();
			if(mob_renderer == null) continue;
			if(mob_renderer.sprite != sprite) continue;
			
			Mob mob = mobs[i].GetComponent<Mob>();
			if(mob == null) continue;
			if(mob.base_color != color) continue;
			
			if(lightning_prefab == null) continue;
			GameObject lightning = GameObject.Instantiate(lightning_prefab,lightning_transform.position,Quaternion.identity) as GameObject;
			
			LightningGeometry lightning_geometry = lightning.GetComponent<LightningGeometry>();
			if(lightning_geometry == null) continue;
			
			lightning_geometry.target = mobs[i].transform;
			
			// hope it'll be destroyed one frame later
			mob.Kill();
		}
	}
	
	public void SetNote(int row,int step,bool enabled) {
		Sequencer.instance.SetNote(row,step,enabled);
		
		int num_steps = Sequencer.instance.steps;
		int id = row * num_steps + step;
		
		// existing note
		if(notes.ContainsKey(id)) {
			notes.Remove(id);
		}
		// new note
		else {
			Note note = new Note() { id = id, lifetime = max_ghost_note_lifetime, ghost = false };
			
			notes.Add(id,note);
			active_notes.Add(note);
		}
		
		if(active_notes.Count > max_active_notes) {
			Note last = active_notes[0];
			last.ghost = true;
			
			active_notes.RemoveAt(0);
			SequencerGrid.instance.SetGhostNote(last.id,true);
		}
	}
	
	// events
	public void OnMobDeath() {
		// TODO: scores?
	}
	
	public void OnBaseHit() {
		GameObject.Destroy(base_transform.gameObject);
		game_over = true;
		
		DialogUI.instance.SetEnabled(true);
	}
	
	public void OnSequencerBar() {
		List<int> remove_list = new List<int>();
		
		// ghost lifetime
		foreach(int id in notes.Keys) {
			Note note = notes[id];
			
			if(note.ghost == false) continue;
			note.lifetime--;
			
			if(note.lifetime > 0) continue;
			remove_list.Add(id);
		}
		
		// note cleanup
		int num_steps = Sequencer.instance.steps;
		
		foreach(int id in remove_list) {
			int row = id / num_steps;
			int step = id % num_steps;
			
			SequencerGrid.instance.SetNote(id,false);
			Sequencer.instance.SetNote(row,step,false);
			notes.Remove(id);
		}
	}
	
	public void OnSequencerNote(int row,int step) {
		int num_steps = Sequencer.instance.steps;
		int id = row * num_steps + step;
		
		if(notes.ContainsKey(id) == false) return;
		
		Note note = notes[id];
		if(note.ghost) return;
		
		Color kill_color = GetStepMobColor(step,num_steps);
		Sprite kill_sprite = Sequencer.instance.GetRowSprite(row);
		
		KillMobs(kill_sprite,kill_color);
	}
	
	// functions
	private void Awake() {
		current_time = 0.0f;
		notes = new Dictionary<int,Note>();
		active_notes = new List<Note>();
	}
	
	private void Start() {
		GameObject base_object = GameObject.FindWithTag("Player");
		if(base_object) base_transform = base_object.transform;
		
		GameObject lightning_object = GameObject.FindWithTag("BaseLightning");
		if(lightning_object) lightning_transform = lightning_object.transform;
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
