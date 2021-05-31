using System;
using LCUtils;

namespace LCGF.Physics
{
	public class AABB
	{
		public Vector2 Min { get; set; }
		public Vector2 Max { get; set; }

		public float Width => Max.x - Min.x;
		public float Height => Max.y - Min.y;

		public Vector2 Center => (Max + Min) / 2f;
		public Vector2 Size => new Vector2(Width, Height);
		public Vector2 Extents => Size / 2f;

		public AABB(Vector2 min, Vector2 max)
		{
			Min = min;
			Max = max;
		}

		public AABB(AABB other)
		{
			Min = other.Min;
			Max = other.Max;
		}

		public bool PointInside(Vector2 point) =>
			point.x >= Min.x && point.x <= Max.x &&
			point.y >= Min.y && point.y <= Max.y;

		public bool Intersects(AABB other)
		{
			if (Max.x < other.Min.x || Min.x > other.Max.x) return false;
			if (Max.y < other.Min.y || Min.y > other.Max.y) return false;
			return true;
		}

		public override string ToString() => $"{Min}-{Max}";
	}
}
