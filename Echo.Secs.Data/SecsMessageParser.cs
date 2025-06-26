using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Echo.Secs.Data
{
    public static class SecsMessageParser
    {
        public static SecsHeader CreateHeader(byte[] buf)
        {
            return CreateHeader4(buf);
        }

        public static SecsHeader CreateHeader1(byte[] headBuf)
        {
            using MemoryStream stream = new(headBuf);
            using BinaryReader reader = new(stream);

            // 读取一个UInt32（大端序）表示消息长度（00 - 03）
            var length = reader.ReadUInt32();
            // 注意：如果需要处理小端序，可以使用 BinaryPrimitives.ReverseEndianness(length) 进行转换
            length = BinaryPrimitives.ReverseEndianness(length);

            // 读取一个UInt16（大端序）表示设备ID（04 -05）
            var deviceId = reader.ReadUInt16();
            // 注意：如果需要处理小端序，可以使用 BinaryPrimitives.ReverseEndianness(deviceId) 进行转换
            deviceId = BinaryPrimitives.ReverseEndianness(deviceId);

            // 读取一个UInt16（大端序）表示命令号和等待标志位（06 - 07）
            var cmdNum = reader.ReadUInt16();
            // 注意：如果需要处理小端序，可以使用 BinaryPrimitives.ReverseEndianness(cmdNum) 进行转换
            cmdNum = BinaryPrimitives.ReverseEndianness(cmdNum);
            // 分离命令和等待标志位
            var cmd = (SecsCommand)(cmdNum & 0x7fff);
            // 判断是否需要等待（如果命令号的最高位为1，则表示需要等待）
            var waitFlag = cmdNum > 0x8000;

            // 跳过一个字节（Ptype）（08）
            _ = reader.ReadByte();

            // 读取一个字节表示命令类型（SType）（09）
            var cmdType = (SecsCommandType)reader.ReadByte();

            // 读取一个UInt32（大端序）表示随机掩码（10 - 13）
            var mask = reader.ReadUInt32();
            mask = BinaryPrimitives.ReverseEndianness(mask);

            SecsHeader message = new()
            {
                Length = length,
                DeviceId = deviceId,
                Cmd = cmd,
                CmdType = cmdType,
                RandomMask = mask,
                Wait = waitFlag
            };

            return message;
        }

        public static SecsHeader CreateHeader2(byte[] buf)
        {
            var buffer = new byte[buf.Length];
            Buffer.BlockCopy(buf, 0, buffer, 0, buf.Length);

            var waitFlag = buffer[6] > 0x80;
            buffer[6] = (byte)(buffer[6] & 0x7f);

            Array.Reverse(buffer, 0, 4); // Length
            Array.Reverse(buffer, 4, 2); // DeviceId
            Array.Reverse(buffer, 6, 2); // Command
            Array.Reverse(buffer, 10, 4); // RandomMask

            SecsHeader message = new SecsHeader()
            {
                Length = BitConverter.ToUInt32(buffer, 0),
                DeviceId = BitConverter.ToUInt16(buffer, 4),
                Cmd = (SecsCommand)BitConverter.ToUInt16(buffer, 6),
                CmdType = (SecsCommandType)buffer[9],
                RandomMask = BitConverter.ToUInt32(buffer, 10),
                Wait = waitFlag
            };

            return message;
        }

        public static SecsHeader CreateHeader3(byte[] buf)
        {
            var buffer = new byte[buf.Length];
            Buffer.BlockCopy(buf, 0, buffer, 0, buf.Length);

            var waitFlag = buffer[6] > 0x80;
            buffer[6] = (byte)(buffer[6] & 0x7f);

            var length = BitConverter.ToUInt32(buffer, 0);
            length = BinaryPrimitives.ReverseEndianness(length);

            var deviceId = BitConverter.ToUInt16(buffer, 4);
            deviceId = BinaryPrimitives.ReverseEndianness(deviceId);

            var cmdNum = BitConverter.ToUInt16(buffer, 6);
            var cmd = (SecsCommand)BinaryPrimitives.ReverseEndianness(cmdNum);

            var mask = BitConverter.ToUInt32(buffer, 10);
            mask = BinaryPrimitives.ReverseEndianness(mask);

            SecsHeader message = new()
            {
                Length = length,
                DeviceId = deviceId,
                Cmd = cmd,
                CmdType = (SecsCommandType)buffer[9],
                RandomMask = mask,
                Wait = waitFlag
            };

            return message;
        }

        public static SecsHeader CreateHeader4(ReadOnlySpan<byte> buf)
        {
            // 读取并处理各字段
            uint length = BinaryPrimitives.ReadUInt32BigEndian(buf);
            ushort deviceId = BinaryPrimitives.ReadUInt16BigEndian(buf[4..]);
            var waitFlag = buf[6] > 0x80;
            ushort cmdNum = BinaryPrimitives.ReadUInt16BigEndian(buf[6..]);
            var cmd = (SecsCommand)(cmdNum & 0x7fff);
            uint mask = BinaryPrimitives.ReadUInt32BigEndian(buf[10..]);

            return new SecsHeader
            {
                Length = length,
                DeviceId = deviceId,
                Cmd = cmd,
                CmdType = (SecsCommandType)buf[9],
                RandomMask = mask,
                Wait = waitFlag
            };
        }

        public static SecsHeader CreateHeader5(ReadOnlySpan<byte> buf)
        {
            int pos = 0;
            // 读取并处理各字段
            var length = buf[pos++] << 24 |
                         buf[pos++] << 16 |
                         buf[pos++] << 8 |
                         buf[pos++];
            var deviceId = buf[pos++] << 8 |
                           buf[pos++];
            var waitFlag = buf[pos] > 0x80;
            var cmdNum = buf[pos++] << 8 |
                         buf[pos++];
            var cmd = cmdNum & 0x7fff;
            pos++;
            var cmdType = buf[pos++];
            var mask = buf[pos++] << 24 |
                       buf[pos++] << 16 |
                       buf[pos++] << 8 |
                       buf[pos++];

            return new SecsHeader
            {
                Length = (uint)length,
                DeviceId = (ushort)deviceId,
                Cmd = (SecsCommand)cmd,
                CmdType = (SecsCommandType)cmdType,
                RandomMask = (uint)mask,
                Wait = waitFlag
            };
        }

        public static SecsHeader CreateHeader6(ReadOnlySpan<byte> buf)
        {
            int pos = 0;
            // 读取并处理各字段
            var length = buf[pos++] << 24 |
                         buf[pos++] << 16 |
                         buf[pos++] << 8 |
                         buf[pos++];
            var deviceId = buf[pos++] << 8 |
                           buf[pos++];
            var waitFlag = buf[pos] > 0x80;
            var cmdNum = (buf[pos++] & 0x7f) << 8 |
                         buf[pos++];
            var cmd = cmdNum;
            pos++;
            var cmdType = buf[pos++];
            var mask = buf[pos++] << 24 |
                       buf[pos++] << 16 |
                       buf[pos++] << 8 |
                       buf[pos++];

            return new SecsHeader
            {
                Length = (uint)length,
                DeviceId = (ushort)deviceId,
                Cmd = (SecsCommand)cmd,
                CmdType = (SecsCommandType)cmdType,
                RandomMask = (uint)mask,
                Wait = waitFlag
            };
        }

        public static SecsHeader CreateHeader6a(ReadOnlySpan<byte> buf)
        {
            int pos = 0;
            // 读取并处理各字段
            var waitFlag = buf[pos + 6] > 0x80;
            var length = buf[pos++] << 24 |
                         buf[pos++] << 16 |
                         buf[pos++] << 8 |
                         buf[pos++];
            var deviceId = buf[pos++] << 8 |
                           buf[pos++];
            var cmdNum = (buf[pos++] & 0x7f) << 8 |
                         buf[pos++];
            var cmd = cmdNum;
            pos++;
            var cmdType = buf[pos++];
            var mask = buf[pos++] << 24 |
                       buf[pos++] << 16 |
                       buf[pos++] << 8 |
                       buf[pos++];

            return new SecsHeader
            {
                Length = (uint)length,
                DeviceId = (ushort)deviceId,
                Cmd = (SecsCommand)cmd,
                CmdType = (SecsCommandType)cmdType,
                RandomMask = (uint)mask,
                Wait = waitFlag
            };
        }

        public static SecsHeader CreateHeader7(ReadOnlySpan<byte> buf)
        {
            int pos = 0;
            var buf6 = buf[pos + 6];
            var waitFlag = buf6 > 0x80;
            // 读取并处理各字段
            var length = buf[pos + 0] << 24 |
                         buf[pos + 1] << 16 |
                         buf[pos + 2] << 8 |
                         buf[pos + 3];
            var deviceId = buf[pos + 4] << 8 |
                           buf[pos + 5];
            var cmdNum = buf6 << 8 |
                         buf[pos + 7];
            var cmd = cmdNum & 0x7fff;
            var cmdType = buf[pos + 9];
            var mask = buf[pos + 10] << 24 |
                       buf[pos + 11] << 16 |
                       buf[pos + 12] << 8 |
                       buf[pos + 13];

            return new SecsHeader
            {
                Length = (uint)length,
                DeviceId = (ushort)deviceId,
                Cmd = (SecsCommand)cmd,
                CmdType = (SecsCommandType)cmdType,
                RandomMask = (uint)mask,
                Wait = waitFlag
            };
        }

        public static SecsHeader CreateHeader71(ReadOnlySpan<byte> buf)
        {
            int pos = 0;
            // 读取并处理各字段
            var buf6 = buf[pos + 6];
            var length = buf[pos + 0] << 24 |
                         buf[pos + 1] << 16 |
                         buf[pos + 2] << 8 |
                         buf[pos + 3];
            var deviceId = buf[pos + 4] << 8 |
                           buf[pos + 5];
            var waitFlag = buf6 > 0x80;
            var cmdNum = buf6 << 8 |
                         buf[pos + 7];
            var cmd = cmdNum & 0x7fff;
            var cmdType = buf[pos + 9];
            var mask = buf[pos + 10] << 24 |
                       buf[pos + 11] << 16 |
                       buf[pos + 12] << 8 |
                       buf[pos + 13];

            return new SecsHeader
            {
                Length = (uint)length,
                DeviceId = (ushort)deviceId,
                Cmd = (SecsCommand)cmd,
                CmdType = (SecsCommandType)cmdType,
                RandomMask = (uint)mask,
                Wait = waitFlag
            };
        }

        public static SecsHeader CreateHeader8(ReadOnlySpan<byte> buf)
        {
            var waitFlag = buf[6] > 0x80;
            // 读取并处理各字段
            var length = buf[0] << 24 |
                         buf[1] << 16 |
                         buf[2] << 8 |
                         buf[3];
            var deviceId = buf[4] << 8 |
                           buf[5];
            var cmdNum = buf[6] << 8 |
                         buf[7];
            var cmd = cmdNum & 0x7fff;
            var cmdType = buf[9];
            var mask = buf[10] << 24 |
                       buf[11] << 16 |
                       buf[12] << 8 |
                       buf[13];
            return new SecsHeader
            {
                Length = (uint)length,
                DeviceId = (ushort)deviceId,
                Cmd = (SecsCommand)cmd,
                CmdType = (SecsCommandType)cmdType,
                RandomMask = (uint)mask,
                Wait = waitFlag
            };
        }

        /// <summary>
        /// 解析SECS-II消息体数据（Item/List结构）
        /// </summary>
        /// <param name="data">消息体字节数组</param>
        /// <returns>解析后的结构化数据</returns>
        public static SecsDataItem ParseData(byte[] data)
        {
            if (data == null || data.Length == 0)
                return new SecsDataItem { Type = SecsItemType.Empty };

            int pos = 0;
            return ParseItem(data, ref pos);
        }

        private static SecsDataItem ParseItem(byte[] data, ref int pos)
        {
            // 1. 读取Format Byte（包含数据类型和长度字节数）
            byte formatByte = data[pos++];
            int lengthBytesCount = (formatByte & 0b00000011); // 取最后2位
            SecsItemType itemType = (SecsItemType)(formatByte >> 2);

            // 2. 读取Length Bytes（大端序）
            int dataLength = 0;
            for (int i = 0; i < lengthBytesCount; i++)
            {
                dataLength = (dataLength << 8) | data[pos++];
            }

            // 3. 处理特殊类型（List）
            if (itemType == SecsItemType.List)
            {
                return ParseList(data, ref pos, dataLength);
            }

            // 4. 解析具体数据项
            byte[] itemData = new byte[dataLength];
            Array.Copy(data, pos, itemData, 0, dataLength);
            pos += dataLength;

            return new SecsDataItem
            {
                Type = itemType,
                Value = DecodeItemValue(itemType, itemData)
            };
        }

        //private static SecsDataItem ParseItem1(ReadOnlySpan<byte> buf, ref int pos)
        //{
        //    var formatByte = buf[pos++];
        //    var lengthBytesCount = (formatByte & 0b00000011);

        //    int dl = lengthBytesCount switch
        //    {
        //        1 => buf[pos],
        //        2 => BinaryPrimitives.ReadUInt16BigEndian(buf[pos..]),
        //        _ => throw new NotImplementedException(),
        //    };

        //    SecsItemType itemType = (SecsItemType)(formatByte >> 2);

        //    // 2. 读取Length Bytes（大端序）
        //    int dataLength = 0;
        //    for (int i = 0; i < lengthBytesCount; i++)
        //    {
        //        dataLength = (dataLength << 8) | buf[pos++];
        //    }

        //    // 3. 处理特殊类型（List）
        //    if (itemType == SecsItemType.List)
        //    {
        //        return ParseList(buf, ref pos, dataLength);
        //    }

        //    // 4. 解析具体数据项
        //    byte[] itemData = new byte[dataLength];
        //    Array.Copy(buf, pos, itemData, 0, dataLength);
        //    pos += dataLength;

        //    return new SecsDataItem
        //    {
        //        Type = itemType,
        //        Value = DecodeItemValue(itemType, itemData)
        //    };
        //}

        private static SecsDataItem ParseList(byte[] data, ref int pos, int elementCount)
        {
            var listItems = new List<SecsDataItem>();
            for (int i = 0; i < elementCount; i++)
            {
                listItems.Add(ParseItem(data, ref pos));
            }

            return new SecsDataItem
            {
                Type = SecsItemType.List,
                Value = listItems
            };
        }

        private static object? DecodeItemValue(SecsItemType type, byte[] data)
        {
            return type switch
            {
                SecsItemType.Binary => data,
                SecsItemType.Boolean => data.Select(b => b != 0).ToArray(),
                SecsItemType.ASCII => System.Text.Encoding.ASCII.GetString(data),
                SecsItemType.I8 => BitConverter.ToInt64(data.Reverse().ToArray(), 0),
                SecsItemType.I4 => BitConverter.ToInt32(data.Reverse().ToArray(), 0),
                SecsItemType.I2 => BitConverter.ToInt16(data.Reverse().ToArray(), 0),
                SecsItemType.I1 => (sbyte)data[0],
                SecsItemType.F8 => BitConverter.ToDouble(data.Reverse().ToArray(), 0),
                SecsItemType.F4 => BitConverter.ToSingle(data.Reverse().ToArray(), 0),
                SecsItemType.U8 => BitConverter.ToUInt64(data.Reverse().ToArray(), 0),
                SecsItemType.U4 => BitConverter.ToUInt32(data.Reverse().ToArray(), 0),
                SecsItemType.U2 => BitConverter.ToUInt16(data.Reverse().ToArray(), 0),
                SecsItemType.U1 => data[0],
                _ => null,
            };
        }
    }
}
