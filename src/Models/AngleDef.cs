﻿using System;
using System.Runtime.InteropServices;

namespace DuGu720DegreeView.Models
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct AngleDef
    {
        public short MsgType;

        public short Length;

        public float Time;

        public float Pitch;

        public float Roll;

        public float Yaw;

        public float Speed;
    }

}
