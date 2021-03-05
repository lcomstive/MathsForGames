using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace Game
{
	public enum FullscreenState { Windowed, Borderless, Fullscreen }

	public class GlobalSettings
	{
		#region Serializable Class
		private class GlobalSettingsData
		{
			public class WindowSettings
			{
				public int Width { get; set; } = 800;
				public int Height { get; set; } = 600;
				public bool VSync { get; set; } = true;
				public int MonitorIndex { get; set; } = 0;
				public FullscreenState FullscreenState { get; set; } = FullscreenState.Windowed;
			}

			/// GAME VERSION ///
			internal Version m_GameVersion = CurrentGameVersion;
			public string GameVersion { get => m_GameVersion.ToString(); set => Version.TryParse(value, out m_GameVersion); }

			/// WINDOW SETTINGS ///
			public WindowSettings Window { get; set; } = new WindowSettings();

			/// PAUSE WHEN WINDOW IS NOT FOCUSED? ///
			public bool PauseNoFocus { get; set; } = false;

			/// SHOW FPS ///
			public bool ShowFPS { get; set; } = false;
		}
		#endregion

		public const string DefaultSettingsPath = "./settings.json";
		public static Version CurrentGameVersion => new Version(0, 0, 1);

		private static GlobalSettingsData s_Data;

		/// <summary>Pause the game if the window is not focused</summary>
		public static bool PauseNoFocus { get => s_Data.PauseNoFocus; set => s_Data.PauseNoFocus = value; }
		public static bool ShowFPS { get => s_Data.ShowFPS; set => s_Data.ShowFPS = value; }
		public static bool VSync { get => s_Data.Window.VSync; set => s_Data.Window.VSync = value; }
		public static int WindowWidth { get => s_Data.Window.Width; set => s_Data.Window.Width = value; }
		public static int WindowHeight { get => s_Data.Window.Height; set => s_Data.Window.Height = value; }
		public static int MonitorIndex { get => s_Data.Window.MonitorIndex; set => s_Data.Window.MonitorIndex = value; }
		public static FullscreenState FullscreenState { get => s_Data.Window.FullscreenState; set => s_Data.Window.FullscreenState = value; }

		public static void Load(string path = DefaultSettingsPath)
		{
			if (!File.Exists(path))
			{
				// Load defaults
				s_Data = new GlobalSettingsData();
				return;
			}
			s_Data = JsonSerializer.Deserialize<GlobalSettingsData>(File.ReadAllText(path));
		}

		public static void Save(string path = DefaultSettingsPath)
		{
			string json = JsonSerializer.Serialize(
				s_Data,
				new JsonSerializerOptions() { WriteIndented = true }
				);

			string directoryName = Path.GetDirectoryName(path);
			if (!Directory.Exists(directoryName))
				Directory.CreateDirectory(directoryName);
			File.WriteAllText(path, json);
		}

		public static string ToReadableString() =>
			"Global Settings:\n" +
			$"\tVersion: v{s_Data.GameVersion}\n" +
			"\tWindow:\n" +
			$"\t\tSize: ({s_Data.Window.Width}, {s_Data.Window.Height})\n" +
			$"\t\tFullscreen Mode: {s_Data.Window.FullscreenState}\n";
	}
}
