using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinect_Explorer
{
    //public enum Method_Visibility { PUBLIC, PRIVATE, PROTECTED };

    public class Method_
    {
        private String name;
        private bool method_is_static = true;
        private Member_Level_Visibility method_visibility = Member_Level_Visibility.PRIVATE;
        private String returnType = "void";
        private bool isSynchronized = false;
        private bool isAbstract = false;
        private List<Variable_> parameters = new List<Variable_>();

        public String Name
        {
            get { return name; }
        }

        public Boolean IsStatic
        {
            get { return this.method_is_static; }
            set { this.method_is_static = value; }
        }

        public Boolean IsAbstract
        {
            get { return this.isAbstract; }
            set { this.isAbstract = value; }
        }

        public Member_Level_Visibility Visibility
        {
            get { return method_visibility; }
            set { method_visibility = value; }
        }

        public String ReturnType
        {
            get { return returnType; }
            set { returnType = value; }
        }

        public bool IsSynchronized
        {
            get { return isSynchronized; }
            set { isSynchronized = value; }
        }

        public List<Variable_> Parameters
        {
            get { return parameters; }
        }




        public Method_(String name, String return_type)
        {
            this.name = name;
            this.ReturnType = return_type;
        }

        public Method_(String name, String return_type, Member_Level_Visibility visibility)
        {
            this.name = name;
            this.ReturnType = return_type;
            this.method_visibility = visibility;
        }

        public void add_parameter(Variable_ parameter)
        {
            parameters.Add(parameter);
        }

    }
}
