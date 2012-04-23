using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinect_Explorer
{
    public enum ClassifierType
    {
        CLASS, INTERFACE
    }

    public enum Member_Level_Visibility
    {
        PUBLIC, PRIVATE, PROTECTED 
    }

    public enum Top_Level_Visibility 
    { 
        PUBLIC, PACKAGE_PRIVATE 
    }

    public enum KinectStates
    { CONNECTED, GETTING_READY, DISCONNECTED }

    public enum SRStates
    { LISTENING, GETTING_READY, NOT_LISTENING }

    public enum GRStates
    { LOOKING, NOT_LOOKING, GETTING_READY }


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

}
