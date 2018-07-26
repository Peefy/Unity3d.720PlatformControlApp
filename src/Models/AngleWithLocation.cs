using System.Runtime.InteropServices;


namespace DuGu720DegreeView.Models
{

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct AngleWithLocation
    {
        public double X;
        public double Y;
        public double Z;
        public double Yaw;
        public double Pitch;
        public double Roll;       
    }
}
