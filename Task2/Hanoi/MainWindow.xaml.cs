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
using System.Timers;

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

        private System.Threading.Tasks.Task t = null;
        private CancellationTokenSource ts = null;
        private  CancellationToken ct;

        private Canvas startCanvas = null;

        private bool inputGesture;
        private List<Point> gesturePositions;

        private Gestures gesture = null;
        private Speech speech = null;
        private Feedback feedback = null;
        private MultiInput multi = null;
        private int timeToMultiInput = 5000;
        private System.Windows.Threading.DispatcherTimer timer = null;
        public int ticks = 0;
        //private System.Threading.Timer timer2 = null;

        static readonly object locker = new object();

        #endregion

        #region Initialization

        public MainWindow()
        {
            InitializeComponent();

            feedback = new Feedback(Messages, InputFeedbackOld, InputFeedbackNew);
            gesture = new Gestures();
            speech = new Speech(feedback);
            speech.SpeechFeedback += onSpeechFeedbackReceived;

            //Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

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
            handleResultFunction(FunctionType.Reset, InputType.Click);
        }

        /// <summary>
        /// Handles the Click event of ButtonSolve to automatically solve the game (if not already in progress).
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void button_Solve_Click(object sender, RoutedEventArgs e)
        {
            handleResultFunction(FunctionType.Solve, InputType.Click);          
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
                    feedback.setInputFeedback("Anzahl Scheiben: " + discAmount);
                    feedback.setMessageBox("Anzahl der Scheiben auf " + discAmount + " geändert.");
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

            switch (sentCanvas.Name)
            {
                case "Canvas1":
                    handleResultFunction(FunctionType.Canvas1, InputType.Click);
                    break;
                case "Canvas2":
                    handleResultFunction(FunctionType.Canvas2, InputType.Click);
                    break;
                case "Canvas3":
                    handleResultFunction(FunctionType.Canvas3, InputType.Click);
                    break;
                default: break;
            }       
        }

        #endregion

        #region Canvas Handling 

        private bool handleCanvasInput(Canvas canvas)
        {
            bool canvasInput = false; 

            if (canvas != null)
            {
                if (this.startCanvas == null)
                {
                    if (canvas.Children.Count > 0){
                        setStartCanvas(canvas);
                        canvasInput = true;
                    }
                        
                    else
                    {
                        feedback.setMessageBox(canvas.Name + " has no discs!");
                        canvasInput = false;
                    }
                }
                else
                {
                    canvasInput = true; 
                    moveDisc(canvas);
                    resetStartCanvas();
                }
            }

            return canvasInput;

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
                feedback.setMessageBox("Reset the Game first!");
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
                            feedback.setMessageBox("Kein gültiger Zug!");
                            return;
                        }

                    }
                    
                    this.startCanvas.Children.Remove(startRect);
                    Canvas.SetTop(startRect, rectTop);
                    targetCanvas.Children.Add(startRect);
                    feedback.setMessageBox("Scheibe " + this.startCanvas.Name + " auf " + targetCanvas.Name + "bewegt.");

                    if (Canvas3.Children.Count == this.discAmount)
                    {
                        feedback.setMessageBox("Du hast gewonnen!");
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

        #region GestureInput Handling

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

            int templateNumber = gesture.GetTemplateNumber(gesturePositions);
        
            switch (templateNumber)
            {
                case 0:
                    feedback.setMessageBox("Geste nicht erkannt, bitte nochmals versuchen.");
                    break;
                case 1:
                    handleResultFunction(FunctionType.Canvas1, InputType.Gesture);
                    break;
                case 2:
                    handleResultFunction(FunctionType.Canvas2, InputType.Gesture);
                    break;
                case 3:
                    handleResultFunction(FunctionType.Canvas3, InputType.Gesture);
                    break;
            }

            gesturePositions.Clear();
        }

        #endregion

        #region SpeechInput Handling

        private void onSpeechFeedbackReceived(object sender, SpeechEventArgs e)
        {
            if (e != null && e.resultFunction != null && !e.resultFunction.Equals(""))
            {
                Console.WriteLine(e.resultFunction.ToString());
                handleResultFunction(e.resultFunction, InputType.Speech);
            }
        }

        #endregion

        #region HelperFunctions

        private void handleResultFunction(FunctionType functionType = FunctionType.None, InputType inputType = InputType.None)
        {
                if (inputType != InputType.None)
                {
                    string feedbackInputType = "";

                    switch (inputType)
                    {
                        case InputType.Speech:
                            feedbackInputType = "Sprache: ";
                            break;
                        case InputType.Click:
                            feedbackInputType = "Klick: ";
                            break;
                        case InputType.Gesture:
                            feedbackInputType = "Geste: ";
                            break;
                        default: break;
                    }

                    feedback.setInputFeedback(feedbackInputType + functionType.ToString());
                }

                if (multi != null)
                {
                    handleMultipleInput(functionType, inputType);
                }
                else
                {

                    if (functionType != FunctionType.None)
                    {
                        switch (functionType)
                        {
                            case FunctionType.Put:
                                multi = new MultiInput();
                                multi.SlotInput += OnSlotFilled;
                                multi.fillSlot(functionType);
                                break;
                            case FunctionType.Close:
                                multi = new MultiInput();
                                multi.SlotInput += OnSlotFilled;
                                multi.fillSlot(functionType);
                                break;

                            case FunctionType.Reset:
                                feedback.setMessageBox("Das Spiel wurde neugestartet!");
                                resetGame();
                                break;
                            case FunctionType.Solve:
                                solveGame();
                                break;
                            case FunctionType.Canvas1:
                                handleCanvasInput(Canvas1);
                                break;
                            case FunctionType.Canvas2:
                                handleCanvasInput(Canvas2);
                                break;
                            case FunctionType.Canvas3:
                                handleCanvasInput(Canvas3);
                                break;

                            default:
                                feedback.setMessageBox("No valid input!");
                                break;
                        }
                    }

                }      
        }

        private void startTimer()
        {
            // create time frame for more inputs
            timer = new System.Windows.Threading.DispatcherTimer();
            timer.Tick += new EventHandler(OnTick);
            timer.Interval = new TimeSpan(0, 0, 10);
            timer.Start();
        }

        private void OnTick(object sender, EventArgs e)
        {
                stopTimer();
                multi = null;
                feedback.setMessageBox("Zeit für Multiinput abgelaufen.");
        }

        private void stopTimer()
        {
            if (timer != null)
            {
                timer.Stop();
            }     
        }

        private void resetTimer()
        {
            stopTimer();
            startTimer();
        }

        private void OnSlotFilled(object sender, MultiInputEventArgs e)
        {
            if (e != null)
            {
                if (e.resultFunction == FunctionType.CloseEnd) closeApplication();
                else 
                {
                    if (e.slotNumber == 0)
                    {
                        feedback.setMessageBox(e.feedback);
                        startTimer();
                    }
                    if (e.slotNumber == 1)
                    {
                        feedback.setMessageBox(e.feedback);
                        startCanvas = null;
                        handleCanvasInput(functionTypeCanvasToCanvas(e.resultFunction));
                        stopTimer();
                        startTimer();
        
                    }
                    else if (e.slotNumber == 2)
                    {
                        feedback.setMessageBox(e.feedback);
                        handleCanvasInput(functionTypeCanvasToCanvas(e.resultFunction));
                        stopTimer();
                        multi = null;

                    }
                }
            }
        }


        private void handleMultipleInput(FunctionType functionType, InputType inputType)
        {
                switch (multi.Key)
                {
                    case FunctionType.Close:
                        if (functionType == FunctionType.MouseOver)
                        {
                            multi.fillSlot(FunctionType.CloseEnd);
                        }
                        else
                        {
                            wrongMultiInput("No valid closing command!");
                        }
                        break;
                    case FunctionType.Put:

                        if (functionType == FunctionType.MouseOver && multi.Source == FunctionType.None)
                        {
                            functionType = getMouseOverCanvas();
                            if (functionType == FunctionType.Canvas1 || functionType == FunctionType.Canvas2 || functionType == FunctionType.Canvas3)
                            {
                                Canvas canvas = functionTypeCanvasToCanvas(functionType);
                                if (canvas.Children.Count > 0) multi.fillSlot(functionType);
                                else wrongMultiInput("Canvas has no children!");
                            }
                            else
                            {
                                wrongMultiInput("No valid Canvas selected!");
                            }

                        }
                        else if (functionType == FunctionType.Canvas1 || functionType == FunctionType.Canvas2 || functionType == FunctionType.Canvas3)
                        {
                            
                            if (multi.Source == FunctionType.None)
                            {
                                Canvas canvas = functionTypeCanvasToCanvas(functionType);
                                if (canvas.Children.Count > 0) multi.fillSlot(functionType);
                                else wrongMultiInput("Canvas has no children!");
                            }
                            else if (multi.Source != FunctionType.None && multi.Goal == FunctionType.None)
                            {
                                multi.fillSlot(functionType);
                            }
                        }

                        else if (functionType == FunctionType.MouseOver2 && multi.Source != FunctionType.None && multi.Goal == FunctionType.None)
                        {
                            functionType = getMouseOverCanvas();
                            if (functionType == FunctionType.Canvas1 || functionType == FunctionType.Canvas2 || functionType == FunctionType.Canvas3)
                            {
                                multi.fillSlot(functionType);

                            }
                            else
                            {
                                wrongMultiInput("No valid Canvas selected!");
                            }
                        }

                        else
                        {
                            wrongMultiInput("No valid command sequence!");
                            resetStartCanvas();
                        }

                        break;
                    default: break;
                }
       }

        private void wrongMultiInput(string p)
        {
            stopTimer();
            multi = null;
            feedback.setMessageBox(p);
        }

  
        private FunctionType getMouseOverCanvas()
        {
            FunctionType functionType = FunctionType.None;

            if (Canvas1.IsMouseOver) functionType = FunctionType.Canvas1; 
            else if (Canvas2.IsMouseOver) functionType = FunctionType.Canvas2; 
            else if (Canvas3.IsMouseOver) functionType = FunctionType.Canvas3;

            return functionType;
        }

        private void closeApplication()
        {
            this.Close();
        }

        private void executeMultiInputMove()
        {
            handleCanvasInput(functionTypeCanvasToCanvas(multi.Goal));
    
            multi = null;
        }

        private Canvas functionTypeCanvasToCanvas(FunctionType functionType)
        {
            Canvas canvas = null;
            switch (functionType)
            {
                case FunctionType.Canvas1:
                    canvas = Canvas1;
                    break;
                case FunctionType.Canvas2:
                    canvas =  Canvas2;
                    break;
                case FunctionType.Canvas3:
                    canvas = Canvas3;
                    break;

                default: break;
            }

            return canvas;
        }

        #endregion


    
    }

    #region Enums
    public enum FunctionType
    {
        None, Reset, Solve, Canvas1, Canvas2, Canvas3, DiscAmount, Feedback, Close, Put, CloseEnd,
        MouseOver, MouseOver2
    }

    public enum InputType
    {
        None, Click, Gesture, Speech, Multi
    }

    #endregion
}

