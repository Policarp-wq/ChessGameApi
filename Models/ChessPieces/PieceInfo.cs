﻿namespace ChessGameApi.Models.ChessPieces
{
    public record PieceInfo(ChessPieceNames Name, ChessColors Color)
    { 
        public PieceInfo(string name, char color)
            : this(Enum.Parse<ChessPieceNames>(name), color == 'w' ? ChessColors.White : ChessColors.Black)
        {
        }
    }
}
