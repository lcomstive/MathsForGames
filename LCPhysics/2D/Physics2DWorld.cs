using System;
using LCUtils;
using LCPhysics2D.Shapes;

namespace LCPhysics2D
{
	public class Physics2DWorld
	{
		private Vector2 m_Gravity = Vector2.zero;

		public Vector2 Gravity { get => m_Gravity; set => m_Gravity = value; }

		public void Update(ref Physics2DObject[] objects, float deltaTime)
		{
			for(int i = 0; i < objects.Length; i++)
			{
				for(int j = 0; j < objects.Length; j++)
				{
					if (i == j)
						continue;
					if(objects[i].CheckCollision(objects[j]))
						Console.WriteLine($"Collision between objects {i} & {j}");
				}

				if (!objects[i].EnableForces)
					continue;

				// Newton's second law
				// F = m * a, rearranged to a = F / m, rewritten as a = F * (1/m)
				Vector2 acceleration = (1.0f / objects[i].Mass * Gravity) * deltaTime;
				objects[i].Velocity += acceleration;
				objects[i].Position += objects[i].Velocity * deltaTime;
			}
		}
	}
}
