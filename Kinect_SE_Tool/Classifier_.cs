using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinect_SE_Tool
{
    public class Classifier_
    {
        private string name;
        private List<Method_> methods;
        private Package_ parent;
        private Top_Level_Visibility visibility;
        private List<Field_> fields = new List<Field_>();

        public Top_Level_Visibility Visibility
        {
            get { return visibility; }
            set { visibility = value; }
        }


        public string Name
        {
            get { return name; }
        }

        public List<Field_> Fields
        {
            get { return fields; }
        }

        public List<Method_> Methods
        {
            get { return methods; }
        }

        public Classifier_(String name)
        {
            this.name = name;
            this.methods = new List<Method_>();
        }

        public Classifier_(String name, List<Method_> methods)
        {
            this.name = name;
            this.methods = methods;
        }

        public Package_ Parent
        {
            get { return parent; }
            set { parent = value; }
        }
        
        public void add_method(Method_ method)
        {
            this.methods.Add(method);
            //method.visibility = Method_.Visibilities.PRIVATE;
        }

        public void add_field(Field_ field)
        {
            fields.Add(field);
        }

        

    }
}
