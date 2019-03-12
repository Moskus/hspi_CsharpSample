using System;

namespace HSPI_CsharpSample.HomeSeerClasses
{
	[Serializable()]
	public class HsAction : HsCollection
	{
		public HsAction()
		{}

		protected HsAction(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context)
		{ }
	}
}