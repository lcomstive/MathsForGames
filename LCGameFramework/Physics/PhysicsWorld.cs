using System;
using LCUtils;
using LCGF.GameObjects;
using System.Collections.Generic;

namespace LCGF.Physics
{
	public struct MTV // Minimum Translation Vector
	{
		public float Value;
		public Vector2 Axis;
	}

	public static class PhysicsWorld
	{
		public static Vector2 Gravity { get; set; } = new Vector2(0, -9.81f);

		private static List<Rigidbody> m_Rigidbodies = null;

		public static void Init()
		{
			if (m_Rigidbodies != null)
			{
				Console.WriteLine("Called PhysicsWorls.Init() but it is already initialised?");
				return;
			}
			m_Rigidbodies = new List<Rigidbody>();
		}

		public static void AddObject(Rigidbody rb)
		{
			if (m_Rigidbodies == null) Init();
			if (!m_Rigidbodies.Contains(rb))
				m_Rigidbodies.Add(rb);
		}

		public static void RemoveObject(Rigidbody rb)
		{
			if (m_Rigidbodies == null) Init();
			if (m_Rigidbodies.Contains(rb))
				m_Rigidbodies.Remove(rb);
		}

		internal static void Update()
		{
			// Make copy of rigidbodies incase OnUpdate, OnTriggered or OnCollided alters m_Rigidbodies
			List<Rigidbody> rbCopy = new List<Rigidbody>(m_Rigidbodies);

			// Update velocity and position
			foreach (Rigidbody rb in rbCopy)
			{
				if (rb.IsKinematic)
					continue;
				if (rb.UseGravity)
				{
					Vector2 acceleration = Gravity * rb.Mass * Time.DeltaTime;
					rb.Velocity += acceleration * Time.DeltaTime;
				}

				rb.Position += rb.Velocity * Time.DeltaTime;
				rb.Update();

				if (rb.Collider != null)
					rb.Collider.Center = rb.GlobalPosition;
			}

			var potentialCollisions = BroadphaseCollision();
			foreach ((int a, int b) in potentialCollisions)
				NarrowPhaseCollision(rbCopy[a], rbCopy[b]);
		}

		private static void NarrowPhaseCollision(Rigidbody a, Rigidbody b)
		{
			if (a.Collider == null || b.Collider == null)
				return;

			MTV mtv = a.Collider.GetOverlap(b.Collider);
			if (mtv.Value <= 0 || mtv.Axis == Vector2.zero)
				return; // No collision occured

			if (!a.Collider.IsTrigger && !b.Collider.IsTrigger)
				ResolveCollision(mtv, a, b);

			a.InformCollision(mtv, b);
			b.InformCollision(mtv, a);
		}

		private static void ResolveCollision(MTV mtv, Rigidbody a, Rigidbody b)
		{
			float sumMass = a.Mass + b.Mass;
			Vector2 aV = a.Velocity;
			Vector2 bV = b.Velocity;
			if (!a.IsKinematic)
			{
				a.Position += mtv.Axis * mtv.Value;

				if (b.IsKinematic)
					a.Velocity.Reflect(mtv.Axis);
				else
					a.Velocity = (aV * (a.Mass - b.Mass) + (2f * b.Mass * bV)) / sumMass;
				a.Position += a.Velocity * Time.DeltaTime;

				a.Update();
				a.Collider.Center = a.GlobalPosition;
			}

			if (!b.IsKinematic)
			{
				b.Position -= mtv.Axis * mtv.Value;
				if (a.IsKinematic)
					b.Velocity.Reflect(mtv.Axis);
				else
					b.Velocity = (bV * (b.Mass - a.Mass) + (2f * a.Mass * aV)) / sumMass;
				b.Position += b.Velocity * Time.DeltaTime;

				b.Update();
				b.Collider.Center = b.GlobalPosition;
			}
		}

		#region Broadphase Detection
		private static List<(int, int)> BroadphaseCollision() => BroadphaseBruteForce(); // TODO: Add more broadphase detection methods

		private static List<(int, int)> BroadphaseBruteForce()
		{
			List<(int, int)> pairs = new List<(int, int)>();
			for (int i = 0; i < m_Rigidbodies.Count; i++)
			{
				if (m_Rigidbodies[i].Collider == null)
					continue;
				for (int j = 0; j < m_Rigidbodies.Count; j++)
				{
					if (i != j && m_Rigidbodies[j].Collider != null)
						pairs.Add((i, j));
				}
			}
			return pairs;
		}
		#endregion
	}
}
