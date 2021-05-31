using System;
using LCUtils;
using LCGF.GameObjects;
using System.Collections.Generic;

namespace LCGF.Physics
{
	public class LineCollider : PolygonCollider
	{
		public LineCollider(Vector2 a, Vector2 b) : base(new Vector2[] { a, b }) { }

		// Based on Randy Gaul's http://www.randygaul.net/2014/07/23/distance-point-to-line-segment/
		public float Distance(Vector2 p) => (float)Math.Sqrt(DistanceSqr(p));

		public float DistanceSqr(Vector2 p)
		{
			Vector2 normal = m_Normals[0];
			Vector2 pointA = p - m_Vertices[0];
			Vector2 c = normal * (pointA.Dot(normal) / normal.Dot(normal));
			Vector2 direction = pointA - c;
			return direction.Dot(direction);
		}

		public override bool PointInside(Vector2 v) => Math.Abs(Distance(v)) < 0.001f;
	}

	public class PolygonCollider
	{
		public Vector2 Center
		{
			get => BoundingBox.Center;
			set
			{
				Vector2 offset = value - BoundingBox.Center;
				for (int i = 0; i < VertexCount; i++)
					m_Vertices[i] += offset;
				RecalculateBoundingBox();
			}
		}

		public AABB BoundingBox => new AABB(m_AABB); // Copy constructor
		private AABB m_AABB;

		public Vector2 Size => BoundingBox.Size;
		public Vector2 Extents => BoundingBox.Extents;
		public GameObject Parent { get; internal set; }

		/// <summary>
		/// When set to true does not react to collisions,
		/// but instead sends an OnTrigger event to the rigidbody
		/// </summary>
		public bool IsTrigger { get; set; } = false;

		public Vector2[] Vertices
		{
			get
			{
				Vector2[] copy = new Vector2[VertexCount];
				Array.Copy(m_Vertices, copy, VertexCount);
				return copy;
			}
		}

		public int VertexCount => m_Vertices.Length;

		protected readonly Vector2[] m_Vertices;
		protected readonly Vector2[] m_Normals;

		public PolygonCollider(List<Vector2> vertices)
		{
			m_Vertices = vertices.ToArray();
			m_Normals = new Vector2[VertexCount];
			RecalculateBoundingBox();
		}

		public PolygonCollider(Vector2[] vertices)
		{
			m_Vertices = new Vector2[vertices.Length];
			m_Normals = new Vector2[VertexCount];
			Array.Copy(vertices, m_Vertices, VertexCount);
			RecalculateBoundingBox();
		}

		public PolygonCollider(AABB bounds)
		{
			m_Vertices = new Vector2[]
			{	// Local coordinates
				new Vector2(-bounds.Extents.x,  bounds.Extents.y), // Top Left
				new Vector2( bounds.Extents.x,  bounds.Extents.y), // Top Right
				new Vector2( bounds.Extents.x, -bounds.Extents.y), // Bottom Right
				new Vector2(-bounds.Extents.x, -bounds.Extents.y) // Bottom Left
			};
			for (int i = 0; i < VertexCount; i++)
				m_Vertices[i] = bounds.Center - m_Vertices[i]; // World coordinates
			m_Normals = new Vector2[VertexCount];
			RecalculateBoundingBox();
		}

		public PolygonCollider(Vector2 min, Vector2 max) : this(new AABB(min, max)) { }

		/// <returns>An array of points</returns>
		/// Returned array is a copy, so the original is not modified
		public Vector2[] GetVertices()
		{
			Vector2[] copy = new Vector2[m_Vertices.Length];
			Array.Copy(m_Vertices, copy, copy.Length);
			return copy;
		}

		public Vector2[] GetNormals()
		{
			Vector2[] copy = new Vector2[m_Normals.Length];
			Array.Copy(m_Normals, copy, copy.Length);
			return copy;
		}

		protected void RecalculateBoundingBox()
		{
			Vector2 min = new Vector2(m_Vertices[0]);
			Vector2 max = new Vector2(m_Vertices[0]);
			foreach (Vector2 v in m_Vertices)
			{
				min.x = Math.Min(min.x, v.x);
				min.y = Math.Min(min.y, v.y);
				max.x = Math.Max(max.x, v.x);
				max.y = Math.Max(max.y, v.y);
			}

			m_AABB = new AABB(min, max);
			for (int i = 0; i < VertexCount; i++)
				m_Normals[i] =
					(m_Vertices[i] - m_Vertices[i < VertexCount - 1 ? (i + 1) : 0])
					.normalized.perpendicular;

			OnBoundingBoxRecalculated();
		}

		protected virtual void OnBoundingBoxRecalculated() { }

		public Vector2 GetClosestVertex(Vector2 point)
		{
			int closestVertex = 0;
			float closestDistance = Math.Abs((m_Vertices[0] - point).MagnitudeSqr());
			for(int i = 1; i < VertexCount; i++)
			{
				float distance = Math.Abs((m_Vertices[i] - point).MagnitudeSqr());
				if (distance >= closestDistance)
					continue;
				closestVertex = i;
				closestDistance = distance;
			}
			return m_Vertices[closestVertex];
		}

		internal void Update()
		{
			if (Parent != null)
				Center = Parent.GlobalPosition;

			OnUpdate();
		}

		internal protected virtual void OnUpdate() { }

		public virtual bool PointInside(Vector2 v)
		{
			if (!BoundingBox.PointInside(v))
				return false; // Not within the AABB, definitely not inside
			foreach(Vector2 normal in m_Normals)
			{
				if (v.Dot(normal) > 0f)
					return false; // Point is in front of a normal, meaning it's outside the polygon
			}
			return true; // Behind each polygon normal, inside of polygon
		}

		#region Separated Axis Theorem
		public virtual MTV GetOverlap(PolygonCollider p)
		{
			MTV mtv = new MTV()
			{
				Value = 0,
				Axis = Vector2.zero
			};
			if (!m_AABB.Intersects(p.m_AABB))
				return mtv;
			if (p.GetType() == typeof(CircleCollider))
				return ((CircleCollider)p).GetOverlap(this);

			float smallestOverlap = float.MaxValue;

			List<Vector2> axes = new List<Vector2>();
			// Polygon with less vertices gets tested first,
			// providing a faster exit if one of their axes doesn't overlap
			axes.AddRange(VertexCount < p.VertexCount ? m_Normals : p.m_Normals);
			axes.AddRange(VertexCount < p.VertexCount ? p.m_Normals : m_Normals);
			foreach (Vector2 axis in axes)
			{
				var overlap = GetOverlap(p, axis);
				if (overlap.Value < 0)
					return new MTV() { Axis = Vector2.zero, Value = 0 };
				if (overlap.Value >= smallestOverlap)
					continue;
				smallestOverlap = overlap.Value;
				mtv = overlap;
			}

			return mtv;
		}

		protected MTV GetOverlap(PolygonCollider p, Vector2 axis)
		{
			Vector2 projection1 = GetProjection(axis);
			Vector2 projection2 = p.GetProjection(axis);

			float projectedOverlap = Math.Min(projection1.y - projection2.x, projection2.y - projection1.x);

			return new MTV()
			{
				Axis = axis,
				Value = projectedOverlap
			};
		}

		/// <summary>
		/// Gets the largest projection with all vertices along an axis
		/// </summary>
		protected Vector2 GetProjection(Vector2 axis)
		{
			float min = m_Vertices[0].Dot(axis);
			float max = min;
			for (int i = 1; i < VertexCount; i++)
			{
				float proj = m_Vertices[i].Dot(axis);
				min = Math.Min(min, proj);
				max = Math.Max(max, proj);
			}

			return new Vector2(min, max);
		}
		#endregion
	}
}
