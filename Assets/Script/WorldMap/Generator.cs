using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#nullable enable
namespace WorldMap
{
    public class Generator : MonoBehaviour
    {
        public static void Generate()
        {
            var area = new LocalArea(20, 100, 5, 20);
            Debug.Log(area.ToString());
        }
    }
}