using System;
using LCUtils;
using System.Collections.Generic;

namespace LCGF.GameObjects
{
	/// <summary>
	/// Base class representing an object in the game world
	/// </summary>
	public abstract class Transform
	{
		/// <summary>
		/// Rotation relative to parent, in degrees
		/// </summary>
		public float Rotation
		{
			get => m_Rotation;
			set
			{
				m_Dirty = true;
				m_Rotation = value;
			}
		}

		/// <summary>
		/// Position relative to parent
		/// </summary>
		public Vector2 Position
		{
			get => m_Position;
			set
			{
				m_Dirty = true;
				m_Position = value;
			}
		}

		/// <summary>
		/// Width & height of object
		/// </summary>
		public Vector2 Size { get; set; } = new Vector2(1, 1);

		/// <summary>
		/// Parent of this object in world hierarchy.
		/// This object's position & rotation is relative to this parent
		/// </summary>
		public GameObject Parent { get; private set; }

		/// <summary>
		/// All of the GameObjects who have set their parent to this object
		/// </summary>
		public GameObject[] Children => m_Children.ToArray();

		#region Cached Globals
		/// <summary>
		/// Overall rotation, equivalent to local plus parent rotation, in degrees
		/// </summary>
		public float GlobalRotation { get; private set; }

		/// <summary>
		/// Overall position, equivalent to parent plus local positions
		/// </summary>
		public Vector2 GlobalPosition { get; private set; }

		/// <summary>
		/// The world direction this object is facing
		/// </summary>
		public Vector2 GlobalForward { get; private set; }

		/// <summary>
		/// The world direction pointing to the right from the center of this object, 90° clockwise to GlobalForward
		/// </summary>
		public Vector2 GlobalRight { get; private set; }
		#endregion

		private Vector2 m_Position;
		private float m_Rotation;	 // Degrees
		private bool m_Dirty = true; // If position or rotation has been changed this frame

		private Matrix3 m_Local = Matrix3.Identity;
		private Matrix3 m_Global = Matrix3.Identity;

		private List<GameObject> m_Children = new List<GameObject>();

		public Transform(GameObject parent = null) : this(Vector2.zero, 0, parent) { }
		public Transform(Vector2 position) : this(position, 0) { }
		public Transform(Vector2 position, float rotation, GameObject parent = null)
		{
			Position = position;
			Rotation = rotation;
			SetParent(parent);

			UpdateMatrices();

			OnInit();
		}

		~Transform() => Destroy();

		/// <summary>
		/// Removes all references to this object, destroying all children in the process
		/// </summary>
		public void Destroy()
		{
			if (m_Local == null)
				return; // Already destroyed
			
			for(int i = m_Children.Count - 1; i >= 0; i--)
				m_Children[i]?.Destroy();

			Parent?.RemoveChild((GameObject)this);
			OnDestroy();
			Destroyed?.Invoke();
			m_Local = null;
		}

		public void SetParent(GameObject parent)
		{
			if (Parent == parent)
				return;
			m_Dirty = true;

			// Remove from existing parent
			if(Parent != null)
				Parent.RemoveChild((GameObject)this);

			// Set and inform new parent
			OnParentChange(Parent, parent);
			Parent = parent;
			parent?.AddChild((GameObject)this);
		}

		public void AddChild(GameObject child)
		{
			if (m_Children.Contains(child))
				return;
			m_Children.Add(child);
			OnChildChange(child, true);

			if (child.Parent != this)
				child.SetParent((GameObject)this);
		}

		public void RemoveChild(GameObject child)
		{
			if (!m_Children.Contains(child))
				return;
			OnChildChange(child, false);
			m_Children.Remove(child);
		}

		/// <summary>
		/// Child objects are drawn from the lowest index first.
		/// Shifting the index of a child changes when it gets drawn relative to other children
		/// </summary>
		public void IncreaseChildIndex(GameObject child)
		{
			if (!m_Children.Contains(child))
				return;
			int index = m_Children.IndexOf(child);
			if (index >= m_Children.Count - 1)
				return; // Already at end
			GameObject temp = m_Children[index + 1];
			m_Children[index + 1] = child;
			m_Children[index] = temp;
		}

		/// <summary>
		/// Child objects are drawn from the lowest index first.
		/// Shifting the index of a child changes when it gets drawn relative to other children
		/// </summary>
		public void DecreaseChildIndex(GameObject child)
		{
			if (!m_Children.Contains(child))
				return;
			int index = m_Children.IndexOf(child);
			if (index == 0)
				return; // Already at start
			GameObject temp = m_Children[index - 1];
			m_Children[index - 1] = child;
			m_Children[index] = temp;
		}

		/// <summary>
		/// Updates the matrices, all children matrices, and calls OnUpdate for gameobject logic
		/// </summary>
		public void Update()
		{
			bool updateChildren = m_Dirty;
			UpdateMatrices();

			// Reverse for loop so objects can be destroyed during Update()
			for(int i = m_Children.Count - 1; i >= 0; i--)
			{
				m_Children[i].m_Dirty = m_Children[i].m_Dirty || updateChildren;
				m_Children[i].Update();
			}

			OnUpdate();
		}

		/// <summary>
		/// Calculates local and global matrices, then caches results into appropriate variables.
		/// </summary>
		/// <param name="force">If true, overrides the check for if changes have been made to the position and rotation</param>
		private void UpdateMatrices(bool force = false)
		{
			if (!m_Dirty && !force)
				return;
			m_Dirty = false;

			m_Local =
				Matrix3.FromRotationZ(MathUtility.ToRadians(m_Rotation)) *
				Matrix3.FromTranslation(m_Position);

			m_Global = new Matrix3(m_Local);
			if (Parent != null)
			{
				Parent.UpdateMatrices(force);
				m_Global *= Parent.m_Global;
			}

			// Cache global variables
			GlobalPosition =  m_Global[2].xy;
			GlobalForward  =  m_Global[1].xy;
			GlobalRight	   = -m_Global[0].xy;
			GlobalRotation = (float)MathUtility.ToDegrees(Math.Atan2(m_Global[0, 1], m_Global[0, 0]));
		}

		// Callbacks for custom gameobject logic
		protected virtual void OnInit() { }
		protected virtual void OnUpdate() { }
		protected virtual void OnDestroy() { }

		protected virtual void OnParentChange(GameObject oldParent, GameObject newParent) { }
		protected virtual void OnChildChange(GameObject child, bool added) { }

		// Events
		public delegate void OnDestroyed();
		public event OnDestroyed Destroyed;
	}
}
