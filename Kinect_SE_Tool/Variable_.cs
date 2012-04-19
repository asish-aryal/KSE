using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinect_Explorer
{
    public class Variable_
    {
        private String name;
        private String type;


        public Variable_(String name, String type)
        {
            this.name = name;
            this.type = type;
        }

        public String Name
        {
            get { return name; }
        }

        public String Type
        {
            get { return type; }
        }


    }
}
