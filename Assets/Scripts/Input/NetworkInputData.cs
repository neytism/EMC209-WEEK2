using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

namespace GNW.InputData
{
    public struct NetworkInputData : INetworkInput
    {
        public const byte SHOOTBUTTON = 1;
        public const byte JUMPBUTTON = 2;
        public NetworkButtons buttons;
        
        public Vector3 Direction;
        
    }

}
