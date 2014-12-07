using UnityEngine;
using System.Collections;

/*
 */
[System.Serializable]
public class Instrument {
	public AudioClip[] sounds;
	public int mob_id = -1;
	public float volume = 1.0f;
	
	public AudioClip GetRandomSound() {
		if(sounds.Length == 0) return null;
		
		return sounds[Random.Range(0,sounds.Length)];
	}
}

/*
 */
public class SequencerRow {
	public bool[] data;
	public bool[] played;
	public Instrument instrument;
	public Sprite sprite;
	
	public void Reset() {
		for(int i = 0; i < played.Length; i++) played[i] = false;
	}
}

/*
 */
[RequireComponent(typeof(AudioSource))]
public class Sequencer : MonoSingleton<Sequencer> {
	
	// data
	public int bpm = 120;
	public int steps = 16;
	public Instrument[] instruments = null;
	
	// components
	private AudioSource cached_audio;
	
	// runtime
	[System.NonSerialized]
	public SequencerRow[] sequencer_rows;
	private float current_time;
	
	// getters/setters/togglers
	public void ToggleStep(int row,int step) {
		if(row < 0 || row >= sequencer_rows.Length) return;
		if(step < 0 || step >= steps) return;
		
		sequencer_rows[row].data[step] = !sequencer_rows[row].data[step];
		sequencer_rows[row].played[step] = false;
	}
	
	public float GetProgress() {
		return Mathf.Clamp01(current_time / steps);
	}
	
	// methods
	private void Awake() {
		current_time = 0.0f;
		cached_audio = GetComponent<AudioSource>();
		
		int num_mobs =  GameLogic.instance.mob_sprites.Length;
		int num_instruments = instruments.Length;
		
		if((steps > 0) && (num_mobs > 0)) {
			sequencer_rows = new SequencerRow[num_mobs];
			for(int i = 0; i < num_mobs; i++) {
				sequencer_rows[i] = new SequencerRow() { 
					data = new bool[steps],
					played = new bool[steps],
					instrument = (num_instruments > 0) ? instruments[i % num_instruments] : null,
					sprite = GameLogic.instance.mob_sprites[i]
				};
			}
		}
	}
	
	private void Update() {
		
		// step
		current_time += Time.deltaTime * (bpm / 60.0f);
		int step = (int)Mathf.Floor(current_time);
		
		while(step >= steps) {
			current_time -= steps;
			step -= steps;
			
			foreach(SequencerRow row in sequencer_rows) {
				row.Reset();
			}
		}
		
		// play instruments
		for(int i = 0; i < sequencer_rows.Length; i++) {
			SequencerRow row = sequencer_rows[i];
			
			if(!row.data[step]) continue;
			if(row.played[step]) continue;
			
			row.played[step] = true;
			
			Color kill_color = GameLogic.instance.GetStepColor(step,steps);
			Sprite kill_sprite = row.sprite;
			GameLogic.instance.KillMobs(kill_sprite,kill_color);
			
			if(row.instrument == null) continue;
			AudioClip sound = row.instrument.GetRandomSound();
			
			if(sound == null) continue;
			cached_audio.PlayOneShot(sound);
		}
	}
}
