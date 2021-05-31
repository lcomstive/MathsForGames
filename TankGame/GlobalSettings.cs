using LCGF;
using System;
using System.IO;
using System.Text.Json;

namespace TankGame
{
	public class GlobalSettings
	{
		#region Serializable Class
		private class GlobalSettingsData
		{
			public static readonly Version GameVersion = new Version(0, 0, 1);

			public int Width { get; set; } = 800;
			public int Height { get; set; } = 600;
			public bool VSync { get; set; } = true;
		}
		#endregion

		public const string DefaultSettingsPath = Resources.AssetPath + "/settings.json";
		public static Version CurrentGameVersion => new Version(0, 0, 1);

		private static GlobalSettingsData s_Data;

		public static bool VSync	   { get => s_Data.VSync;  set => s_Data.VSync = value; }
		public static int WindowWidth  { get => s_Data.Width;  set => s_Data.Width = value; }
		public static int WindowHeight { get => s_Data.Height; set => s_Data.Height = value; }

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
			$"\tVersion: v{GlobalSettingsData.GameVersion}\n" +
			"\tWindow Size: ({s_Data.Width}, {s_Data.Height})\n";
	}
}
