using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Xml;

namespace Kinect_Explorer
{
    class File_handler
    {

        public File_handler()
        { }


        public string browse_for_file()
        {
            Microsoft.Win32.OpenFileDialog fle = new Microsoft.Win32.OpenFileDialog();
            fle.FileName = "classes.xml";
            fle.Filter = "Text documents (.xml)|*.xml";

            Nullable<bool> res = fle.ShowDialog();
            if (res == true)
            {
                // Open document
                return fle.FileName;
            }
            else
            {
                return null;
            }
        }

        public Package_ get_root_package(String path_to_file)
        {
            //XmlTextReader reader = new XmlTextReader(path_to_file);
            XmlDocument doc = new XmlDocument();
            doc.Load(path_to_file);

            foreach (XmlNode node in doc)
            {
                if (node.Name == "javadoc")
                {
                    return traverse_nodes(node.ChildNodes, "root");
                }
            }

            return null;
            
        }


        private Package_ traverse_nodes(XmlNodeList nodes, String node_name)
        {
            //node.Attributes.Item(0).Name
            Package_ package = new Package_(node_name);
            foreach (XmlNode node in nodes)
            {
                if (node.Name.ToUpper() == "PACKAGE")
                {
                    package.add_child(traverse_nodes(node.ChildNodes, node.Attributes.Item(0).Value));
                }
                else if((node.Name.ToUpper() == "CLASS") || (node.Name.ToUpper() == "INTERFACE"))
                {
                    package.add_child(get_classifier_info(node));
                    //package.add_child(new Class_(node.Attributes["name"].Value));
                }
            }
            return package;
        }

        private Classifier_ get_classifier_info(XmlNode node)
        {
            Classifier_ classifier;
            if (node.Name.ToUpper() == "CLASS")
            { classifier = new Classifier_(node.Attributes["name"].Value, ClassifierType.CLASS);}
            
            else if (node.Name.ToUpper() == "INTERFACE")
            {  classifier = new Classifier_(node.Attributes["name"].Value, ClassifierType.INTERFACE);}
            else
            {  return null;}

            if ((node.Attributes["visibility"] == null) || (node.Attributes["visibility"].Value == ""))
            { classifier.Visibility = Top_Level_Visibility.PACKAGE_PRIVATE; }
            else if (node.Attributes["visibility"].Value.ToUpper() == "PUBLIC")
            { classifier.Visibility = Top_Level_Visibility.PUBLIC;}



            foreach (XmlNode child_node in node.ChildNodes)
            {
                if (child_node.Name.ToUpper() == "FIELD")
                {   
                    Field_ field = new Field_(child_node.Attributes["name"].Value, child_node.Attributes["type"].Value);

                    if (child_node.Attributes["visibility"] != null)
                    {
                        if (child_node.Attributes["visibility"].Value.ToUpper() == "PRIVATE")
                        { field.Visibility = Member_Level_Visibility.PRIVATE; }
                        else if (child_node.Attributes["visibility"].Value.ToUpper() == "PUBLIC")
                        { field.Visibility = Member_Level_Visibility.PUBLIC; }
                        else if (child_node.Attributes["visibility"].Value.ToUpper() == "PROTECTED")
                        { field.Visibility = Member_Level_Visibility.PROTECTED; }
                    }

                    if (child_node.Attributes["isstatic"] != null)
                    {
                        if (child_node.Attributes["isstatic"].Value == "false")
                        { field.IsStatic = false; }
                        else
                        { field.IsStatic = true; }
                    }
                    classifier.add_field(field);
                }

                if (child_node.Name.ToUpper() == "METHOD")
                {
                    Method_ method = new Method_(child_node.Attributes["name"].Value,child_node.Attributes["returntype"].Value );

                    if (child_node.Attributes["visibility"] != null)
                    {
                        if (child_node.Attributes["visibility"].Value.ToUpper() == "PRIVATE")
                        { method.Visibility = Member_Level_Visibility.PRIVATE; }
                        else if (child_node.Attributes["visibility"].Value.ToUpper() == "PUBLIC")
                        { method.Visibility = Member_Level_Visibility.PUBLIC; }
                        else if (child_node.Attributes["visibility"].Value.ToUpper() == "PROTECTED")
                        { method.Visibility = Member_Level_Visibility.PROTECTED; }
                    }

                    if (child_node.Attributes["isstatic"] != null)
                    {
                        if (child_node.Attributes["isstatic"].Value == "false")
                        { method.IsStatic = false; }
                        else
                        { method.IsStatic = true; }
                    }

                    foreach (XmlNode parameter in child_node.ChildNodes)
                    {
                        if (parameter.Name.ToUpper() == "PARAMETER")
                        {
                            method.add_parameter( new Variable_(parameter.Attributes["name"].Value,parameter.Attributes["type"].Value));
                        }
                    
                    }

                    classifier.add_method(method);
                }
                

            }

            return classifier;
        }

    }
}
