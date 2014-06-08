using System;

namespace DKK.DataModel
{
	public enum SequentialGuidType
	{
		SequentialAsString,
		SequentialAsBinary,
		SequentialAtEnd
	}

	/// <summary>
	/// Based on http://www.codeproject.com/Articles/388157/GUIDs-as-fast-primary-keys-under-multiple-database
	/// </summary>
	public static class SequentialGuidGenerator
	{
		public static Guid NewSequentialGuid(SequentialGuidType guidType)
		{
			return NewSequentialGuid(DateTime.UtcNow, Guid.NewGuid(), guidType);
		}

		public static Guid NewSequentialGuid(DateTime created, Guid sourceGuid, SequentialGuidType resultingGuidType)
		{
			byte[] randomBytes = sourceGuid.ToByteArray();

			long timestamp = created.Ticks / 10000L;
			byte[] timestampBytes = BitConverter.GetBytes(timestamp);

			if (BitConverter.IsLittleEndian)
				Array.Reverse(timestampBytes);

			byte[] guidBytes = new byte[16];

			switch (resultingGuidType)
			{
				case (SequentialGuidType.SequentialAsString):
				case (SequentialGuidType.SequentialAsBinary):
					{
						Buffer.BlockCopy(timestampBytes, 2, guidBytes, 0, 6);
						Buffer.BlockCopy(randomBytes, 0, guidBytes, 6, 10);

						// If formatting as a string, we have to reverse the order
						// of the Data1 and Data2 blocks on little-endian systems.
						if (resultingGuidType == SequentialGuidType.SequentialAsString &&
							BitConverter.IsLittleEndian)
						{
							Array.Reverse(guidBytes, 0, 4);
							Array.Reverse(guidBytes, 4, 2);
						}
						break;
					}

				case (SequentialGuidType.SequentialAtEnd):
					{
						Buffer.BlockCopy(randomBytes, 0, guidBytes, 0, 10);
						Buffer.BlockCopy(timestampBytes, 2, guidBytes, 10, 6);
						break;
					}
			}

			return new Guid(guidBytes);
		}
	}
}