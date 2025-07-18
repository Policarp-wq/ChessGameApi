﻿namespace ChessGameApi.Models
{
    public sealed class ChessLocation
    {
        private int _x;
        public int X
        {
            get => _x;
            set
            {
                if(IsValueAllowed(value))
                    _x = value;
            }
        } // left -> right
        private int _y;
        public int Y
        {
            get => _y;
            set
            {
                if (IsValueAllowed(value))
                    _y = value;
            }
        } // down -> up

        public string ChessFormat => (char)('a' + X) + Y.ToString();

        public ChessLocation(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static bool IsValueAllowed(int value) => value >= 0 && value <= 7;
        public override bool Equals(object? obj)
        {
            if (obj is not ChessLocation loc) return false;
            return loc.X == X && loc.Y == Y;
        }
    }
}
