using System;
using System.Collections.Generic;
using System.Text;

namespace LCECS
{
	public abstract class System
	{
		protected internal World World;

		internal void PreInit(World parent)
		{
			World = parent;
			Init();
		}

		protected internal virtual void Init() { }
		protected internal virtual void Update(float deltaTime) { }
		protected internal virtual void Destroy() { }
	}
}
