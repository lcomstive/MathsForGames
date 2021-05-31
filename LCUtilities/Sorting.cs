/*
 Useful links:
 - https://www.toptal.com/developers/sorting-algorithms
 */

using System;
using System.Collections;
using System.Collections.Generic;

namespace LCUtils
{
	public static class SortingUtility
	{
		public static int[] InsertionSort(int[] elements)
		{
			int[] temp = new int[elements.Length];
			Array.Copy(elements, temp, temp.Length);
			InsertionSortInline(temp);
			return temp;
		}

		public static void InsertionSortInline(int[] elements)
		{
			for(int i = 2; i < elements.Length; i++)
			{
				for(int k = i; k > 1 && elements[k] < elements[k - 1]; k--)
				{
					int temp = elements[k];
					elements[k] = elements[k - 1];
					elements[k - 1] = temp;
				}
			}
		}
	}
}
