using LCUtils;
using LCGF.Physics;

using static Raylib_cs.Raylib;

namespace LCGF.GameObjects.UI
{
	/// <summary>
	/// Base class for UI elements
	/// </summary>
	public abstract class UIElement : GameObject
	{
		public bool IsMouseInside => m_MouseInside;

		/// <summary>
		/// Collider to check against mouse position.
		/// (use SetCollider to change this)
		/// </summary>
		protected PolygonCollider Collider { get; private set; }

		private bool m_MouseInside = false;

		public UIElement(Vector2 size, string name = "UI Element", GameObject parent = null) : this(Vector2.zero, size, name, parent) { }
		public UIElement(Vector2 position, Vector2 size, string name = "UI Element", GameObject parent = null)
			: base(position, name, parent)
			=> Size = size;

		protected void SetCollider(PolygonCollider collider)
		{
			if (Collider != null) // If already has a collider, remove this as the parent
				Collider.Parent = null;

			Collider = collider;
			if (Collider == null)
				return;
			Collider.IsTrigger = true;
			Collider.Parent = this;
		}

		protected override void OnUpdate()
		{
			if (Collider == null)
				return;
			Collider.BoundingBox.Min = GlobalPosition - Size / 2f;
			Collider.BoundingBox.Max = GlobalPosition + Size / 2f;
			Collider.Update(); // Update internal values

			// Check for mouse inside collider
			bool mouseInside = Collider.PointInside(Application.MousePos);
			if (mouseInside && !m_MouseInside) OnMouseEnter();
			if (!mouseInside && m_MouseInside) OnMouseExit();

			m_MouseInside = mouseInside;
		}

		protected virtual void OnMouseExit()  { }
		protected virtual void OnMouseEnter() { }

		protected override void OnDraw()
		{
#if DEBUG
			if (Collider == null)
				return;

			// Draw collider
			if (Collider.GetType() == typeof(CircleCollider))
			{
				CircleCollider circle = (CircleCollider)Collider;
				DrawCircleLines((int)circle.Position.x, (int)circle.Position.y, circle.Radius, Colour.Green);
			}
			else
			{
				Vector2[] vertices = Collider.GetVertices();

				System.Numerics.Vector2[] points = new System.Numerics.Vector2[vertices.Length + 1];
				for (int i = 0; i < vertices.Length; i++)
					points[i] = vertices[i];
				points[^1] = points[0]; // Last = First

				DrawLineStrip(points, points.Length, Colour.Green);
			}
#endif
		}
	}
}
