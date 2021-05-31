using LCUtils;
using Raylib_cs;
using System.Collections.Generic;

namespace LCGF.GameObjects
{
	/// <summary>
	/// A GameObject that draws sprites (texture) in order
	/// </summary>
	public class AnimatedSpriteObject : SpriteObject
	{
		/// <summary>
		/// Whether to start from the beginning once the animation has finished.
		/// If set to false, destroys self once animation has finished.
		/// </summary>
		public bool Loop { get; set; } = true;

		/// <summary>
		/// Speed at which to change the sprite, in seconds
		/// </summary>
		public float AnimationSpeed { get; set; } = 0.1f;

		private Texture2D[] m_Textures;
		private int m_CurrentSprite = 0;
		private float m_TimeSinceLastChange = 0f;

		public AnimatedSpriteObject(string[] texturePaths, string name = "", GameObject parent = null) : this(texturePaths, Vector2.zero, name, parent) { }
		public AnimatedSpriteObject(string[] texturePaths, Vector2 position, string name = "", GameObject parent = null) : base(string.Empty, position, 0f, name, parent)
		{
			List<Texture2D> textures = new List<Texture2D>();
			foreach (string path in texturePaths)
				textures.Add(Resources.LoadTexture(path));
			if (textures.Count == 0)
			{
				Destroy(); // No GameObject for you!
				return;
			}
			m_Textures = textures.ToArray();

			SetTexture(m_Textures[0]);
			Size = new Vector2(TextureSize);
		}

		protected override void OnUpdate()
		{
			// Check if time since the last sprite was changed exceeds the "speed" of the animation
			m_TimeSinceLastChange += Time.DeltaTime;
			if (m_TimeSinceLastChange < AnimationSpeed)
				return;

			// Next frame
			m_TimeSinceLastChange = 0f;
			m_CurrentSprite++;
			if (m_CurrentSprite >= m_Textures.Length - 1)
			{
				m_CurrentSprite = 0;

				if (!Loop)
					Destroy();
			}

			// Update texture in SpriteObject
			SetTexture(m_Textures[m_CurrentSprite]);
		}
	}
}
