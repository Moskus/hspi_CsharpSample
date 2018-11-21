using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace hspi_CsharpSample.HomeSeerClasses
{
	///<summary>
	///A custom dictionary for storing serializable objects
	///</summary>
	///<remarks></remarks>
	[Serializable()]
	public class HsCollection : Dictionary<string, object>
	{
		public Collection<string> KeyIndex = new Collection<string>();

		public HsCollection() : base()
		{
		}

		protected HsCollection(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context)
		{
		}

		public List<string> GetAllKeys()
		{
			return base.Keys.ToList();
		}

		public void Add(object value, string key)
		{
			if (!base.ContainsKey(key))
			{
				base.Add(key, value);
				KeyIndex.Add(key);
			}
			else
			{
				base[key] = value;

			}
		}

		public new void Remove(string key)
		{
			if (base.ContainsKey(key))
				base.Remove(key);

			if (KeyIndex.Contains(key))
				KeyIndex.Remove(key);
		}

		public void Remove(int index)
		{
			base.Remove(KeyIndex[index]);
			KeyIndex.RemoveAt(index);
		}
		

		public new Object Keys(int index)
		{
			int i = 0;
			string foundKey = string.Empty;
			foreach (var key in base.Keys)
			{
				foundKey = key;
				if (i == index)
				{
					break;
				}
				else
				{
					i++;
				}
			}
			return foundKey;
		}

		public object GetItem(int index)
		{
			return base[KeyIndex[index]];
		}

		public void SetItem(int index, object value)
		{
			base[KeyIndex[index]] = value;
		}
		//Default Public Overloads Property Item(ByVal index As Integer) As Object
		//	Get
		//		Return MyBase.Item(KeyIndex(index))
		//	End Get
		//	Set(ByVal value As Object)
		//		MyBase.Item(KeyIndex(index)) = value
		//	End Set
		//End Property

		public object GetItem(string key)
		{
			return base[key];
		}

		public void SetItem(string key, object value)
		{
			base[key] = value;
		}
		//Default Public Overloads Property Item(ByVal Key As String) As Object
		//	Get
		//		On Error Resume Next
		//		Return MyBase.Item(Key)
		//	End Get
		//	Set(ByVal value As Object)
		//		If Not MyBase.ContainsKey(Key) Then
		//			Add(value, Key)
		//		Else
		//			MyBase.Item(Key) = value
		//		End If
		//	End Set
		//End Property
	}
}