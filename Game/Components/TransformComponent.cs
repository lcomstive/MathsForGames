using LCUtils;
using LCECS;

namespace Game.Components
{
	public class TransformComponent
	{
		/// <summary>
		/// 2D Position with z-index, relative to parent.
		/// (x, y, z-index)
		/// </summary>
		public Vector3 Position = Vector3.zero;

		/// <summary>
		/// 3D Scale, relative to world (width, height).
		/// </summary>
		public Vector3 Scale = new Vector3(1, 1, 1);

		/// <summary>
		/// Euler angles of rotation, in degrees
		/// </summary>
		public Vector3 Rotation = Vector3.zero;

		/// <summary>
		/// The entity this component moves relative to
		/// </summary>
		public EntityID Parent = EntityID.InvalidID;
	}
}
