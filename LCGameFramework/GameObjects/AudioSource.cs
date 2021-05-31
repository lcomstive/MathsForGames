using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LCUtils;
using Raylib_cs;
using static Raylib_cs.Raylib;

namespace LCGF.GameObjects
{
	/// <summary>
	/// Sound-playing GameObject
	/// </summary>
	public class AudioSource : GameObject
	{
		public enum State { Stopped, Playing, Paused }

		public bool Loop { get; set; } = false;
		public bool DestroyOnFinish { get; set; } = false;

		public State SoundState
		{
			get => m_SoundState;
			set
			{
				switch (value)
				{
					default:
					case State.Stopped:
						Stop();
						break;
					case State.Paused:
						Pause();
						break;
					case State.Playing:
						Play();
						break;
				}
			}
		}

		public float Volume
		{
			get => m_Volume;
			set => SetSoundVolume(m_Sound, m_Volume = Math.Clamp(value, 0f, 1f));
		}

		public float Pitch
		{
			get => m_Pitch;
			set => SetSoundPitch(m_Sound, m_Pitch = value);
		}

		public float Duration { get; private set; }
		public float TimeUntilFinish { get; private set; }

		public bool IsPlaying => SoundState == State.Playing;
		public bool IsSoundValid => m_Sound.sampleCount > 0;

		private Sound m_Sound;
		private float m_Pitch = 1f;
		private float m_Volume = 1f;
		private State m_SoundState = State.Stopped;

		public AudioSource(string audioClip, GameObject parent = null) : this(audioClip, true, "Audio Source", parent) { }
		public AudioSource(string audioClip, bool autoPlay = true, string name = "Audio Source", GameObject parent = null) : base(Vector2.zero, name, parent)
		{
			SetSound(audioClip);
			TimeUntilFinish = 0f;

			if (autoPlay)
				Play();
		}

		/// <summary>
		/// Stops the current sound and sets a new one
		/// </summary>
		/// <param name="play">Play the sound once loaded?</param>
		public void SetSound(string filepath, bool play = false)
		{
			if(!string.IsNullOrEmpty(filepath))
			{
				SetSound(Resources.LoadSound(filepath));
				return;
			}

			Stop();
			m_Sound = new Sound();
		}

		/// <summary>
		/// Stops the current sound and sets a new one
		/// </summary>
		/// <param name="play">Play the sound once loaded?</param>
		public void SetSound(Sound sound, bool play = false)
		{
			Stop();

			m_Sound = sound;
			Duration = m_Sound.sampleCount / (float)m_Sound.stream.sampleRate;

			if (play) Play();
		}

		protected override void OnUpdate()
		{
			if (m_SoundState != State.Playing)
				return; // Nothing to update
			TimeUntilFinish -= Time.DeltaTime;

			if (TimeUntilFinish > 0f)
				return; // Sound hasn't finished

			if(DestroyOnFinish)
			{
				Destroy();
				return;
			}
			if (Loop)
				Play(true);
		}

		public void Play(bool restart = false)
		{
			TimeUntilFinish = Duration;
			switch(m_SoundState)
			{
				default:
				case State.Playing:
					if (!restart)
						break;
					StopSound(m_Sound);
					PlaySound(m_Sound);
					TimeUntilFinish = Duration;
					break;
				case State.Stopped:
					PlaySound(m_Sound);
					break;
				case State.Paused:
					ResumeSound(m_Sound);
					break;
			}
			m_SoundState = State.Playing;
		}

		public void Stop()
		{
			m_SoundState = State.Stopped;
			StopSound(m_Sound);
			TimeUntilFinish = 0f;
		}

		public void Pause()
		{
			if (m_SoundState != State.Playing)
				return;
			PauseSound(m_Sound);
			m_SoundState = State.Paused;
		}
	}
}
