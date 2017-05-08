using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Hanoi
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Public Parameters
        public double canvasBottomMargin = 0;
        public double rectangleHeight = 0;
        public double discAmount = 0;
        public Canvas startCanvas = null;
        #endregion

        #region Initialization
        public MainWindow()
        {
            InitializeComponent();
            initializeGame();
        }

        #endregion

        #region Button handling

        private void button_Reset_Click(object sender, RoutedEventArgs e)
        {
            resetGame();
        }


        private void button_Solve_Click(object sender, RoutedEventArgs e)
        {
            solveAlgorithm();
        }

        #endregion

        #region Game Status Handling

        private void initializeGame()
        {
            rectangleHeight = 20;
            canvasBottomMargin = 10;
            discAmount = 3;
            startGame();
        }

        private void startGame()
        {
            double width = 150;
            double top = LeftCanvas.Height - rectangleHeight - canvasBottomMargin;

            for (double x = 0; x < this.discAmount; x++)
            {
                Rectangle rect = new Rectangle();
                rect.Height = rectangleHeight;
                rect.Width = width;
                double left = LeftCanvas.Width * 0.5 - width * 0.5;
                width = width * 0.85;
                rect.Fill = new SolidColorBrush(Colors.Black);

                Canvas.SetLeft(rect, left);
                Canvas.SetTop(rect, top);
                top = top - rect.Height;

                LeftCanvas.Children.Add(rect);
            }
        }

        private void resetGame()
        {
            LeftCanvas.Children.Clear();
            MidCanvas.Children.Clear();
            RightCanvas.Children.Clear();
            this.startCanvas = null;

            startGame();

        }

        #endregion

        #region Parameter Handling

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            this.discAmount = e.NewValue;
            resetGame();
        }

        #endregion 

        #region Manual Game Handling

        private void canvas_Click(Object sender, MouseButtonEventArgs e)
        {
            if (this.startCanvas == null) {
                this.startCanvas =  sender as Canvas;
            }
            else
            {
                moveDisc(sender as Canvas, 5, 5);
                this.startCanvas = null;
            }
            
        }

        private void moveDisc(Canvas nach, int top, int left)
        {
            if (nach != null && this.startCanvas!=null && this.startCanvas != nach)
            {
                if (startCanvas.Children.Count > 0) {
                    Rectangle startRect = this.startCanvas.Children[this.startCanvas.Children.Count - 1] as Rectangle;

                    double rectTop = nach.Height - canvasBottomMargin - rectangleHeight;

                    if (nach.Children.Count > 0)
                    {
                        Rectangle nachRect = nach.Children[nach.Children.Count - 1] as Rectangle;

                        if (startRect.Width < nachRect.Width)
                        {
                            rectTop = rectTop - (nach.Children.Count * rectangleHeight);
                        }
                        else
                        {
                            MessageBox.Show("Kein gültiger Zug!");
                            return;
                        }

                    }

                    this.startCanvas.Children.Remove(startRect);
                    Canvas.SetTop(startRect, rectTop);
                    nach.Children.Add(startRect);

                    if (RightCanvas.Children.Count == this.discAmount)
                    {
                        MessageBox.Show("Juhu, du hast gewonnen!");
                    }
                }
            }

        }
        #endregion

        #region Automated Game Handling


        private void solveAlgorithm()
        {
            throw new NotImplementedException();
        }

        #endregion


    }
}
