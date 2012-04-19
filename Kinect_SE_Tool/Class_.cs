using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinect_Explorer
{
    public class Class_ :  Classifier_
    {
        private List<Class_> children_classes = new List<Class_>();
        
        
        public List<Class_> Children
        {
            get { return children_classes; }
            private set { children_classes = value; }
        }

        public Class_(String name) 
            : base(name)
        { }

        public Class_(String name, List<Method_> methods)
            :base(name,methods)
        { }


        public void add_child_class(Class_ childClass)
        {
            children_classes.Add(childClass);
        }

    }
}
