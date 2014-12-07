using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/*
 */
public class SequencerUI : MonoSingleton<SequencerUI> {
	
	// data
	public GameObject button;
	public GameObject bar;
	public GameObject icon;
	
	public float ghost_alpha = 0.3f;
	public Sprite ghost_sprite = null;
	
	// components
	private RectTransform cached_transform;
	
	// runtime
	private float button_width;
	private float button_height;
	private RectTransform bar_transform;
	private Dictionary<int,Toggle> grid;
	
	// interface
	public void ToggleNote(int id) {
		if(grid.ContainsKey(id) == false) return;
		
		SetGhostNote(id,false);
		
		Toggle toggle = grid[id];
		toggle.isOn = !toggle.isOn;
	}
	
	public void SetGhostNote(int id,bool enabled) {
		if(grid.ContainsKey(id) == false) return;
		
		Toggle toggle = grid[id];
		Image checkmark = toggle.graphic as Image;
		
		float new_alpha = (enabled) ? ghost_alpha : 1.0f;
		checkmark.color = checkmark.color.WithA(new_alpha);
	}
	
	public void SetEnabled(bool enabled) {
		foreach(RectTransform child in cached_transform) {
			child.gameObject.SetActive(enabled);
		}
	}
	
	// functions
	private void Awake() {
		cached_transform = GetComponent<RectTransform>();
		grid = new Dictionary<int,Toggle>();
	}
	
	private void Start() {
		if(button == null) return;
		if(bar == null) return;
		if(icon == null) return;
		
		RectTransform button_transform = button.GetComponent<RectTransform>();
		
		int num_rows = Sequencer.instance.sequencer_rows.Length;
		int num_steps = Sequencer.instance.steps;
		
		button_width = button_transform.rect.width;
		button_height = button_transform.rect.height;
		
		// icons
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
		
		// toggles
		for(int i = 0; i < num_rows; i++) {
			float y = button_height * (i - num_rows / 2);
			
			for(int j = 0; j < num_steps; j++) {
				float x = button_width * (j - num_steps / 2);
				
				GameObject runtime = GameObject.Instantiate(button) as GameObject;
				RectTransform runtime_transform = runtime.transform as RectTransform;
				runtime_transform.anchoredPosition = new Vector2(x,y);
				
				cached_transform.AddChild(runtime_transform);
				
				Toggle toggle = runtime.GetComponent<Toggle>();
				if(toggle == null) continue;
				
				var data = new { row = i, step = j }; // hack hack hack
				
				toggle.isOn = false;
				toggle.onValueChanged.AddListener((unused) => GameLogic.instance.ToggleNote(data.row,data.step));
				
				Color step_color = GameLogic.instance.GetStepColor(j,num_steps);
				ColorBlock colors = toggle.colors;
				colors.normalColor = step_color.WithA(colors.normalColor.a);
				toggle.colors = colors;
				
				int id = i * num_steps + j;
				grid.Add(id,toggle);
			}
		}
		
		// bar
		GameObject bar_runtime = GameObject.Instantiate(bar) as GameObject;
		bar_transform = bar_runtime.transform as RectTransform;
		bar_transform.sizeDelta = bar_transform.sizeDelta.WithY(num_rows * button_height);
		bar_transform.anchoredPosition = bar_transform.anchoredPosition.WithY(-button_height / 2);
		
		cached_transform.AddChild(bar_transform);
	}
	
	private void LateUpdate() {
		if(bar == null) return;
		if(GameLogic.instance.game_over) return;
		
		int half_steps = Sequencer.instance.steps / 2;
		float x = Mathf.Lerp(-half_steps,half_steps,Sequencer.instance.GetProgress()) * button_width - button_width / 2;
		
		bar_transform.anchoredPosition = bar_transform.anchoredPosition.WithX(x);
	}
}
