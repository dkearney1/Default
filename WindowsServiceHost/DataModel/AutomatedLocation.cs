using System;
using System.Linq;
using IbEm = IdeaBlade.EntityModel;
using IdeaBlade.Core;

namespace DKK.DataModel
{
	// The IdeaBlade DevForce EDM Designer Extension generates this class once 
	// and will not overwrite any changes you make.  You can place your custom 
	// application-specific business logic in this file. Generated: 4/16/2014 6:06:00 PM
	public partial class AutomatedLocation : EntityBase
	{
		public AutomatedLocation()
		{
			this.Id = this.GetNewId();
		}

		public AutomatedLocation(Guid oldId, DateTime createDate)
		{
			this.Id = this.GetNewId(oldId, createDate);
		}
	}
}
