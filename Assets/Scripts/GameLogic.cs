using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Mob {
	public GameObject gameObject;
	public Color color;
}

public class GameLogic : MonoSingleton<GameLogic> {
	
	// data
	public GameObject[] mobs = null;
	public float spawn_interval = 1.0f;
	
	// runtime
	private List<Mob> active_mobs;
	private float current_time;
	
	// functions
	private void Awake() {
		current_time = 0.0f;
		active_mobs = new List<Mob>();
	}
	
	private void Update() {
		if(mobs.Length == 0) return;
	}
}
