using LCUtils;
using Raylib_cs;
using static Raylib_cs.Raylib;

namespace LCGF.GameObjects
{
	/// <summary>
	/// A GameObject that draws a sprite (texture) every frame
	/// </summary>
	public class SpriteObject : GameObject
	{
		/// <summary>
		/// Tint of the sprite, set to white for no change in colour
		/// </summary>
		public Colour Tint = Colour.White;

		/// <summary>
		/// Size of the loaded texture
		/// </summary>
		public Vector2 TextureSize { get; private set; }

		/// <summary>
		/// Offset the sprite's position (and therefor rotation origin) relative to parent
		/// </summary>
		public Vector2 Offset = Vector2.zero;

		private Texture2D m_Texture;

		/// <summary>
		/// Should some debug information be displayed over the sprite?
		/// Only draws info when compiled in DEBUG configuration
		/// </summary>
		public bool ShouldDrawDebugInfo { get; set; } = true;

		public SpriteObject(string texturePath = "", float rotation = 0, string name = "", GameObject parent = null) : this(texturePath, Vector2.zero, rotation, name, parent) { }
		public SpriteObject(string texturePath, Vector2 position, float rotation = 0, string name = "", GameObject parent = null) : base(position, rotation, name, parent)
		{
			SetTexture(texturePath);
			Size = TextureSize;
		}

		public void SetTexture(string texturePath)
		{
			if (!string.IsNullOrEmpty(texturePath))
			{
				SetTexture(Resources.LoadTexture(texturePath));
				return;
			}

			m_Texture = new Texture2D();
			TextureSize = Vector2.zero;
		}

		public void SetTexture(Texture2D texture)
		{
			m_Texture = texture;
			TextureSize = new Vector2(m_Texture.width, m_Texture.height);
		}

		protected override void OnDraw()
		{
			if (TextureSize.x > 0 && TextureSize.y > 0)
				DrawTexturePro(
					m_Texture,
					// Texture Position Offset, Width & Height
					new Rectangle(0, 0, TextureSize.x, TextureSize.y),
					// Draw Position, Width & Height
					new Rectangle(GlobalPosition.x, GlobalPosition.y, Size.x, Size.y),
					// Rotation origin (relative to position)
					(Size / 2f) - Offset,
					GlobalRotation,
					Tint
					);
			else
				DrawRectanglePro( // Draw pink square, texture is invalid
					new Rectangle(GlobalPosition.x, GlobalPosition.y, Size.x, Size.y),
					(Size / 2f) - Offset,
					GlobalRotation,
					Colour.Magenta
					);

			DrawDebugInfo();
			base.OnDraw();
		}

		private void DrawDebugInfo()
		{
#if DEBUG
			if (!ShouldDrawDebugInfo)
				return;
			DrawLineEx(GlobalPosition, GlobalPosition + GlobalRight * 15f, 2f, Colour.Red);	   // Right direction
			DrawCircleLines((int)GlobalPosition.x, (int)GlobalPosition.y, 2f, Colour.Green);   // Center of sprite
			DrawLineEx(GlobalPosition, GlobalPosition + GlobalForward * 30f, 2f, Colour.Blue); // Forward direction
#endif
		}
	}
}
