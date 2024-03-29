﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinect_Explorer
{
    public class Classifier_
    {
        private string name;
        private List<Method_> methods;
        private Package_ parent;
        private Top_Level_Visibility visibility;
        private ClassifierType classifierType;
        private List<Field_> fields;

        public Top_Level_Visibility Visibility
        {
            get { return visibility; }
            set { visibility = value; }
        }


        public string Name
        {
            get { return name; }
        }

        public ClassifierType ClassiferType
        {
            get { return classifierType; }
        }

        public List<Field_> Fields
        {
            get { return fields; }
        }

        public List<Method_> Methods
        {
            get { return methods; }
        }

        public Classifier_(String name, ClassifierType type)
        {
            this.name = name;
            this.classifierType = type;
            this.methods = new List<Method_>();
            this.fields = new List<Field_>();

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
