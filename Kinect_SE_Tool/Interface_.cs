using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinect_SE_Tool
{
    class Interface_ : Classifier_
    {
        

        public Interface_(String name)
            : base(name)
        { }

        public Interface_(String name, List<Method_> methods)
            :base(name,methods)
        { }

    }
}
