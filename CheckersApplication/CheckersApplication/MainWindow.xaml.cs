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
        string fileName;

        public static bool same(ChessField[,] board, ChessField[,] board2)
        {
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                    if (board[i, j].Value != board2[i, j].Value)
                        return false;
            return true;
        }
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
