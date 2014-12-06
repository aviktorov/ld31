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
	
	public void Reset() {
		for(int i = 0; i < played.Length; i++) played[i] = false;
	}
}

/*
 */
public class Sequencer : MonoSingleton<Sequencer> {
	
	// data
	public int bpm = 120;
	public int steps = 16;
	public Instrument[] instruments = null;
	
	public Color[] colors = new Color[] {
		Color.red,
		Color.green,
		Color.blue,
		Color.yellow,
	};
	
	// components
	private AudioSource cached_audio;
	
	// runtime
	private float current_time;
	private SequencerRow[] instrument_rows;
	
	// getters/setters/togglers
	public void ToggleStep(int instrument,int step) {
		if(instrument < 0 || instrument >= instruments.Length) return;
		if(step < 0 || step >= steps) return;
		
		instrument_rows[instrument].data[step] = !instrument_rows[instrument].data[step];
		instrument_rows[instrument].played[step] = false;
	}
	
	public float GetProgress() {
		return Mathf.Clamp01(current_time / steps);
	}
	
	public Color GetStepColor(int time) {
		if(colors.Length == 0) return Color.black;
		
		int step = time / colors.Length;
		return colors[step % colors.Length];
	}
	
	// methods
	private void Awake() {
		current_time = 0.0f;
		cached_audio = GetComponent<AudioSource>();
		
		if((steps > 0) && (instruments.Length > 0)) {
			instrument_rows = new SequencerRow[instruments.Length];
			for(int i = 0; i < instruments.Length; i++) {
				instrument_rows[i] = new SequencerRow() { data = new bool[steps], played = new bool[steps] };
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
			
			foreach(SequencerRow row in instrument_rows) {
				row.Reset();
			}
		}
		
		// play instruments
		if(cached_audio == null) return;
		
		for(int i = 0; i < instruments.Length; i++) {
			Instrument instrument = instruments[i];
			SequencerRow row = instrument_rows[i];
			
			if(!row.data[step]) continue;
			if(row.played[step]) continue;
			
			row.played[step] = true;
			AudioClip sound = instrument.GetRandomSound();
			
			if(sound == null) continue;
			cached_audio.PlayOneShot(sound);
		}
	}
}
