using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckersApplication
{
    public partial class MovesJumps
    {
        //JUMPS
        public static void CheckJumpsForBlack(ChessField[,] board, int indexx) // Check jumps, Black going up
        {
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
                        else if (CheckLeftJumpForBlack2(board, row, col) == true || CheckRightJumpForBlack2(board, row, col) == true)
                        {
                            indexxxBlack++;
                            indexx++;
                            var newFields = ChessField.GetEmptyFields();
                            move_matrix_buffer_black.Add(newFields);
                            for (int i = 0; i < 8; i++)
                            {
                                for (int j = 0; j < 8; j++)
                                {
                                    move_matrix_buffer_black[indexx][i, j].Value = board[i, j].Value;
                                }
                            }
                            CheckLeftJumpForBlack(board, row, col, ref move_matrix_buffer_black, indexx);
                            CheckRightJumpForBlack(board, row, col, ref move_matrix_buffer_black, indexx);
                        }
                    }
                }
        }
        public static bool CheckRightJumpForBlack2(ChessField[,] board, int row, int col)
        {
            if (row >= 2)
            {
                if (board[row - 1, col + 1].Value == (int)Player.White &&
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
                if (board[row - 1, col - 1].Value == (int)Player.White &&
                   board[row - 2, col - 2].Value < 2)
                {
                    return true;
                }
                else return false;
            }
            return false;
        }
        private static void CheckRightJumpForBlack(ChessField[,] board, int row, int col, ref List<ChessField[,]> jumpList, int indexx) //checks valid right-hand jump
        {
            if (row >= 2)
            {
                if (board[row - 1, col + 1].Value == (int)Player.White &&
                    board[row - 2, col + 2].Value < 2)
                {
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
                if (board[row - 1, col - 1].Value == (int)Player.White &&
                    board[row - 2, col - 2].Value < 2)
                {
                    jumpList[indexx][row - 1, col - 1].Value = 1;
                    jumpList[indexx][row, col].Value = 1;
                    jumpList[indexx][row - 2, col - 2].Value = 2;
                    if ((col - 2) < 2 /*&& board[row, col] == (int)Player.BlackMen*/)
                        CheckRightJumpForBlack(board, row - 2, col - 2, ref move_matrix_buffer_black, indexx);
                    if ((col - 2) > 5 /*&& board[row, col] == (int)Player.BlackMen*/)
                        CheckLeftJumpForBlack(board, row - 2, col - 2, ref move_matrix_buffer_black, indexx);
                    if ((col - 2) > 1 && (col - 2) < 6 /*&& board[row, col] == (int)Player.BlackMen*/)
                    {
                        if (CheckRightJumpForBlack2(jumpList[indexx], row - 2, col - 2) == true && CheckLeftJumpForBlack2(jumpList[indexx], row - 2, col - 2) == true)
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
        //MOVES
        private static void CheckLeftMovesForBlack(ChessField[,] board, int row, int col, ref ChessField[,] board2, ref bool ruch) //checks left moves for black checkers piece
        {
            if (board[row - 1, col - 1].Value == (int)Player.WhiteSquare)
            {
                ruch = true;
                board2[row, col].Value = 1;
                board2[row - 1, col - 1].Value = 2;
            }
        }
        private static void CheckRightMovesForBlack(ChessField[,] board, int row, int col, ref ChessField[,] board2, ref bool ruch) //checks right moves for white checkers piece
        {
            if (board[row - 1, col + 1].Value == (int)Player.WhiteSquare)
            {
                ruch = true;
                board2[row, col].Value = 1;
                board2[row - 1, col + 1].Value = 2;
            }
        }
        private static void CheckMovesForBlack(ChessField[,] board) //checks moves for black checkers pieces
        {
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
                                move_matrix_buffer_black.Add(board2);
                            }
                        }
                    }
                }
            }
        }

        //RUN
        public static List<ChessField[,]> RunBlack(ChessField[,] board)
        {
            bool bicie = false;

            move_matrix_buffer_black = new List<ChessField[,]>();

            var newFields = ChessField.GetEmptyFields();
            move_matrix_buffer_black.Add(newFields);
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    move_matrix_buffer_black[0][i, j].Value = board[i, j].Value;
                }
            }

            for (int i = 2; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (board[i, j].Value == (int)Player.Black)
                    {
                        if ((j) < 2 /*&& board[row, col] == (int)Player.BlackMen*/)
                        {
                            if (CheckRightJumpForBlack2(board, i, j) == true)
                                bicie = true;
                        }
                        else if ((j) > 5 /*&& board[row, col] == (int)Player.BlackMen*/)
                        {
                            if (CheckLeftJumpForBlack2(board, i, j) == true)
                                bicie = true;
                        }
                        else if (j > 1 && j < 6 /*&& board[row, col] == (int)Player.BlackMen*/)
                        {
                            if (CheckRightJumpForBlack2(board, i, j) == true || CheckLeftJumpForBlack2(board, i, j) == true)
                                bicie = true;
                        }
                    }
                }
            }

            CheckJumpsForBlack(board, 0);

            if (bicie == false)
            {
                move_matrix_buffer_black.Clear();
                CheckMovesForBlack(board);
            }

            return move_matrix_buffer_black;
        }
    }
}
