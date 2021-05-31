using System;
using LCUtils;
using Raylib_cs;
using System.Collections.Generic;
using LCGF.Physics;
using System.Linq;

namespace LCGF.Physics
{
	public class CircleCollider : PolygonCollider
	{
		public float Radius { get; set; }
		public Vector2 Position { get; set; }

		public CircleCollider(Vector2 position, float radius)
			: base(
				position - new Vector2(radius, radius),
				position + new Vector2(radius, radius)
			)
		{
			Radius = radius;
			Position = position;
		}

		protected override void OnBoundingBoxRecalculated()
		{
			Position = Center;
		}

		public MTV GetOverlap(CircleCollider other)
		{
			Vector2 distanceV = other.Position - Position;
			if (distanceV.x == 0 && distanceV.y == 0)
				return new MTV() { Axis = Vector2.up, Value = Math.Min(Radius, other.Radius) };
			float distance = distanceV.Magnitude();
			float radii = Radius + other.Radius;

			MTV mtv = new MTV() { Axis = Vector2.zero, Value = 0f };
			if (distance > radii)
				return mtv;

			mtv.Axis = -distanceV.normalized;
			mtv.Value = radii - distance;

			return mtv;
		}

		public override MTV GetOverlap(PolygonCollider p)
		{
			MTV mtv = new MTV()
			{
				Value = 0,
				Axis = Vector2.zero
			};
			if (!BoundingBox.Intersects(p.BoundingBox))
				return mtv;
			if (p.GetType() == typeof(CircleCollider))
				return GetOverlap((CircleCollider)p);

			float smallestOverlap = float.MaxValue;

			List<Vector2> normals = p.GetNormals().ToList();
			foreach (Vector2 axis in normals)
			{
				var overlap = GetOverlap(p, axis);
				if (overlap.Value < 0)
					return new MTV() { Axis = Vector2.zero, Value = 0 };
				if (overlap.Value >= smallestOverlap)
					continue;
				smallestOverlap = overlap.Value;
				mtv = overlap;
			}

			mtv.Axis *= -1.0f; // Flip as axis is from opposite polygon
			return mtv;
		}
	}
}
