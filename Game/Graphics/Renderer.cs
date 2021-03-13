using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LCUtils;
using LCECS;
using Game.Components;
using Raylib_cs;

namespace Game.Graphics
{
	public class Renderer
	{
		public Renderer()
		{
			
		}

		public void Draw(Camera camera, EntityWorld world)
		{
			camera.Orthographic = true;
			camera.CalculateMatrices(Raylib.IsWindowResized()); // Re-calculate view and projection matrices

			Raylib.SetMatrixProjection(camera.ProjectionMatrix);
			Raylib.SetMatrixModelview(camera.ViewMatrix);

			// 2D Shapes
			DrawRects(camera, ref world);
			DrawCircles(camera, ref world);
		}

		#region 2D Shapes
		private void DrawRects(Camera camera, ref EntityWorld world)
		{
			(EntityID[] entities, TransformComponent[] transforms, ColouredRectComponent[] rects) = world.GetEntitiesWithComponents<TransformComponent, ColouredRectComponent>();

			if (entities == null)
				return; // No entities with components found

			TransformComponent transform;
			for (int i = 0; i < entities.Length; i++)
			{
				transform = transforms[i];

				Vector3 position = transform.Position - (transform.Scale / 2f);
				if (transform.Parent != EntityID.InvalidID)
					position += world.GetComponent<TransformComponent>(transform.Parent)?.Position ?? Vector3.zero;

				Raylib.DrawRectanglePro(new Rectangle()
				{
					x = (int)transform.Position.x,
					y = (int)transform.Position.y,
					width = (int)transform.Scale.x,
					height = (int)transform.Scale.y
				}, transform.Scale.xy / 2f, transform.Rotation.z, Color.RED);
			}
		}

		private void DrawCircles(Camera camera, ref EntityWorld world)
		{
			(EntityID[] entities, TransformComponent[] transforms, ColouredCircleComponent[] circles) = world.GetEntitiesWithComponents<TransformComponent, ColouredCircleComponent>();

			if (entities == null)
				return; // No entities with components found

			TransformComponent transform;
			for (int i = 0; i < entities.Length; i++)
			{
				transform = transforms[i];

				Vector3 position = transform.Position;
				if (transform.Parent != EntityID.InvalidID)
					position += world.GetComponent<TransformComponent>(transform.Parent)?.Position ?? Vector3.zero;

				Raylib.DrawCircle((int)position.x, (int)position.y, transform.Scale.x, Color.YELLOW);
			}
		}
		#endregion
	}
}
