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
		/// 2D Size, relative to world (width, height).
		/// </summary>
		public Vector2 Size = new Vector2(1, 1);

		/// <summary>
		/// Angle of rotation, in degrees, relative to world up (or (0, 1) as a vector)
		/// </summary>
		public float Rotation = 0;

		/// <summary>
		/// The entity this component moves relative to
		/// </summary>
		public EntityID Parent = EntityID.InvalidID;
	}
}
