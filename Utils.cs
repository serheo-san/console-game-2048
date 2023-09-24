using System;

namespace Game2048 {
	public static class Utils {
		// fields
		private static Random random = new Random( (int) DateTime.Now.Ticks );

		// methods
		public static int RandomRange( int min, int max ) {
			return random.Next( min, max );
		}
	}
}