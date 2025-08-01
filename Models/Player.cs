﻿using System.Text.Json.Serialization;

namespace ChessGameApi.Models
{
    public class Player
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ChessColors ChessSide { get; set; }
        public List<ChessPiece> Owns { get; set; }

        public Player(int id, string name, ChessColors chessSide, List<ChessPiece> owns)
        {
            Id = id;
            Name = name;
            ChessSide = chessSide;
            Owns = owns;
        }
    }
}