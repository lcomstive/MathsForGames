using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LCUtils;
using Raylib_cs;

namespace LCGF.GameObjects.UI
{
	public class UIButton : UIRect
	{
		public string Text { get; set; } = "";
		public int FontSize { get; set; } = 20;

		public Colour TextColour = Colour.White,
					  HoverTint  = Colour.White,
					  PressTint  = Colour.White;

		private Colour m_OriginalTint = Colour.White;

		public UIButton(Vector2 size, GameObject parent = null) : this(Vector2.zero, size, string.Empty, Colour.White, parent) { }
		public UIButton(Vector2 position, Vector2 size, GameObject parent = null) : this(position, size, string.Empty, Colour.White, parent) { }
		public UIButton(Vector2 position, Vector2 size, string texture, GameObject parent = null) : this(position, size, texture, Colour.White, parent) { }
		public UIButton(Vector2 position, Vector2 size, string texture, Colour tint, GameObject parent = null)
			: base(position, size, parent) { }

		protected override void OnUpdate()
		{
			base.OnUpdate();

			if (IsMouseInside && Raylib.IsMouseButtonPressed(MouseButton.MOUSE_LEFT_BUTTON))  ButtonClicked?.Invoke();
			if (IsMouseInside && Raylib.IsMouseButtonPressed(MouseButton.MOUSE_RIGHT_BUTTON)) AltButtonClicked?.Invoke();

			if (IsMouseInside && (
				Raylib.IsMouseButtonDown(MouseButton.MOUSE_LEFT_BUTTON) ||
				Raylib.IsMouseButtonDown(MouseButton.MOUSE_RIGHT_BUTTON)
				))
			{
				Tint = PressTint;
			}
			else if (IsMouseInside)
				Tint = HoverTint;
		}

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

		protected override void OnMouseExit()  => Tint = m_OriginalTint;
		protected override void OnMouseEnter() => m_OriginalTint = Tint;

		public delegate void OnButtonClicked();
		public event OnButtonClicked ButtonClicked;

		public delegate void OnAltButtonClicked();
		public event OnAltButtonClicked AltButtonClicked;
	}
}
