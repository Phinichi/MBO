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
using System.Linq;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using System.Speech.Recognition;

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

        private List<double> templateOne;
        private List<double> templateTwo;
        private List<double> templateThree;

        #endregion

        #region Initialization

        public MainWindow()
        {
            InitializeComponent();
            initializeTemplates();
            initializeGame();
            initializeSpeechRec();
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

        private void initializeTemplates()
        {
            templateOne = new List<double>();
            templateTwo = new List<double>();
            templateThree = new List<double>();

            MyClass.Deserialize(templateOne, @"../../template1.xml");

            MyClass.Deserialize(templateTwo, @"../../template2.xml");

            MyClass.Deserialize(templateThree, @"../../template3.xml");      
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
            solveGame();           
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

        private void solveGame()
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
                IInputElement iE = (IInputElement) sender;
                Point pos = e.GetPosition(iE);
                gesturePositions.Add(pos);
            }
                 
        }

        //Register gesture input, input start point
        private void Window_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            inputGesture = true;
            IInputElement iE = (IInputElement)sender;
            gesturePositions.Add(e.GetPosition(iE));
        }

        private void Window_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            inputGesture = false;
            List<double> gestureAngles = calculateAngles();
            //String gestureClass = classifyGesture(gestureAngles);

            // Write angles List to xml for templating
            //(new System.Xml.Serialization.XmlSerializer(gestureAngles.GetType())).Serialize(new System.IO.StreamWriter(@"\template.xml"), gestureAngles);

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

            //Creating Templates in template1,2,3.xml here
            //MyClass.SerializeObject(gestureAngles, "template3.xml");

            return gestureAngles;
        }

      

        private String classifyGesture(List<double> gestureAngles)
        {

            Array localDistancesToTemplate1 =  calculateLocalDistances(gestureAngles, templateOne);
            Array localDistancesToTemplate2 =  calculateLocalDistances(gestureAngles, templateTwo);
            Array localDistancesToTemplate3 =  calculateLocalDistances(gestureAngles, templateThree);

 
           // List<double distancesToTemplate1 = calculateDistances(localDistancesToTemplate1);
           // List<double distancesToTemplate2 = calculateDistances(localDistancesToTemplate2);
           // List<double distancesToTemplate3 = calculateDistances(localDistancesToTemplate3);

            return "";
        }

        private Array calculateLocalDistances(List<double> gestureAngles, List<double> templateAngles)
        {
            

            double[][] distances =  new double[0][];
            //ARRAY WITH ONE DIM GESTUREANGLES.COUNT AND ONE templateAngels.COUNT

            //distance = new double[gestureAngles.Count][templateAngles.Count];
            //Array lengths = Array.CreateInstance(typeof(int), 2);
            //lengths.SetValue(gestureAngles.Count, 0);
            //lengths.SetValue(templateAngles.Count, 1);

            //Array lowerBounds = Array.CreateInstance(typeof(int), 2);
            //lengths.SetValue(0, 0);
            //lengths.SetValue(0, 1);


            //Array distances = Array.CreateInstance(typeof(Int32), lengths, lowerBounds);


            for (int i = 0; i < gestureAngles.Count; i++)
            {
                for (int j = 0; j < templateAngles.Count; j++)
			    {
                    double angleDistance;

			        if (Math.Abs(gestureAngles[i] - templateOne[i]) < Math.PI) angleDistance = ((1 / Math.PI) * (gestureAngles[i] - templateOne[i]));                     
                    else angleDistance = ((1 / Math.PI) * ((2 * Math.PI) - gestureAngles[i] - templateOne[i]));
                      
                    //Normalize distance
                    angleDistance = (angleDistance * (1 / (gestureAngles.Count + templateAngles.Count)));

                    distances[i][j] = angleDistance;
                    //FILL DISTANCES
			    }
            }

            return distances;
        }

        private Array calculateDistances(Array distances)
        {

            Array distancesToTemplates = null;
                //Array.CreateInstance(int, distances., distances.GetLength(1));

            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
			    {
			        //C algorithm with min + d
			    }
            }

            return distancesToTemplates;
        }

        // Transform Radiant value to Degree
        private double RadToDeg(double rad)
        {
            return rad * (180.0 / Math.PI);
        }

        #endregion

        #region Speech Handling

        private void initializeSpeechRec()
        {
            SpeechRecognizer sr = new SpeechRecognizer();

            Choices commands = new Choices();
            commands.Add(new string[] {
                "move left to middle",
                "move left to right",
                "move middle to left",
                "move middle to right",
                "move right to left",
                "move right to middle",
                "start game",
                "reset game",
                "solve game"
            });

            GrammarBuilder gb = new GrammarBuilder();
            gb.Append(commands);

            // Create the Grammar instance.
            Grammar g = new Grammar(gb);

            sr.LoadGrammar(g);

            sr.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(onSpeechRecog);
        }

        private void onSpeechRecog(object sender, SpeechRecognizedEventArgs e)
        {
            switch (e.Result.Text)
            {
                case "start game":
                    startGame();
                    break;
                case "reset game":
                    resetGame();
                    break;
                case "solve game":
                    solveGame();
                    break;
                case "move left to middle":
                    this.startCanvas = this.LeftCanvas;
                    moveDisc(this.MidCanvas);
                    break;
                case "move left to right":
                    this.startCanvas = this.LeftCanvas;
                    moveDisc(this.RightCanvas);
                    break;
                case "move middle to left":
                    this.startCanvas = this.MidCanvas;
                    moveDisc(this.LeftCanvas);
                    break;
                case "move middle to right":
                    this.startCanvas = this.MidCanvas;
                    moveDisc(this.LeftCanvas);
                    break;
                case "move right to left":
                    this.startCanvas = this.RightCanvas;
                    moveDisc(this.LeftCanvas);
                    break;
                case "move right to middle":
                    this.startCanvas = this.RightCanvas;
                    moveDisc(this.MidCanvas);
                    break;
                default:
                    onNotRecoq();
                    break;
            }
        }

        private void onNotRecoq()
        {
            System.Speech.Synthesis.SpeechSynthesizer synthesizer = new System.Speech.Synthesis.SpeechSynthesizer();

            synthesizer.Speak("Sorry, I did not understand you.");
        }

        #endregion
    }

    public static class MyClass
    {
        public static void SerializeObject(this List<double> list, string fileName)
        {
            var serializer = new XmlSerializer(typeof(List<double>));
            using (var stream = File.OpenWrite(fileName))
            {
                serializer.Serialize(stream, list);
               
            }
            
        }

        public static void Deserialize(this List<double> list, string fileName)
        {
            var serializer = new XmlSerializer(typeof(List<double>));
            using (var stream = File.OpenRead(fileName))
            {
                var other = (List<double>)(serializer.Deserialize(stream));
                list.Clear();
                list.AddRange(other);

            }
        }
    }
}

