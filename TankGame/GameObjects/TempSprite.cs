using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LCGF;
using LCGF.GameObjects;
using LCUtils;

namespace TankGame.GameObjects
{
	public class TempSprite : SpriteObject
	{
		public bool FadeOverTime { get; set; } = false;
		public float TimeUntilDestroy { get; set; } = 10f;

		private readonly float m_InitialTimeUntilDestroy;

		public TempSprite(string texturePath, Vector2 position, float lifetime, GameObject parent = null) : this(texturePath, position, 0f, lifetime, parent) { }
		public TempSprite(string texturePath, Vector2 position, float rotation = 0f, float lifetime = 10f, GameObject parent = null)
			: base(texturePath, position, rotation, "Sprite", parent)
		{
			Size = TextureSize;
			TimeUntilDestroy = lifetime;
			m_InitialTimeUntilDestroy = lifetime;
			ShouldDrawDebugInfo = false;
		}

		protected override void OnUpdate()
		{
			TimeUntilDestroy -= Time.DeltaTime;
			if (FadeOverTime)
				Tint.SetAlpha((byte)(255 * Math.Floor(TimeUntilDestroy / m_InitialTimeUntilDestroy)));
			if (TimeUntilDestroy <= 0f)
				Destroy();
		}
	}
}
