using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LCUtils;

namespace LCPhysics2D.Shapes
{
	public class Circle
	{
		public float Radius { get => m_Radius; set { m_Radius = value; CalculateAABB(); } }
		public Vector2 Position { get => m_Position; set { m_Position = value; CalculateAABB(); } }
		public Rectangle AABB => m_AABB;

		private float m_Radius;
		private Vector2 m_Position;
		private Rectangle m_AABB;

		public Circle(Vector2 position, float radius)
		{
			m_Radius = radius;
			m_Position = position;
			CalculateAABB();
		}
		
		public Circle(Vector3 position, float radius)
		{
			m_Radius = radius;
			m_Position = new Vector2(position.x, position.y);
			CalculateAABB();
		}

		private void CalculateAABB() =>
			m_AABB = new Rectangle(
				Position.x - Radius, // Min X
				Position.y + Radius, // Min Y
				Radius * 2,			 // Width
				Radius * 2			 // Height
				);

		// TODO: Test circle<->circle collision
		public bool Intersects(Circle other)
		{
			float radii = Radius + other.Radius;
			radii *= radii;
			Vector2 position = Position + other.Position;
			return radii < (Position.x * Position.x + Position.y * Position.y);
		}

		// TODO: Fix rectangle<->circle collision detection. Doesn't work correctly?
		public bool Intersects(Rectangle other)
		{
			if (!AABB.Intersects(other)) return false;
			if (other.IsPointInside(Position)) return true;
			if (other.IsPointInside(Position + new Vector2(Radius, 0))) { Console.WriteLine("right");	return true; } // Right edge of circle
			if (other.IsPointInside(Position - new Vector2(Radius, 0))) { Console.WriteLine("Left");	return true; } // Left edge of circle
			if (other.IsPointInside(Position + new Vector2(0, Radius))) { Console.WriteLine("Top");		return true; } // Top edge of circle
			if (other.IsPointInside(Position - new Vector2(0, Radius))) { Console.WriteLine("Bottom");  return true; } // Bottom edge of circle

			// 0.785398 radians = 45°
			float cos = (float)Math.Cos(0.785398) * Radius;
			float sin = (float)Math.Sin(0.785398) * Radius;
			if (other.IsPointInside(Position + new Vector2( cos,  sin))) { Console.WriteLine("Top-Right");	return true; } // Top-right edge of circle
			if (other.IsPointInside(Position + new Vector2( cos, -sin))) { Console.WriteLine("Bottom-Right"); return true; } // Bottom-right edge of circle
			if (other.IsPointInside(Position + new Vector2(-cos, -sin))) { Console.WriteLine("Bottom-Left");	return true; } // Bottom-left edge of circle
			if (other.IsPointInside(Position + new Vector2(-cos,  sin))) { Console.WriteLine("Top-Left");		return true; } // Top-left edge of circle

			return false;
		}
	}

	public class CirclePhysics2DObject : Physics2DObject
	{
		internal Circle Object;
		protected override Physics2DShape PhysicsShape => Physics2DShape.Circle;

		public CirclePhysics2DObject(Circle c) => Object = c;
		public CirclePhysics2DObject(Vector2 position, float radius) => Object = new Circle(position, radius);
		public CirclePhysics2DObject(Vector3 position, float radius) => Object = new Circle(position, radius);

		internal override bool CheckCollision(Rectangle rect) => Object.Intersects(rect);
		internal override bool CheckCollision(Circle circle) => Object.Intersects(circle);

		internal override void ResolveCollision(Circle circle)
		{

		}

		internal override void ResolveCollision(Rectangle rect)
		{

		}
	}
}
