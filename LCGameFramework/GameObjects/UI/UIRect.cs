using LCUtils;
using Raylib_cs;
using LCGF.Physics;

namespace LCGF.GameObjects.UI
{
	public class UIRect : UIElement
	{
		public Colour Tint = Colour.White;
		public Texture2D Texture = new Texture2D();

		public bool IsTextureValid => Texture.width > 0;

		public UIRect(Vector2 size, GameObject parent = null) : this(Vector2.zero, size, parent) { }
		public UIRect(Vector2 position, Vector2 size, GameObject parent = null)
			: base(position, size, "UIRect", parent)
		{
			Size = size;
			Position = position;

			SetCollider(BoxCollider.FromCenter(GlobalPosition, Size));
		}

		protected override void OnDraw()
		{
			Rectangle drawRect = new Rectangle(GlobalPosition.x, GlobalPosition.y, Size.x, Size.y);
			if (IsTextureValid)
			{
				Rectangle sourceRect = new Rectangle(0, 0, Texture.width, Texture.height);
				Raylib.DrawTexturePro(Texture, sourceRect, drawRect, Size / 2f, GlobalRotation, Tint);
			}
			else
				Raylib.DrawRectanglePro(drawRect, Size / 2f, GlobalRotation, Tint);

			base.OnDraw();
		}
	}
}
