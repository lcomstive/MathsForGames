using Microsoft.VisualStudio.TestTools.UnitTesting;
using LCUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
	[TestClass]
	public class Sorting
	{
		[TestMethod]
		public void InsertionSortInlineTest()
		{
			int[] input = { 1, 5, 13, 6, 11, 12 };
			int[] expectedOutput = { 1, 5, 6, 11, 12, 13 };

			SortingUtility.InsertionSortInline(input);

			for (int i = 0; i < input.Length; i++)
				Assert.AreEqual(expectedOutput[i], input[i]);
		}
	}
}