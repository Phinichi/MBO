using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hanoi
{
    class MultiInput
    {
        private FunctionType key;

        private FunctionType source;

        public FunctionType Source
        {
            get
            {
                return this.source;
            }
            set
            {
                if (source == null) source = value;
            }
        }

        private FunctionType goal;

        public FunctionType Goal
        {
            get
            {
                return this.goal;
            }
            set
            {
                if (goal == null) goal = value;
            }
        }

        public MultiInput(FunctionType key)
        {
            this.key = key;
        }
    }
}
