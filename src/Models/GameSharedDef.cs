using System.Runtime.InteropServices;

namespace DuGu720DegreeView.Models
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct GameSharedDef
    {
        [MarshalAs(UnmanagedType.Struct)]
        public VectorAngExp Angle;

        public int GameStatus;
    }

}
