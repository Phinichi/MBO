using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Hanoi
{
    class Feedback
    {

        private Label messages = null;
        private Label infoFeedbackOld = null;
        private Label infoFeedbackNew = null;
        private int inputFeedbackRows = 0;

        public Feedback(Label messageBox, Label info1, Label info2)
        {
            messages = messageBox;
            infoFeedbackOld = info1;
            infoFeedbackNew = info2;

        }

        #region InfoFeedback Handling

        public void setInputFeedback(string lastInput)
        {
            if (lastInput != null)
            {
                infoFeedbackOld.Content = infoFeedbackNew.Content + "\n" + infoFeedbackOld.Content;
                inputFeedbackRows = inputFeedbackRows + 1;


                infoFeedbackNew.Content = "[" + inputFeedbackRows + "] " + lastInput;

            }

        }

        public void setMessageBox(string text)
        {
            messages.Content = text;
            //Messages.ForeColor = color;
        }

        #endregion
    }
}
