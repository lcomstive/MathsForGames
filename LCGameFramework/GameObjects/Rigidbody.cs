using LCUtils;
using Raylib_cs;
using LCGF.Physics;
using static Raylib_cs.Raylib;

namespace LCGF.GameObjects
{
	/// <summary>
	/// Phyics-capable GameObject
	/// </summary>
	public class Rigidbody : GameObject
	{
		public float Mass { get; set; } = 1f;
		public Vector2 Velocity { get; set; } = Vector2.zero;

		/// <summary>
		/// LCGF.Physics collider associated with this object
		/// </summary>
		public PolygonCollider Collider { get; private set; }

		/// <summary>
		/// Should gravity affect this object's position and velocity
		/// </summary>
		public bool UseGravity { get; set; } = true;

		/// <summary>
		/// Should this object's velocity and position be affected by collisions
		/// </summary>
		public bool IsKinematic { get; set; } = false;

		public Rigidbody(string name = "", GameObject parent = null) : this(Vector2.zero, 0f, name, parent) { }
		public Rigidbody(Vector2 position, string name = "", GameObject parent = null) : this(position, 0f, name, parent) { }
		public Rigidbody(Vector2 position, float rotation, string name = "", GameObject parent = null) : base(position, rotation, name, parent)
			=> PhysicsWorld.AddObject(this);

		~Rigidbody() => Destroy();

		// Callback to destroy collider and remove rigidbody from physics calculations
		protected override void OnDestroy()
		{
			SetCollider(null);
			PhysicsWorld.RemoveObject(this);
		}

		// A collision has occurred involving this rigidbody, call the appropriate callback and event
		internal void InformCollision(MTV mtv, Rigidbody other)
		{
			if (Collider.IsTrigger || other.Collider.IsTrigger)
			{
				OnTriggered(mtv, other);
				Trigger?.Invoke(mtv, other);
			}
			else
			{
				OnCollided(mtv, other);
				Collision?.Invoke(mtv, other);
			}
		}

		protected override void OnUpdate() => Collider?.Update();

		protected virtual void OnCollided(MTV mtv, Rigidbody other) { }
		protected virtual void OnTriggered(MTV mtv, Rigidbody other) { }

		public void SetCollider(PolygonCollider collider)
		{
			if(Collider != null)
				Collider.Parent = null;
			ColliderChanged?.Invoke(Collider, collider);
			Collider = collider;
			if(Collider != null)
				Collider.Parent = this;
		}

		protected override void OnDraw()
		{
#if DEBUG
			if (Collider == null)
				return;

			// Draw bounding box
			DrawRectangleLinesEx(
					new Rectangle(
						Collider.BoundingBox.Min.x,
						Collider.BoundingBox.Min.y,
						Collider.BoundingBox.Size.x,
						Collider.BoundingBox.Size.y
						),
					2,
					Colour.Green
					);

			// Draw collider
			if(Collider.GetType() == typeof(CircleCollider))
			{
				CircleCollider circle = (CircleCollider)Collider;
				DrawCircleLines((int)circle.Position.x, (int)circle.Position.y, circle.Radius, Colour.Green);
			}
			else
			{
				Vector2[] vertices = Collider.GetVertices();

				System.Numerics.Vector2[] points = new System.Numerics.Vector2[vertices.Length + 1];
				for (int i = 0; i < vertices.Length; i++)
					points[i] = vertices[i];
				points[^1] = points[0];

				DrawLineStrip(points, points.Length, Colour.Purple);
			}
#endif
		}

		#region Events
		public delegate void OnCollision(MTV mtv, Rigidbody other);
		public event OnCollision Collision;

		public delegate void OnTrigger(MTV mtv, Rigidbody other);
		public event OnTrigger Trigger;

		public delegate void OnColliderChanged(PolygonCollider oldCollider, PolygonCollider newCollider);
		public event OnColliderChanged ColliderChanged;
		#endregion
	}
}
