using System;
using System.Collections.Generic;
using Net.Share;

namespace Binding
{
	public struct NetColorBind : ISerialize<Net.Color>, ISerialize
	{
		public void Write(Net.Color value, Segment stream)
		{
			int pos = stream.Position;
			stream.Position += 1;
			byte[] bits = new byte[1];
			if(value.r != 0)
			{
				NetConvertBase.SetBit(ref bits[0], 1, true);
				stream.WriteValue(value.r);
			}
			if(value.g != 0)
			{
				NetConvertBase.SetBit(ref bits[0], 2, true);
				stream.WriteValue(value.g);
			}
			if(value.b != 0)
			{
				NetConvertBase.SetBit(ref bits[0], 3, true);
				stream.WriteValue(value.b);
			}
			if(value.a != 0)
			{
				NetConvertBase.SetBit(ref bits[0], 4, true);
				stream.WriteValue(value.a);
			}
			if (!string.IsNullOrEmpty(value.hex))
			{
				NetConvertBase.SetBit(ref bits[0], 5, true);
				stream.WriteValue(value.hex);
			}
			int pos1 = stream.Position;
			stream.Position = pos;
			stream.Write(bits, 0, 1);
			stream.Position = pos1;
		}

		public Net.Color Read(Segment stream)
		{
			byte[] bits = stream.Read(1);
			var value = new Net.Color();
			if(NetConvertBase.GetBit(bits[0], 1))
				value.r = stream.ReadValue<Single>();
			if(NetConvertBase.GetBit(bits[0], 2))
				value.g = stream.ReadValue<Single>();
			if(NetConvertBase.GetBit(bits[0], 3))
				value.b = stream.ReadValue<Single>();
			if(NetConvertBase.GetBit(bits[0], 4))
				value.a = stream.ReadValue<Single>();
			if(NetConvertBase.GetBit(bits[0], 5))
				value.hex = stream.ReadValue<String>();
			return value;
		}

		public void WriteValue(object value, Segment stream)
		{
			Write((Net.Color)value, stream);
		}

		public object ReadValue(Segment stream)
		{
			return Read(stream);
		}
	}
}

namespace Binding
{
	public struct NetColorArrayBind : ISerialize<Net.Color[]>, ISerialize
	{
		public void Write(Net.Color[] value, Segment stream)
		{
			int count = value.Length;
			stream.WriteValue(count);
			if (count == 0) return;
			NetColorBind bind = new NetColorBind();
			foreach (var value1 in value)
				bind.Write(value1, stream);
		}

		public Net.Color[] Read(Segment stream)
		{
			var count = stream.ReadValue<int>();
			var value = new Net.Color[count];
			if (count == 0) return value;
			NetColorBind bind = new NetColorBind();
			for (int i = 0; i < count; i++)
				value[i] = bind.Read(stream);
			return value;
		}

		public void WriteValue(object value, Segment stream)
		{
			Write((Net.Color[])value, stream);
		}

		public object ReadValue(Segment stream)
		{
			return Read(stream);
		}
	}
}

namespace Binding
{
	public struct NetColorGenericBind : ISerialize<List<Net.Color>>, ISerialize
	{
		public void Write(List<Net.Color> value, Segment stream)
		{
			int count = value.Count;
			stream.WriteValue(count);
			if (count == 0) return;
			NetColorBind bind = new NetColorBind();
			foreach (var value1 in value)
				bind.Write(value1, stream);
		}

		public List<Net.Color> Read(Segment stream)
		{
			var count = stream.ReadValue<int>();
			var value = new List<Net.Color>();
			if (count == 0) return value;
			NetColorBind bind = new NetColorBind();
			for (int i = 0; i < count; i++)
				value.Add(bind.Read(stream));
			return value;
		}

		public void WriteValue(object value, Segment stream)
		{
			Write((List<Net.Color>)value, stream);
		}

		public object ReadValue(Segment stream)
		{
			return Read(stream);
		}
	}
}
