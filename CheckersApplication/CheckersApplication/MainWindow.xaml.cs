using System;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Threading;
using System.Windows.Interop;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using System.Text;
using System.Collections.ObjectModel;

using Emgu.CV;
using Emgu.CV.UI;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Emgu.CV.Features2D;
using Emgu.CV.CvEnum;

using DirectShowLib;

namespace CheckersApplication
{
    /// <summary>
    /// Interaction logic for CameraCapture.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Camera camera;
        ChessBoardState chessBoardState = new ChessBoardState();
        public static  System.Windows.Media.Color player1Color = new System.Windows.Media.Color();
        public static  System.Windows.Media.Color player2Color = new System.Windows.Media.Color();
        public static bool player1Detected = false;
        public static bool player2Detected = false;
        public static List<ChessField[,]> buffer_move_white;
        public static List<ChessField[,]> buffer_move_black;
        public bool goodMove;
        private bool ifInitComponent;
        int currentMove = 0;
        int shownMove = 0;

        public MainWindow()
        {
            goodMove = false;
            ifInitComponent = false;
            InitializeComponent();
            ifInitComponent = true;
            DiscoverUsbCameras();
            InitValuePickers();
            CvInvoke.UseOpenCL = (bool)CB_OpenCL.IsChecked;
            this.ChessBoard.ItemsSource = chessBoardState.piecesObservable;
        }

        private void InitValuePickers() //set values for white
        {
            //maybe use in future
        }

        public void DiscoverUsbCameras()
        {
            List<KeyValuePair<int, string>> ListCamerasData = new List<KeyValuePair<int, string>>();
            //-> Find systems cameras with DirectShow.Net dll
            DsDevice[] _SystemCamereas = DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice);
            int _DeviceIndex = 0;
            foreach (DirectShowLib.DsDevice _Camera in _SystemCamereas)
            {
                CO_Cameras.Items.Add(new KeyValuePair<int, string>(_DeviceIndex, _Camera.Name));
                _DeviceIndex++;
            }
            if (CO_Cameras.Items.Count > 0)
                CO_Cameras.SelectedIndex = 0;
        }

        public void updateFrames(object sender, EventArgs e)
        {
            try
            {
                camera.imageViewer.Image = camera.capture.QueryFrame();
                var image = new Image<Bgr, Byte>(camera.imageViewer.Image.Bitmap);
                Detect(cameraCapture: image);
            }
            catch (Exception ex)
            {
                ComponentDispatcher.ThreadIdle -= new EventHandler(updateFrames);
                System.Windows.MessageBox.Show(ex.Message);
                ChangeBtnStartStop();
            }
        }

        private void ChangeBtnStartStop()
        {
            BT_Start.IsEnabled = !BT_Start.IsEnabled;
            BT_Stop.IsEnabled = !BT_Stop.IsEnabled;
        }

        public void CameraShow()
        {
            ComponentDispatcher.ThreadIdle += new System.EventHandler(updateFrames);
        }

        private void BT_Start_Click(object sender, RoutedEventArgs e)
        {
            ChangeBtnStartStop();
            if (CB_DefaultCamera.IsChecked == true && CO_Cameras.Items.Count > 0)
                camera = new Camera(Convert.ToString(CO_Cameras.SelectedIndex));
            else
                camera = new Camera(TB_CameraSource.Text);
            CameraShow();
            CO_Cameras.IsEnabled = false;
        }

        private void BT_Stop_Click(object sender, RoutedEventArgs e)
        {
            ChangeBtnStartStop();
            ComponentDispatcher.ThreadIdle -= new EventHandler(updateFrames);
            camera.capture.Stop();
            camera.capture.Dispose();
            CO_Cameras.IsEnabled = true;
        }

        private void CB_DefaultCamera_Click(object sender, RoutedEventArgs e)
        {
            CO_Cameras.IsEnabled = !CO_Cameras.IsEnabled;
            TB_CameraSource.IsEnabled = !(bool)CB_DefaultCamera.IsChecked;
            if (!TB_CameraSource.IsEnabled)
            {
                TB_CameraSource.Text = "URL streamu, ID kamery, lub plik wideo,\nnp. (http://IP:PORT/mjpegfeed)";
                TB_CameraSource.Foreground = System.Windows.Media.Brushes.Gray;
            }
        }

        private void CB_OpenCL_Click(object sender, RoutedEventArgs e)
        {
            CvInvoke.UseOpenCL = !CvInvoke.UseOpenCL;
        }

        private void BT_ImageTest_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "Image files (*.jpg, *.png) | *.jpg; *.png";
            openFileDialog1.RestoreDirectory = true;
            DialogResult result = openFileDialog1.ShowDialog();
            string path = String.Empty;
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    Detect(openFileDialog1.FileName);
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show("Komunikat błędu: " + ex.Message);
                }
            }
        }

        private void TB_CameraSource_GotFocus(object sender, RoutedEventArgs e)
        {
            TB_CameraSource.Text = "";
            TB_CameraSource.Foreground = System.Windows.Media.Brushes.Black;
        }

        private void Detect(string filePath = null, Image<Bgr, byte> cameraCapture = null)
        {
            if (currentMove != shownMove)
                return;
            Image<Bgr, byte> image, resultImage;
            List<Color> currentColors = new List<Color>();


            if (filePath != null)
                image = new Image<Bgr, byte>(filePath).Resize(400, 400, Inter.Linear, true);
            else
                image = cameraCapture;

            resultImage = image.Copy();
            System.Drawing.Point[] points = Detection.GetRectanglePoints(image);

            if (points != null)
            {
                resultImage.Draw(points, new Bgr(Color.DarkOrange), 2);
                ChessField[,] fields = ChessField.GetChessFields(points);
                if (fields != null)
                {
                    foreach (var field in fields)
                        resultImage.Draw(field.points, new Bgr(Color.Green), 2);

                    CircleF[] circles = Detection.GetCircles(image);

                    //define color of board fields
                    fields = DifferenceBoardSquareColors(fields);
                    //

                    Mat filtring1 = new Mat();
                    Mat filtring2 = new Mat();

                    int[] minColorVal = new int[] { 3, 2, 1 };

                    int autoColorsRange = 15;

                    if (circles != null)
                    {
                        if (CB_AutoDetectColors.IsChecked == true)
                        {
                            foreach (CircleF circle in circles)
                            {
                                resultImage.Draw(circle, new Bgr(Color.Red), 3);
                                ChessField.Pons(fields, circles, (int)Player.White); //only white?!
                                chessBoardState.Clear();
                            }

                            Detection.DetectPlayersColors(circles, ref player1Color, ref player2Color, image);

                            if (player1Color.R - autoColorsRange < 0)
                                RS_Slider1R.LowerValue = 0;
                            else
                                RS_Slider1R.LowerValue = player1Color.R - autoColorsRange;

                            if (player1Color.G - autoColorsRange < 0)
                                RS_Slider1G.LowerValue = 0;
                            else
                                RS_Slider1G.LowerValue = player1Color.G - autoColorsRange;

                            if (player1Color.B - autoColorsRange < 0)
                                RS_Slider1B.LowerValue = 0;
                            else
                                RS_Slider1B.LowerValue = player1Color.B - autoColorsRange;

                            if (player1Color.R + autoColorsRange > 255)
                                RS_Slider1R.HigherValue = 255;
                            else
                                RS_Slider1R.HigherValue = player1Color.R + autoColorsRange;

                            if (player1Color.G + autoColorsRange > 255)
                                RS_Slider1G.HigherValue = 255;
                            else
                                RS_Slider1G.HigherValue = player1Color.G + autoColorsRange;

                            if (player1Color.B + autoColorsRange > 255)
                                RS_Slider1B.HigherValue = 255;
                            else
                                RS_Slider1B.HigherValue = player1Color.B + autoColorsRange;

                            CV_Player1Color_Min.Background = new System.Windows.Media.SolidColorBrush(
                                System.Windows.Media.Color.FromRgb((byte)RS_Slider1R.LowerValue, (byte)RS_Slider1G.LowerValue, (byte)RS_Slider1B.LowerValue));
                            CV_Player1Color_Max.Background = new System.Windows.Media.SolidColorBrush(
                                System.Windows.Media.Color.FromRgb((byte)RS_Slider1R.HigherValue, (byte)RS_Slider1G.HigherValue, (byte)RS_Slider1B.HigherValue));

                            System.Windows.Application.Current.Resources["Player1Color"] = CV_Player1Color_Max.Background;

                            if (player2Color.R - autoColorsRange < 0)
                                RS_Slider2R.LowerValue = 0;
                            else
                                RS_Slider2R.LowerValue = player2Color.R - autoColorsRange;

                            if (player2Color.G - autoColorsRange < 0)
                                RS_Slider2G.LowerValue = 0;
                            else
                                RS_Slider2G.LowerValue = player2Color.G - autoColorsRange;

                            if (player2Color.B - autoColorsRange < 0)
                                RS_Slider2B.LowerValue = 0;
                            else
                                RS_Slider2B.LowerValue = player2Color.B - autoColorsRange;

                            if (player2Color.R + autoColorsRange > 255)
                                RS_Slider2R.HigherValue = 255;
                            else
                                RS_Slider2R.HigherValue = player2Color.R + autoColorsRange;

                            if (player2Color.G + autoColorsRange > 255)
                                RS_Slider2G.HigherValue = 255;
                            else
                                RS_Slider2G.HigherValue = player2Color.G + autoColorsRange;

                            if (player2Color.B + autoColorsRange > 255)
                                RS_Slider2B.HigherValue = 255;
                            else
                                RS_Slider2B.HigherValue = player2Color.B + autoColorsRange;

                            CV_Player2Color_Min.Background = new System.Windows.Media.SolidColorBrush(
                                System.Windows.Media.Color.FromRgb((byte)RS_Slider2R.LowerValue, (byte)RS_Slider2G.LowerValue, (byte)RS_Slider2B.LowerValue));
                            CV_Player2Color_Max.Background = new System.Windows.Media.SolidColorBrush(
                                System.Windows.Media.Color.FromRgb((byte)RS_Slider2R.HigherValue, (byte)RS_Slider2G.HigherValue, (byte)RS_Slider2B.HigherValue));

                            System.Windows.Application.Current.Resources["Player2Color"] = CV_Player2Color_Max.Background;
                        }
                        else
                        {
                            var circleFor1 = Detection.FilterSomeColors(
                                image,
                                ref filtring1,
                                new double[] { RS_Slider1B.LowerValue, RS_Slider1G.LowerValue, RS_Slider1R.LowerValue },
                                new double[] { RS_Slider1B.HigherValue, RS_Slider1G.HigherValue, RS_Slider1R.HigherValue });
                            var circleFor2 = Detection.FilterSomeColors(
                                image,
                                ref filtring2,
                                new double[] { RS_Slider2B.LowerValue, RS_Slider2G.LowerValue, RS_Slider2R.LowerValue },
                                new double[] { RS_Slider2B.HigherValue, RS_Slider2G.HigherValue, RS_Slider2R.HigherValue });

                            chessBoardState.Clear();

                            foreach (CircleF circle in circleFor1)
                            {
                                resultImage.Draw(circle, new Bgr(Color.Green), 3);
                                ChessField.Pons(fields, circleFor1, (int)Player.Black);
                                chessBoardState.AddPieces(fields);
                            }

                            foreach (CircleF circle in circleFor2)
                            {
                                resultImage.Draw(circle, new Bgr(Color.Blue), 3);
                                ChessField.Pons(fields, circleFor2, (int)Player.White);
                                chessBoardState.AddPieces(fields);
                            }

                            IMG_Filter1.Source = ToBitmapConverter.Convert(filtring1);
                            IMG_Filter2.Source = ToBitmapConverter.Convert(filtring2);

                            //Show correct moves/jumps
                                //goodMove = VerifyMoveJumps(fields);
                            //PutMoveJumpsToDatagrid(fields);
                            //
                        }
                        
                    }
                }
            }
            IMG_Detected.Source = ToBitmapConverter.Convert(resultImage);
        }

        private ChessField[,] DifferenceBoardSquareColors(ChessField[,] fields)
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j += 2)
                {
                    if (i % 2 == 0)
                    {
                        fields[i, j].Value = (int)Player.BlackEmptySquare;
                        fields[i, j + 1].Value = (int)Player.WhiteSquare;
                    }
                    else
                    {
                        fields[i, j].Value = (int)Player.WhiteSquare;
                        fields[i, j + 1].Value = (int)Player.BlackEmptySquare;
                    }
                }
            }

            return fields;
        }
        /*
        private void PutMoveJumpsToDatagrid(ChessField[,] fields)
        {
            //Show correct moves/jumps (static methods - no change sequence of calling methods

            MovesJumps.CheckMovesForWhite(fields);
            MovesJumps.CheckJumpsForWhite(fields);
            MovesJumps.CheckMovesForBlack(fields);
            MovesJumps.CheckJumpsForBlack(fields);

            List<MovesBody> list1 = new List<MovesBody>();
            foreach (var item in MovesJumps.jump_buffer_white)
            {
                list1.Add(item);
            }
            foreach (var item in MovesJumps.move_buffer_white)
            {
                list1.Add(item);
            }
            foreach (var item in MovesJumps.jump_buffer_black)
            {
                list1.Add(item);
            }
            foreach (var item in MovesJumps.move_buffer_black)
            {
                list1.Add(item);
            }

            DG_Moves.ItemsSource = null;
            DG_Moves.ItemsSource = list1;
        }
        */

        private void CB_AutoDetectColors_Click(object sender, RoutedEventArgs e)
        {
            if (CB_AutoDetectColors.IsChecked == true)
            {
                RS_Slider1R.IsEnabled = false;
                RS_Slider2R.IsEnabled = false;
                RS_Slider1G.IsEnabled = false;
                RS_Slider2G.IsEnabled = false;
                RS_Slider1B.IsEnabled = false;
                RS_Slider2B.IsEnabled = false;
            }
            else
            {
                RS_Slider1R.IsEnabled = true;
                RS_Slider2R.IsEnabled = true;
                RS_Slider1G.IsEnabled = true;
                RS_Slider2G.IsEnabled = true;
                RS_Slider1B.IsEnabled = true;
                RS_Slider2B.IsEnabled = true;
            }
        }

        private void RangeSlider_Player1_ValueChanged(object sender, RoutedEventArgs e)
        {
            if (ifInitComponent)
            {
                LB_Range1R.Content = RS_Slider1R.LowerValue.ToString() + " - " + RS_Slider1R.HigherValue.ToString();
                LB_Range1G.Content = RS_Slider1G.LowerValue.ToString() + " - " + RS_Slider1G.HigherValue.ToString();
                LB_Range1B.Content = RS_Slider1B.LowerValue.ToString() + " - " + RS_Slider1B.HigherValue.ToString();

                if (CB_AutoDetectColors.IsChecked == false)
                { 
                    System.Windows.Media.Color minColor = System.Windows.Media.Color.FromRgb((byte)RS_Slider1R.LowerValue, (byte)RS_Slider1G.LowerValue, (byte)RS_Slider1B.LowerValue);
                    CV_Player1Color_Min.Background = new System.Windows.Media.SolidColorBrush(minColor);

                    System.Windows.Media.Color maxColor = System.Windows.Media.Color.FromRgb((byte)RS_Slider1R.HigherValue, (byte)RS_Slider1G.HigherValue, (byte)RS_Slider1B.HigherValue);
                    CV_Player1Color_Max.Background = new System.Windows.Media.SolidColorBrush(maxColor);

                    System.Windows.Application.Current.Resources["Player1Color"] = CV_Player1Color_Max.Background;
                }
            }
        }

        private void RangeSlider_Player2_ValueChanged(object sender, RoutedEventArgs e)
        {
            if (ifInitComponent)
            {
                LB_Range2R.Content = RS_Slider2R.LowerValue.ToString() + " - " + RS_Slider2R.HigherValue.ToString();
                LB_Range2G.Content = RS_Slider2G.LowerValue.ToString() + " - " + RS_Slider2G.HigherValue.ToString();
                LB_Range2B.Content = RS_Slider2B.LowerValue.ToString() + " - " + RS_Slider2B.HigherValue.ToString();

                if (CB_AutoDetectColors.IsChecked == false)
                {
                    System.Windows.Media.Color minColor = System.Windows.Media.Color.FromRgb((byte)RS_Slider2R.LowerValue, (byte)RS_Slider2G.LowerValue, (byte)RS_Slider2B.LowerValue);
                    CV_Player2Color_Min.Background = new System.Windows.Media.SolidColorBrush(minColor);

                    System.Windows.Media.Color maxColor = System.Windows.Media.Color.FromRgb((byte)RS_Slider2R.HigherValue, (byte)RS_Slider2G.HigherValue, (byte)RS_Slider2B.HigherValue);
                    CV_Player2Color_Max.Background = new System.Windows.Media.SolidColorBrush(maxColor);

                    System.Windows.Application.Current.Resources["Player2Color"] = CV_Player2Color_Max.Background;
                }
            }
        }

        private void BT_SaveMove_Click(object sender, RoutedEventArgs e)
        {
            if (currentMove == 0)
            {
                currentMove++;
                shownMove++;
                TB_MoveNr.Text = "Nr ruchu: " + (shownMove + 1).ToString() + " (bieżący)";
                List<CheckersPiece> pieces = new List<CheckersPiece>();
                foreach (var p in chessBoardState.piecesObservable)
                    pieces.Add(p.Copy());




                chessBoardState.history.Add(pieces);
            }
            else if(currentMove%2==1)
            {
                ChessField[,] board = ChessField.GetEmptyFields();
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
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                        File.AppendAllText("log13",(board[i, j].Value).ToString());
                    File.AppendAllText("log13", "\r\n");
                }

                buffer_move_white = MovesJumps.RunWhite(board);


                System.Windows.MessageBox.Show("Niepoprawny ruch.");
            }
            else
            {

            }
        }

        private void BT_GoBack_Click(object sender, RoutedEventArgs e)
        {
            if (shownMove < 1)
                return;

            TB_MoveNr.Text = "Nr ruchu: " + shownMove.ToString();
            BT_SaveMove.IsEnabled = false;
            shownMove--;
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
                TB_MoveNr.Text = "Nr ruchu: "+ (shownMove+1).ToString()+" (bieżący)";
                return;
            }

            TB_MoveNr.Text = "Nr ruchu: " + (shownMove+1).ToString();
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
                TB_MoveNr.Text = "Nr ruchu: 1 (bieżący)";
                BT_SaveMove.IsEnabled = true;
            }
        }

        private bool VerifyMoveJumps(ChessField[,] fields)
        {
            var move_matrix_buffer_white = new List<ChessField[,]>();
            var move_matrix_buffer_black = new List<ChessField[,]>();
            //MovesJumps.Run(fields, ref move_matrix_buffer_white, ref move_matrix_buffer_black);

            if (currentMove % 2 == 0) //black as first
            {
                foreach (var item in move_matrix_buffer_black)
                {
                    if (item == fields)
                        return true;
                }
            }

            if (currentMove % 2 == 1) //white
            {
                foreach (var item in move_matrix_buffer_white)
                {
                    if (item == fields)
                        return true;
                }
            }

            return false;
        }

    }

}
