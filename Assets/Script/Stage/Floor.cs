using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WorldMap;


#nullable enable
namespace Stage
{
    public class Floor : MonoBehaviour, Structure
    {
        public TileContainer? tileContainer { set; private get; }
    }
}
