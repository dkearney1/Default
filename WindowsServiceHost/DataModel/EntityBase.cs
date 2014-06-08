using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DKK.DataModel
{
	public partial class EntityBase
	{
		public Guid GetNewId()
		{
			return SequentialGuidGenerator.NewSequentialGuid(SequentialGuidType.SequentialAtEnd);
		}

		public Guid GetNewId(Guid oldId, DateTime createDate)
		{
			return SequentialGuidGenerator.NewSequentialGuid(createDate, oldId, SequentialGuidType.SequentialAtEnd);
		}

		public string EntityTypeName { get { return this.GetType().Name; } }
	}
}
