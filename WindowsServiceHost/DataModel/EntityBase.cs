using System;

namespace DKK.DataModel
{
	public partial class EntityBase
	{
        public Guid GetNewId() => SequentialGuidGenerator.NewSequentialGuid(SequentialGuidType.SequentialAtEnd);

        public Guid GetNewId(Guid oldId, DateTime createDate) => SequentialGuidGenerator.NewSequentialGuid(createDate, oldId, SequentialGuidType.SequentialAtEnd);

        public string EntityTypeName => this.GetType().Name;
    }
}
