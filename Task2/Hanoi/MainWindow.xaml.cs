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
        public MainWindow()
        {
            InitializeComponent();
            // Array Discs = new Array()
            
        }

        public int counter = 0;
        public double discAmount = 3;
        public Canvas startCanvas;

        private void button_Start_Click(object sender, RoutedEventArgs e)
        {
            double width = 150;
            double top = LeftCanvas.Height - 20 - 10;
            for (double x = 0; x < this.discAmount; x++)
            {
                Rectangle rect = new Rectangle();
                rect.Height = 20;
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

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            this.discAmount = e.NewValue;
        }

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
            if (this.startCanvas != nach)
            {
                if ( startCanvas.Children.Count > 0 ) {
                    Rectangle startRect = this.startCanvas.Children[this.startCanvas.Children.Count - 1] as Rectangle;

                    if (nach.Children.Count > 0)
                    {
                        Rectangle nachRect = nach.Children[nach.Children.Count - 1] as Rectangle;

                        if (startRect.Width < nachRect.Width)
                        {
                            this.startCanvas.Children.Remove(startRect);
                            Canvas.SetTop(startRect, nach.Height - nach.Children.Count * 20 - 20 - 10);
                            nach.Children.Add(startRect);
                        }
                        else
                        {
                            MessageBox.Show("Kein gültiger Zug!");
                        }
                    }
                    else
                    {
                        this.startCanvas.Children.Remove(startRect);
                        Canvas.SetTop(startRect, nach.Height - nach.Children.Count * 20 - 20 - 10);
                        nach.Children.Add(startRect);
                    }
                }
            }

            // Sieg
            if (RightCanvas.Children.Count == this.discAmount)
            {
                MessageBox.Show("Juhu, du hast gewonnen!");
            }
        }
    }
}
