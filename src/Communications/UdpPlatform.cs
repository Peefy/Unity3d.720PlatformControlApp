using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace DuGu720DegreeView.Communications
{
    public class UdpPlatform : IDisposable
    {

        private object thisLock = new object();

        private static UdpPlatform _instance;

        public static UdpPlatform Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new UdpPlatform();
                return _instance;
            }
        }

        private int remotePort = 16384;

        private int myselfPort = 15000;

        private IPEndPoint remoteEndPoint;

        private UdpClient server;

        public UdpPlatform()
        {
            server = new UdpClient(myselfPort);
            remoteEndPoint = new IPEndPoint(IPAddress.Broadcast, remotePort);
        }

        public byte[] RecieveData()
        {
            var ip = new IPEndPoint(IPAddress.Any,0);
            return server.Receive(ref ip);
        }

        public byte[] RecieveData(ref IPEndPoint ip)
        {
            return server.Receive(ref ip);
        }

        public void SendData(object structObj)
        {
            lock (thisLock)
            {
                int num = Marshal.SizeOf(structObj);
                byte[] array = new byte[num];
                IntPtr intPtr = Marshal.AllocHGlobal(num);
                Marshal.StructureToPtr(structObj, intPtr, false);
                Marshal.Copy(intPtr, array, 0, num);
                Marshal.FreeHGlobal(intPtr);
                if (server != null)
                {
                    server.Send(array, array.Length, remoteEndPoint);
                }
                else
                {
                    Debug.Log("Socket error");
                }
            }
        }

        public void SendData(object structObj, IPEndPoint ip)
        {
            lock (thisLock)
            {
                int num = Marshal.SizeOf(structObj);
                byte[] array = new byte[num];
                IntPtr intPtr = Marshal.AllocHGlobal(num);
                Marshal.StructureToPtr(structObj, intPtr, false);
                Marshal.Copy(intPtr, array, 0, num);
                Marshal.FreeHGlobal(intPtr);
                if (server != null)
                {
                    server.Send(array, array.Length, ip);
                }
                else
                {
                    Debug.Log("Socket error");
                }
            }
        }

        public void SendString(string str)
        {
            lock (thisLock)
            {
                var datas = Encoding.UTF8.GetBytes(str);
                if (server != null)
                    server.Send(datas, datas.Length, remoteEndPoint);
            }
        }

        public object BytesToStruct(byte[] bytes, Type type)
        {
            int num = Marshal.SizeOf(type);
            if (num > bytes.Length)
            {
                return null;
            }
            IntPtr intPtr = Marshal.AllocHGlobal(num);
            Marshal.Copy(bytes, 0, intPtr, num);
            object result = Marshal.PtrToStructure(intPtr, type);
            Marshal.FreeHGlobal(intPtr);
            return result;
        }

        public object BytesTo<T>(byte[] bytes)
        {
            int num = Marshal.SizeOf(typeof(T));
            if (num > bytes.Length)
            {
                return null;
            }
            IntPtr intPtr = Marshal.AllocHGlobal(num);
            Marshal.Copy(bytes, 0, intPtr, num);
            object result = Marshal.PtrToStructure(intPtr, typeof(T));
            Marshal.FreeHGlobal(intPtr);
            return result;
        }

        ~UdpPlatform()
        {
            Dispose(true);
        }

        #region IDisposable Support
        private bool disposedValue = false; // 要检测冗余调用

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)。
                }

                // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                // TODO: 将大型字段设置为 null。

                if (server != null)
                {
                    server.Close();
                    server = null;
                }

                disposedValue = true;
            }
        }

        // TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
        // ~UdpPlatform() {
        //   // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
        //   Dispose(false);
        // }

        // 添加此代码以正确实现可处置模式。
        void IDisposable.Dispose()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(true);
            // TODO: 如果在以上内容中替代了终结器，则取消注释以下行。
            // GC.SuppressFinalize(this);
        }
        #endregion

    }
}
