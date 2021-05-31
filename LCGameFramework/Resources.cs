using System;
using System.Collections.Generic;
using Raylib_cs;
using System.IO;

namespace LCGF
{
	/// <summary>
	/// Static class to load and cache common assets
	/// </summary>
	public static class Resources
	{
		/// <summary>
		/// Relative filepath to assets folder, where files will be searched for
		/// </summary>
		public const string AssetPath = "./Assets";

		#region Texture2D
		private static Dictionary<string, Texture2D> s_CachedTextures = new Dictionary<string, Texture2D>();

		/// <summary>
		/// Loads and caches texture at given path.
		/// </summary>
		/// <param name="path">Filepath relative to AssetPath</param>
		/// <param name="forceReload">When true the file is loaded from disk even if contained in the cache</param>
		/// <returns>Valid texture file, otherwise empty Texture2D</returns>
		public static Texture2D LoadTexture(string path, bool forceReload = false)
		{
			if (s_CachedTextures.ContainsKey(path) && !forceReload)
				return s_CachedTextures[path]; // Already cached

			string finalPath = $"{AssetPath}/{path}";
			if (!File.Exists(finalPath))
			{
				Console.WriteLine($"Tried to load texture '{finalPath}' but file does not exist");
				return new Texture2D(); // Not found, return empty texture
			}
			s_CachedTextures.Add(path, Raylib.LoadTexture(finalPath));
			return s_CachedTextures[path]; // Now cached
		}

		/// <summary>
		/// Loads and caches texture at given path.
		/// </summary>
		/// <param name="path">Filepath relative to AssetPath</param>
		/// <param name="forceReload">When true the file is loaded from disk even if contained in the cache</param>
		/// <returns>True if texture was valid and loaded, otherwise false</returns>
		public static bool TryLoadTexture(string path, out Texture2D texture, bool forceReload = false) => (texture = LoadTexture(path, forceReload)).width > 0;
		#endregion

		#region Sound
		private static Dictionary<string, Sound> s_CachedSounds = new Dictionary<string, Sound>();

		/// <summary>
		/// Loads and caches sound at given path.
		/// </summary>
		/// <param name="path">Filepath relative to AssetPath</param>
		/// <param name="forceReload">When true the file is loaded from disk even if contained in the cache</param>
		/// <returns>Valid sound file, otherwise empty Sound</returns>
		public static Sound LoadSound(string path, bool forceReload = false)
		{
			if (s_CachedSounds.ContainsKey(path) && !forceReload)
				return s_CachedSounds[path];

			string finalPath = $"{AssetPath}/{path}";
			if(!File.Exists(finalPath))
			{
				Console.WriteLine($"Tried to load sound '{finalPath}' but file does not exist");
				return new Sound();
			}
			s_CachedSounds.Add(path, Raylib.LoadSound(finalPath));
			return s_CachedSounds[path];
		}

		/// <summary>
		/// Loads and caches sound at given path.
		/// </summary>
		/// <param name="path">Filepath relative to AssetPath</param>
		/// <param name="forceReload">When true the file is loaded from disk even if contained in the cache</param>
		/// <returns>True if sound was valid and loaded, otherwise false</returns>
		public static bool TryLoadSound(string path, out Sound sound, bool forceReload = false) => (sound = LoadSound(path, forceReload)).sampleCount > 0;
		#endregion
	}
}
