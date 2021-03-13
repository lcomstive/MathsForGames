using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LCUtils;
using LCPhysics2D.Shapes;

namespace LCPhysics2D
{
	public abstract class Physics2DObject
	{
		protected enum Physics2DShape { Unknown, Rectangle, Circle }
		protected abstract Physics2DShape PhysicsShape { get; }

		public uint ReferenceID;		 // An ID used outside of the library to keep track of object changes
		public bool EnableForces = true; // If disabled, velocity and collision won't affect object

		public float Mass = 1.0f;
		public Vector2 Position = Vector2.zero;
		public Vector2 Velocity = Vector2.zero;

		internal abstract bool CheckCollision(Circle circle);
		internal abstract bool CheckCollision(Rectangle rect);

		internal abstract void ResolveCollision(Circle circle);
		internal abstract void ResolveCollision(Rectangle rect);

		internal bool CheckCollision(Physics2DObject obj)
		{
			switch(obj.PhysicsShape)
			{
				default:
				case Physics2DShape.Unknown:
					return false;
				case Physics2DShape.Rectangle:
					return CheckCollision(((RectanglePhysics2DObject)obj).Object);
				case Physics2DShape.Circle:
					return CheckCollision(((CirclePhysics2DObject)obj).Object);
			}
		}

		internal void ResolveCollision(Physics2DObject obj)
		{
			switch (obj.PhysicsShape)
			{
				default:
				case Physics2DShape.Unknown:
					break;
				case Physics2DShape.Rectangle:
					CheckCollision(((RectanglePhysics2DObject)obj).Object);
					break;
				case Physics2DShape.Circle:
					CheckCollision(((CirclePhysics2DObject)obj).Object);
					break;
			}
		}
	}
}
