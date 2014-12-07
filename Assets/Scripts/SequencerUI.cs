﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/*
 */
public class SequencerUI : MonoBehaviour {
	
	// data
	public GameObject button;
	public GameObject bar;
	public GameObject icon;
	
	// runtime
	private float button_width;
	private float button_height;
	private RectTransform bar_transform;
	
	// components
	private RectTransform cached_transform;
	
	// functions
	private void Awake() {
		cached_transform = GetComponent<RectTransform>();
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
				
				var data = new { instrument = i, step = j }; // hack hack hack
				
				toggle.isOn = false;
				toggle.onValueChanged.AddListener((unused) => Sequencer.instance.ToggleStep(data.instrument,data.step));
				
				Color step_color = GameLogic.instance.GetStepColor(j,num_steps);
				ColorBlock colors = toggle.colors;
				colors.normalColor = step_color.WithA(colors.normalColor.a);
				toggle.colors = colors;
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
		
		int half_steps = Sequencer.instance.steps / 2;
		float x = Mathf.Lerp(-half_steps,half_steps,Sequencer.instance.GetProgress()) * button_width - button_width / 2;
		
		bar_transform.anchoredPosition = bar_transform.anchoredPosition.WithX(x);
	}
}
