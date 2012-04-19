using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows.Controls;

namespace Kinect_Explorer
{
    enum Gestures
    { 
        ZoomIn, ZoomOut, MovePointer
    }

    enum ApplicationStates
    { 
        SoftwareLoaded, SoftwareNotLoaded, KinectConnected, KinectNotConnected, BrowseDialogueOpen, BrowseDialogueNotOpen
    }

    enum ZoomStates
    { 
        maximumZoomReached, minimumZoomReached, zoomingAllowed
    }
    class Controller
    {


        public Controller()
        {
        }

        public Package_ get_package()
        {
            File_handler file_handler = new File_handler();
            string filename = file_handler.browse_for_file();
            if (filename != null)
            {
                //List<Classifier_> classes = file_handler.parse_file_to_classifiers(filename);
                //List<Package_> packages = file_handler.parse_file_to_packages(filename);
                return file_handler.get_root_package(filename);
                //root.add_children(classes);
                //root.add_children(packages);
                //return root;
            }
            else
            {
                return null;
            }
        }

        public void processGesture(Gestures gesture)
        {

        }



    }
}
