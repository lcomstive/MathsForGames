using System;

namespace LCUtils
{
	public class Matrix4
	{
		private float[] m_Components;

		public Matrix4 identity => new Matrix4(
			0, 0, 0, 0,
			0, 1, 0, 0,
			0, 0, 1, 0,
			0, 0, 0, 1
			);

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

		public Vector4 this[uint i]
		{
			get => i >= 4 ? Vector4.zero : new Vector4(m_Components[i], m_Components[i + 4], m_Components[i + 8], m_Components[i + 12]);
			set
			{
				if (i >= 4)
					return;
				m_Components[i] = value.x;
				m_Components[i + 3] = value.y;
				m_Components[i + 6] = value.z;
				m_Components[i + 9] = value.w;
			}
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
				m[0][0], m[0][1], m[0][2], m[0][3],
				m[1][0], m[1][1], m[1][2], m[1][3],
				m[2][0], m[2][1], m[2][2], m[2][3],
				m[3][0], m[3][1], m[3][2], m[3][3]
				);

		public float this[uint i, uint j]
		{
			get => (i >= 4 || j >= 4) ? 0 : m_Components[i + (j * 4)];
			set
			{
				if (i >= 4 || j >= 4)
					return;
				m_Components[i + (j * 4)] = value;
			}
		}
	}
}
