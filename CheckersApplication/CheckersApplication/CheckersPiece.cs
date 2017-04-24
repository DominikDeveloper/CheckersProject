using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using System.Drawing;

namespace CheckersApplication
{
    public enum Player
    {
        White,
        Black
    }

    class CheckersPiece : ViewModelBase
    {
        private Point pos;

        public Point Pos
        {
            get { return pos; }
            set { pos.X= value.X*60;pos.Y = value.Y * 60; RaisePropertyChanged(() => pos); }
        }

        private Player player;

        public Player Player
        {
            get { return player; }
            set { player = value; RaisePropertyChanged(() => player); }
        }
    }
}
