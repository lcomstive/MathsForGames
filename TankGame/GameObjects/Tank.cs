using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LCGF;
using LCGF.Physics;
using LCGF.GameObjects;
using LCGF.GameObjects.UI;
using LCUtils;

namespace TankGame.GameObjects
{
	public class Tank : Rigidbody
	{
		#region Assets
		// #colour gets replaced at runtime with GetColouredTankAsset()
		private const string TankBodyPath = "Tank Sprites/Tanks/tankBody_#colour_outline.png";
		private const string TankBarrelPath = "Tank Sprites/Barrels/tank#colour_barrel2_outline.png";
		private const string TankBulletPath = "Tank Sprites/Bullets/bullet#colour1_outline.png";
		private const string MuzzleFlashPath = "Tank Sprites/Particles/shotOrange.png";
		private const string TankTracksPath = "Tank Sprites/Tanks/tracksSmall.png";

		/// <summary>
		/// Returns the path with the colour component replaced with the input colour
		/// </summary>
		/// <param name="path">Path to emplace the colour in</param>
		/// <param name="colour">Colour of the tank, converted to have first letter uppercase and the remainder lowercase</param>
		private static string GetColouredTankAsset(string path, string colour) =>
			path.Replace("#colour", colour[0] + colour[1..].ToLower());
		#endregion

		public const float MaxHealth = 100f;

		public readonly string m_Colour;
		public float Health { get; set; } = MaxHealth;
		public SpriteObject Turret { get; private set; }

		private UISliderText m_HealthSlider;

		#region Tank Track Variables
		private const int TankTrackPoolSize = 100;
		private const byte TankTrackMinAlpha = 10;
		private const float m_TankTrackFadeSpeed = 0.5f;
		private const float m_TankTrackCreationTime = 0.1f; // Seconds

		private SpriteObject[] m_TankTracks;
		private int m_TankTrackIndex = 0;
		private float m_LastTankTrackCreationTime = m_TankTrackCreationTime;
		private Vector2 m_LastPosition;
		#endregion

		public Tank(Vector2 position, string colour = "Red", GameObject parent = null) : base(position, 180f, "Tank", parent)
		{
			m_Colour = colour;
			float width = Application.ScreenSize.x / 20;
			Size = new Vector2(width, width);
			m_LastPosition = Position;
			SetCollider(BoxCollider.FromCenter(GlobalPosition, Size));

			// Tank Tracks
			m_TankTracks = new SpriteObject[TankTrackPoolSize];
			for (int i = 0; i < TankTrackPoolSize; i++)
			{
				m_TankTracks[i] = new SpriteObject(TankTracksPath, 0, "Tank Tracks", Parent);
				m_TankTracks[i].Size = Size;
				m_TankTracks[i].Tint.SetAlpha(0); // Hide
				m_TankTracks[i].Position = Position;
				m_TankTracks[i].ShouldDrawDebugInfo = false;
	
				Parent.IncreaseChildIndex(this); // Draw tank after this track
			}

			// Tank Sprite
			SpriteObject sprite = new SpriteObject(
				GetColouredTankAsset(TankBodyPath, m_Colour),
				0f, // Rotation
				"Tank Sprite",
				this
				);
			sprite.Size = Size;

			// Turret
			Turret = new SpriteObject(
				GetColouredTankAsset(TankBarrelPath, m_Colour),
				0, // Rotation
				"Turret",
				this);
			Turret.Size   = new Vector2(Size.x * 0.25f, Size.y * 0.8f);
			Turret.Offset = new Vector2(Turret.Offset.x, Turret.Size.y * 0.75f);

			// Health Bar
			m_HealthSlider = new UISliderText(
				Vector2.up * Size.y, // Position
				new Vector2(Size.x * 1.25f, 20), // Size
				Parent	// Parent
				);
			m_HealthSlider.Tint = Colour.Black;
			m_HealthSlider.Text = $"{Health}/{MaxHealth}";
			m_HealthSlider.ForegroundTint = Colour.Red;
			m_HealthSlider.Value = (Health / MaxHealth);
			m_HealthSlider.FontSize = 14;
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			m_HealthSlider.Position = Position + Vector2.up * Size.y;
			m_HealthSlider.Text = $"{Health}/{MaxHealth}";
			m_HealthSlider.Value = Health / MaxHealth;

			#region Tank Tracks Update
			float fadeAmount = m_TankTrackFadeSpeed * Time.DeltaTime;
			for (int i = 0; i < TankTrackPoolSize; i++)
				if(m_TankTracks[i].Tint.a > TankTrackMinAlpha)
					m_TankTracks[i].Tint.a = (byte)Math.Clamp(m_TankTracks[i].Tint.a - fadeAmount, TankTrackMinAlpha, 255);

			m_LastTankTrackCreationTime -= Time.DeltaTime;
			if (m_LastPosition.Equals(Position) || m_LastTankTrackCreationTime > 0f)
				return;
			m_LastPosition = Position;
			m_LastTankTrackCreationTime = m_TankTrackCreationTime;

			m_TankTracks[m_TankTrackIndex].Position = Position;
			m_TankTracks[m_TankTrackIndex].Rotation = 180f - GlobalRotation;
			m_TankTracks[m_TankTrackIndex].Tint.SetAlpha(255);
			m_TankTracks[m_TankTrackIndex].Update();

			m_TankTrackIndex = (m_TankTrackIndex + 1) % TankTrackPoolSize;
			#endregion
		}

		protected override void OnTriggered(MTV mtv, Rigidbody other)
		{
			Bullet b = other as Bullet;
			if (b == null)
				return; // It wasn't a bullet..

			Health -= b.Damage;
			if (Health <= 0)
			{
				m_HealthSlider.Destroy();
				Destroy();
			}
		}

		public void ShootBullet(float bulletSpeed)
		{
			Vector2 size = new Vector2(Size.x / 7.5f, Size.y / 4f);
			string texturePath = GetColouredTankAsset(TankBulletPath, m_Colour);
			Vector2 position = (Turret.GlobalPosition - Parent.Position) + Turret.GlobalForward * Turret.Size.y * 1.5f;
			Bullet b = new Bullet(texturePath, position, size, Parent);
			b.Velocity = Turret.GlobalForward * bulletSpeed;
			b.Rotation = 180f - Turret.GlobalRotation;

			Parent.AddChild(new TempSprite(MuzzleFlashPath, b.Position, b.Rotation - 180f, 0.1f));
		}
	}
}
