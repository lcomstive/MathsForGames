using LCECS;
using LCUtils;
using System.Linq;
using LCPhysics2D;
using Game.Components;
using LCPhysics2D.Shapes;
using System.Collections.Generic;
using Game.Components.Physics2D;

namespace Game.Systems
{
	public class Physics2DSystem : LCECS.System
	{
		public Vector2 Gravity
		{
			get => m_Physics?.Gravity ?? Vector2.zero;
			set { if (m_Physics != null && value != null) m_Physics.Gravity = value; }
		}

		private Physics2DWorld m_Physics;

		protected override void Init() => m_Physics = new Physics2DWorld();

		protected override void Update(float deltaTime)
		{
			Dictionary<EntityID, Physics2DObject> mappedEntities = new Dictionary<EntityID, Physics2DObject>();

			// Get entities with physics properties
			MapEntitiesWithBox2DCollider(ref mappedEntities);
			MapEntitiesWithCircle2DCollider(ref mappedEntities);

			if (mappedEntities.Count == 0)
				return;

			// Run physics calculations
			Physics2DObject[] physicsObjects = mappedEntities.Values.ToArray();
			m_Physics.Update(ref physicsObjects, deltaTime);

			for (int i = 0; i < physicsObjects.Length; i++)
				mappedEntities[physicsObjects[i].ReferenceID] = physicsObjects[i];

			// Update rigidbodies
			(EntityID[] ids, TransformComponent[] transforms, Rigidbody2DComponent[] rigidbodies) = World.GetEntitiesWithComponents<TransformComponent, Rigidbody2DComponent>();

			for (int i = 0; i < ids.Length; i++)
			{
				if (!mappedEntities.ContainsKey(ids[i]))
					continue;
				transforms[i].Position = new Vector3(mappedEntities[ids[i]].Position);
				rigidbodies[i].Velocity = mappedEntities[ids[i]].Velocity;
			}
		}

		private void MapEntitiesWithBox2DCollider(ref Dictionary<EntityID, Physics2DObject> entities)
		{
			(EntityID[] ids, Rigidbody2DComponent[] rigidbodies, Box2DColliderComponent[] colliders) = World.GetEntitiesWithComponents<Rigidbody2DComponent, Box2DColliderComponent>();
			if (ids == null)
				return; // No entities found with Box2DCollider

			TransformComponent transform;
			for (int i = 0; i < ids.Length; i++)
			{
				// Get transform component
				transform = World.GetComponent<TransformComponent>(ids[i]);
				if (transform == null)
					continue;

				// Create rectangle bounds
				Rectangle rect = Rectangle.FromCenterPoint(transform.Position, colliders[i].Size + colliders[i].Offset);

				// Create physics object
				RectanglePhysics2DObject physicsObject = new RectanglePhysics2DObject(rect);
				physicsObject.ReferenceID = ids[i];
				physicsObject.EnableForces = rigidbodies[i].EnableForces;
				physicsObject.Position = transform.Position.xy;

				physicsObject.Mass = rigidbodies[i].Mass;
				physicsObject.Velocity = rigidbodies[i].Velocity;

				// Add to the entity list
				entities.Add(ids[i], physicsObject);
			}
		}

		private void MapEntitiesWithCircle2DCollider(ref Dictionary<EntityID, Physics2DObject> entities)
		{
			(EntityID[] ids, Rigidbody2DComponent[] rigidbodies, Circle2DColliderComponent[] colliders)	= World.GetEntitiesWithComponents<Rigidbody2DComponent, Circle2DColliderComponent>();
			if (ids == null)
				return; // No entities found with Circle2DCollider

			TransformComponent transform;
			for (int i = 0; i < ids.Length; i++)
			{
				// Get transform component
				transform = World.GetComponent<TransformComponent>(ids[i]);
				if (transform == null)
					continue;
				// Create rectangle bounds
				Circle circle = new Circle(transform.Position + colliders[i].Offset, colliders[i].Radius);

				// Create physics object
				CirclePhysics2DObject physicsObject = new CirclePhysics2DObject(circle);
				physicsObject.ReferenceID = ids[i];
				physicsObject.EnableForces = rigidbodies[i].EnableForces;

				physicsObject.Position = transform.Position.xy;
				physicsObject.Mass = rigidbodies[i].Mass;
				physicsObject.Velocity = rigidbodies[i].Velocity;

				// Add to the entity list
				entities.Add(ids[i], physicsObject);
			}
		}
	}
}
