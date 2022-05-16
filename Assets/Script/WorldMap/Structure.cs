using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WorldMap;


#nullable enable
namespace WorldMap
{
    public interface Structure
    {
        public TileContainer? tileContainer { set; }
    }
}