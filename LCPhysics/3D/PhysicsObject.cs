using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LCUtils;

namespace LCPhysics
{
	public abstract class PhysicsObject
	{
		public uint ReferenceID; // An ID used outside of the library to keep track of object changes

		public float Mass;
		public Vector3 Position;
		public Vector3 Velocity;
		public Vector3 AngularVelocity;

		// internal abstract bool CheckCollision(Cube cube);
		// internal abstract bool CheckCollision(Sphere sphere);
	}
}
