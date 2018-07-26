
using System.Collections;
using System.Collections.Generic;

using System.Runtime.InteropServices;
using UnityEngine;

namespace DuGu720DegreeView.Models
{
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct TeachingCommand
        {

            public byte HeaderOne;



            public byte HeaderTwo;



            public byte ExperimentIndex;



            public byte IsStart;



            public float ReservedSingle1;



            public float ReservedSingle2;



        }
}


