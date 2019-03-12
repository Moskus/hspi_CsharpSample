using System;

namespace HSPI_CsharpSample.HomeSeerClasses
{
	[Serializable()]
	public class HsTrigger : HsCollection
	{
		public HsTrigger()
		{}

		protected HsTrigger(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context)
		{ }
	}
}