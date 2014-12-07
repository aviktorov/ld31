using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 */
internal struct Segment {
	public Vector3 start;
	public Vector3 end;
}

/*
 */
[RequireComponent(typeof(LineRenderer))]
public class LightningGeometry : MonoBehaviour {
	
	// data
	public int generations = 5;
	public float width = 0.2f;
	public float randomOffset = 1.0f;
	public Transform target = null;
	
	// components
	private LineRenderer cached_renderer;
	private Transform cached_transform;
	
	// interface
	void GenerateGeometry() {
		List<Segment> segments = new List<Segment>();
		segments.Add(new Segment() { start = cached_transform.position, end = target.position });
		
		for(int generation = 0; generation < generations; generation++) {
			int num_segments = segments.Count;
			for(int i = 0; i < num_segments; i++) {
				Segment current = segments[0];
				segments.RemoveAt(0);
				
				Vector3 perpendicular = Vector3.zero;
				perpendicular.x = current.start.y - current.end.y;
				perpendicular.y = current.end.x - current.start.x;
				
				Vector3 mid = (current.start + current.end) * 0.5f;
				mid += perpendicular * Random.Range(-randomOffset,randomOffset);
				
				segments.Add(new Segment() { start = current.start, end = mid });
				segments.Add(new Segment() { start = mid, end = current.end });
			}
		}
		
		cached_renderer.SetWidth(width,width);
		cached_renderer.SetVertexCount(segments.Count + 1);
		
		int index = 0;
		for(; index < segments.Count; index++) {
			cached_renderer.SetPosition(index, segments[index].start);
		}
		
		cached_renderer.SetPosition(index, segments[index - 1].end);
	}
	
	// functions
	private void Awake() {
		cached_renderer = GetComponent<LineRenderer>();
		cached_transform = GetComponent<Transform>();
	}
	
	private void Start() {
		GenerateGeometry();
	}
}
