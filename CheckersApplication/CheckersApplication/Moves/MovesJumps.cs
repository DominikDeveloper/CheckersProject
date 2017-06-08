//MovesJumps.cs

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
        //public static List<int[,]> move_matrix_buffer_white;
        public static List<ChessField[,]> move_matrix_buffer_white;
        public static List<ChessField[,]> move_matrix_buffer_black;

        public static int indexxx = 0;
        public static int indexxxBlack = 0;


        public static void CheckJumpsForWhite(ChessField[,] board, int indexx) // Check jumps, White going down
        {
            jump_buffer_white = new List<MovesBody>();
            for (int row = 0; row < 6; row++)
                for (int col = 0; col < 8; col++)
                {
                    
                    if (col < 2 && board[row, col].Value == (int)Player.White)
                    {
                        CheckRightJumpForWhite(board, row, col, ref move_matrix_buffer_white, indexx);
                    }
                    if (col > 5 && board[row, col].Value == (int)Player.White)
                    {
                        CheckLeftJumpForWhite(board, row, col, ref move_matrix_buffer_white, indexx);
                    }
                    if (col > 1 && col < 6 && board[row, col].Value == (int)Player.White)
                    {
                        if (CheckLeftJumpForWhite2(board, row, col) == true && CheckRightJumpForWhite2(board, row, col) == true)
                        {
                            indexxx++;
                            var newFields = ChessField.GetEmptyFields();
                            move_matrix_buffer_white.Add(newFields);
                            for (int i = 0; i < 8; i++)
                            {
                                for (int j = 0; j < 8; j++)
                                {
                                    move_matrix_buffer_white[indexx + 1][i, j].Value = board[i, j].Value;
                                }
                            }
                            CheckLeftJumpForWhite(board, row, col, ref move_matrix_buffer_white, indexx);
                            CheckRightJumpForWhite(board, row, col, ref move_matrix_buffer_white, indexx + 1);
                        }
                        else
                        {
                            CheckLeftJumpForWhite(board, row, col, ref move_matrix_buffer_white, indexx);
                            CheckRightJumpForWhite(board, row, col, ref move_matrix_buffer_white, indexx);
                        }
                        
                    }
                }
        }

        public static bool CheckRightJumpForWhite2(ChessField[,] board, int row, int col)
        {
            if (row <= 5)
            {
                if (board[row + 1, col + 1].Value >= (int)Player.Black &&
                    board[row + 1, col + 1].Value == (int)Player.Black &&
                    board[row + 2, col + 2].Value < 2)
                {
                    return true;
                }
                else return false;
            }
            return false;
        }

        public static bool CheckLeftJumpForWhite2(ChessField[,] board, int row, int col)
        {
            if (row <= 5)
            {
                if (board[row + 1, col - 1].Value >= (int)Player.Black &&
                   board[row + 1, col - 1].Value == (int)Player.Black &&
                   board[row + 2, col - 2].Value < 2)
                {
                    return true;
                }
                else return false;
            }
            return false;
        }

        private static void CheckRightJumpForWhite(ChessField[,] board, int row, int col, ref List<ChessField[,]> jumpList, int indexx) //checks valid right-hand jump
        {
            if (row <= 5)
            {
                if (board[row + 1, col + 1].Value >= (int)Player.Black &&
                    board[row + 1, col + 1].Value == (int)Player.Black &&
                    board[row + 2, col + 2].Value < 2)
                {
                    //board[row, col] = 9;
                    MovesBody oneMove = new MovesBody(
                    "[" + row.ToString() + "," + col.ToString() + "]",
                    "[" + (row + 2).ToString() + "," + (col + 2).ToString() + "]",
                    1,
                    "White"
                    );
                    move_buffer_white.Add(oneMove);
                    //jump_buffer_white.Add("[" + row.ToString() + "," + col.ToString() + "] > " + "[" + (row + 2).ToString() + "," + (col + 2).ToString() + "]");
                    jumpList[indexx][row + 1, col + 1].Value = 1;
                    jumpList[indexx][row, col].Value = 1;
                    jumpList[indexx][row + 2, col + 2].Value = 3;
                    if ((col + 2) < 2 /*&& board[row, col] == (int)Player.BlackMen*/)
                        CheckRightJumpForWhite(board, row + 2, col + 2, ref move_matrix_buffer_white, indexx);
                    if ((col + 2) > 5 /*&& board[row, col] == (int)Player.BlackMen*/)
                        CheckLeftJumpForWhite(board, row + 2, col + 2, ref move_matrix_buffer_white, indexx);
                    if ((col + 2) > 1 && (col + 2) < 6 /*&& board[row, col] == (int)Player.BlackMen*/)
                    {
                        if (CheckLeftJumpForWhite2(jumpList[indexx], row + 2, col + 2) == true && CheckRightJumpForWhite2(jumpList[indexx], row + 2, col + 2) == true)
                        {
                            indexxx++;
                            var newFields = ChessField.GetEmptyFields();
                            move_matrix_buffer_white.Add(newFields);
                            for (int i = 0; i < 8; i++)
                            {
                                for (int j = 0; j < 8; j++)
                                {
                                    move_matrix_buffer_white[indexxx][i, j].Value = jumpList[indexx][i, j].Value;
                                }
                            }
                            CheckLeftJumpForWhite(board, row + 2, col + 2, ref move_matrix_buffer_white, indexx);
                            CheckRightJumpForWhite(board, row + 2, col + 2, ref move_matrix_buffer_white, indexxx);
                        }
                        else
                        {
                            CheckLeftJumpForWhite(board, row + 2, col + 2, ref move_matrix_buffer_white, indexx);
                            CheckRightJumpForWhite(board, row + 2, col + 2, ref move_matrix_buffer_white, indexx);
                        }

                    }
                }
            }
        }

        private static void CheckLeftJumpForWhite(ChessField[,] board, int row, int col, ref List<ChessField[,]> jumpList, int indexx) //check valid left-hand jump
        {
            if (row <= 5)
            {
                if (board[row + 1, col - 1].Value >= (int)Player.Black &&
                    board[row + 1, col - 1].Value == (int)Player.Black &&
                    board[row + 2, col - 2].Value < 2)
                {
                    //board[row, col] = 9;
                    MovesBody oneMove = new MovesBody(
                    "[" + row.ToString() + "," + col.ToString() + "]",
                    "[" + (row + 2).ToString() + "," + (col - 2).ToString() + "]",
                    1,
                    "White"
                    );
                    move_buffer_white.Add(oneMove);
                    //jump_buffer_white.Add("[" + row.ToString() + "," + col.ToString() + "] > " + "[" + (row + 2).ToString() + "," + (col - 2).ToString() + "]");
                    jumpList[indexx][row + 1, col - 1].Value = 1;
                    jumpList[indexx][row, col].Value = 1;
                    jumpList[indexx][row + 2, col - 2].Value = 3;
                    if ((col - 2) < 2 /*&& board[row, col] == (int)Player.BlackMen*/)
                        CheckRightJumpForWhite(board, row + 2, col - 2, ref move_matrix_buffer_white, indexx);
                    if ((col - 2) > 5 /*&& board[row, col] == (int)Player.BlackMen*/)
                        CheckLeftJumpForWhite(board, row + 2, col - 2, ref move_matrix_buffer_white, indexx);
                    if ((col - 2) > 1 && (col - 2) < 6 /*&& board[row, col] == (int)Player.BlackMen*/)
                    {
                        if (CheckRightJumpForWhite2(jumpList[indexx], row + 2, col - 2) == true && CheckLeftJumpForWhite2(jumpList[indexx], row + 2, col - 2) == true)
                        {
                            indexxx++;
                            var newFields = ChessField.GetEmptyFields();
                            move_matrix_buffer_white.Add(newFields);
                            for (int i = 0; i < 8; i++)
                            {
                                for (int j = 0; j < 8; j++)
                                {
                                    move_matrix_buffer_white[indexxx][i, j].Value = jumpList[indexx][i, j].Value;
                                }
                            }
                            CheckLeftJumpForWhite(board, row + 2, col - 2, ref move_matrix_buffer_white, indexx);
                            CheckRightJumpForWhite(board, row + 2, col - 2, ref move_matrix_buffer_white, indexxx);
                        }
                        else
                        {
                            CheckLeftJumpForWhite(board, row + 2, col - 2, ref move_matrix_buffer_white, indexx);
                            CheckRightJumpForWhite(board, row + 2, col - 2, ref move_matrix_buffer_white, indexx);
                        }

                    }

                }
            }
        }

        private static void CheckLeftMovesForWhite(ChessField[,] board, int row, int col, ref ChessField[,] board2, ref bool ruch) //checks left moves for black checkers piece
        {
            if (board[row + 1, col - 1].Value == (int)Player.WhiteSquare)
            {
                ruch = true;
                board2[row, col].Value = 1;
                board2[row + 1, col - 1].Value = 3;
                //showBoard(board2);
                //move_matrix_buffer_white.Add(board2);
                MovesBody oneMove = new MovesBody(
                    "[" + row.ToString() + "," + col.ToString() + "]",
                    "[" + (row + 1).ToString() + "," + (col - 1).ToString() + "]",
                    1,
                    "White"
                    );
                move_buffer_white.Add(oneMove);
                //move_buffer_white.Add("[" + row.ToString() + "," + col.ToString() + "] > " + "[" + (row + 1).ToString() + "," + (col - 1).ToString() + "]");
            }
        }

        private static void CheckRightMovesForWhite(ChessField[,] board, int row, int col, ref ChessField[,] board2, ref bool ruch) //checks right moves for white checkers piece
        {
            if (board[row + 1, col + 1].Value == (int)Player.WhiteSquare)
            {
                ruch = true;
                board2[row, col].Value = 1;
                board2[row + 1, col + 1].Value = 3;
                //showBoard(board2);
                //move_matrix_buffer_white.Add(board2);
                MovesBody oneMove = new MovesBody(
                    "[" + row.ToString() + "," + col.ToString() + "]",
                    "[" + (row + 1).ToString() + "," + (col + 1).ToString() + "]",
                    1,
                    "White"
                    );
                move_buffer_white.Add(oneMove);
                //move_buffer_white.Add("[" + row.ToString() + "," + col.ToString() + "] > " + "[" + (row + 1).ToString() + "," + (col + 1).ToString() + "]");
            }
        }

        private static void CheckMovesForWhite(ChessField[,] board) //checks moves for black checkers pieces
        {
            move_buffer_white = new List<MovesBody>();
            for (int row = 0; row < 7; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    bool ruch = false;
                    ChessField[,] board2 = ChessField.GetEmptyFields();
                    for (int i = 0; i < 8; i++)
                    {
                        for (int j = 0; j < 8; j++)
                        {
                            board2[i, j].Value = board[i, j].Value;
                        }
                    }
                    if (board[row, col].Value == (int)Player.White)
                    {
                        if (col == 0)
                        {
                            CheckRightMovesForWhite(board, row, col, ref board2, ref ruch);
                            if (ruch == true)
                            {
                                //showBoard(board2);
                                if (move_matrix_buffer_white == null) { move_matrix_buffer_white = new List<ChessField[,]>(); }
                                move_matrix_buffer_white.Add(board2);
                                ruch = false;
                            }
                        }
                        else if (col == 7)
                        {
                            CheckLeftMovesForWhite(board, row, col, ref board2, ref ruch);
                            if (ruch == true)
                            {
                                //showBoard(board2);
                                if (move_matrix_buffer_white == null) { move_matrix_buffer_white = new List<ChessField[,]>(); }
                                move_matrix_buffer_white.Add(board2);
                                ruch = false;
                            }
                        }
                        else
                        {
                            CheckLeftMovesForWhite(board, row, col, ref board2, ref ruch);
                            if (ruch == true)
                            {
                                //showBoard(board2);
                                if (move_matrix_buffer_white == null) { move_matrix_buffer_white = new List<ChessField[,]>(); }
                                move_matrix_buffer_white.Add(board2);
                                ruch = false;
                                board2 = ChessField.GetEmptyFields();
                                for (int i = 0; i < 8; i++)
                                {
                                    for (int j = 0; j < 8; j++)
                                    {
                                        board2[i, j].Value = board[i, j].Value;
                                    }
                                }
                            }
                            CheckRightMovesForWhite(board, row, col, ref board2, ref ruch);
                            if (ruch == true)
                            {
                                //showBoard(board2);
                                move_matrix_buffer_white.Add(board2);
                            }
                        }
                    }
                }
            }
        }


        public static bool CheckRightJumpForBlack2(ChessField[,] board, int row, int col)
        {
            if (row >= 2)
            {
                if (board[row - 1, col + 1].Value >= (int)Player.White &&
                    board[row - 1, col + 1].Value == (int)Player.White &&
                    board[row - 2, col + 2].Value < 2)
                {
                    return true;
                }
                else return false;
            }
            return false;
        }

        public static bool CheckLeftJumpForBlack2(ChessField[,] board, int row, int col)
        {
            if (row >= 2)
            {
                if (board[row - 1, col - 1].Value >= (int)Player.White &&
                   board[row - 1, col - 1].Value == (int)Player.White &&
                   board[row - 2, col - 2].Value < 2)
                {
                    return true;
                }
                else return false;
            }
            return false;
        }


        public static void CheckJumpsForBlack(ChessField[,] board, int indexx) // Check jumps, Black going up
        {
            jump_buffer_black = new List<MovesBody>();
            for (int row = 2; row < 8; row++)
                for (int col = 0; col < 8; col++)
                {
                    if (col < 2 && board[row, col].Value == (int)Player.Black)
                    {
                        CheckRightJumpForBlack(board, row, col, ref move_matrix_buffer_black, indexx);
                    }
                    if (col > 5 && board[row, col].Value == (int)Player.Black)
                    {
                        CheckLeftJumpForBlack(board, row, col, ref move_matrix_buffer_black, indexx);
                    }
                    if (col > 1 && col < 6 && board[row, col].Value == (int)Player.Black)
                    {
                        if (CheckLeftJumpForBlack2(board, row, col) == true && CheckRightJumpForBlack2(board, row, col) == true)
                        {
                            indexxxBlack++;
                            var newFields = ChessField.GetEmptyFields();
                            move_matrix_buffer_black.Add(newFields);
                            for (int i = 0; i < 8; i++)
                            {
                                for (int j = 0; j < 8; j++)
                                {
                                    move_matrix_buffer_black[indexx + 1][i, j].Value = board[i, j].Value;
                                }
                            }
                            CheckLeftJumpForBlack(board, row, col, ref move_matrix_buffer_black, indexx);
                            CheckRightJumpForBlack(board, row, col, ref move_matrix_buffer_black, indexx + 1);
                        }
                        else
                        {
                            CheckLeftJumpForBlack(board, row, col, ref move_matrix_buffer_black, indexx);
                            CheckRightJumpForBlack(board, row, col, ref move_matrix_buffer_black, indexx);
                        }
                        //CheckRightJumpForWhite(board, row, col, ref board2);

                        //CheckLeftJumpForWhite(board, row, col, ref board2);

                    }
                }
            //showBoard(board2);  
        }


        private static void CheckLeftMovesForBlack(ChessField[,] board, int row, int col) //checks left moves for black checkers piece
        {
            if (board[row - 1, col - 1].Value == (int)Player.WhiteSquare)
            {
                MovesBody oneMove = new MovesBody(
                    "[" + row.ToString() + "," + col.ToString() + "]",
                    "[" + (row - 1).ToString() + "," + (col - 1).ToString() + "]",
                    1,
                    "Black"
                    );
                move_buffer_black.Add(oneMove);
                //move_buffer_black.Add("[" + row.ToString() + "," + col.ToString() + "] > " + "[" + (row - 1).ToString() + "," + (col - 1).ToString() + "]");
            }
        }

        private static void CheckRightMovesForBlack(ChessField[,] board, int row, int col) //checks right moves for black checkers piece
        {
            if (board[row - 1, col + 1].Value == (int)Player.WhiteSquare)
            {
                MovesBody oneMove = new MovesBody(
                    "[" + row.ToString() + "," + col.ToString() + "]",
                    "[" + (row - 1).ToString() + "," + (col + 1).ToString() + "]",
                    1,
                    "Black"
                    );
                move_buffer_black.Add(oneMove);
                //move_buffer_black.Add("[" + row.ToString() + "," + col.ToString() + "] > " + "[" + (row - 1).ToString() + "," + (col + 1).ToString() + "]");
            }
        }



        private static void CheckRightJumpForBlack(ChessField[,] board, int row, int col, ref List<ChessField[,]> jumpList, int indexx) //checks valid right-hand jump
        {
            if (row >= 2)
            {
                if (board[row - 1, col + 1].Value >= (int)Player.White &&
                    board[row - 1, col + 1].Value == (int)Player.White &&
                    board[row - 2, col + 2].Value < 2)
                {
                    //board[row, col] = 9;
                    //jump_buffer_white.Add("[" + row.ToString() + "," + col.ToString() + "] > " + "[" + (row + 2).ToString() + "," + (col + 2).ToString() + "]");
                    if (jumpList == null) { jumpList = new List<ChessField[,]>(); }
                    jumpList[indexx][row - 1, col + 1].Value = 1;
                    jumpList[indexx][row, col].Value = 1;
                    jumpList[indexx][row - 2, col + 2].Value = 2;
                    if ((col + 2) < 2 /*&& board[row, col] == (int)Player.BlackMen*/)
                        CheckRightJumpForBlack(board, row - 2, col + 2, ref move_matrix_buffer_black, indexx);
                    if ((col + 2) > 5 /*&& board[row, col] == (int)Player.BlackMen*/)
                        CheckRightJumpForBlack(board, row - 2, col + 2, ref move_matrix_buffer_black, indexx);
                    if ((col + 2) > 1 && (col + 2) < 6 /*&& board[row, col] == (int)Player.BlackMen*/)
                    {
                        if (CheckLeftJumpForBlack2(jumpList[indexx], row - 2, col + 2) == true && CheckRightJumpForBlack2(jumpList[indexx], row - 2, col + 2) == true)
                        {
                            indexxxBlack++;
                            var newFields = ChessField.GetEmptyFields();
                            move_matrix_buffer_black.Add(newFields);
                            for (int i = 0; i < 8; i++)
                            {
                                for (int j = 0; j < 8; j++)
                                {
                                    move_matrix_buffer_black[indexxxBlack][i, j].Value = jumpList[indexx][i, j].Value;
                                }
                            }
                            CheckLeftJumpForBlack(board, row - 2, col + 2, ref move_matrix_buffer_black, indexx);
                            CheckRightJumpForBlack(board, row - 2, col + 2, ref move_matrix_buffer_black, indexxxBlack);
                        }
                        else
                        {
                            CheckLeftJumpForBlack(board, row - 2, col + 2, ref move_matrix_buffer_black, indexx);
                            CheckRightJumpForBlack(board, row - 2, col + 2, ref move_matrix_buffer_black, indexx);
                        }

                    }
                }
            }
        }

        private static void CheckLeftJumpForBlack(ChessField[,] board, int row, int col, ref List<ChessField[,]> jumpList, int indexx) //check valid left-hand jump
        {
            if (row >= 2)
            {
                if (board[row - 1, col - 1].Value >= (int)Player.White &&
                    board[row - 1, col - 1].Value == (int)Player.White &&
                    board[row - 2, col - 2].Value < 2)
                {
                    //board[row, col] = 9;
                    //jump_buffer_black.Add("[" + row.ToString() + "," + col.ToString() + "] > " + "[" + (row + 2).ToString() + "," + (col - 2).ToString() + "]");
                    jumpList[indexx][row - 1, col - 1].Value = 1;
                    jumpList[indexx][row, col].Value = 1;
                    jumpList[indexx][row - 2, col - 2].Value = 2;
                    if ((col - 2) < 2 /*&& board[row, col] == (int)Player.BlackMen*/)
                        CheckRightJumpForBlack(board, row - 2, col - 2, ref move_matrix_buffer_black, indexx);
                    if ((col - 2) > 5 /*&& board[row, col] == (int)Player.BlackMen*/)
                        CheckLeftJumpForBlack(board, row - 2, col - 2, ref move_matrix_buffer_black, indexx);
                    if ((col - 2) > 1 && (col - 2) < 6 /*&& board[row, col] == (int)Player.BlackMen*/)
                    {
                        if (CheckRightJumpForBlack2(jumpList[indexx], row - 2, col - 2) == true && CheckRightJumpForBlack2(jumpList[indexx], row - 2, col - 2) == true)
                        {
                            indexxxBlack++;
                            var newFields = ChessField.GetEmptyFields();
                            move_matrix_buffer_black.Add(newFields);
                            for (int i = 0; i < 8; i++)
                            {
                                for (int j = 0; j < 8; j++)
                                {
                                    move_matrix_buffer_black[indexxxBlack][i, j].Value = jumpList[indexx][i, j].Value;
                                }
                            }
                            CheckLeftJumpForBlack(board, row - 2, col - 2, ref move_matrix_buffer_black, indexx);
                            CheckRightJumpForBlack(board, row - 2, col - 2, ref move_matrix_buffer_black, indexxxBlack);
                        }
                        else
                        {
                            CheckLeftJumpForBlack(board, row - 2, col - 2, ref move_matrix_buffer_black, indexx);
                            CheckRightJumpForBlack(board, row - 2, col - 2, ref move_matrix_buffer_black, indexx);
                        }

                    }

                }
            }
        }

        private static void CheckLeftMovesForBlack(ChessField[,] board, int row, int col, ref ChessField[,] board2, ref bool ruch) //checks left moves for black checkers piece
        {
            if (board[row - 1, col - 1].Value == (int)Player.WhiteSquare)
            {
                ruch = true;
                board2[row, col].Value = 1;
                board2[row - 1, col - 1].Value = 2;
                //showBoard(board2);
                //move_matrix_buffer_white.Add(board2);
                //move_buffer_white.Add("[" + row.ToString() + "," + col.ToString() + "] > " + "[" + (row + 1).ToString() + "," + (col - 1).ToString() + "]");
            }
        }

        private static void CheckRightMovesForBlack(ChessField[,] board, int row, int col, ref ChessField[,] board2, ref bool ruch) //checks right moves for white checkers piece
        {
            if (board[row - 1, col + 1].Value == (int)Player.WhiteSquare)
            {
                ruch = true;
                board2[row, col].Value = 1;
                board2[row - 1, col + 1].Value = 2;
                //showBoard(board2);
                //move_matrix_buffer_white.Add(board2);
                //move_buffer_black.Add("[" + row.ToString() + "," + col.ToString() + "] > " + "[" + (row + 1).ToString() + "," + (col + 1).ToString() + "]");
            }
        }

        private static void CheckMovesForBlack(ChessField[,] board) //checks moves for black checkers pieces
        {
            //move_buffer_white = new List<string>();
            for (int row = 1; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    bool ruch = false;
                    ChessField[,] board2 = ChessField.GetEmptyFields();
                    for (int i = 0; i < 8; i++)
                    {
                        for (int j = 0; j < 8; j++)
                        {
                            board2[i, j].Value = board[i, j].Value;
                        }
                    }
                    if (board[row, col].Value == (int)Player.Black)
                    {
                        if (col == 0)
                        {
                            CheckRightMovesForBlack(board, row, col, ref board2, ref ruch);
                            if (ruch == true)
                            {
                                //showBoard(board2);
                                if (move_matrix_buffer_black == null) { move_matrix_buffer_black = new List<ChessField[,]>(); }
                                move_matrix_buffer_black.Add(board2);
                                ruch = false;
                            }
                        }
                        else if (col == 7)
                        {
                            CheckLeftMovesForBlack(board, row, col, ref board2, ref ruch);
                            if (ruch == true)
                            {
                                //showBoard(board2);
                                if (move_matrix_buffer_black == null) { move_matrix_buffer_black = new List<ChessField[,]>(); }
                                move_matrix_buffer_black.Add(board2);
                                ruch = false;
                            }
                        }
                        else
                        {
                            CheckLeftMovesForBlack(board, row, col, ref board2, ref ruch);
                            if (ruch == true)
                            {
                                //showBoard(board2);
                                if (move_matrix_buffer_black == null) { move_matrix_buffer_black = new List<ChessField[,]>(); }
                                move_matrix_buffer_black.Add(board2);
                                ruch = false;
                                board2 = ChessField.GetEmptyFields();
                                for (int i = 0; i < 8; i++)
                                {
                                    for (int j = 0; j < 8; j++)
                                    {
                                        board2[i, j].Value = board[i, j].Value;
                                    }
                                }
                            }
                            CheckRightMovesForBlack(board, row, col, ref board2, ref ruch);
                            if (ruch == true)
                            {
                                //showBoard(board2);
                                move_matrix_buffer_black.Add(board2);
                            }
                        }
                    }
                }
            }
        }

        public static List<ChessField[,]> RunWhite(ChessField[,] board)
        {
            //creating chessboard matrix
            //0 - black
            //1 - white


            move_matrix_buffer_white = new List<ChessField[,]>();
            //move_matrix_buffer_black = new List<ChessField[,]>();

            var newFields = ChessField.GetEmptyFields();
            move_matrix_buffer_white.Add(newFields);
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    move_matrix_buffer_white[0][i, j].Value = board[i, j].Value;
                }
            }

            //var newFields2 = ChessField.GetEmptyFields();
            //move_matrix_buffer_black.Add(newFields2);
            //for (int i = 0; i < 8; i++)
            //{
            //    for (int j = 0; j < 8; j++)
            //    {
            //        move_matrix_buffer_black[0][i, j].Value = board[i, j].Value;
            ////    }
            //}

            CheckJumpsForWhite(board, 0);

            if (move_matrix_buffer_white.Count == 1 && move_matrix_buffer_white[0] == board)
            {
                move_matrix_buffer_white.Clear();
                CheckMovesForWhite(board);
            }

            //if (move_matrix_buffer_black.Count == 1 && move_matrix_buffer_black[0] == board)
            //{
            //    move_matrix_buffer_black.Clear();
            //    CheckMovesForBlack(board);
            //}
            return move_matrix_buffer_white;
        }
    }
}