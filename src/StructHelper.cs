using System;

using System.Collections.Generic;

using System.Linq;

using System.Text;

using System.Runtime.InteropServices;

namespace Utils
{
    public class StructHelper
    {
        public static object BytesToStruct(byte[] bytes, Type type)
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



        public static T BytesToStruct<T>(byte[] bytes)
        {

            var type = typeof(T);

            int num = Marshal.SizeOf(type);

            if (num > bytes.Length)
            {

                return default(T);

            }

            IntPtr intPtr = Marshal.AllocHGlobal(num);

            Marshal.Copy(bytes, 0, intPtr, num);

            var result = Marshal.PtrToStructure(intPtr, type);

            Marshal.FreeHGlobal(intPtr);

            return (T)result;

        }



        public static int GetStructSize<T>()
        {

            var type = typeof(T);

            int num = Marshal.SizeOf(type);

            return num;

        }



        public static float Rad2Deg(float rad)
        {

            return (float)Math.Round(rad * 180.0f / Math.PI, 2);

        }



        public static float Deg2Rad(float deg)
        {

            return (float)Math.Round(deg / 180.0f * Math.PI, 2);

        }
    }
}
