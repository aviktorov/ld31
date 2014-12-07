using UnityEngine;
using System.Collections;

public class DialogUI : MonoSingleton<DialogUI> {
	
	// components
	private RectTransform cached_transform;
	
	// methods
	public void SetEnabled(bool enabled) {
		foreach(RectTransform child in cached_transform) {
			child.gameObject.SetActive(enabled);
		}
	}
	
	// events
	public void OnRestart() {
		Application.LoadLevel("SequencerPrototype");
	}
	
	// functions
	private void Awake() {
		cached_transform = GetComponent<RectTransform>();
	}
}
