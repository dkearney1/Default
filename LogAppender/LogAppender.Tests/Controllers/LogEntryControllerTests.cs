using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogAppender.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace LogAppender.Controllers.Tests
{
	[TestClass()]
	public class LogEntryControllerTests
	{
		[TestMethod()]
		public void PostTest()
		{
			var controller = new LogEntryController();
			
			controller.Post(@"This is on the first line
And this is on the second line");

		}
	}
}
