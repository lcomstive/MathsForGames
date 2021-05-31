using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LCUtils;
using LCGF.GameObjects;
using Raylib_cs;

namespace LCGF.GameObjects.UI
{
	public class UISlider : UIRect
	{
		public Colour ForegroundTint = Colour.White;
		public Texture2D ForegroundTexture = new Texture2D();

		public bool IsForegroundTextureValid => ForegroundTexture.width > 0;

		public float Value
		{
			get => m_Value;
			set => m_Value = Math.Clamp(value, 0f, 1f);
		}

		private float m_Value = 1f;

		public UISlider(Vector2 size, GameObject parent = null) : this(Vector2.zero, size, parent) { }
		public UISlider(Vector2 position, Vector2 size, GameObject parent = null) : base(position, size, parent) { }
			
		protected override void OnDraw()
		{
			base.OnDraw();

			Rectangle drawRect = new Rectangle(GlobalPosition.x, GlobalPosition.y, Size.x * Value, Size.y);
			if (IsForegroundTextureValid)
			{
				Rectangle sourceRect = new Rectangle(0, 0, ForegroundTexture.width, ForegroundTexture.height);
				Raylib.DrawTexturePro(ForegroundTexture, sourceRect, drawRect, Size / 2f, GlobalRotation, ForegroundTint);
			}
			else
				Raylib.DrawRectanglePro(drawRect, Size / 2f, GlobalRotation, ForegroundTint);
		}
	}

	public class UISliderText : UISlider
	{
		public string Text { get; set; } = "";
		public int FontSize { get; set; } = 20;
		public Colour TextColour = Colour.White;

		public UISliderText(Vector2 size, GameObject parent = null) : this(Vector2.zero, size, parent) { }
		public UISliderText(Vector2 position, Vector2 size, GameObject parent = null) : base(position, size, parent) { }

		protected override void OnDraw()
		{
			base.OnDraw();

			int textWidth = Raylib.MeasureText(Text, FontSize);
			Raylib.DrawText(
				Text,
				(int)(GlobalPosition.x - textWidth / 2f),
				(int)(GlobalPosition.y - FontSize / 2f),
				FontSize,
				TextColour);
		}
	}
}
