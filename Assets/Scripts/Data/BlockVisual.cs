using System;
using UnityEngine;

namespace NumbersBlast.Data
{
    /// <summary>
    /// Serializable data struct mapping a block value to its associated color and sprite.
    /// </summary>
    [Serializable]
    public struct BlockVisual
    {
        public int Value;
        public Color Color;
        public Sprite Sprite;
    }
}
