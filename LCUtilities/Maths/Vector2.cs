using System;
using System.Runtime.CompilerServices;

namespace LCUtils
{
	public struct Vector2
	{
		private float[] m_Components;

		public float x { get => m_Components[0]; set => m_Components[0] = value; }
		public float y { get => m_Components[1]; set => m_Components[1] = value; }

		public static Vector2 zero	=> new Vector2( 0,  0);
		public static Vector2 up	=> new Vector2( 0,  1);
		public static Vector2 down	=> new Vector2( 0, -1);
		public static Vector2 left	=> new Vector2(-1,  0);
		public static Vector2 right => new Vector2( 1,  0);

		public Vector2 normalized
		{
			get
			{
				Vector2 vector = new Vector2(this);
				vector.Normalize();
				return vector;
			}
		}

		public Vector2 perpendicular => new Vector2(m_Components[1], -m_Components[0]);

		#region Constructors
		public Vector2(float x = 0, float y = 0) => m_Components = new float[] { x, y };

		// Copy constructor
		public Vector2(Vector2 other) : this(other.x, other.y) { }
		#endregion
		
		public float Dot(Vector2 b) => m_Components[0] * b.m_Components[0] + m_Components[1] * b.m_Components[1];

		public float Magnitude() => (float)Math.Sqrt(m_Components[0] * m_Components[0] + m_Components[1] * m_Components[1]);

		// Magnitude * Magnitude
		public float MagnitudeSqr() => m_Components[0] * m_Components[0] + m_Components[1] * m_Components[1];

		public void Normalize()
		{
			float magnitude = Magnitude();
			m_Components[0] /= magnitude;
			m_Components[1] /= magnitude;
		}

		public float Distance(Vector2 other) => (this - other).Magnitude();
		public static float Distance(Vector2 a, Vector2 b) => a.Distance(b);

		public float DistanceSqr(Vector2 other) => (this - other).MagnitudeSqr();
		public static float DistanceSqr(Vector2 a, Vector2 b) => a.DistanceSqr(b);

		public float Angle(Vector2 other) => (float)Math.Acos(Dot(other) / (Magnitude() * other.Magnitude()));

		/// <summary>
		/// Sets this vector to the reflection off the normal
		/// </summary>
		/// <param name="normal">Normal to reflect against</param>
		public void Reflect(Vector2 normal)
		{
			Vector2 reflected = Reflected(normal);
			m_Components[0] = reflected.m_Components[0];
			m_Components[1] = reflected.m_Components[1];
		}

		/// <summary>
		/// Reflects this vector without modifying it
		/// </summary>
		/// <param name="normal">Normal to reflect against</param>
		public Vector2 Reflected(Vector2 normal) => -2f * Dot(normal) * normal + this;

		/// <param name="angle">Angle to rotate, in radians</param>
		public Vector2 Rotate(float angle)
		{
			float cos = (float)Math.Cos(angle);
			float sin = (float)Math.Sin(angle);
			float cx = x, cy = y;

			x = (cx * cos) - (cy * sin);
			y = (cx * sin) + (cy * cos);
			return this;
		}

		/// <param name="angle">Angle to rotate, in radians</param>
		public Vector2 RotateAround(Vector2 origin, float angle)
		{
			float cos = (float)Math.Cos(angle);
			float sin = (float)Math.Sin(angle);
			float cx = x, cy = y;

			x = ((cx - origin.x) * cos) - ((cy - origin.y) * sin) + origin.x;
			y = ((cx - origin.x) * sin) + ((cy - origin.y) * cos) + origin.y;

			return this;
		}

		#region Operator Overloads
		/// ADD ///
		public static Vector2 operator +(Vector2 a, Vector2 b) => new Vector2(a.m_Components[0] + b.m_Components[0], a.m_Components[1] + b.m_Components[1]);

		/// SUBTRACT ///
		public static Vector2 operator -(Vector2 a) => new Vector2(-a.m_Components[0], -a.m_Components[1]);
		public static Vector2 operator -(Vector2 a, Vector2 b) => new Vector2(a.m_Components[0] - b.m_Components[0], a.m_Components[1] - b.m_Components[1]);

		/// DIVIDE ///
		public static Vector2 operator /(Vector2 a, float value) => new Vector2(a.m_Components[0] / value, a.m_Components[1] / value);
		public static Vector2 operator /(float value, Vector2 a) => new Vector2(a.m_Components[0] / value, a.m_Components[1] / value);
		public static Vector2 operator /(Vector2 a, Vector2 b) => new Vector2(a.m_Components[0] / b.m_Components[0], a.m_Components[1] / b.m_Components[1]);

		/// MULTIPLY ///
		public static Vector2 operator *(Vector2 a, float value) => new Vector2(a.m_Components[0] * value, a.m_Components[1] * value);
		public static Vector2 operator *(float value, Vector2 a) => new Vector2(a.m_Components[0] * value, a.m_Components[1] * value);
		public static Vector2 operator *(Vector2 a, Vector2 b) => new Vector2(a.m_Components[0] * b.m_Components[0], a.m_Components[1] * b.m_Components[1]);

		public static bool operator ==(Vector2 a, Vector2 b) => a.x == b.x && a.y == b.y;
		public static bool operator !=(Vector2 a, Vector2 b) => a.x != b.x || a.y != b.y;

		/// CONVERSIONS ///
		public static implicit operator Vector2(System.Numerics.Vector2 v) => new Vector2(v.X, v.Y);
		public static implicit operator System.Numerics.Vector2(Vector2 v) => new System.Numerics.Vector2(v.x, v.y);

		public override string ToString() => $"({x}, {y})";

		public override bool Equals(object obj)
		{
			if (obj is Vector2) return this == (Vector2)obj;
			return false;
		}

		public override int GetHashCode() => base.GetHashCode();

		public float this[int i]
		{
			get => i >= 2 || i < 0 ? 0 : m_Components[i];
			set
			{
				if (i >= 2)
					return;
				m_Components[i] = value;
			}
		}
		#endregion
	}
}
