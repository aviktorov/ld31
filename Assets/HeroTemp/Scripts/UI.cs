using UnityEngine;
using System.Collections;

public class UI : MonoSingleton<UI> {
	
	public bool gameOver = false;
	
	private void OnGUI() {
		if(!gameOver) return;
		
		Rect rect = new Rect((Screen.width - 200) / 2,(Screen.height - 20) / 2,200,20);
		rect.y -= 30;
		
		GUI.Label(rect,"You loose!");
		rect.y += 30;
		
		if(GUI.Button(rect,"Restart")) {
			Application.LoadLevel("HeroTemp");
		}
	}
}
