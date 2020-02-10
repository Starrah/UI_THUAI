using System;
using UnityEngine;

namespace GameData
{
    [Serializable]
    public class Point
    {
        public int x;
        public int y;

        public Point() : this(0, 0) {}

        public Point(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public static Point FromVector2(Vector2Int vec)
        {
            return new Point(vec.x, vec.y);
        }
        
        public static Point FromVector2(Vector2 vec)
        {
            return new Point((int) Math.Round(vec.x), (int)Math.Round(vec.y));
        }

        public Vector2Int ToVector2Int()
        {
            return new Vector2Int(x,y);
        }
        
        public Vector2 ToVector2()
        {
            return new Vector2(x,y);
        }
    }
}