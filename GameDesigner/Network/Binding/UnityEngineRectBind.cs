using System;
using System.Collections.Generic;
using Net.Share;

namespace Binding
{
	public struct UnityEngineRectBind : ISerialize<UnityEngine.Rect>, ISerialize
	{
		public void Write(UnityEngine.Rect value, Segment stream)
		{
			int pos = stream.Position;
			stream.Position += 2;
			byte[] bits = new byte[2];
			if(value.x != 0)
			{
				NetConvertBase.SetBit(ref bits[0], 1, true);
				stream.WriteValue(value.x);
			}
			if(value.y != 0)
			{
				NetConvertBase.SetBit(ref bits[0], 2, true);
				stream.WriteValue(value.y);
			}
			if(value.position != default)
			{
				NetConvertBase.SetBit(ref bits[0], 3, true);
				UnityEngineVector2Bind bind = new UnityEngineVector2Bind();
				bind.Write(value.position, stream);
			}
			if(value.center != default)
			{
				NetConvertBase.SetBit(ref bits[0], 4, true);
				UnityEngineVector2Bind bind = new UnityEngineVector2Bind();
				bind.Write(value.center, stream);
			}
			if(value.min != default)
			{
				NetConvertBase.SetBit(ref bits[0], 5, true);
				UnityEngineVector2Bind bind = new UnityEngineVector2Bind();
				bind.Write(value.min, stream);
			}
			if(value.max != default)
			{
				NetConvertBase.SetBit(ref bits[0], 6, true);
				UnityEngineVector2Bind bind = new UnityEngineVector2Bind();
				bind.Write(value.max, stream);
			}
			if(value.width != 0)
			{
				NetConvertBase.SetBit(ref bits[0], 7, true);
				stream.WriteValue(value.width);
			}
			if(value.height != 0)
			{
				NetConvertBase.SetBit(ref bits[0], 8, true);
				stream.WriteValue(value.height);
			}
			if(value.size != default)
			{
				NetConvertBase.SetBit(ref bits[1], 1, true);
				UnityEngineVector2Bind bind = new UnityEngineVector2Bind();
				bind.Write(value.size, stream);
			}
			if(value.xMin != 0)
			{
				NetConvertBase.SetBit(ref bits[1], 2, true);
				stream.WriteValue(value.xMin);
			}
			if(value.yMin != 0)
			{
				NetConvertBase.SetBit(ref bits[1], 3, true);
				stream.WriteValue(value.yMin);
			}
			if(value.xMax != 0)
			{
				NetConvertBase.SetBit(ref bits[1], 4, true);
				stream.WriteValue(value.xMax);
			}
			if(value.yMax != 0)
			{
				NetConvertBase.SetBit(ref bits[1], 5, true);
				stream.WriteValue(value.yMax);
			}
			int pos1 = stream.Position;
			stream.Position = pos;
			stream.Write(bits, 0, 2);
			stream.Position = pos1;
		}

		public UnityEngine.Rect Read(Segment stream)
		{
			byte[] bits = stream.Read(2);
			var value = new UnityEngine.Rect();
			if(NetConvertBase.GetBit(bits[0], 1))
				value.x = stream.ReadValue<Single>();
			if(NetConvertBase.GetBit(bits[0], 2))
				value.y = stream.ReadValue<Single>();
			if(NetConvertBase.GetBit(bits[0], 3))
			{
				UnityEngineVector2Bind bind = new UnityEngineVector2Bind();
				value.position = bind.Read(stream);
			}
			if(NetConvertBase.GetBit(bits[0], 4))
			{
				UnityEngineVector2Bind bind = new UnityEngineVector2Bind();
				value.center = bind.Read(stream);
			}
			if(NetConvertBase.GetBit(bits[0], 5))
			{
				UnityEngineVector2Bind bind = new UnityEngineVector2Bind();
				value.min = bind.Read(stream);
			}
			if(NetConvertBase.GetBit(bits[0], 6))
			{
				UnityEngineVector2Bind bind = new UnityEngineVector2Bind();
				value.max = bind.Read(stream);
			}
			if(NetConvertBase.GetBit(bits[0], 7))
				value.width = stream.ReadValue<Single>();
			if(NetConvertBase.GetBit(bits[0], 8))
				value.height = stream.ReadValue<Single>();
			if(NetConvertBase.GetBit(bits[1], 1))
			{
				UnityEngineVector2Bind bind = new UnityEngineVector2Bind();
				value.size = bind.Read(stream);
			}
			if(NetConvertBase.GetBit(bits[1], 2))
				value.xMin = stream.ReadValue<Single>();
			if(NetConvertBase.GetBit(bits[1], 3))
				value.yMin = stream.ReadValue<Single>();
			if(NetConvertBase.GetBit(bits[1], 4))
				value.xMax = stream.ReadValue<Single>();
			if(NetConvertBase.GetBit(bits[1], 5))
				value.yMax = stream.ReadValue<Single>();
			return value;
		}

		public void WriteValue(object value, Segment stream)
		{
			Write((UnityEngine.Rect)value, stream);
		}

		public object ReadValue(Segment stream)
		{
			return Read(stream);
		}
	}
}

namespace Binding
{
	public struct UnityEngineRectArrayBind : ISerialize<UnityEngine.Rect[]>, ISerialize
	{
		public void Write(UnityEngine.Rect[] value, Segment stream)
		{
			int count = value.Length;
			stream.WriteValue(count);
			if (count == 0) return;
			UnityEngineRectBind bind = new UnityEngineRectBind();
			foreach (var value1 in value)
				bind.Write(value1, stream);
		}

		public UnityEngine.Rect[] Read(Segment stream)
		{
			var count = stream.ReadValue<int>();
			var value = new UnityEngine.Rect[count];
			if (count == 0) return value;
			UnityEngineRectBind bind = new UnityEngineRectBind();
			for (int i = 0; i < count; i++)
				value[i] = bind.Read(stream);
			return value;
		}

		public void WriteValue(object value, Segment stream)
		{
			Write((UnityEngine.Rect[])value, stream);
		}

		public object ReadValue(Segment stream)
		{
			return Read(stream);
		}
	}
}

namespace Binding
{
	public struct UnityEngineRectGenericBind : ISerialize<List<UnityEngine.Rect>>, ISerialize
	{
		public void Write(List<UnityEngine.Rect> value, Segment stream)
		{
			int count = value.Count;
			stream.WriteValue(count);
			if (count == 0) return;
			UnityEngineRectBind bind = new UnityEngineRectBind();
			foreach (var value1 in value)
				bind.Write(value1, stream);
		}

		public List<UnityEngine.Rect> Read(Segment stream)
		{
			var count = stream.ReadValue<int>();
			var value = new List<UnityEngine.Rect>();
			if (count == 0) return value;
			UnityEngineRectBind bind = new UnityEngineRectBind();
			for (int i = 0; i < count; i++)
				value.Add(bind.Read(stream));
			return value;
		}

		public void WriteValue(object value, Segment stream)
		{
			Write((List<UnityEngine.Rect>)value, stream);
		}

		public object ReadValue(Segment stream)
		{
			return Read(stream);
		}
	}
}
