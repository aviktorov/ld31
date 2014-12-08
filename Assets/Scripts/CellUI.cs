using UnityEngine;
using System.Collections;

/*
 */
public class CellUI : MonoBehaviour {
	
	// data
	public Color base_color = Color.white;
	public Color highlight_color = Color.white;
	public float smoothness = 8.0f;
	
	public float highlight_add = 0.1f;
	public float pressed_add = 0.2f;
	
	// components
	private Transform cached_transform;
	private Renderer cached_renderer;
	private Renderer[] cached_highlighters;
	
	// runtime
	private Color target_color;
	private Color target_highlight_color;
	
	// interface
	public void SetHighlightingEnabled(bool enabled) {
		foreach(Renderer highlighter in cached_highlighters) {
			if(highlighter == cached_renderer) continue;
			
			highlighter.enabled = enabled;
		}
	}
	
	// functions
	private void Awake() {
		cached_transform = GetComponent<Transform>();
		cached_renderer = GetComponent<Renderer>();
		cached_highlighters = GetComponentsInChildren<Renderer>();
		
		target_color = base_color;
		target_highlight_color = highlight_color.WithA(0.0f);
	}
	
	private void Start() {
		cached_renderer.material.color = target_color;
		
		foreach(Renderer highlighter in cached_highlighters) {
			if(highlighter == cached_renderer) continue;
			
			highlighter.material.color = target_highlight_color;
		}
	}
	
	private void Update() {
		cached_renderer.material.color = Color.Lerp(cached_renderer.material.color,target_color,Time.deltaTime * smoothness);
		foreach(Renderer highlighter in cached_highlighters) {
			if(highlighter == cached_renderer) continue;
			
			highlighter.material.color = Color.Lerp(highlighter.material.color,target_highlight_color,Time.deltaTime * smoothness);
		}
	}
	
	// events
	private void OnMouseEnter() {
		target_highlight_color = highlight_color;
		target_color = base_color * (1.0f + highlight_add);
	}
	
	private void OnMouseExit() {
		target_highlight_color = highlight_color.WithA(0.0f);
		target_color = base_color;
	}
	
	private void OnMouseDown() {
		cached_renderer.material.color = base_color * (1.0f + pressed_add);
	}
}
