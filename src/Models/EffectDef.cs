using System.Runtime.InteropServices;

namespace DuGu720DegreeView.Models
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct EffectDef
    {
        public short MsgType;

        public short Length;

        public short EffectType;

        public short Status;
    }

}
