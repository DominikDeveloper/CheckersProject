using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckersPossibleMovesMatrices
{
    class Program
    {
        public enum FieldValues
        {
            BlackEmptySquare = 0,
            WhiteSquare = 1,
            BlackMen = 2,
            WhiteMen = 3
        }

        public static List<string> jump_buffer_white;
        public static List<string> jump_buffer_black;
        public static List<string> move_buffer_black;
        public static List<string> move_buffer_white;
        public static List<int[,]> jump_matrix_buffer_white;

        public static void showBoard(int[,] board) //shows current state of board
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    switch (board[i, j])
                    {
                        case (int)FieldValues.BlackEmptySquare: Console.ForegroundColor = ConsoleColor.DarkGray; break;
                        case (int)FieldValues.WhiteSquare: Console.ForegroundColor = ConsoleColor.White; break;
                        case (int)FieldValues.BlackMen: Console.ForegroundColor = ConsoleColor.Red; break;
                        case (int)FieldValues.WhiteMen: Console.ForegroundColor = ConsoleColor.Green; break;
                        default: Console.ForegroundColor = ConsoleColor.Gray; break;
                    }

                    Console.Write(board[i, j].ToString());

                    Console.ForegroundColor = ConsoleColor.Gray;
                }
                Console.WriteLine();
            }

            Console.WriteLine("########");
        }
        public static void CheckJumpsForWhite(int[,] board) // Check jumps, White going down
        {
            jump_buffer_white = new List<string>();
            jump_matrix_buffer_white = new List<int[,]>();
            




            for (int row = 0; row < 6; row++)
                for (int col = 0; col < 8; col++)
                {
                    int[,] board2 = new int[8, 8];
                    for (int i = 0; i < 8; i++)
                    {
                        for (int j = 0; j < 8; j++)
                        {
                            board2[i, j] = board[i, j];
                        }
                    }
                    if (col < 2 && board[row, col] == (int)FieldValues.WhiteMen)
                    {
                        CheckRightJumpForWhite(board, row, col, ref board2);
                        showBoard(board2);
                        jump_matrix_buffer_white.Add(board2);
                    }
                    if (col > 5 && board[row, col] == (int)FieldValues.WhiteMen)
                    {
                        CheckLeftJumpForWhite(board, row, col, ref board2);
                        showBoard(board2);
                        jump_matrix_buffer_white.Add(board2);
                    }
                    if (col > 1 && col < 6 && board[row, col] == (int)FieldValues.WhiteMen)
                    {
                        CheckRightJumpForWhite(board, row, col, ref board2);
                        CheckLeftJumpForWhite(board, row, col, ref board2);
                        showBoard(board2);
                        jump_matrix_buffer_white.Add(board2);
                    }
                }
            //showBoard(board2);  
        }
        private static void CheckRightJumpForWhite(int[,] board, int row, int col,ref int[,] board2) //checks valid right-hand jump
        {
            if (row <= 5)
            {
                if (board[row + 1, col + 1] >= (int)FieldValues.BlackMen &&
                    board[row + 1, col + 1] == (int)FieldValues.BlackMen &&
                    board[row + 2, col + 2] < 2)
                {
                    //board[row, col] = 9;
                    jump_buffer_white.Add("[" + row.ToString() + "," + col.ToString() + "] > " + "[" + (row + 2).ToString() + "," + (col + 2).ToString() + "]");
                    board2[row + 1,col + 1] = 1;
                    board2[row, col] = 1;
                    board2[row + 2, col+2] = 3;
                    if ((col + 2) < 2 /*&& board[row, col] == (int)FieldValues.BlackMen*/)
                        CheckRightJumpForWhite(board, row + 2, col + 2, ref board2);
                    if ((col + 2) > 5 /*&& board[row, col] == (int)FieldValues.BlackMen*/)
                        CheckLeftJumpForWhite(board, row + 2, col + 2, ref board2);
                    if ((col + 2) > 1 && (col + 2) < 6 /*&& board[row, col] == (int)FieldValues.BlackMen*/)
                    {
                        CheckRightJumpForWhite(board, row + 2, col + 2, ref board2);
                        CheckLeftJumpForWhite(board, row + 2, col + 2, ref board2);
                    }
                }
            }
        }

        private static void CheckLeftJumpForWhite(int[,] board, int row, int col,ref int[,] board2) //check valid left-hand jump
        {
            if (row <= 5)
            {
                if (board[row + 1, col - 1] >= (int)FieldValues.BlackMen &&
                    board[row + 1, col - 1] == (int)FieldValues.BlackMen &&
                    board[row + 2, col - 2] < 2)
                {
                    //board[row, col] = 9;
                    jump_buffer_white.Add("[" + row.ToString() + "," + col.ToString() + "] > " + "[" + (row + 2).ToString() + "," + (col - 2).ToString() + "]");
                    board2[row + 1, col - 1] = 1;
                    board2[row, col] = 1;
                    board2[row + 2, col - 2] = 3;
                    if ((col - 2) < 2 /*&& board[row, col] == (int)FieldValues.BlackMen*/)
                        CheckRightJumpForWhite(board, row + 2, col - 2, ref board2);
                    if ((col - 2) > 5 /*&& board[row, col] == (int)FieldValues.BlackMen*/)
                        CheckLeftJumpForWhite(board, row + 2, col - 2 , ref board2);
                    if ((col - 2) > 1 && (col - 2) < 6 /*&& board[row, col] == (int)FieldValues.BlackMen*/)
                    {
                        CheckRightJumpForWhite(board, row + 2, col - 2, ref board2);
                        CheckLeftJumpForWhite(board, row + 2, col - 2, ref board2);
                    }

                }
            }
        }

        private static void CheckRightJumpForWhite2(int[,] board, int row, int col)
        {
            throw new NotImplementedException();
        }

        private static void CheckLeftJumpForWhite2(int[,] board, int row, int col)
        {
            throw new NotImplementedException();
        }

        public static void CheckJumpsForBlack(int[,] board) // Check jumps, Black going up
        {
            jump_buffer_black = new List<string>();
            
            for (int row = 2; row < 8; row++)
                for (int col = 0; col < 8; col++)
                {
                    if (col < 2 && board[row, col] == (int)FieldValues.BlackMen)
                        CheckRightJumpForBlack(board, row, col);
                    if (col > 5 && board[row, col] == (int)FieldValues.BlackMen)
                        CheckLeftJumpForBlack(board, row, col);
                    if (col > 1 && col < 6 && board[row, col] == (int)FieldValues.BlackMen)
                    {
                        CheckRightJumpForBlack(board, row, col);
                        CheckLeftJumpForBlack(board, row, col);
                    }

                }
        }

        public static void CheckJumps(int[,] board)
        {
            CheckJumpsForWhite(board);
            CheckJumpsForBlack(board);
        }

        private static void CheckLeftMovesForBlack(int[,] board, int row, int col) //checks left moves for black checkers piece
        {
            if (board[row - 1, col - 1] == (int)FieldValues.WhiteSquare)
            {
                move_buffer_black.Add("[" + row.ToString() + "," + col.ToString() + "] > " + "[" + (row - 1).ToString() + "," + (col - 1).ToString() + "]");
            }
        }

        private static void CheckRightMovesForBlack(int[,] board, int row, int col) //checks right moves for black checkers piece
        {
            if (board[row - 1, col + 1] == (int)FieldValues.WhiteSquare)
            {
                move_buffer_black.Add("[" + row.ToString() + "," + col.ToString() + "] > " + "[" + (row - 1).ToString() + "," + (col + 1).ToString() + "]");
            }
        }
        private static void CheckMovesForBlack(int[,] board) //checks moves for black checkers pieces
        {
            move_buffer_black = new List<string>();
            for (int row = 1; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    if (board[row, col] == (int)FieldValues.BlackMen)
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

        private static void CheckLeftMovesForWhite(int[,] board, int row, int col) //checks left moves for black checkers piece
        {
            if (board[row + 1, col - 1] == (int)FieldValues.WhiteSquare)
            {
                move_buffer_white.Add("[" + row.ToString() + "," + col.ToString() + "] > " + "[" + (row + 1).ToString() + "," + (col - 1).ToString() + "]");
            }
        }

        private static void CheckRightMovesForWhite(int[,] board, int row, int col) //checks right moves for black checkers piece
        {
            if (board[row + 1, col + 1] == (int)FieldValues.WhiteSquare)
            {
                move_buffer_white.Add("[" + row.ToString() + "," + col.ToString() + "] > " + "[" + (row + 1).ToString() + "," + (col + 1).ToString() + "]");
            }
        }
        private static void CheckMovesForWhite(int[,] board) //checks moves for black checkers pieces
        {
            move_buffer_white = new List<string>();
            for (int row = 0; row < 7; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    if (board[row, col] == (int)FieldValues.WhiteMen)
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
        

        private static void CheckRightJumpForBlack(int[,] board, int row, int col) //checks valid right-hand jump
        {

            if (row >= 2)
            {
                if (board[row - 1, col + 1] >= (int)FieldValues.WhiteMen &&
                board[row - 1, col + 1] == (int)FieldValues.WhiteMen &&
                board[row - 2, col + 2] < 2)
                {
                    //board[row, col] = 8; //didn't test
                    jump_buffer_black.Add("[" + row.ToString() + "," + col.ToString() + "] > " + "[" + (row - 2).ToString() + "," + (col + 2).ToString() + "]");
                    if ((col + 2) < 2 /*&& board[row, col] == (int)FieldValues.BlackMen*/)
                        CheckRightJumpForBlack(board, row - 2, col + 2);
                    if ((col + 2) > 5 /*&& board[row, col] == (int)FieldValues.BlackMen*/)
                        CheckLeftJumpForBlack(board, row - 2, col + 2);
                    if ((col + 2) > 1 && (col + 2) < 6 /*&& board[row, col] == (int)FieldValues.BlackMen*/)
                    {
                        CheckRightJumpForBlack(board, row - 2, col + 2);
                        CheckLeftJumpForBlack(board, row - 2, col + 2);
                    }
                }
                //return true;
            }
            //else return false;
        }

        private static void CheckLeftJumpForBlack(int[,] board, int row, int col) //check valid left-hand jump
        {
            if (row >= 2)
            {
                if (board[row - 1, col - 1] >= (int)FieldValues.WhiteMen &&
                board[row - 1, col - 1] == (int)FieldValues.WhiteMen &&
                board[row - 2, col - 2] < 2)
                {
                    //board[row, col] = 8; //didn't test
                    jump_buffer_black.Add("[" + row.ToString() + "," + col.ToString() + "] > " + "[" + (row - 2).ToString() + "," + (col - 2).ToString() + "]");

                    if ((col - 2) < 2 /*&& board[row, col] == (int)FieldValues.BlackMen*/)
                        CheckRightJumpForBlack(board, row - 2, col - 2);
                    if ((col - 2) > 5 /*&& board[row, col] == (int)FieldValues.BlackMen*/)
                        CheckLeftJumpForBlack(board, row - 2, col - 2);
                    if ((col - 2) > 1 && (col - 2) < 6 /*&& board[row, col] == (int)FieldValues.BlackMen*/)
                    {
                        CheckRightJumpForBlack(board, row - 2, col - 2);
                        CheckLeftJumpForBlack(board, row - 2, col - 2);
                    }
                }
                //return true;
            }
            //else return false;
        }

        static void Main(string[] args)
        {
            //creating chessboard matrix
            //0 - black
            //1 - white
            int[,] board = new int[8, 8];
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j += 2)
                {
                    if (i % 2 == 0)
                    {
                        board[i, j] = (int)FieldValues.BlackEmptySquare;
                        board[i, j + 1] = (int)FieldValues.WhiteSquare;
                    }
                    else
                    {
                        board[i, j] = (int)FieldValues.WhiteSquare;
                        board[i, j + 1] = (int)FieldValues.BlackEmptySquare;
                    }
                }
            }

            showBoard(board);
            //placing checkers pieces on board
            //2 - black
            //3 - white

            //tests 
            /*board[6, 1] = 3;
            board[5, 4] = 3;
            board[7, 2] = 2;
            board[6, 5] = 2;
            board[1, 1] = 3;
            board[2, 2] = 2;*/
            //board[4, 4] = 2;

            /*
            board[3, 5] = 3;
            board[4, 6] = 2;
            board[6, 6] = 2;

            board[3, 3] = 3;
            board[4, 2] = 2;
            board[4, 4] = 2;
            */

            //board[7, 0] = 2;
            //board[7, 2] = 2;
            //board[7, 4] = 2;
            //board[7, 6] = 2;
            //board[0, 1] = 3;
            //board[0, 1] = 2;
            //board[6, 1] = 3;

            board[0, 7] = 3;
            board[1, 6] = 2;
            board[0, 3] = 3;
            board[1, 2] = 2;
            //board[7, 4] = 2;
            //board[0, 1] = 3;
            //board[2, 3] = 3;
            //board[6, 1] = 3;
            //board[4, 3] = 3;
            /*
            board[6, 6] = 3;
            board[5, 5] = 3;
            board[6, 4] = 2;
            board[7, 5] = 2;
            */
            //board[2, 6] = 2;
            //board[2, 2] = 2;

            showBoard(board);

            //Checking for jumps 9 if checkerspiece can jump //Comment of "9"  
            CheckJumps(board);

            showBoard(board);

            Console.Write("Bicia (poj.) - białe: ");
            foreach (var item in jump_buffer_white)
                Console.Write(item + " ; ");
            Console.Write("-"); Console.WriteLine();

            Console.Write("Bicia (poj.) - czarne: ");
            foreach (var item in jump_buffer_black)
                Console.Write(item + " ; ");
            Console.Write("-"); Console.WriteLine();

            CheckMovesForBlack(board);
            CheckMovesForWhite(board);

            Console.Write("Murzynskie ruchy: ");
            foreach (var item in move_buffer_black)
                Console.Write(item + " ; ");
            Console.Write("-"); Console.WriteLine();

            Console.Write("Masterrace ruchy: ");
            foreach (var item in move_buffer_white)
                Console.Write(item + " ; ");
            Console.Write("-"); Console.WriteLine();

            Console.ReadKey();
        }
    }
}