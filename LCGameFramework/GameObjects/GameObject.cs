using LCUtils;

namespace LCGF.GameObjects
{
	public class GameObject : Transform
	{
		public string Name { get; set; }

		#region Constructors
		public GameObject(string name = "", GameObject parent = null) : this(Vector2.zero, 0, name, parent) { }

		public GameObject(Vector2 position, string name = "", GameObject parent = null) : this(position, 0, name, parent) { }

		public GameObject(Vector2 position, float rotation, string name = "", GameObject parent = null) : base(position, rotation, parent)
			=> Name = name;

		/// <summary>
		/// Draws all children, then calls OnDraw()
		/// </summary>
		public void Draw()
		{
			foreach (GameObject child in Children)
				child.Draw();
			OnDraw();
		}

		protected virtual void OnDraw() { }
		#endregion
	}
}
