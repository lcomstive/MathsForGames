using LCUtils;
using Raylib_cs;

namespace LCGF.GameObjects.UI
{
	public class UIText : UIElement
	{
		public const int DefaultFontSize = 20;

		public Colour Colour = Colour.White;
		public int FontSize	 { get; set; }
		public string Text   { get; set; }

		public UIText(string text = "", int fontSize = DefaultFontSize, GameObject parent = null) : this(text, Vector2.zero, Colour.White, fontSize, parent) { }
		public UIText(string text, Vector2 position, UIElement parent = null) : this(text, position, Colour.White, DefaultFontSize, parent) { }
		public UIText(string text, Vector2 position, int fontSize = DefaultFontSize, GameObject parent = null) : this(text, position, Colour.White, fontSize, parent) { }
		public UIText(string text, Vector2 position, Colour colour, int fontSize = DefaultFontSize, GameObject parent = null)
			: base(position, Vector2.zero, "UIText", parent)
		{
			Text = text;
			Colour = colour;
			Position = position;
			FontSize = fontSize;

			Size = new Vector2(Raylib.MeasureText(text, fontSize), fontSize);
		}

		protected override void OnDraw() => Raylib.DrawText(Text, (int)(GlobalPosition.x - Size.x / 2f), (int)(GlobalPosition.y - Size.y / 2f), FontSize, Colour);
	}
}
