using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 */
public class SequencerGrid : MonoSingleton<SequencerGrid> {
	
	// data
	public GameObject cell_prefab = null;
	public float size = 1.0f;
	
	// components
	private Transform cached_transform;
	
	// runtime
	private Dictionary<int,CellUI> grid;
	
	// interface
	public void ToggleNote(int id) {
		if(grid.ContainsKey(id) == false) return;
	}
	
	public void SetGhostNote(int id,bool enabled) {
		if(grid.ContainsKey(id) == false) return;
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
		
		// icons
		
		/*
		for(int i = 0; i < num_rows; i++) {
			float x = button_width * (-(num_steps / 2) - 1);
			float y = button_height * (i - num_rows / 2);
			
			GameObject runtime = GameObject.Instantiate(icon) as GameObject;
			RectTransform runtime_transform = runtime.transform as RectTransform;
			runtime_transform.anchoredPosition = new Vector2(x,y);
			
			cached_transform.AddChild(runtime_transform);
			
			Image image = runtime.GetComponent<Image>();
			image.sprite = Sequencer.instance.sequencer_rows[i].sprite;
		}
		/**/
		
		// cells
		for(int i = 0; i < num_rows; i++) {
			float y = size * i;
			
			for(int j = 0; j < num_steps; j++) {
				float x = size * j;
				
				GameObject runtime = GameObject.Instantiate(cell_prefab) as GameObject;
				Transform runtime_transform = runtime.transform;
				runtime_transform.SetParent(cached_transform);
				runtime_transform.localPosition = new Vector3(x,0.0f,y);
				
				CellUI cell = runtime.GetComponent<CellUI>();
				if(cell == null) continue;
				
				cell.row = i;
				cell.step = j;
				cell.toggled = false;
				cell.base_color = GameLogic.instance.GetStepColor(j,num_steps);
				
				int id = i * num_steps + j;
				grid.Add(id,cell);
			}
		}
		
		// bar
		/*
		GameObject bar_runtime = GameObject.Instantiate(bar) as GameObject;
		bar_transform = bar_runtime.transform as RectTransform;
		bar_transform.sizeDelta = bar_transform.sizeDelta.WithY(num_rows * button_height);
		bar_transform.anchoredPosition = bar_transform.anchoredPosition.WithY(-button_height / 2);
		
		cached_transform.AddChild(bar_transform);
		/**/
	}
}
