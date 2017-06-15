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
        public static void CheckJumpsForWhite(ChessField[,] board, int indexx) // Check jumps, White going down
        {
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
                if (board[row + 1, col + 1].Value == (int)Player.Black &&
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
                if (board[row + 1, col - 1].Value == (int)Player.Black &&
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
                if (board[row + 1, col + 1].Value == (int)Player.Black &&
                    board[row + 2, col + 2].Value < 2)
                {
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
                if (board[row + 1, col - 1].Value == (int)Player.Black &&
                    board[row + 2, col - 2].Value < 2)
                {
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

        //MOVES
        private static void CheckLeftMovesForWhite(ChessField[,] board, int row, int col, ref ChessField[,] board2, ref bool ruch) //checks left moves for black checkers piece
        {
            if (board[row + 1, col - 1].Value == (int)Player.WhiteSquare)
            {
                ruch = true;
                board2[row, col].Value = 1;
                board2[row + 1, col - 1].Value = 3;
            }
        }
        private static void CheckRightMovesForWhite(ChessField[,] board, int row, int col, ref ChessField[,] board2, ref bool ruch) //checks right moves for white checkers piece
        {
            if (board[row + 1, col + 1].Value == (int)Player.WhiteSquare)
            {
                ruch = true;
                board2[row, col].Value = 1;
                board2[row + 1, col + 1].Value = 3;
            }
        }
        private static void CheckMovesForWhite(ChessField[,] board) //checks moves for black checkers pieces
        {
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
                                move_matrix_buffer_white.Add(board2);
                            }
                        }
                    }
                }
            }
        }

        //RUN
        public static List<ChessField[,]> RunWhite(ChessField[,] board)
        {
            bool bicie = false;

            move_matrix_buffer_white = new List<ChessField[,]>();

            var newFields = ChessField.GetEmptyFields();
            move_matrix_buffer_white.Add(newFields);
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    move_matrix_buffer_white[0][i, j].Value = board[i, j].Value;
                }
            }

            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (board[i, j].Value == (int)Player.White)
                    {
                        if ((j) < 2 /*&& board[row, col] == (int)Player.BlackMen*/)
                        {
                            if (CheckRightJumpForWhite2(board, i, j) == true)
                                bicie = true;
                        }
                        else if ((j) > 5 /*&& board[row, col] == (int)Player.BlackMen*/)
                        {
                            if (CheckLeftJumpForWhite2(board, i, j) == true)
                                bicie = true;
                        }
                        else if (j > 1 && j < 6 /*&& board[row, col] == (int)Player.BlackMen*/)
                        {
                            if (CheckRightJumpForWhite2(board, i, j) == true || CheckLeftJumpForWhite2(board, i, j) == true)
                                bicie = true;
                        }
                    }
                }
            }

            CheckJumpsForWhite(board, 0);

            if (bicie == false)
            {
                move_matrix_buffer_white.Clear();
                CheckMovesForWhite(board);
            }
            return move_matrix_buffer_white;
        }
    }
}
