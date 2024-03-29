﻿using System.Collections.ObjectModel;

namespace Chess.Core.Model
{
    public abstract class Piece
    {
        public abstract string Letter { get; }
        public char Color { get; private set; }

        public bool IsAlive { get; set; }

        protected Collection<Rule> Rules;

        public Piece(PieceColor color)
        {
            Rules = new Collection<Rule>();

            Color = color == PieceColor.White ? 'W' : 'B';

            IsAlive = true;

            InitializeRules();
        }

        public bool IsValidMovement(bool withCaputure, int startRow, int startColumn, int endRow, int endColumn)
        {
            var movement = new Movement
            {
                WithCaputure = withCaputure,
                StartX = startColumn,
                StartY = startRow,
                EndX = endColumn,
                EndY = endRow
            };

            return Rules.Where(r => r.Validate(movement)).Any();
        }

        public abstract void InitializeRules();

        public override string ToString() => $"{Letter}{Color}";
    }
}
