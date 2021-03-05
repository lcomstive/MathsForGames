using System;

namespace LCUtils
{
	public class Matrix3
	{
		private float[] m_Components;

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

		public Vector3 this[uint i]
		{
			get => i >= 3 ? Vector3.zero : new Vector3(m_Components[i], m_Components[i + 3], m_Components[i + 6]);
			set
			{
				if (i >= 3)
					return;
				m_Components[i]		= value.x;
				m_Components[i + 3] = value.y;
				m_Components[i + 6] = value.z;
			}
		}

		public float this[uint i, uint j]
		{
			get => (i >= 3 || j >= 3) ? 0 : m_Components[i + (j * 3)];
			set
			{
				if (i >= 3 || j >= 3)
					return;
				m_Components[i + (j * 3)] = value;
			}
		}
	}
}
