using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

// Token: 0x020001E3 RID: 483
public class Packet : IDisposable
{
	// Token: 0x06000A6B RID: 2667 RVA: 0x000087A1 File Offset: 0x000069A1
	public Packet()
	{
		this.buffer = new List<byte>();
		this.readPos = 0;
	}

	// Token: 0x06000A6C RID: 2668 RVA: 0x000087BB File Offset: 0x000069BB
	public Packet(int _id)
	{
		this.buffer = new List<byte>();
		this.readPos = 0;
		this.Write(_id);
	}

	// Token: 0x06000A6D RID: 2669 RVA: 0x000087DC File Offset: 0x000069DC
	public Packet(byte[] _data)
	{
		this.buffer = new List<byte>();
		this.readPos = 0;
		this.SetBytes(_data);
	}

	// Token: 0x06000A6E RID: 2670 RVA: 0x000087FD File Offset: 0x000069FD
	public void SetBytes(byte[] _data)
	{
		this.Write(_data);
		this.readableBuffer = this.buffer.ToArray();
	}

	// Token: 0x06000A6F RID: 2671 RVA: 0x00008817 File Offset: 0x00006A17
	public void WriteLength()
	{
		this.buffer.InsertRange(0, BitConverter.GetBytes(this.buffer.Count));
	}

	// Token: 0x06000A70 RID: 2672 RVA: 0x00008835 File Offset: 0x00006A35
	public void InsertInt(int _value)
	{
		this.buffer.InsertRange(0, BitConverter.GetBytes(_value));
	}

	// Token: 0x06000A71 RID: 2673 RVA: 0x00008849 File Offset: 0x00006A49
	public byte[] ToArray()
	{
		this.readableBuffer = this.buffer.ToArray();
		return this.readableBuffer;
	}

	// Token: 0x06000A72 RID: 2674 RVA: 0x00008862 File Offset: 0x00006A62
	public int Length()
	{
		return this.buffer.Count;
	}

	// Token: 0x06000A73 RID: 2675 RVA: 0x0000886F File Offset: 0x00006A6F
	public int UnreadLength()
	{
		return this.Length() - this.readPos;
	}

	// Token: 0x06000A74 RID: 2676 RVA: 0x0000887E File Offset: 0x00006A7E
	public void Reset(bool _shouldReset = true)
	{
		if (_shouldReset)
		{
			this.buffer.Clear();
			this.readableBuffer = null;
			this.readPos = 0;
			return;
		}
		this.readPos -= 4;
	}

	// Token: 0x06000A75 RID: 2677 RVA: 0x000088AB File Offset: 0x00006AAB
	public void Write(byte _value)
	{
		this.buffer.Add(_value);
	}

	// Token: 0x06000A76 RID: 2678 RVA: 0x000088B9 File Offset: 0x00006AB9
	public void Write(byte[] _value)
	{
		this.buffer.AddRange(_value);
	}

	// Token: 0x06000A77 RID: 2679 RVA: 0x000088C7 File Offset: 0x00006AC7
	public void Write(short _value)
	{
		this.buffer.AddRange(BitConverter.GetBytes(_value));
	}

	// Token: 0x06000A78 RID: 2680 RVA: 0x000088DA File Offset: 0x00006ADA
	public void Write(int _value)
	{
		this.buffer.AddRange(BitConverter.GetBytes(_value));
	}

	// Token: 0x06000A79 RID: 2681 RVA: 0x000088ED File Offset: 0x00006AED
	public void Write(long _value)
	{
		this.buffer.AddRange(BitConverter.GetBytes(_value));
	}

	// Token: 0x06000A7A RID: 2682 RVA: 0x00008900 File Offset: 0x00006B00
	public void Write(float _value)
	{
		this.buffer.AddRange(BitConverter.GetBytes(_value));
	}

	// Token: 0x06000A7B RID: 2683 RVA: 0x00008913 File Offset: 0x00006B13
	public void Write(double _value)
	{
		this.buffer.AddRange(BitConverter.GetBytes(_value));
	}

	// Token: 0x06000A7C RID: 2684 RVA: 0x00008926 File Offset: 0x00006B26
	public void Write(bool _value)
	{
		this.buffer.AddRange(BitConverter.GetBytes(_value));
	}

	// Token: 0x06000A7D RID: 2685 RVA: 0x0003D8BC File Offset: 0x0003BABC
	public void Write(string _value)
	{
		byte[] bytes = Encoding.UTF8.GetBytes(_value);
		this.Write(bytes.Length);
		this.buffer.AddRange(bytes);
	}

	// Token: 0x06000A7E RID: 2686 RVA: 0x00008939 File Offset: 0x00006B39
	public void Write(Vector3 _value)
	{
		this.Write(_value.x);
		this.Write(_value.y);
		this.Write(_value.z);
	}

	// Token: 0x06000A7F RID: 2687 RVA: 0x0000895F File Offset: 0x00006B5F
	public void Write(Quaternion _value)
	{
		this.Write(_value.x);
		this.Write(_value.y);
		this.Write(_value.z);
		this.Write(_value.w);
	}

	// Token: 0x06000A80 RID: 2688 RVA: 0x00008991 File Offset: 0x00006B91
	public byte ReadByte(bool _moveReadPos = true)
	{
		if (this.buffer.Count > this.readPos)
		{
			byte b = this.readableBuffer[this.readPos];
			if (_moveReadPos)
			{
				this.readPos++;
			}
			return b;
		}
		throw new Exception("Could not read value of type 'byte'!");
	}

	// Token: 0x06000A81 RID: 2689 RVA: 0x0003D8EC File Offset: 0x0003BAEC
	public byte[] ReadBytes(int _length, bool _moveReadPos = true)
	{
		if (this.buffer.Count > this.readPos)
		{
			byte[] array = this.buffer.GetRange(this.readPos, _length).ToArray();
			if (_moveReadPos)
			{
				this.readPos += _length;
			}
			return array;
		}
		throw new Exception("Could not read value of type 'byte[]'!");
	}

	// Token: 0x06000A82 RID: 2690 RVA: 0x0003D944 File Offset: 0x0003BB44
	public short ReadShort(bool _moveReadPos = true)
	{
		if (this.buffer.Count > this.readPos)
		{
			short num = BitConverter.ToInt16(this.readableBuffer, this.readPos);
			if (_moveReadPos)
			{
				this.readPos += 2;
			}
			return num;
		}
		throw new Exception("Could not read value of type 'short'!");
	}

	// Token: 0x06000A83 RID: 2691 RVA: 0x0003D994 File Offset: 0x0003BB94
	public int ReadInt(bool _moveReadPos = true)
	{
		if (this.buffer.Count > this.readPos)
		{
			int num = BitConverter.ToInt32(this.readableBuffer, this.readPos);
			if (_moveReadPos)
			{
				this.readPos += 4;
			}
			return num;
		}
		throw new Exception("Could not read value of type 'int'!");
	}

	// Token: 0x06000A84 RID: 2692 RVA: 0x0003D9E4 File Offset: 0x0003BBE4
	public long ReadLong(bool _moveReadPos = true)
	{
		if (this.buffer.Count > this.readPos)
		{
			long num = BitConverter.ToInt64(this.readableBuffer, this.readPos);
			if (_moveReadPos)
			{
				this.readPos += 8;
			}
			return num;
		}
		throw new Exception("Could not read value of type 'long'!");
	}

	// Token: 0x06000A85 RID: 2693 RVA: 0x0003DA34 File Offset: 0x0003BC34
	public float ReadFloat(bool _moveReadPos = true)
	{
		if (this.buffer.Count > this.readPos)
		{
			float num = BitConverter.ToSingle(this.readableBuffer, this.readPos);
			if (_moveReadPos)
			{
				this.readPos += 4;
			}
			return num;
		}
		throw new Exception("Could not read value of type 'float'!");
	}

	// Token: 0x06000A86 RID: 2694 RVA: 0x0003DA84 File Offset: 0x0003BC84
	public double ReadDouble(bool _moveReadPos = true)
	{
		if (this.buffer.Count > this.readPos)
		{
			double num = BitConverter.ToDouble(this.readableBuffer, this.readPos);
			if (_moveReadPos)
			{
				this.readPos += 8;
			}
			return num;
		}
		throw new Exception("Could not read value of type 'double'!");
	}

	// Token: 0x06000A87 RID: 2695 RVA: 0x0003DAD4 File Offset: 0x0003BCD4
	public bool ReadBool(bool _moveReadPos = true)
	{
		if (this.buffer.Count > this.readPos)
		{
			bool flag = BitConverter.ToBoolean(this.readableBuffer, this.readPos);
			if (_moveReadPos)
			{
				this.readPos++;
			}
			return flag;
		}
		throw new Exception("Could not read value of type 'bool'!");
	}

	// Token: 0x06000A88 RID: 2696 RVA: 0x0003DB24 File Offset: 0x0003BD24
	public string ReadString(bool _moveReadPos = true)
	{
		string text;
		try
		{
			int num = this.ReadInt(true);
			string @string = Encoding.UTF8.GetString(this.readableBuffer, this.readPos, num);
			if (_moveReadPos && @string.Length > 0)
			{
				this.readPos += num;
			}
			text = @string;
		}
		catch
		{
			throw new Exception("Could not read value of type 'string'!");
		}
		return text;
	}

	// Token: 0x06000A89 RID: 2697 RVA: 0x000089D1 File Offset: 0x00006BD1
	public Vector3 ReadVector3(bool _moveReadPos = true)
	{
		return new Vector3(this.ReadFloat(_moveReadPos), this.ReadFloat(_moveReadPos), this.ReadFloat(_moveReadPos));
	}

	// Token: 0x06000A8A RID: 2698 RVA: 0x000089ED File Offset: 0x00006BED
	public Quaternion ReadQuaternion(bool _moveReadPos = true)
	{
		return new Quaternion(this.ReadFloat(_moveReadPos), this.ReadFloat(_moveReadPos), this.ReadFloat(_moveReadPos), this.ReadFloat(_moveReadPos));
	}

	// Token: 0x06000A8B RID: 2699 RVA: 0x00008A10 File Offset: 0x00006C10
	protected virtual void Dispose(bool _disposing)
	{
		if (!this.disposed)
		{
			if (_disposing)
			{
				this.buffer = null;
				this.readableBuffer = null;
				this.readPos = 0;
			}
			this.disposed = true;
		}
	}

	// Token: 0x06000A8C RID: 2700 RVA: 0x00008A3C File Offset: 0x00006C3C
	public void Dispose()
	{
		this.Dispose(true);
		GC.SuppressFinalize(this);
	}

	// Token: 0x04000D0C RID: 3340
	private List<byte> buffer;

	// Token: 0x04000D0D RID: 3341
	private byte[] readableBuffer;

	// Token: 0x04000D0E RID: 3342
	private int readPos;

	// Token: 0x04000D0F RID: 3343
	private bool disposed;
}
