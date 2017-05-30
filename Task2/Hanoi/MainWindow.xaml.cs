using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
        #region Global Parameters

        private double canvasBottomMargin = 0;
        private double rectangleHeight = 0;
        private double rectangleStartWidth = 0;
        private int discAmount = 0;

        private System.Threading.Tasks.Task t = null;
        private CancellationTokenSource ts = null;
        private  CancellationToken ct;

        private Canvas startCanvas = null;

        private bool inputGesture;
        private List<Point> gesturePositions;

        #endregion

        #region Initialization

        public MainWindow()
        {
            InitializeComponent();
            initializeGame();       
        }

        /// <summary>
        /// Initializes the game.
        /// </summary>
        private void initializeGame()
        {
            rectangleHeight = 20;
            rectangleStartWidth = 150;
            canvasBottomMargin = 10;
            discAmount = 3;
            inputGesture = false;
            gesturePositions = new List<Point>();
            
            resetGame();
        }

        #endregion


        #region Interaction Handling

        /// <summary>
        /// Handles the Click event of ButtonReset to reset the game.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void button_Reset_Click(object sender, RoutedEventArgs e)
        {
            resetGame();
        }


        /// <summary>
        /// Handles the Click event of ButtonSolve to automatically solve the game (if not already in progress).
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void button_Solve_Click(object sender, RoutedEventArgs e)
        {
            if (MidCanvas.Children.Count == 0 && RightCanvas.Children.Count == 0 && t == null)
            {
                ts = new CancellationTokenSource();
                ct = ts.Token;

                t = new System.Threading.Tasks.Task(() =>
                {
                    solveAlgorithm(discAmount, LeftCanvas, RightCanvas, MidCanvas);
                }, ct);

                t.Start();
            }
            else
            {
                MessageBox.Show("Reset the Game first!");
            }
           
        }

        /// <summary>
        /// Handles the ValueChanged event of the Slider.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedPropertyChangedEventArgs{System.Double}"/> instance containing the event data.</param>
        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            this.discAmount = (int) e.NewValue;
            resetGame();
        }

        /// <summary>
        /// Handles the Click event of the canvas.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
        private void canvas_Click(Object sender, MouseButtonEventArgs e)
        {
            Canvas sentCanvas = sender as Canvas;

            if (this.startCanvas == null)
            {
                if (sentCanvas.Children.Count > 0)
                    this.startCanvas = sentCanvas;
            }
            else
            {
                moveDisc(sender as Canvas);
                this.startCanvas = null;
            }

        }

        #endregion

        #region Game Handling

        /// <summary>
        /// Starts the game.
        /// </summary>
        private void startGame()
        {
            double width = rectangleStartWidth;

            double top = LeftCanvas.Height - rectangleHeight - canvasBottomMargin;

            for (double x = 0; x < this.discAmount; x++)
            {
                Rectangle rect = new Rectangle();
                rect.Height = rectangleHeight;
                rect.Width = width;
                double left = LeftCanvas.Width * 0.5 - width * 0.5;
                width = width * 0.85;
                rect.Fill = new SolidColorBrush(Colors.Black);
                rect.Name = "disc"+ ((discAmount - x)).ToString();

                Canvas.SetLeft(rect, left);
                Canvas.SetTop(rect, top);
                top = top - rect.Height;

                LeftCanvas.Children.Add(rect);
            }

            Console.WriteLine("\n Start Game with " + discAmount + " discs.");
        }

        /// <summary>
        /// Resets the game.
        /// </summary>
        private void resetGame()
        {
            Console.WriteLine("\n Reset Game.");
            if (t != null && !t.IsCompleted)
            {
                ts.Cancel();
                ts = null;
                ct = new CancellationToken();
                t = null;
            }

                LeftCanvas.Children.Clear();
                MidCanvas.Children.Clear();
                RightCanvas.Children.Clear();
                this.startCanvas = null;

                inputGesture = false;
                startGame();  
        }

        #endregion


        #region Manual Game Handling


        /// <summary>
        /// Moves the disc from global startCanvas to targetCanvas.
        /// </summary>
        /// <param name="targetCanvas">The target Canvas.</param>
        /// <param name="top">The top.</param>
        /// <param name="left">The left.</param>
        private void moveDisc(Canvas targetCanvas)
        {
            if (targetCanvas != null && this.startCanvas!=null && this.startCanvas != targetCanvas)
            {
                if (startCanvas.Children.Count > 0) {
                    Rectangle startRect = this.startCanvas.Children[this.startCanvas.Children.Count - 1] as Rectangle;

                    double rectTop = targetCanvas.Height - canvasBottomMargin - rectangleHeight;

                    if (targetCanvas.Children.Count > 0)
                    {
                        Rectangle nachRect = targetCanvas.Children[targetCanvas.Children.Count - 1] as Rectangle;

                        if (startRect.Width < nachRect.Width)
                        {
                            rectTop = rectTop - (targetCanvas.Children.Count * rectangleHeight);
                        }
                        else
                        {
                            MessageBox.Show("Kein gültiger Zug!");
                            return;
                        }

                    }
                    
                    this.startCanvas.Children.Remove(startRect);
                    Canvas.SetTop(startRect, rectTop);
                    targetCanvas.Children.Add(startRect);

                    if (RightCanvas.Children.Count == this.discAmount)
                    {
                        MessageBox.Show("Juhu, du hast gewonnen!");
                    }
                }
            }

        }
        #endregion

        #region Automated Game Handling

        /// <summary>
        /// Solves the Hanoi algorithm in a parallel thread automatically.
        /// </summary>
        /// <param name="n">Disc amount n.</param>
        /// <param name="strtCanvas">The start canvas.</param>
        /// <param name="tarCanvas">The target canvas.</param>
        /// <param name="othrCanvas">The extra canvas.</param>
        private void solveAlgorithm(double n, Canvas strtCanvas, Canvas tarCanvas, Canvas othrCanvas)
        {
            if (n == 1)
            {
               this.Dispatcher.Invoke(
                    new Action(() =>
                    {
                        if (ts != null && !ts.IsCancellationRequested)
                        {
                            startCanvas = strtCanvas;
                            Rectangle startRect = strtCanvas.Children[strtCanvas.Children.Count - 1] as Rectangle;
                            Console.WriteLine("\n Move " + startRect.Name + " from rod " + strtCanvas.Name + " to rod" + tarCanvas.Name);
                            moveDisc(tarCanvas);
                        }
                        else return;
                       
                    }));
            }
            else
            {
                solveAlgorithm(n - 1, strtCanvas, othrCanvas, tarCanvas);

                this.Dispatcher.Invoke(
                   new Action(() =>
                   {
                       if (ts != null && !ts.IsCancellationRequested)
                       {
                           startCanvas = strtCanvas;
                           Rectangle startRect = strtCanvas.Children[strtCanvas.Children.Count - 1] as Rectangle;
                           Console.WriteLine("\n Move " + startRect.Name + " from " + strtCanvas.Name + " to " + tarCanvas.Name);
                           moveDisc(tarCanvas);
                       }
                       else return;
                   }));

                solveAlgorithm(n - 1, othrCanvas, tarCanvas, strtCanvas);
            }       
        }

        #endregion

        #region Gesture Handling
        private void Window_MouseMove(object sender, MouseEventArgs e)
        {

            if(inputGesture && e != null){
                IInputElement iE = hanoiGrid;
                Point pos = e.GetPosition(iE);
                gesturePositions.Add(pos);
            }
                 
        }

        //Register gesture input, input start point
        private void Window_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            inputGesture = true;
            IInputElement iE = hanoiGrid;
            gesturePositions.Add(e.GetPosition(iE));
        }

        private void Window_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            inputGesture = false;
            List<double> gestureAngles = calculateAngles();
            String gestureClass = classifyGesture(gestureAngles);

            // Write angles List to xml for templating
            (new System.Xml.Serialization.XmlSerializer(gestureAngles.GetType())).Serialize(new System.IO.StreamWriter(@"c:\temp\template.xml"), gestureAngles);

            gesturePositions.Clear();
        }

        private List<double> calculateAngles()
        {
            List<double> gestureAngles = new List<double>();

            for (int i = 0; i < gesturePositions.Count - 1; i++)
            {
                // A
                Point p1 = gesturePositions[0];
                // B (Mitte)
                Point p2 = gesturePositions[i];
                // C
                Point p3 = gesturePositions[i+1];

                double a, b, c;
                // Anliegend an Mitte: a und b
                a = Math.Sqrt(Math.Pow((p1.X - p2.X), 2) + Math.Pow((p1.Y - p2.Y), 2));
                b = Math.Sqrt(Math.Pow((p2.X - p3.X), 2) + Math.Pow((p2.Y - p3.Y), 2));
                c = Math.Sqrt(Math.Pow((p3.X - p1.X), 2) + Math.Pow((p3.Y - p1.Y), 2));

                double angle = Math.Acos((Math.Pow(a, 2) + Math.Pow(b, 2) - Math.Pow(c, 2))/ (2 *  a * b));
                gestureAngles.Add(RadToDeg(angle));
            }
            
            //1 mittlere
            //sqrt((P1x - P2x)2 + (P1y - P2y)2)
            //arccos((P122 + P132 - P232) / (2 * P12 * P13))

            return gestureAngles;
        }

        private List<double> TemplateOne()
        {
            return new List<double>();
        }

        private List<double> TemplateTwo()
        {
            return new List<double>();
        }

        private List<double> TemplateThree()
        {
            return new List<double>();
        }

        private String classifyGesture(List<double> gestureAngles)
        {
            return "";
        }

        // Transform Radiant value to Degree
        private double RadToDeg(double rad)
        {
            return rad * (180.0 / Math.PI);
        }

        #endregion

     




    }
}
