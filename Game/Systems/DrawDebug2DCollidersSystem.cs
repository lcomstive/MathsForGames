using LCECS;
using LCUtils;
using Raylib_cs;
using Game.Components;
using Game.Components.Physics2D;

namespace Game.Systems
{
	public class DrawDebug2DCollidersSystem : LCECS.System
	{
		public bool Draw = true;

		protected override void Update(float deltaTime)
		{
			if (!Draw)
				return;

			DrawBoxColliders();
			DrawCircleColliders();
		}

		private void DrawBoxColliders()
		{
			(EntityID[] ids, TransformComponent[] transforms, Box2DColliderComponent[] colliders) = World.GetEntitiesWithComponents<TransformComponent, Box2DColliderComponent>();
			if (ids == null)
				return;

			for (int i = 0; i < ids.Length; i++)
			{
				Box2DColliderComponent col = colliders[i];
				TransformComponent transform = transforms[i];
				Raylib.DrawRectangleLines(
					(int)(transform.Position.x - col.Size.x / 2f),
					(int)(transform.Position.y - col.Size.y / 2f),
					(int)col.Size.x, (int)col.Size.y, Color.GREEN);
			}
		}

		private void DrawCircleColliders()
		{
			(EntityID[] ids, TransformComponent[] transforms, Circle2DColliderComponent[] colliders) = World.GetEntitiesWithComponents<TransformComponent, Circle2DColliderComponent>();
			if (ids == null)
				return;

			for (int i = 0; i < ids.Length; i++)
			{
				Circle2DColliderComponent col = colliders[i];
				TransformComponent transform = transforms[i];
				Vector2 position = transform.Position.xy;
				Raylib.DrawCircleLines((int)position.x, (int)position.y, col.Radius, Color.GREEN);
			}
		}
	}
}
