using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace TurbidCurrent.NetWork
{
    public class ByteArray
    {
        //默认大小
        const int DEFAULT_SIZE = 1024;
        //初始大小
        int initSzie = 0;
        //缓冲区
        public byte[] bytes;
        //读写位置
        public int readIndex = 0;
        public int writeIndex = 0;
        //容量
        private int capacity = 0;

        //剩余空间
        public int remain { get => capacity - writeIndex ; }
        //数据长度
        public int length { get => writeIndex - readIndex; }

        //构造函数
        public ByteArray (int size = DEFAULT_SIZE)
        {
            bytes = new byte[size];
            capacity = size;
            initSzie = size;
            readIndex = 0;
            writeIndex = 0;
        }

        public ByteArray (byte[] defaultBytes)
        {
            bytes = defaultBytes;
            capacity = defaultBytes.Length;
            initSzie = defaultBytes.Length;
            readIndex = 0;
            writeIndex = defaultBytes.Length;
        }
        //重新设置大小
        public void ResetSize(int size)
        {
            if (size < length) return;
            if (size < initSzie) return;
            int n = 1;
            while (n < size)
                n *= 2;
            capacity = n;
            byte[] newBytes = new byte[capacity];
            Array.Copy(bytes, readIndex, newBytes, 0, writeIndex - readIndex);
            bytes = newBytes;
            writeIndex = length;
            readIndex = 0;
        }

        //写入数据
        public int WriteBytes(byte[] bytes,int offset,int count)
        {
            if(remain < count)
            {
                ResetSize(length + count);
            }
            Array.Copy(bytes, offset, bytes, writeIndex, count);
            writeIndex += count;
            return count;
        }

        //读取数据
        public int ReadBytes(byte[] bytes,int offset,int count)
        {
            count = Math.Min(count, length);
            Array.Copy(bytes, 0, bytes, offset, count);
            readIndex += count;
            CheckAndMoveBytes();
            return count;

        }
        //检查并移动数据
        public void CheckAndMoveBytes()
        {
            if (length < 8)
            {
                MoveBytes();
            }
        }
        //移动数据
        public void MoveBytes()
        {
            Array.Copy(bytes, readIndex, bytes, 0, length);
            writeIndex = length;
            readIndex = 0;
        }

        //读取Int16
        public Int16 ReadInt16()
        {
            if (length < 2) return 0;
            Int16 ret = BitConverter.ToInt16(bytes, readIndex);
            readIndex += 2;
            CheckAndMoveBytes();
            return ret;
        }

        //读取Int32
        public Int32 ReadInt32()
        {
            if (length < 4) return 0;
            Int32 ret = BitConverter.ToInt32(bytes, readIndex);
            readIndex += 4;
            CheckAndMoveBytes();
            return ret;
        }
        //打印缓冲区
        public override string ToString()
        {
            return BitConverter.ToString(bytes, readIndex, length);
        }

        //打印调试信息
        public string Debug()
        {
            return string.Format("readIdx({0}) writeIdx({1}) bytes({2})",
            readIndex,
            writeIndex,
            BitConverter.ToString(bytes, 0, capacity)
            );
        }
    }
}
