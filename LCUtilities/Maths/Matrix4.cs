using System;

namespace LCUtils
{
	public class Matrix4
	{
		private float[] m_Components;

		public static Matrix4 Identity => new Matrix4(
			1, 0, 0, 0,
			0, 1, 0, 0,
			0, 0, 1, 0,
			0, 0, 0, 1
			);

		public Matrix4()
		{
			m_Components = new float[4 * 4];
			for (int i = 0; i < m_Components.Length; i++)
				m_Components[i] = 0;
		}

		public Matrix4(float[] components)
		{
			m_Components = new float[4 * 4];

			// Copy components into m_Components
			Array.Copy(components, m_Components, Math.Min(components.Length, m_Components.Length));

			// Fill remainder of array if input components are incomplete
			for (int i = components.Length; i < m_Components.Length; i++)
				m_Components[i] = 0;
		}

		public Matrix4(float m00, float m10, float m20, float m30,
					   float m01, float m11, float m21, float m31,
					   float m02, float m12, float m22, float m32,
					   float m03, float m13, float m23, float m33)
		{
			m_Components = new float[]
			{
				m00, m10, m20, m30,
				m01, m11, m21, m31,
				m02, m12, m22, m32,
				m03, m13, m23, m33
			};
		}

		public static Matrix4 CreateOrthographic(float width, float height, float depthNear, float depthFar)
			=> CreateOrthographic(0, width, 0, height, depthNear, depthFar);

		public static Matrix4 CreateOrthographic(
			float left, float right,
			float top, float bottom,
			float depthNear, float depthFar)
		{
			Matrix4 value = Identity;

			value[0, 0] = 2f / (right - left);
			value[1, 1] = 2f / (top - bottom);
			value[2, 2] = -2f / (depthFar - depthNear);
			value[3, 0] = -(right + left) / (right - left);
			value[3, 1] = -(top + bottom) / (top - bottom);
			value[3, 2] = -(depthFar + depthNear) / (depthFar - depthNear);

			return value;
		}

		// TODO: Fix Perspective, don't think it gets created properly..
		public static Matrix4 CreatePerspective(float width, float height, float fov, float depthNear, float depthFar)
			=> CreatePerspective(width / height, fov, depthNear, depthFar);

		public static Matrix4 CreatePerspective(float aspectRatio, float fov, float depthNear, float depthFar)
		{
			Matrix4 value = new Matrix4();
			float tanHalfFOV = (float)Math.Tan(MathUtility.ToRadians(fov / 2.0f));

			value[0, 0] = 1.0f / (tanHalfFOV * aspectRatio);
			value[1, 1] = 1.0f / tanHalfFOV;
			value[2, 2] = (depthFar + depthNear) / (depthFar - depthNear);
			value[2, 3] = 1.0f;
			value[3, 2] = -(2f * depthFar * depthNear) / (depthFar - depthNear);

			return value;
		}

		#region Conversions & Operators
		public static Vector4 operator *(Matrix4 m, Vector4 v) => new Vector4(
			m[0][0] * v[0] + m[1][0] * v[1] + m[2][0] * v[2] + m[3][0] * v[3],
			m[0][1] * v[0] + m[1][1] * v[1] + m[2][1] * v[2] + m[3][1] * v[3],
			m[0][2] * v[0] + m[1][2] * v[1] + m[2][2] * v[2] + m[3][2] * v[3],
			m[0][3] * v[0] + m[1][3] * v[1] + m[2][3] * v[2] + m[3][3] * v[3]
			);

		public static Vector4 operator *(Vector4 v, Matrix4 m) => new Vector4(
			 m[0][0] * v[0] + m[0][1] * v[1] + m[0][2] * v[2] + m[0][3] * v[3],
			 m[1][0] * v[0] + m[1][1] * v[1] + m[1][2] * v[2] + m[1][3] * v[3],
			 m[2][0] * v[0] + m[2][1] * v[1] + m[2][2] * v[2] + m[2][3] * v[3],
			 m[3][0] * v[0] + m[3][1] * v[1] + m[3][2] * v[2] + m[3][3] * v[3]
			 );

		public static Matrix4 operator *(Matrix4 a, Matrix4 b)
		{
			Matrix4 value = new Matrix4();
			for (int i = 0; i < 4; i++)
				for (int j = 0; j < 4; j++)
					for (int k = 0; k < 4; k++)
						value[j, i] += a[k, i] * b[j, k];
			return value;
		}

		public static implicit operator Matrix4(System.Numerics.Matrix4x4 m) =>
			new Matrix4(
				m.M11, m.M21, m.M31, m.M41,
				m.M12, m.M22, m.M32, m.M42,
				m.M13, m.M23, m.M33, m.M43,
				m.M14, m.M24, m.M34, m.M44
				);

		public static implicit operator System.Numerics.Matrix4x4(Matrix4 m) =>
			new System.Numerics.Matrix4x4(
					m[0, 0], m[1, 0], m[2, 0], m[3, 0],
					m[0, 1], m[1, 1], m[2, 1], m[3, 1],
					m[0, 2], m[1, 2], m[2, 2], m[3, 2],
					m[0, 3], m[1, 3], m[2, 3], m[3, 3]
					);

		public override string ToString()
		{
			string s = string.Empty;
			for (int i = 0; i < m_Components.Length; i++)
				s += m_Components[i] + (i < m_Components.Length - 1 ? ", " : "");
			return $"[{s}]";
		}

		public Vector4 this[int i]
		{
			get => i >= 4 || i < 0 ? Vector4.zero : new Vector4(m_Components[i], m_Components[i + 4], m_Components[i + 8], m_Components[i + 12]);
			set
			{
				if (i >= 4 || i < 0)
					return;
				m_Components[i] = value.x;
				m_Components[i + 3] = value.y;
				m_Components[i + 6] = value.z;
				m_Components[i + 9] = value.w;
			}
		}

		public float this[int i, int j]
		{
			get => (i >= 4 || j >= 4 || i < 0 || j < 0) ? 0 : m_Components[i + (j * 4)];
			set
			{
				if (i >= 4 || j >= 4 || i < 0 || j < 0)
					return;
				m_Components[i + (j * 4)] = value;
			}
		}
		#endregion
	}
}
