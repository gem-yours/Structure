using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Spell
{
    string imageName { get; }
    string description { get; }
    float damage { get; }
}
