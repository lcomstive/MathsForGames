using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LCUtils;
using LCECS;
using Game.Components;
using Game.Components.Physics2D;
using Raylib_cs;

namespace Game.Graphics
{
	public class Renderer
	{
		public bool DrawDebugAABBs { get; set; } = false;

		public void Draw(Camera camera, EntityWorld world)
		{
			camera.CalculateMatrices(Raylib.IsWindowResized()); // Re-calculate view and projection matrices

			Raylib.SetMatrixProjection(camera.ProjectionMatrix);
			Raylib.SetMatrixModelview(camera.ViewMatrix);

			// 2D Shapes
			DrawRects(ref world);
			DrawCircles(ref world);
		}

		private Vector3 GetEntityPosition(TransformComponent transform, ref EntityWorld world)
		{
			Vector3 position = transform.Position;
			EntityID parentID = transform.Parent;
			while(parentID != EntityID.InvalidID)
			{
				TransformComponent parentTransform = world.GetComponent<TransformComponent>(parentID);
				if (parentTransform == null)
					break;
				position += parentTransform.Position; // TODO: Check for scale as well?
													  // TODO: Rotation
				parentID = parentTransform.Parent;
			}
			return position;
		}

		#region 2D Shapes
		private void DrawRects(ref EntityWorld world)
		{
			(EntityID[] entities, TransformComponent[] transforms, ColouredRectComponent[] rects) = world.GetEntitiesWithComponents<TransformComponent, ColouredRectComponent>();

			if (entities == null)
				return; // No entities with components found

			TransformComponent transform;
			Box2DColliderComponent collider;
			for (int i = 0; i < entities.Length; i++)
			{
				transform = transforms[i];

				Vector3 position = GetEntityPosition(transform, ref world);

				Raylib.DrawRectanglePro(new Rectangle()
				{
					x = (int)position.x,
					y = (int)position.y,
					width = (int)transform.Scale.x,
					height = (int)transform.Scale.y
				}, transform.Scale.xy / 2f, transform.Rotation.z, rects[i].Colour);

				if(DrawDebugAABBs && (collider = world.GetComponent<Box2DColliderComponent>(entities[i])) != null)
				{
					Vector2 AABBPos = position.xy + collider.Offset;
					Raylib.DrawRectangleLines(
						(int)(AABBPos.x - collider.Size.x / 2f),
						(int)(AABBPos.y - collider.Size.y / 2f),
						(int)collider.Size.x,
						(int)collider.Size.y,
						Color.GREEN
						);
	
					Raylib.DrawText(position.xy.ToString(), (int)position.x, (int)position.y, 20, Color.WHITE);
				}
			}
		}

		private void DrawCircles(ref EntityWorld world)
		{
			(EntityID[] entities, TransformComponent[] transforms, ColouredCircleComponent[] circles) = world.GetEntitiesWithComponents<TransformComponent, ColouredCircleComponent>();

			if (entities == null)
				return; // No entities with components found

			TransformComponent transform;
			Circle2DColliderComponent collider;
			for (int i = 0; i < entities.Length; i++)
			{
				transform = transforms[i];

				Vector3 position = GetEntityPosition(transform, ref world);

				Raylib.DrawCircle((int)position.x, (int)position.y, transform.Scale.x, circles[i].Colour);

				if (DrawDebugAABBs && (collider = world.GetComponent<Circle2DColliderComponent>(entities[i])) != null)
				{
					Raylib.DrawCircleLines(
						(int)(position.x + collider.Offset.x),
						(int)(position.y + collider.Offset.y),
						(int)collider.Radius,
						Color.GREEN
						);
					Raylib.DrawRectangleLines(
						(int)(position.x + collider.Offset.x - collider.Radius),
						(int)(position.y + collider.Offset.y - collider.Radius),
						(int)(collider.Radius * 2f),
						(int)(collider.Radius * 2f),
						Color.GREEN
						);
	
					Raylib.DrawText(position.xy.ToString(), (int)position.x, (int)position.y, 20, Color.WHITE);
				}
			}
		}
		#endregion
	}
}
