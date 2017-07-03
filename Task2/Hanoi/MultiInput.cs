using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hanoi
{
    class MultiInput
    {
        protected FunctionType key;

        public FunctionType Key
        {
            get
            {
                return this.key;
            }
            set
            {
                if (key == FunctionType.None) key = value;
            }
        }

        protected FunctionType source;

        public FunctionType Source
        {
            get
            {
                return this.source;
            }
            set
            {
                if (source == FunctionType.None) source = value;
            }
        }

        protected FunctionType goal;

        public FunctionType Goal
        {
            get
            {
                return this.goal;
            }
            set
            {
                if (goal == FunctionType.None) goal = value;
            }
        }

        public event EventHandler<MultiInputEventArgs> SlotInput;

        public MultiInput()
        {
            this.Key = FunctionType.None;
            this.Source = FunctionType.None;
            this.Goal = FunctionType.None;
        }

        internal void fillSlot(FunctionType functionType)
        {
            MultiInputEventArgs args = new MultiInputEventArgs();

            if (Key == FunctionType.None && (functionType == FunctionType.Put || functionType == FunctionType.Close))
            {
                    Key = functionType;
                    args.slotNumber = 0;
                    args.resultFunction = Key;
                    args.feedback = "Combined Input: " + Key.ToString();
                    OnSlotInput(args);
            }
            else if (Key == FunctionType.Close && functionType == FunctionType.CloseEnd)
            {
                Goal = functionType;
                args.slotNumber = 2;
                args.resultFunction = FunctionType.CloseEnd;
                args.feedback = "Combined Input: " + Key.ToString();
                OnSlotInput(args);
            }
            else if (Key == FunctionType.Put && functionTypeIsCanvas(functionType))
            {
                 if (Source == FunctionType.None){
                    Source = functionType;
                    args.slotNumber = 1;
                    args.feedback = "Combined Input: " + Key.ToString() + " " + Source.ToString();
                    args.resultFunction = Source;
                    OnSlotInput(args);
                }

                else if (Source != FunctionType.None && Goal == FunctionType.None)
                {
                    Goal = functionType;
                    args.slotNumber = 2;
                    args.resultFunction = Goal;
                    args.feedback = "Combined Input: " + Key.ToString() + " " + Source.ToString() + " to " + Goal.ToString();
                    OnSlotInput(args);
                }
            }
        }


        private bool functionTypeIsCanvas(FunctionType functionType)
        {
            bool isCanvas = false;

            if ((functionType == FunctionType.Canvas1 || functionType == FunctionType.Canvas2 || functionType == FunctionType.Canvas3)) isCanvas = true;

            return isCanvas;
        }

        protected virtual void OnSlotInput(MultiInputEventArgs e)
        {
            EventHandler<MultiInputEventArgs> handler = SlotInput;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }

    public class MultiInputEventArgs: EventArgs
    {
        public int slotNumber;
        public FunctionType resultFunction;
        public string feedback;
    }
}
