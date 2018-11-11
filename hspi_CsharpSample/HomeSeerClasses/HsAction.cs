using System;

namespace hspi_CsharpSample.HomeSeerClasses
{
	[Serializable()]
	public class HsActions : HsCollection
	{
		public HsActions()
		{}

		protected HsActions(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context)
		{ }
	}
}