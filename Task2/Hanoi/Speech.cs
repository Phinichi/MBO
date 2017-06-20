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
                "reset",
                "löse",
                "put",
                "number 1",
                "number 2",
                "number 3",
                "that",
                "there",
                "close"
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
            //setMessageBox("Speech input not recognized!");
        }

        private void onSpeechRecog(object sender, SpeechRecognizedEventArgs e)
        {
          
            SpeechEventArgs args = new SpeechEventArgs();
 

            switch (e.Result.Text)
            {
                // case "start":
                //    startGame();
                //     break;
               
                case "reset":
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
                case "put": break;
                    args.resultFunction = FunctionType.Put;
                    break;
                case "that": break;
                    args.resultFunction = FunctionType.MouseOver;
                    break;
                case "there": break;
                    args.resultFunction = FunctionType.MouseOver;
                    break;
                case "Number 1": break;
                    args.resultFunction = FunctionType.Canvas1;
                    break;
                case "Number 2": break;
                    args.resultFunction = FunctionType.Canvas2;
                    break;
                case "Number 3": break;
                    args.resultFunction = FunctionType.Canvas3;
                    break;
                case "close": break;
                    args.resultFunction = FunctionType.Close;
                    break;

                default:
                    onNotRecoq();
                    break;
            }

            OnSpeechFeedback(args);
        }

        private void onNotRecoq()
        {
            System.Speech.Synthesis.SpeechSynthesizer synthesizer = new System.Speech.Synthesis.SpeechSynthesizer();

            synthesizer.Speak("Sorry, I did not understand you.");
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
