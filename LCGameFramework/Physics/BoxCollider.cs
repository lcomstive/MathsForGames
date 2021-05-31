using System;
using LCUtils;
using Raylib_cs;
using static Raylib_cs.Raylib;

namespace LCGF.Physics
{
	public class BoxCollider : PolygonCollider
	{
		public float Rotation
		{
			get => m_Rotation;
			set
			{
				m_RotationDelta = (m_Rotation - value);
				m_Rotation = value;
				UpdatePoints();
			}
		}
		private float m_Rotation = 0f, m_RotationDelta = 0f;

		public BoxCollider(AABB boundingBox, float rotation = 0f) : base(boundingBox)
			=> Rotation = rotation;

		public BoxCollider(Vector2 min, Vector2 max, float rotation = 0f) : this(new AABB(min, max), rotation) { }

		public static BoxCollider FromCenter(Vector2 position, Vector2 size)
			=> new BoxCollider(new AABB(position - size / 2f, position + size / 2f));

		public MTV GetOverlap(CircleCollider circle) => circle.GetOverlap(this);

		public override bool PointInside(Vector2 v) => BoundingBox.PointInside(v);

		private void UpdatePoints()
		{
			float radians = MathUtility.ToRadians(m_RotationDelta);
			for (int i = 0; i < VertexCount; i++)
				m_Vertices[i].RotateAround(Center, -radians);

			RecalculateBoundingBox();
		}

		protected internal override void OnUpdate()
		{
			if (Parent != null)
				Rotation = Parent.GlobalRotation;
		}
	}
}
