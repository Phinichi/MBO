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
                if (key == null) key = value;
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
                if (source == null) source = value;
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
                if (goal == null) goal = value;
            }
        }

        public MultiInput(FunctionType key)
        {
            this.Key = key;
        }
    }
}
