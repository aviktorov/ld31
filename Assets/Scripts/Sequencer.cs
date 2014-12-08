using UnityEngine;
using System.Collections;

/*
 */
[System.Serializable]
public class Instrument {
	public string name;
	public AudioClip[] sounds;
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
	public int tempo = 120;
	public int steps = 16;
	public int rows_per_mob = 4;
	
	public Instrument[] instruments = null;
	
	// components
	private AudioSource cached_audio;
	private AudioEchoFilter cached_echo;
	
	// runtime
	[System.NonSerialized]
	public SequencerRow[] sequencer_rows;
	private float current_time;
	
	// getters/setters/togglers
	public void ToggleNote(int row,int step) {
		if(row < 0 || row >= sequencer_rows.Length) return;
		if(step < 0 || step >= steps) return;
		
		sequencer_rows[row].data[step] = !sequencer_rows[row].data[step];
		sequencer_rows[row].played[step] = (current_time < step);
	}
	
	public float GetProgress() {
		return Mathf.Clamp01(current_time / steps);
	}
	
	public Sprite GetRowSprite(int row) {
		return sequencer_rows[row].sprite;
	}
	
	// methods
	private void Awake() {
		current_time = 0.0f;
		cached_audio = GetComponent<AudioSource>();
		cached_echo = Camera.main.GetComponent<AudioEchoFilter>();
		
		int num_mobs =  GameLogic.instance.mob_sprites.Length;
		int num_instruments = instruments.Length;
		int num_rows = num_mobs * rows_per_mob;
		
		if((steps > 0) && (num_rows > 0)) {
			sequencer_rows = new SequencerRow[num_rows];
			
			for(int i = 0; i < num_rows; i++) {
				sequencer_rows[i] = new SequencerRow() { 
					data = new bool[steps],
					played = new bool[steps],
					instrument = (num_instruments > 0) ? instruments[(num_rows - 1 - i) % num_instruments] : null,
					sprite = GameLogic.instance.mob_sprites[i / rows_per_mob]
				};
			}
		}
	}
	
	private void Update() {
		
		// delay
		float bps = (tempo * 4.0f / 60.0f);
		cached_echo.delay = 1000.0f * (4.0f / bps);
		
		// step
		current_time += Time.deltaTime * bps;
		int step = (int)Mathf.Floor(current_time);
		
		while(step >= steps) {
			current_time -= steps;
			step -= steps;
			
			foreach(SequencerRow row in sequencer_rows) {
				row.Reset();
			}
			
			GameLogic.instance.OnSequencerBar();
		}
		
		// play instruments
		for(int i = 0; i < sequencer_rows.Length; i++) {
			SequencerRow row = sequencer_rows[i];
			
			if(!row.data[step]) continue;
			if(row.played[step]) continue;
			
			row.played[step] = true;
			GameLogic.instance.OnSequencerNote(i,step);
			
			Instrument instrument = row.instrument;
			if(instrument == null) continue;
			
			AudioClip sound = instrument.GetRandomSound();
			if(sound == null) continue;
			
			cached_audio.PlayOneShot(sound,instrument.volume);
		}
	}
}
