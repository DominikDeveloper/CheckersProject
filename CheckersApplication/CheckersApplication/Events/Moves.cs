using System.Collections.Generic;
using System.Windows;
using System.Windows.Interop;

namespace CheckersApplication
{
    public partial class MainWindow : Window
    {
        private void BT_SaveMove_Click(object sender, RoutedEventArgs e)
        {
            //ComponentDispatcher.ThreadIdle -= (updateFrames);
            if (currentMove == 0)
            {
                currentMove++;
                shownMove++;
                TB_MoveNr.Text = "Nr ruchu: " + (shownMove).ToString() + " (bieżący)";
                List<CheckersPiece> pieces = new List<CheckersPiece>();
                foreach (var p in chessBoardState.piecesObservable)
                    pieces.Add(p.Copy());




                chessBoardState.history.Add(pieces);
            }
            else if (currentMove % 2 == 1)
            {
                goodMove = false;
                ChessField[,] board = ChessField.GetEmptyFields();
                ChessField[,] board2 = ChessField.GetEmptyFields();
                List<CheckersPiece> pieces = new List<CheckersPiece>();
                pieces = chessBoardState.history[chessBoardState.history.Count - 1];

                foreach (var i in pieces)
                {
                    //if (i.Player == Player.BlackMen)
                    //    board[i.Pos.X / 60, i.Pos.Y / 60].Value = (int)Player.BlackMen;
                    //if (i.Player == Player.WhiteMen)
                    //    board[i.Pos.X / 60, i.Pos.Y / 60].Value = (int)Player.WhiteMen;
                    board[i.Pos.Y / 60, i.Pos.X / 60].Value = (int)i.Player;
                }
                //for (int i = 0; i < 8; i++)
                //{
                //    for (int j = 0; j < 8; j++)
                //        File.AppendAllText("log13", (board[i, j].Value).ToString());
                //    File.AppendAllText("log13", "\r\n");
                //}

                List<CheckersPiece> pieces2 = new List<CheckersPiece>();
                foreach (var p in chessBoardState.piecesObservable)
                    pieces2.Add(p.Copy());

                foreach (var i in pieces2)
                {
                    //if (i.Player == Player.BlackMen)
                    //    board[i.Pos.X / 60, i.Pos.Y / 60].Value = (int)Player.BlackMen;
                    //if (i.Player == Player.WhiteMen)
                    //    board[i.Pos.X / 60, i.Pos.Y / 60].Value = (int)Player.WhiteMen;
                    board2[i.Pos.Y / 60, i.Pos.X / 60].Value = (int)i.Player;
                }

                buffer_move_white = MovesJumps.RunWhite(board);

                //for (int i = 0; i < 8; i++)
                //{
                //    for (int j = 0; j < 8; j++)
                //        Console.Write(board2[i, j].Value);
                //    Console.WriteLine();
                //}

                foreach (var v in buffer_move_white)
                {
                    if (same(v, board2))
                        goodMove = true;
                }

                if (goodMove == false)
                    System.Windows.MessageBox.Show("Niepoprawny ruch.");
                else
                {

                    currentMove++;
                    shownMove++;
                    TB_MoveNr.Text = "Nr ruchu: " + (shownMove).ToString() + " (bieżący)";
                    chessBoardState.history.Add(pieces2);
                }
            }
            else
            {
                goodMove = false;
                ChessField[,] board = ChessField.GetEmptyFields();
                ChessField[,] board2 = ChessField.GetEmptyFields();
                List<CheckersPiece> pieces = new List<CheckersPiece>();
                pieces = chessBoardState.history[chessBoardState.history.Count - 1];

                foreach (var i in pieces)
                {
                    //if (i.Player == Player.BlackMen)
                    //    board[i.Pos.X / 60, i.Pos.Y / 60].Value = (int)Player.BlackMen;
                    //if (i.Player == Player.WhiteMen)
                    //    board[i.Pos.X / 60, i.Pos.Y / 60].Value = (int)Player.WhiteMen;
                    board[i.Pos.Y / 60, i.Pos.X / 60].Value = (int)i.Player;
                }
                //for (int i = 0; i < 8; i++)
                //{
                //    for (int j = 0; j < 8; j++)
                //        File.AppendAllText("log13", (board[i, j].Value).ToString());
                //    File.AppendAllText("log13", "\r\n");
                //}

                List<CheckersPiece> pieces2 = new List<CheckersPiece>();
                foreach (var p in chessBoardState.piecesObservable)
                    pieces2.Add(p.Copy());

                foreach (var i in pieces2)
                {
                    //if (i.Player == Player.BlackMen)
                    //    board[i.Pos.X / 60, i.Pos.Y / 60].Value = (int)Player.BlackMen;
                    //if (i.Player == Player.WhiteMen)
                    //    board[i.Pos.X / 60, i.Pos.Y / 60].Value = (int)Player.WhiteMen;
                    board2[i.Pos.Y / 60, i.Pos.X / 60].Value = (int)i.Player;
                }

                buffer_move_black = MovesJumps.RunBlack(board);

                //for (int i = 0; i < 8; i++)
                //{
                //    for (int j = 0; j < 8; j++)
                //        Console.Write(board2[i, j].Value);
                //    Console.WriteLine();
                //}

                foreach (var v in buffer_move_black)
                {
                    if (same(v, board2))
                        goodMove = true;
                }

                if (goodMove == false)
                    System.Windows.MessageBox.Show("Niepoprawny ruch.");
                else
                {

                    currentMove++;
                    shownMove++;
                    TB_MoveNr.Text = "Nr ruchu: " + (shownMove).ToString() + " (bieżący)";
                    chessBoardState.history.Add(pieces2);
                }
            }
            //ComponentDispatcher.ThreadIdle += (updateFrames);
        }

        private void BT_GoBack_Click(object sender, RoutedEventArgs e)
        {
            if (shownMove < 1)
                return;

            shownMove--;

            if (shownMove == 0)
                TB_MoveNr.Text = "Nr ruchu: Ustawianie pionków";
            else
                TB_MoveNr.Text = "Nr ruchu: " + shownMove.ToString();

            BT_SaveMove.IsEnabled = false;
            BT_ImageTest.IsEnabled = false;

            chessBoardState.piecesObservable.Clear();
            foreach (var p in chessBoardState.history[shownMove])
                chessBoardState.piecesObservable.Add(p.Copy());

        }

        private void BT_GoForward_Click(object sender, RoutedEventArgs e)
        {
            if (shownMove == currentMove)
                return;

            shownMove++;


            if (shownMove == currentMove)
            {
                BT_SaveMove.IsEnabled = true;
                BT_ImageTest.IsEnabled = true;
                TB_MoveNr.Text = "Nr ruchu: " + (shownMove).ToString() + " (bieżący)";
                return;
            }

            TB_MoveNr.Text = "Nr ruchu: " + (shownMove).ToString();
            chessBoardState.piecesObservable.Clear();
            foreach (var p in chessBoardState.history[shownMove])
                chessBoardState.piecesObservable.Add(p.Copy());
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = System.Windows.MessageBox.Show("Czy na pewno chcesz rozpocząć grę od nowa?\nDotychczasowy postęp zostanie utracony", "Checkers Project", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                chessBoardState.history.Clear();
                currentMove = 0;
                shownMove = 0;
                TB_MoveNr.Text = "Nr ruchu: Ustawianie pionków";
                BT_SaveMove.IsEnabled = true;
            }
        }
    }
}
