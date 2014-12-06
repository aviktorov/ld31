using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/*
 */
public class SequencerUI : MonoBehaviour {
	
	// data
	public GameObject button;
	
	// components
	private RectTransform cached_transform;
	
	// functions
	private void Awake() {
		cached_transform = GetComponent<RectTransform>();
	}
	
	private void Start() {
		if(button == null) return;
		
		int num_instruments = Sequencer.instance.instruments.Length;
		int num_steps = Sequencer.instance.steps;
		
		for(int i = 0; i < num_instruments; i++) {
			float y = 30.0f * (i - num_instruments / 2);
			
			for(int j = 0; j < num_steps; j++) {
				float x = 30.0f * (j - num_steps / 2);
				
				GameObject runtime = GameObject.Instantiate(button) as GameObject;
				
				cached_transform.AddChild(runtime.transform as RectTransform);
				runtime.transform.localPosition = new Vector3(x,y,0.0f);
				
				Toggle toggle = runtime.GetComponent<Toggle>();
				if(toggle == null) continue;
				
				var data = new { instrument = i, step = j }; // hack hack hack
				
				toggle.isOn = false;
				toggle.onValueChanged.AddListener(
					(unused) => {
						Sequencer.instance.ToggleStep(data.instrument,data.step);
					}
				);
			}
		}
	}
}
