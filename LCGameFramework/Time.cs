using System.Diagnostics;

namespace LCGF
{
	/// <summary>
	/// Keeps track of game-related time
	/// </summary>
	public static class Time
	{
		private static int s_FrameCount = 0; // Frames that have passed during this second
		private static float
			s_FrameCountTime = 0,
			s_FPS = 0,
			s_LastTime = 0,
			s_GameTime = 0;
		private static Stopwatch s_Stopwatch;

		/// <summary>
		/// Amount of frames rendered last second
		/// </summary>
		public static float FPS => s_FPS;

		/// <summary>
		/// Time, in milliseconds, since application launch
		/// </summary>
		public static float GameTime => s_GameTime;

		/// <summary>
		/// Time, in milliseconds, of the current frame
		/// </summary>
		public static float DeltaTime { get; private set; }

		internal static void Init()
		{
			if (s_Stopwatch != null)
				return;
			s_Stopwatch = new Stopwatch();
			s_Stopwatch.Start();
		}

		internal static void Update()
		{
			long currentTime = s_Stopwatch.ElapsedMilliseconds;
			DeltaTime = (currentTime - s_LastTime) / 1000.0f;

			s_LastTime = currentTime;
			s_FrameCountTime += DeltaTime;
			s_GameTime += DeltaTime;

			if(s_FrameCountTime >= 1.0f) // 1 second has passed
			{
				s_FPS = s_FrameCount;
				s_FrameCount = 0;
				s_FrameCountTime = 0;
			}
			s_FrameCount++;
		}
	}
}
