using UnityEngine;
using System.Collections;

/*
 */
public class CellUI : MonoBehaviour {
	
	// data
	public Color color = Color.white;
	public float highlight_add = 0.1f;
	
	public float smoothness = 8.0f;
	
	public int step = 0;
	public int row = 0;
	public bool toggled = false;
	
	// components
	private Renderer cached_renderer;
	private Renderer[] cached_highlighters;
	
	// runtime
	private Color target_color;
	
	// functions
	private void Awake() {
		cached_renderer = GetComponent<Renderer>();
		cached_highlighters = GetComponentsInChildren<Renderer>();
	}
	
	private void Start() {
		target_color = color;
		
		cached_renderer.material.color = target_color;
		
		foreach(Renderer highlighter in cached_highlighters) {
			if(highlighter == cached_renderer) continue;
			highlighter.material.color = color.WithA(0.0f);
		}
	}
	
	private void Update() {
		cached_renderer.material.color = Color.Lerp(cached_renderer.material.color,target_color,Time.deltaTime * smoothness);
		
		foreach(Renderer highlighter in cached_highlighters) {
			if(highlighter == cached_renderer) continue;
			Color target_highlight_color = (toggled) ? color : color.WithA(0.0f);
			
			highlighter.material.color = Color.Lerp(highlighter.material.color,target_highlight_color,Time.deltaTime * smoothness);
		}
	}
	
	// events
	private void OnMouseEnter() {
		target_color = color + Color.white * highlight_add;
	}
	
	private void OnMouseExit() {
		target_color = color;
	}
	
	private void OnMouseDown() {
		cached_renderer.material.color = Color.white;
		
		foreach(Renderer highlighter in cached_highlighters) {
			if(highlighter == cached_renderer) continue;
			highlighter.material.color = Color.white;
		}
		
		toggled = !toggled;
		GameLogic.instance.SetNote(row,step,toggled);
	}
}
