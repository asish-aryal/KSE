using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinect_SE_Tool
{
    public class Package_
    {
        private String name;
        private Package_ parent;
        private List<Classifier_> children_classifiers = new List<Classifier_>();
        private List<Package_> children_packages = new List<Package_>();

        public Package_(String name)
        { this.name = name; }



        public List<Package_> Children_Packages
        {
            get { return children_packages; }
        }

        public List<Classifier_> Children_Classifiers
        {
            get { return children_classifiers; }
        }

        public String Name
        {
            get { return name; }
        }

        public Package_ Parent
        {
            get { return parent; }
            set { parent = value; }
        }

        public void add_child(Package_ package)
        {
            package.Parent = this;
            children_packages.Add(package);
        }

        public void add_child(Classifier_ classifier)
        {
            classifier.Parent = this;
            children_classifiers.Add(classifier);
        }

        public void add_children(List<Package_> packages)
        {
            foreach (Package_ package in packages)
            { add_child(package); }
        }

        public void add_children(List<Classifier_> classifiers )
        {
            foreach (Classifier_ classifier in classifiers)
            { add_child(classifier); }
        }
        
    }
}
