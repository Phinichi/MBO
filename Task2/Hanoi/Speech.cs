using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Speech.Recognition;
using System.Text;
using System.Threading.Tasks;

namespace Hanoi
{
    class Speech
    {
        private Feedback feedback = null;

        public event EventHandler<SpeechEventArgs> SpeechFeedback;

        public Speech(Feedback feedback)
        {
            this.feedback = feedback;
            initializeSpeechRec();
        }

        private void initializeSpeechRec()
        {

            CultureInfo ci = new CultureInfo("de-DE");

            //NEEDS TO SWITCH TO en-US
            SpeechRecognitionEngine sre = new SpeechRecognitionEngine(ci);

            //SpeechRecognizer sr = new SpeechRecognizer();
            //sre.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(onSpeechRecog);
            sre.SpeechRecognized += onSpeechRecog;

            sre.SpeechDetected += onSpeechDetection;

            //INPUT NEW CHOICES; NUMBER 1,2,3, That, There
            Choices commands = new Choices();
            commands.Add(new string[] {

                "eins",
                "zwei",
                "drei",

               // "start",
                "neustart",
                "löse",
                "bewege",
                "nummer eins",
                "nummer zwei",
                "nummer drei",
                "dies",
                "dorthin",
                "schließe"
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

            //feedback.setMessageBox("Sprache erkannt!");
            Console.WriteLine("Speech detected!");
        }

        private void onSpeechRecog(object sender, SpeechRecognizedEventArgs e)
        {
          
            SpeechEventArgs args = new SpeechEventArgs();
 
            switch (e.Result.Text)
            {
                // case "start":
                //    startGame();
                //     break;
               
                case "neustart":
                    args.resultFunction = FunctionType.Reset;
                    break;
                //NEED TO BE ENGLISH!!!!
                case "löse":
                    args.resultFunction = FunctionType.Solve;
                    break;
                case "eins":
                    args.resultFunction = FunctionType.Canvas1;
                    break;
                case "zwei":
                    args.resultFunction = FunctionType.Canvas2;
                    break;
                case "drei":
                    args.resultFunction = FunctionType.Canvas3;
                    break;

                    //MULTIINPUTS
                case "bewege": 
                    args.resultFunction = FunctionType.Put;
                    break;
                case "dies": 
                    args.resultFunction = FunctionType.MouseOver;
                    break;
                case "dorthin":
                    args.resultFunction = FunctionType.MouseOver2;
                    break;
                case "nummer eins": 
                    args.resultFunction = FunctionType.Canvas1;
                    break;
                case "nummer zwei":
                    args.resultFunction = FunctionType.Canvas2;
                    break;
                case "nummer drei":
                    args.resultFunction = FunctionType.Canvas3;
                    break;
                case "schließe": 
                    args.resultFunction = FunctionType.Close;
                    break;
                default:
                    onNotRecog();
                    break;
            }

            OnSpeechFeedback(args);
        }

        private void onNotRecog()
        {
            feedback.setMessageBox("Speech not recogniced!");
        }

        protected virtual void OnSpeechFeedback(SpeechEventArgs e)
        {
            EventHandler<SpeechEventArgs> handler = SpeechFeedback;
            if (handler != null)
            {
                handler(this, e);
            }
        }

    }

    
    public class SpeechEventArgs:EventArgs
    {
        public FunctionType resultFunction;

    }
}
