using System.Runtime.InteropServices;

namespace DuGu720DegreeView.Models
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ShakeDef
    {
        public short MsgType;

        public short Length;

        public short SharkType;

        public short Status;

        public float Amplitude;
    }

}
