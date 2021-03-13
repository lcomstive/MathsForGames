using LCUtils;

namespace Game.Components.Physics2D
{
	public class Rigidbody2DComponent
	{
		public float Mass = 1.0f;
		public Vector2 Velocity = Vector2.zero;

		public bool EnableForces = true; // If disabled, velocity and collision won't affect object
	}
}
