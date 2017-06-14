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
using System.Xml.Linq;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using System.Speech;
using System.Speech.Recognition;
using System.Globalization;
using System.Drawing;

namespace Hanoi
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        #region Global Parameters

        private bool discAmountChanged = false;
        private double canvasBottomMargin = 0;
        private double rectangleHeight = 0;
        private double rectangleStartWidth = 0;
        private int discAmount = 0;
        private int inputFeedbackRows = 0;

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

            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");


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
            setInputFeedback("Button: Reset");
            setMessageBox("You reseted the game.");

            resetGame();
        }


        /// <summary>
        /// Handles the Click event of ButtonSolve to automatically solve the game (if not already in progress).
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void button_Solve_Click(object sender, RoutedEventArgs e)
        {
            setInputFeedback("Button: Solve");
            solveGame();           
        }

        /// <summary>
        /// Handles the ValueChanged event of the Slider.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedPropertyChangedEventArgs{System.Double}"/> instance containing the event data.</param>
        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int newDiscAmount = (int) e.NewValue;

            if(this.discAmount != newDiscAmount){

                this.discAmount = newDiscAmount;
                discAmountChanged = true;
                resetGame();
            }       
        }

        private void Slider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {

            if (discAmount > 0)
            {
                if (discAmountChanged)
                {
                    setInputFeedback("Disc Amount: " + discAmount);
                    setMessageBox("You changed the disc amount to " + discAmount + ".");
                    discAmountChanged = false;
                }              
            }
        }
                      

        /// <summary>
        /// Handles the Click event of the canvas.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
        private void canvas_Click(Object sender, MouseButtonEventArgs e)
        {
            Canvas sentCanvas = sender as Canvas;

            setInputFeedback("Click: " + sentCanvas.Name);

            handleCanvasInput(sentCanvas);
          
        }

        #endregion

        #region Canvas Handling 

        private void handleCanvasInput(Canvas canvas)
        {
            if (canvas != null)
            {
                if (this.startCanvas == null)
                {
                    if (canvas.Children.Count > 0)
                        setStartCanvas(canvas);
                    else
                    {
                        setMessageBox(canvas.Name + " has no discs!");
                    }
                }
                else
                {
                    moveDisc(canvas);
                    resetStartCanvas();
                }
            }

        }

        private void setStartCanvas(Canvas canvas)
        {
            if (canvas != null)
            {
                this.startCanvas = canvas;
                Brush oldback = this.startCanvas.Background;
                this.startCanvas.Background = new SolidColorBrush(Colors.Green);
            }
        }

        private void resetStartCanvas()
        {
            if (startCanvas != null)
            {

                this.startCanvas.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFF0F0F0"));

                this.startCanvas = null;
            }
        }

        #endregion

        #region InfoFeedback Handling

        private void setInputFeedback(string lastInput){
            if (lastInput != null)
            {
                InputFeedbackOld.Content = InputFeedbackNew.Content + "\n" + InputFeedbackOld.Content;
                inputFeedbackRows = inputFeedbackRows + 1;


                InputFeedbackNew.Content = "[" + inputFeedbackRows + "] " + lastInput;
                
            }
            
        }

        private void setMessageBox(string text)
        {
            Messages.Content = text;
            //Messages.ForeColor = color;
        }

        #endregion

        #region Game Handling

        /// <summary>
        /// Starts the game.
        /// </summary>
        private void startGame()
        {
            double width = rectangleStartWidth;

            double top = Canvas1.Height - rectangleHeight - canvasBottomMargin;

            for (double x = 0; x < this.discAmount; x++)
            {
                Rectangle rect = new Rectangle();
                rect.Height = rectangleHeight;
                rect.Width = width;
                double left = Canvas1.Width * 0.5 - width * 0.5;
                width = width * 0.85;
                rect.Fill = new SolidColorBrush(Colors.Black);
                rect.Name = "disc"+ ((discAmount - x)).ToString();

                Canvas.SetLeft(rect, left);
                Canvas.SetTop(rect, top);
                top = top - rect.Height;

                Canvas1.Children.Add(rect);
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

                Canvas1.Children.Clear();
                Canvas2.Children.Clear();
                Canvas3.Children.Clear();
                this.startCanvas = null;

                inputGesture = false;
                startGame();  
        }

        private void solveGame()
        {
            if (Canvas2.Children.Count == 0 && Canvas3.Children.Count == 0 && t == null)
            {
                ts = new CancellationTokenSource();
                ct = ts.Token;

                t = new System.Threading.Tasks.Task(() =>
                {
                    solveAlgorithm(discAmount, Canvas1, Canvas3, Canvas2);
                }, ct);

                t.Start();
            }
            else
            {
                setMessageBox("Reset the Game first!");
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
                            setMessageBox("No valid move!");
                            return;
                        }

                    }
                    
                    this.startCanvas.Children.Remove(startRect);
                    Canvas.SetTop(startRect, rectTop);
                    targetCanvas.Children.Add(startRect);
                    setMessageBox("Moved Disc from Canvas " + this.startCanvas.Name + " to " + targetCanvas.Name + ".");

                    if (Canvas3.Children.Count == this.discAmount)
                    {
                        setMessageBox("You won the game!");
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

            int templateNumber = classifyGesture(gestureAngles);

            Canvas canvas = null;
            Console.Write(templateNumber);

            switch (templateNumber)
            {
                case 1: canvas = Canvas1; 
                    setInputFeedback("Gesture: 1");
                    break;
                case 2: canvas = Canvas2; 
                    setInputFeedback("Gesture: 2");
                    break;
                case 3: canvas = Canvas3; 
                    setInputFeedback("Gesture: 3");
                    break;
            }

            

            handleCanvasInput(canvas);
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
                if (i > 1)
                {
                    // A
                    Point p1 = gesturePositions[0];
                    // B (Mitte)
                    Point p2 = gesturePositions[i];
                    // C
                    Point p3 = gesturePositions[i + 1];

                    double a, b, c;
                    // Anliegend an Mitte: a und b
                    a = Math.Sqrt(Math.Pow((p1.X - p2.X), 2) + Math.Pow((p1.Y - p2.Y), 2));
                    b = Math.Sqrt(Math.Pow((p2.X - p3.X), 2) + Math.Pow((p2.Y - p3.Y), 2));
                    c = Math.Sqrt(Math.Pow((p3.X - p1.X), 2) + Math.Pow((p3.Y - p1.Y), 2));

                    //1 mittlere
                    //sqrt((P1x - P2x)2 + (P1y - P2y)2)
                    //arccos((P122 + P132 - P232) / (2 * P12 * P13))

                    double angle = Math.Acos((Math.Pow(a, 2) + Math.Pow(b, 2) - Math.Pow(c, 2)) / (2 * a * b));

                    gestureAngles.Add(RadToDeg(angle));
                }
               
            }
            
            //Creating Templates in template1,2,3.xml here
            //MyClass.SerializeObject(gestureAngles, "template3.xml");

            return gestureAngles;
        }

      

        private int classifyGesture(List<double> gestureAngles)
        {

            double localDistanceToTemplate1 = calculateLocalDistance(gestureAngles, templateOne);
            double localDistanceToTemplate2 = calculateLocalDistance(gestureAngles, templateTwo);
            double localDistanceToTemplate3 = calculateLocalDistance(gestureAngles, templateThree);

            int i = 0;
            if (localDistanceToTemplate1 < localDistanceToTemplate2 && localDistanceToTemplate1 < localDistanceToTemplate3) i = 1;
            else if (localDistanceToTemplate2 < localDistanceToTemplate1 && localDistanceToTemplate2 < localDistanceToTemplate3) i = 2;
            else i = 3;

            return i;
        }

        private double calculateLocalDistance(List<double> gestureAngles, List<double> templateAngles)
        {


            double[,] distances = new double[gestureAngles.Count, templateAngles.Count];

            //DTW Distances
            //d(i,j) =... {}

            for (int i = 0; i < distances.GetLength(0); i++)
            {
                for (int j = 0; j < distances.GetLength(1); j++)
			    {
                    double angleDistance = 0;

                    if (Math.Abs(gestureAngles[i] - templateAngles[j]) < Math.PI) angleDistance = ((1 / Math.PI) * Math.Abs(gestureAngles[i] - templateAngles[j]));
                    else angleDistance = ((1 / Math.PI) * ((2 * Math.PI) - Math.Abs(gestureAngles[i] - templateAngles[j])));

                    distances[i, j] = Math.Abs(angleDistance);

			    }
            }

            //Local Distances 
            //C(i, j) = min(C(i, j -1),2×C(i -1, j -1),C(i -1, j))+ d(i, j)

            double[,] localDistances = new double[gestureAngles.Count, templateAngles.Count];

            for (int i = 0; i < localDistances.GetLength(0); i++)
            {
                for (int j = 0; j < localDistances.GetLength(1); j++)
                {
                    double[] localMinimums = new double[3];

                    if (j == 0 && i == 0) {
                        localMinimums[0] = 0;
                        localMinimums[1] = 0;
                        localMinimums[2] = 0;
                    }
                    else if (j == 0 && i != 0){
                        localMinimums[0] = 0;
                        localMinimums[1] = 0;
                        localMinimums[2] = localDistances[i - 1, j];
                    }
                    else if (i == 0 && j != 0)
                    {
                        localMinimums[0] = localDistances[i, j - 1];
                        localMinimums[1] = 0;
                        localMinimums[2] = 0;
                    }
                    else
                    {
                        localMinimums[0] = localDistances[i, j - 1];
                        localMinimums[1] = 2 * localDistances[i - 1, j - 1];
                        localMinimums[2] = localDistances[i - 1, j];

                    }

                        double localMinimum = Math.Min((Math.Min(localMinimums[0], localMinimums[1])), localMinimums[2]);
                        localDistances[i, j] = localMinimum + distances[i, j];
                    

                }
                
            }

            //Normalizing
            //C(i,j)* 1/N+M

            double normalizerVal = (1.0 / ((double) (localDistances.GetLength(0) + localDistances.GetLength(1))));

            double distanceSum = 0;

            for (int i = 0; i < localDistances.GetLength(0); i++)
            {
                for (int j = 0; j < localDistances.GetLength(1); j++)
                {
                    distanceSum = distanceSum + localDistances[i, j];
                }
            }

            distanceSum = distanceSum * normalizerVal;

            return (distanceSum);
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

            CultureInfo ci = new CultureInfo("de-DE");
 
                //NEEDS TO SWITCH TO en-US
                SpeechRecognitionEngine sre = new SpeechRecognitionEngine(ci);

                //SpeechRecognizer sr = new SpeechRecognizer();
                //sre.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(onSpeechRecog);
                sre.SpeechRecognized += onSpeechRecog;

                sre.SpeechDetected += onSpeechDetection;

                Choices commands = new Choices();
                commands.Add(new string[] {
                "one",
                "two",
                "three",

               // "start",
                "reset",
                "solve",
            });

                GrammarBuilder gb = new GrammarBuilder();
                gb.Append(commands);

                // Create the Grammar instance.
                Grammar g = new Grammar(gb);

                sre.LoadGrammar(g);



                // Configure the input to the speech recognizer.
                sre.SetInputToDefaultAudioDevice();

                // Start asynchronous, continuous speech recognition.
                sre.RecognizeAsync(RecognizeMode.Multiple);

           
        }

        private void onSpeechDetection(object sender, SpeechDetectedEventArgs e)
        {
            setMessageBox("Speech input not recognized!");
        }

        private void onSpeechRecog(object sender, SpeechRecognizedEventArgs e)
        {
            setInputFeedback("Speech: " + e.Result.Text);

            switch (e.Result.Text)
            {
               // case "start":
                //    startGame();
               //     break;
                case "reset":
                    setMessageBox("You reseted the game.");
                    resetGame();
                    break;
                case "solve":
                    solveGame();
                    break;
                case "one":
                    handleCanvasInput(Canvas1);
                    break;
                case "two":
                    handleCanvasInput(Canvas2);
                    break;
                case "three":
                    handleCanvasInput(Canvas3);
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
            setMessageBox("Speech input not recognized!");
        }

        #endregion

    }


    #region XML Handling
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

#endregion
}

