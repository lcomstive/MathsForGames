using System;

namespace LCUtils
{
	public class Matrix3
	{
		private float[] m_Components;

		public static Matrix3 Identity => new Matrix3(
			1, 0, 0,
			0, 1, 0,
			0, 0, 1
			);

		public Matrix3()
		{
			m_Components = new float[3 * 3];
			for (int i = 0; i < m_Components.Length; i++)
				m_Components[i] = 0;
		}

		public Matrix3(float[] components)
		{
			m_Components = new float[3 * 3];

			// Copy components into m_Components
			Array.Copy(components, m_Components, Math.Min(components.Length, m_Components.Length));

			// Fill remainder of array if input components are incomplete
			for (int i = components.Length; i < m_Components.Length; i++)
				m_Components[i] = 0;
		}

		public Matrix3(float m00, float m10, float m20,
					   float m01, float m11, float m21,
					   float m02, float m12, float m22)
		{
			m_Components = new float[]
			{
				m00, m10, m20,
				m01, m11, m21,
				m02, m12, m22
			};
		}

		public static Vector3 operator *(Matrix3 m, Vector3 v) => new Vector3(
			m[0][0] * v[0] + m[1][0] * v[1] + m[2][0] * v[2],
			m[0][1] * v[0] + m[1][1] * v[1] + m[2][1] * v[2],
			m[0][2] * v[0] + m[1][2] * v[1] + m[2][2] * v[2]
			);

		public static Vector3 operator *(Vector3 v, Matrix3 m) => new Vector3(
			 m[0][0] * v[0] + m[0][1] * v[1] + m[0][2] * v[2],
			 m[1][0] * v[0] + m[1][1] * v[1] + m[1][2] * v[2],
			 m[2][0] * v[0] + m[2][1] * v[1] + m[2][2] * v[2]
			 );

		public static Matrix3 operator *(Matrix3 a, Matrix3 b)
		{
			Matrix3 value = new Matrix3();
			for (int i = 0; i < 3; i++)
				for (int j = 0; j < 3; j++)
					for (int k = 0; k < 3; k++)
						value[j, i] += a[k, i] * b[j, k];
			return value;
		}

		public override string ToString()
		{
			string s = string.Empty;
			for (int i = 0; i < m_Components.Length; i++)
				s += m_Components[i] + (i < m_Components.Length - 1 ? ", " : "");
			return $"[{s}]";
		}

		public Vector3 this[int i]
		{
			get => i >= 3 || i < 0 ? Vector3.zero : new Vector3(m_Components[i], m_Components[i + 3], m_Components[i + 6]);
			set
			{
				if (i >= 3)
					return;
				m_Components[i]		= value.x;
				m_Components[i + 3] = value.y;
				m_Components[i + 6] = value.z;
			}
		}

		public float this[int i, int j]
		{
			get => (i >= 3 || j >= 3 || i < 0 || j < 0) ? 0 : m_Components[i + (j * 3)];
			set
			{
				if (i >= 3 || j >= 3)
					return;
				m_Components[i + (j * 3)] = value;
			}
		}
	}
}
