using LCUtils;
using LCGF;
using LCGF.Physics;
using LCGF.GameObjects;
using Raylib_cs;

namespace TankGame.GameObjects
{
	class Bullet : Rigidbody
	{
		public float TimeBeforeDestroy { get; set; } = 30f; // Seconds
		public float Damage { get; set; } = 10f;

		private Texture2D m_Texture;

		private static readonly string[] ExplosionTextures =
		{
			"Tank Sprites/Particles/explosion1.png",
			"Tank Sprites/Particles/explosion2.png",
			"Tank Sprites/Particles/explosion3.png",
			"Tank Sprites/Particles/explosion4.png"
		};
		private static readonly string ExplosionSound = "Sounds/Tink.wav";
		private static readonly Vector2 ExplosionSoundPitchRange = new Vector2(0.8f, 1.25f);

		public Bullet(string texturePath, Vector2 position, Vector2 size, GameObject parent = null)
			: base(position, 0f, "Bullet", parent)
		{
			if (!Resources.TryLoadTexture(texturePath, out m_Texture))
			{
				Destroy(); // No texture, no bullet
				return;
			}

			Size = size;
			SetCollider(BoxCollider.FromCenter(GlobalPosition, Size));
			Collider.IsTrigger = true;
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();

			TimeBeforeDestroy -= Time.DeltaTime;
			if (TimeBeforeDestroy <= 0f)
				Destroy();
		}

		protected override void OnDraw()
		{
			base.OnDraw();

			Raylib.DrawTexturePro(
				m_Texture,
				// Texture Position Offset, Width & Height
				new Rectangle(0, 0, m_Texture.width, m_Texture.height),
				// Draw Position, Width & Height
				new Rectangle(GlobalPosition.x, GlobalPosition.y, Size.x, Size.y),
				// Rotation origin (relative to position)
				(Size / 2f),
				GlobalRotation,
				Colour.White
				);
 		}

		protected override void OnTriggered(MTV mtv, Rigidbody other)
		{
			// Explosion animated sprite
			AnimatedSpriteObject explosion = new AnimatedSpriteObject(ExplosionTextures, Position, "Bullet Explosion", Parent);
			explosion.AnimationSpeed = 0.05f;
			explosion.Loop = false;

			// Sound
			AudioSource audio = new AudioSource(ExplosionSound, Parent);
			audio.DestroyOnFinish = true;
			audio.Pitch = MathUtility.Random(ExplosionSoundPitchRange.x, ExplosionSoundPitchRange.y);

			Destroy();
		}
	}
}