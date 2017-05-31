using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckersApplication
{
    public class MovesBody
    {
        public MovesBody()
        {

        }

        public MovesBody(string locFrom, string locTo, int jumps, string who)
        {
            this.BoardLocationFrom = locFrom;
            this.BoardLocationTo = locTo;
            this.NumberOfJumps = jumps;
            this.Player = who;
        }

        public string BoardLocationFrom { get; set; }
        public string BoardLocationTo { get; set; }
        public int NumberOfJumps { get; set; }
        public string Player { get; set; }
    }

    public class MovesJumps
    {
        public static List<MovesBody> jump_buffer_white;
        public static List<MovesBody> jump_buffer_black;
        public static List<MovesBody> move_buffer_black;
        public static List<MovesBody> move_buffer_white;

        public static void CheckJumpsForWhite(ChessField[,] board) // Check jumps, White going down
        {
            jump_buffer_white = new List<MovesBody>();

            for (int row = 0; row < 6; row++)
                for (int col = 0; col < 8; col++)
                {
                    if (col < 2 && board[row, col].Value == (int)Player.WhiteMen)
                        CheckRightJumpForWhite(board, row, col);
                    if (col > 5 && board[row, col].Value == (int)Player.WhiteMen)
                        CheckLeftJumpForWhite(board, row, col);
                    if (col > 1 && col < 6 && board[row, col].Value == (int)Player.WhiteMen)
                    {
                        CheckRightJumpForWhite(board, row, col);
                        CheckLeftJumpForWhite(board, row, col);
                    }
                }
        }

        public static void CheckJumpsForBlack(ChessField[,] board) // Check jumps, Black going up
        {
            jump_buffer_black = new List<MovesBody>();

            for (int row = 2; row < 8; row++)
                for (int col = 0; col < 8; col++)
                {
                    if (col < 2 && board[row, col].Value == (int)Player.BlackMen)
                        CheckRightJumpForBlack(board, row, col);
                    if (col > 5 && board[row, col].Value == (int)Player.BlackMen)
                        CheckLeftJumpForBlack(board, row, col);
                    if (col > 1 && col < 6 && board[row, col].Value == (int)Player.BlackMen)
                    {
                        CheckRightJumpForBlack(board, row, col);
                        CheckLeftJumpForBlack(board, row, col);
                    }

                }
        }

        public static void CheckJumps(ChessField[,] board)
        {
            CheckJumpsForWhite(board);
            CheckJumpsForBlack(board);
        }

        private static void CheckLeftMovesForBlack(ChessField[,] board, int row, int col) //checks left moves for black checkers piece
        {
            if (board[row - 1, col - 1].Value == (int)Player.BlackEmptySquare)
            {
                MovesBody oneMove = new MovesBody(
                    "[" + row.ToString() + "," + col.ToString() + "]",
                    "[" + (row - 1).ToString() + "," + (col - 1).ToString() + "]",
                    0,
                    "Black"
                    );
                move_buffer_black.Add(oneMove);
            }
        }

        private static void CheckRightMovesForBlack(ChessField[,] board, int row, int col) //checks right moves for black checkers piece
        {
            if (board[row - 1, col + 1].Value == (int)Player.BlackEmptySquare)
            {
                MovesBody oneMove = new MovesBody(
                    "[" + row.ToString() + "," + col.ToString() + "]",
                    "[" + (row - 1).ToString() + "," + (col + 1).ToString() + "]",
                    0,
                    "Black"
                    );
                move_buffer_black.Add(oneMove);
            }
        }
        public static void CheckMovesForBlack(ChessField[,] board) //checks moves for black checkers pieces
        {
            move_buffer_black = new List<MovesBody>();
            for (int row = 1; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    if (board[row, col].Value == (int)Player.BlackMen)
                    {
                        if (col == 0)
                        {
                            CheckRightMovesForBlack(board, row, col);
                        }
                        else if (col == 7)
                        {
                            CheckLeftMovesForBlack(board, row, col);
                        }
                        else
                        {
                            CheckLeftMovesForBlack(board, row, col);
                            CheckRightMovesForBlack(board, row, col);
                        }
                    }
                }
            }
        }

        private static void CheckLeftMovesForWhite(ChessField[,] board, int row, int col) //checks left moves for black checkers piece
        {
            if (board[row + 1, col - 1].Value == (int)Player.BlackEmptySquare)
            {
                MovesBody oneMove = new MovesBody(
                    "[" + row.ToString() + "," + col.ToString() + "]",
                    "[" + (row + 1).ToString() + "," + (col - 1).ToString() + "]",
                    0,
                    "White"
                    );
                move_buffer_white.Add(oneMove);
            }
        }

        private static void CheckRightMovesForWhite(ChessField[,] board, int row, int col) //checks right moves for black checkers piece
        {
            if (board[row + 1, col + 1].Value == (int)Player.BlackEmptySquare)
            {
                MovesBody oneMove = new MovesBody(
                    "[" + row.ToString() + "," + col.ToString() + "]",
                    "[" + (row + 1).ToString() + "," + (col + 1).ToString() + "]",
                    0,
                    "White"
                    );
                move_buffer_white.Add(oneMove);
            }
        }
        public static void CheckMovesForWhite(ChessField[,] board) //checks moves for black checkers pieces
        {
            move_buffer_white = new List<MovesBody>();
            move_buffer_white.Clear();

            for (int row = 0; row < 7; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    if (board[row, col].Value == (int)Player.WhiteMen)
                    {
                        if (col == 0)
                        {
                            CheckRightMovesForWhite(board, row, col);
                        }
                        else if (col == 7)
                        {
                            CheckLeftMovesForWhite(board, row, col);
                        }
                        else
                        {
                            CheckLeftMovesForWhite(board, row, col);
                            CheckRightMovesForWhite(board, row, col);
                        }
                    }
                }
            }
        }
        private static void CheckRightJumpForWhite(ChessField[,] board, int row, int col) //checks valid right-hand jump
        {
            if (board[row + 1, col + 1].Value >= (int)Player.BlackMen &&
                board[row + 1, col + 1].Value == (int)Player.BlackMen &&
                board[row + 2, col + 2].Value < 2)
            {
                //board[row, col] = 9;
                MovesBody oneJump = new MovesBody(
                    "[" + row.ToString() + "," + col.ToString() + "]",
                    "[" + (row + 2).ToString() + "," + (col + 2).ToString() + "]",
                    1,
                    "White"
                    );
                jump_buffer_white.Add(oneJump);
            }
        }

        private static void CheckLeftJumpForWhite(ChessField[,] board, int row, int col) //check valid left-hand jump
        {
            if (board[row + 1, col - 1].Value >= (int)Player.BlackMen &&
                board[row + 1, col - 1].Value == (int)Player.BlackMen &&
                board[row + 2, col - 2].Value < 2)
            {
                //board[row, col] = 9;
                MovesBody oneJump = new MovesBody(
                    "[" + row.ToString() + "," + col.ToString() + "]",
                    "[" + (row + 2).ToString() + "," + (col - 2).ToString() + "]",
                    1,
                    "White"
                    );
                jump_buffer_white.Add(oneJump);
            }
        }

        private static void CheckRightJumpForBlack(ChessField[,] board, int row, int col) //checks valid right-hand jump
        {
            if (board[row - 1, col + 1].Value >= (int)Player.WhiteMen &&
                board[row - 1, col + 1].Value == (int)Player.WhiteMen &&
                board[row - 2, col + 2].Value < 2)
            {
                //board[row, col] = 8; //didn't test
                MovesBody oneJump = new MovesBody(
                   "[" + row.ToString() + "," + col.ToString() + "]",
                   "[" + (row - 2).ToString() + "," + (col + 2).ToString() + "]",
                   1,
                   "Black"
                   );
                jump_buffer_black.Add(oneJump);
            }
        }

        private static void CheckLeftJumpForBlack(ChessField[,] board, int row, int col) //check valid left-hand jump
        {
            if (board[row - 1, col - 1].Value >= (int)Player.WhiteMen &&
                board[row - 1, col - 1].Value == (int)Player.WhiteMen &&
                board[row - 2, col - 2].Value < 2)
            {
                //board[row, col] = 8; //didn't test
                MovesBody oneJump = new MovesBody(
                   "[" + row.ToString() + "," + col.ToString() + "]",
                   "[" + (row - 2).ToString() + "," + (col - 2).ToString() + "]",
                   1,
                   "Black"
                   );
                jump_buffer_black.Add(oneJump);
            }
        }
    }
}