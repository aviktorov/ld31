using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 */
public class SequencerGrid : MonoSingleton<SequencerGrid> {
	
	// data
	public GameObject cell_prefab = null;
	public GameObject bar_prefab = null;
	public GameObject icon_prefab = null;
	
	public float size = 1.0f;
	
	// components
	private Transform cached_transform;
	
	// runtime
	private Transform bar_transform;
	private Dictionary<int,CellUI> grid;
	
	// interface
	public void SetNote(int id,bool enabled) {
		if(grid.ContainsKey(id) == false) return;
		
		CellUI cell = grid[id];
		cell.toggled = enabled;
	}
	
	public void SetGhostNote(int id,bool enabled) {
		if(grid.ContainsKey(id) == false) return;
		
		// TODO: ghost note visualisation
		CellUI cell = grid[id];
		cell.toggled = false;
	}
	
	public void SetEnabled(bool enabled) {
		foreach(Transform child in cached_transform) {
			child.gameObject.SetActive(enabled);
		}
	}
	
	// functions
	private void Awake() {
		cached_transform = GetComponent<Transform>();
		grid = new Dictionary<int,CellUI>();
	}
	
	private void Start() {
		if(cell_prefab == null) return;
		
		int num_rows = Sequencer.instance.sequencer_rows.Length;
		int num_steps = Sequencer.instance.steps;
		
		float offset_x = size * (num_rows - 1) * 0.5f;
		float offset_z = size * (num_steps - 1) * 0.5f;
		
		// icons
		for(int i = 0; i < num_rows; i++) {
			float x = size * i - offset_x;
			float z = offset_z + size;
			
			GameObject runtime = GameObject.Instantiate(icon_prefab) as GameObject;
			
			runtime.transform.SetParent(cached_transform);
			runtime.transform.localPosition = new Vector3(x,0.0f,z);
			
			Texture2D icon = Sequencer.instance.sequencer_rows[i].sprite.texture;
			runtime.renderer.material.SetTexture("_MainTex",icon);
		}
		
		// cells
		for(int i = 0; i < num_rows; i++) {
			float x = size * i - offset_x;
			
			for(int j = 0; j < num_steps; j++) {
				float z = offset_z - size * j;
				
				GameObject runtime = GameObject.Instantiate(cell_prefab) as GameObject;
				runtime.transform.SetParent(cached_transform);
				runtime.transform.localPosition = new Vector3(x,0.0f,z);
				
				CellUI cell = runtime.GetComponent<CellUI>();
				if(cell == null) continue;
				
				cell.row = i;
				cell.step = j;
				cell.toggled = false;
				cell.color = GameLogic.instance.GetStepColor(j,num_steps);
				
				int id = i * num_steps + j;
				grid.Add(id,cell);
			}
		}
		
		// bar
		GameObject bar_runtime = GameObject.Instantiate(bar_prefab) as GameObject;
		bar_transform = bar_runtime.transform;
		
		Vector3 scale = bar_transform.localScale;
		
		bar_transform.SetParent(cached_transform);
		bar_transform.localScale = scale.WithY(num_rows * size);
		bar_transform.localPosition = Vector3.zero;
	}
	
	private void Update() {
		if(bar_transform == null) return;
		if(GameLogic.instance.game_over) return;
		
		int num_steps = Sequencer.instance.steps;
		float total_width = size * num_steps;
		float current_position = Sequencer.instance.GetProgress() * size * num_steps;
		
		bar_transform.localPosition = Vector3.zero.WithYZ(0.1f,total_width * 0.5f - current_position);
	}
}
