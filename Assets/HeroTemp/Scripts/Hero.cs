using UnityEngine;
using System.Collections;

/*
 */
[RequireComponent(typeof(Rigidbody))]
public class Hero : MonoBehaviour {
	
	public float speed = 5.0f;
	
	private Rigidbody cachedBody;
	
	private void Awake() {
		cachedBody = GetComponent<Rigidbody>();
	}
	
	private Vector2 GetMovementVector() {
		Vector2 direction = Vector2.zero;
		
		direction = new Vector2(Input.GetAxis("Horizontal"),Input.GetAxis("Vertical"));
		if(direction.sqrMagnitude > 1.0f) direction.Normalize();
		
		return direction;
	}
	
	private void Update() {
		if(cachedBody == null) return;
		
		Vector2 movement = GetMovementVector() * speed;
		Vector3 velocity = cachedBody.velocity;
		
		velocity.x = movement.x;
		velocity.z = movement.y;
		
		cachedBody.velocity = velocity;
	}
	
	private void OnTriggerEnter(Collider collider) {
		if(collider.gameObject.tag != "Exit") return;
		
		Destroy(this.gameObject);
		UI.instance.gameOver = true;
	}
}
