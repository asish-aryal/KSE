using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;

namespace Kinect_Explorer
{
    class SkeletonHistoryManager
    {
        int skeletonsInHistory;

        private Dictionary<JointType, List<Joint>> jointHistoryList;

        private List<Joint> rightHand = new List<Joint>();
        private List<Joint> leftHand = new List<Joint>();
        private List<Joint> head = new List<Joint>();
        private List<Joint> leftHip = new List<Joint>();
        private List<Joint> rightHip = new List<Joint>();
        private List<Joint> centerHip = new List<Joint>();
        private List<Joint> leftAnkle = new List<Joint>();
        private List<Joint> rightAnkle = new List<Joint>();
        private List<Joint> leftElbow = new List<Joint>();
        private List<Joint> rightElbow = new List<Joint>();
        private List<Joint> leftFoot = new List<Joint>();
        private List<Joint> rightFoot = new List<Joint>();
        private List<Joint> leftKnee = new List<Joint>();
        private List<Joint> rightKnee = new List<Joint>();
        private List<Joint> leftShoulder = new List<Joint>();
        private List<Joint> rightShoulder = new List<Joint>();
        private List<Joint> centerShoulder = new List<Joint>();
        private List<Joint> spine = new List<Joint>();
        private List<Joint> leftWrist = new List<Joint>();
        private List<Joint> rightWrist = new List<Joint>();


        public SkeletonHistoryManager(int seconds)
        {
            skeletonsInHistory = seconds*30;
            jointHistoryList = new Dictionary<JointType, List<Joint>>();
            createDictionary();
        }

        
        private void  createDictionary()
        { 
            jointHistoryList.Add(JointType.Head, head);

            jointHistoryList.Add(JointType.ShoulderCenter, centerShoulder);
            jointHistoryList.Add(JointType.ShoulderLeft, leftShoulder);
            jointHistoryList.Add(JointType.ShoulderRight, rightShoulder);

            jointHistoryList.Add(JointType.ElbowLeft, leftElbow);
            jointHistoryList.Add(JointType.ElbowRight, rightElbow);

            jointHistoryList.Add(JointType.HandLeft, leftHand);
            jointHistoryList.Add(JointType.HandRight, rightHand);

            jointHistoryList.Add(JointType.WristLeft, leftWrist);
            jointHistoryList.Add(JointType.WristRight, rightWrist);

            jointHistoryList.Add(JointType.Spine, spine);

            jointHistoryList.Add(JointType.HipCenter, centerHip);
            jointHistoryList.Add(JointType.HipRight, rightHip);
            jointHistoryList.Add(JointType.HipLeft, leftHip);

            jointHistoryList.Add(JointType.KneeLeft, leftKnee);
            jointHistoryList.Add(JointType.KneeRight, rightKnee);

            jointHistoryList.Add(JointType.AnkleLeft, leftAnkle);
            jointHistoryList.Add(JointType.AnkleRight, rightAnkle);

            jointHistoryList.Add(JointType.FootLeft, leftFoot);
            jointHistoryList.Add(JointType.FootRight, rightFoot);
        }

        public Boolean IsReady
        {
            get
            {
                if (head.Count >= skeletonsInHistory)
                { return true; }
                else
                { return false; }
            }
        }

        public Joint getJoint(JointType jointName, double seconds)
        {
            return jointHistoryList[jointName][skeletonsInHistory - (int)(seconds*30) -1];
        }

        public void clearHistory()
        {
            foreach (KeyValuePair<JointType, List<Joint>> jointList in jointHistoryList)
            {
                jointList.Value.Clear();
            }
            
        }

        public void addToHistory(Skeleton skeleton)
        {
            if (skeleton == null) return;

            if (head.Count >= skeletonsInHistory)
            { removeOldestFromHistory(); }

            foreach (Joint joint in skeleton.Joints)
            { 
                jointHistoryList[joint.JointType].Add(skeleton.Joints[joint.JointType]);
            }
        
        }

        private void removeOldestFromHistory()
        {
            foreach (KeyValuePair<JointType, List<Joint>> jointList in jointHistoryList)
            {
                jointList.Value.RemoveAt(0);
            }
        }
    }
}
