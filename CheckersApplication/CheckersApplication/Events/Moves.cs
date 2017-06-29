using System.Collections.Generic;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace CheckersApplication
{
    public partial class MainWindow : Window
    {
        private void SaveMove()
        {
            if (currentMove == 0)
            {
                if (chessBoardState.piecesObservable.Count <= 0)
                {
                    string monit = "Nie wykryto żadnych pionków na planszy. "
                        + "Jeżeli na planszy są rozstawione pionki, odznacz opcję wykrywania kolorów i ustaw ich kolorów używając suwaków.";
                    MessageBox.Show(monit);
                    return;
                }

                currentMove++;
                shownMove++;
                TB_MoveNr.Text = "Nr ruchu: " + (shownMove).ToString() + " (bieżący)";
                TB_Info.Text = "Rozpoczyna gracz nr 2";
                C2.Visibility = Visibility.Visible;
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
                    board[i.Pos.Y / 60, i.Pos.X / 60].Value = (int)i.Player;
                }

                List<CheckersPiece> pieces2 = new List<CheckersPiece>();
                foreach (var p in chessBoardState.piecesObservable)
                    pieces2.Add(p.Copy());

                foreach (var i in pieces2)
                {
                    board2[i.Pos.Y / 60, i.Pos.X / 60].Value = (int)i.Player;
                }

                buffer_move_white = MovesJumps.RunWhite(board);

                int whiteIter = 0; int whiteRequestIter = -1;
                foreach (var v in buffer_move_white)
                {
                    if (same(v, board2))
                    {
                        goodMove = true;
                        whiteRequestIter = whiteIter;
                    }
                    whiteIter++;
                }

                if (whiteRequestIter >= 0 && same(buffer_move_white[whiteRequestIter], board))
                {
                    buffer_move_white.RemoveAt(whiteRequestIter);
                    goodMove = false;
                }

                if (goodMove == false)
                    System.Windows.MessageBox.Show("Niepoprawny ruch.");
                else
                {

                    currentMove++;
                    shownMove++;
                    TB_MoveNr.Text = "Nr ruchu: " + (shownMove).ToString() + " (bieżący)";
                    TB_Info.Text = "Teraz kolej dla gracza 1.";
                    C2.Visibility = Visibility.Collapsed;
                    C1.Visibility = Visibility.Visible;
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
                    board[i.Pos.Y / 60, i.Pos.X / 60].Value = (int)i.Player;
                }

                List<CheckersPiece> pieces2 = new List<CheckersPiece>();
                foreach (var p in chessBoardState.piecesObservable)
                    pieces2.Add(p.Copy());

                foreach (var i in pieces2)
                {
                    board2[i.Pos.Y / 60, i.Pos.X / 60].Value = (int)i.Player;
                }

                buffer_move_black = MovesJumps.RunBlack(board);

                int blackIter = 0; int blackRequestIter = -1;
                foreach (var v in buffer_move_black)
                {
                    if (same(v, board2))
                    {
                        goodMove = true;
                        blackRequestIter = blackIter;
                    }
                    blackIter++;
                }

                if (blackRequestIter >= 0 && same(buffer_move_black[blackRequestIter], board))
                {
                    buffer_move_black.RemoveAt(blackRequestIter);
                    goodMove = false;
                }

                if (goodMove == false)
                    System.Windows.MessageBox.Show("Niepoprawny ruch.");
                else
                {

                    currentMove++;
                    shownMove++;
                    TB_MoveNr.Text = "Nr ruchu: " + (shownMove).ToString() + " (bieżący)";
                    TB_Info.Text = "Oczekiwanie na ruch gracza 2.";
                    C1.Visibility = Visibility.Collapsed;
                    C2.Visibility = Visibility.Visible;
                    chessBoardState.history.Add(pieces2);
                }
            }
        }

        private void BT_SaveMove_Click(object sender, RoutedEventArgs e)
        {
            SaveMove();
        }

        private void Key_MainWindow_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.M && (e.OriginalSource is System.Windows.Controls.TextBox) == false)
            {
                SaveMove();
            }
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
                TB_Info.Text = "1. Przygotować planszę 8x8. Wybrać kamerę, rozpocząć transmisję.\n2. Rozstawić pionki gracza 1 na dole planszy, a gracza 2 - na górze.\n3. Używając suwaków ustawić skrajne wartości kolorów pionków dla obu graczy. Jeśli wszystko jest wykrywane, kliknąć \"Wykonaj ruch.\" \n* Dla testowych zdjęć pominąć pkt. 1 i 2, wczytać plik graf.";
                C2.Visibility = Visibility.Collapsed;
                C1.Visibility = Visibility.Collapsed;
                BT_SaveMove.IsEnabled = true;
            }
        }
    }
}
