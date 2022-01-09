using System.Collections.Generic;

namespace BPaNSResize
{
	public static class ListExtension
	{
		public static void AddIfNotContains<T>(this List<T> list, T item)
		{
			if (!list.Contains(item))
				list.Add(item);
		}
	}
}
