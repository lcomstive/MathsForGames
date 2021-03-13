using System;
using LCUtils;

namespace LCPhysics2D.Shapes
{
	public struct Rectangle
	{
		public Vector2 Minimum => m_Minimum;
		public Vector2 Maximum => m_Maximum;

		public float Width => Maximum.x - Minimum.x;
		public float Height => Maximum.y - Minimum.y;
		public Vector2 Center => (Maximum + Minimum) / 2f;

		private Vector2 m_Minimum, m_Maximum;

		public Rectangle(Vector2 min, Vector2 max)
		{
			m_Minimum = min;
			m_Maximum = max;
		}

		public Rectangle(float minX, float minY, float width, float height) : this(new Vector2(minX, minY), new Vector2(minX + width, minY + height)) { }

		public static Rectangle FromCenterPoint(Vector3 center, Vector2 size) => FromCenterPoint(center.xy, size);
		public static Rectangle FromCenterPoint(Vector2 center, Vector2 size) => new Rectangle(center - size / 2f, center + size / 2f);

		public bool IsPointInside(Vector2 point) =>
			point.x > Minimum.x && point.x < Maximum.x &&
			point.y > Minimum.y && point.y < Maximum.y;

		// TODO: Implement rotation
		public bool Intersects(Rectangle rect)
		{
			if (Maximum.x < rect.Minimum.x || Minimum.x > rect.Maximum.x) return false;
			if (Maximum.y < rect.Minimum.y || Minimum.y > rect.Maximum.y) return false;

			return true;
		}

		public bool Intersects(Circle circle) => circle.Intersects(this);
	}

	public class RectanglePhysics2DObject : Physics2DObject
	{
		internal Rectangle Object;
		protected override Physics2DShape PhysicsShape => Physics2DShape.Rectangle;

		public RectanglePhysics2DObject(Rectangle obj) => Object = obj;
		public RectanglePhysics2DObject(Vector2 min, Vector2 max) => Object = new Rectangle(min, max);

		internal override bool CheckCollision(Rectangle rect) => Object.Intersects(rect);
		internal override bool CheckCollision(Circle circle) => Object.Intersects(circle);

		internal override void ResolveCollision(Circle circle)
		{

		}

		internal override void ResolveCollision(Rectangle rect)
		{

		}
	}
}
