using System;

namespace hspi_CsharpSample.HomeSeerClasses
{
	[Serializable()]
	public class HsTriggers : HsCollection
	{
		public HsTriggers()
		{}

		protected HsTriggers(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context)
		{ }
	}
}