using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/*
 */
public class SequencerUI : MonoBehaviour {
	
	// data
	public GameObject button;
	public GameObject bar;
	
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
		
		RectTransform button_transform = button.GetComponent<RectTransform>();
		
		int num_instruments = Sequencer.instance.instruments.Length;
		int num_steps = Sequencer.instance.steps;
		
		button_width = button_transform.rect.width;
		button_height = button_transform.rect.height;
		
		// toggles
		for(int i = 0; i < num_instruments; i++) {
			float y = button_height * (i - num_instruments / 2);
			
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
			}
		}
		
		// bar
		GameObject bar_runtime = GameObject.Instantiate(bar) as GameObject;
		bar_transform = bar_runtime.transform as RectTransform;
		bar_transform.sizeDelta = bar_transform.sizeDelta.WithY(num_instruments * button_height);
		
		cached_transform.AddChild(bar_transform);
	}
	
	private void LateUpdate() {
		if(button == null) return;
		if(bar == null) return;
		
		int half_steps = Sequencer.instance.steps / 2;
		float x = Mathf.Lerp(-half_steps,half_steps,Sequencer.instance.GetProgress()) * button_width - button_width / 2;
		
		bar_transform.anchoredPosition = bar_transform.anchoredPosition.WithX(x);
	}
}
