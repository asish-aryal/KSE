using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;

namespace Kinect_SE_Tool
{
    class SkeletonHistoryManager
    {
        int skeletonsInHistory;

        private Dictionary<JointNames.Joints, List<Joint>> jointHistoryList;

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


        public SkeletonHistoryManager(int itemsInHistory)
        {
            skeletonsInHistory = itemsInHistory;
            jointHistoryList = new Dictionary<JointNames.Joints, List<Joint>>();
            addToDictionary();
        }

        private void addToDictionary()
        { 
            jointHistoryList.Add(JointNames.Joints.Head, head);

            jointHistoryList.Add(JointNames.Joints.ShoulderCenter, centerShoulder);
            jointHistoryList.Add(JointNames.Joints.ShoulderLeft, leftShoulder);
            jointHistoryList.Add(JointNames.Joints.ShoulderRight, rightShoulder);

            jointHistoryList.Add(JointNames.Joints.ElbowLeft, leftElbow);
            jointHistoryList.Add(JointNames.Joints.ElbowRight, rightElbow);

            jointHistoryList.Add(JointNames.Joints.HandLeft, leftHand);
            jointHistoryList.Add(JointNames.Joints.HandRight, rightHand);

            jointHistoryList.Add(JointNames.Joints.WristLeft, leftWrist);
            jointHistoryList.Add(JointNames.Joints.WristRight, rightWrist);

            jointHistoryList.Add(JointNames.Joints.Spine, spine);

            jointHistoryList.Add(JointNames.Joints.HipCenter, centerHip);
            jointHistoryList.Add(JointNames.Joints.HipRight, rightHip);
            jointHistoryList.Add(JointNames.Joints.HipLeft, leftHip);

            jointHistoryList.Add(JointNames.Joints.KneeLeft, leftKnee);
            jointHistoryList.Add(JointNames.Joints.KneeRight, rightKnee);

            jointHistoryList.Add(JointNames.Joints.AnkleLeft, leftAnkle);
            jointHistoryList.Add(JointNames.Joints.AnkleRight, rightAnkle);

            jointHistoryList.Add(JointNames.Joints.FootLeft, leftFoot);
            jointHistoryList.Add(JointNames.Joints.FootRight, rightFoot);
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

        public List<Joint> getJointHistory(JointNames.Joints jointName)
        {
            return jointHistoryList[jointName];
        }

        public void clearHistory()
        {
            foreach (KeyValuePair<JointNames.Joints, List<Joint>> jointList in jointHistoryList)
            {
                jointList.Value.Clear();
            }

            //head.Clear();
            //rightShoulder.Clear();
            //leftShoulder.Clear();
            //centerShoulder.Clear();
            //leftWrist.Clear();
            //rightWrist.Clear();
            //leftHand.Clear();
            //rightHand.Clear();
            //leftElbow.Clear();
            //rightElbow.Clear();
            //spine.Clear();
            //leftHip.Clear();
            //rightHip.Clear(); ;
            //centerHip.Clear();
            //rightKnee.Clear();
            //leftKnee.Clear();
            //rightAnkle.Clear();
            //leftAnkle.Clear();
            //rightFoot.Clear();
            //leftFoot.Clear();
            
        }

        public void addToHistory(Skeleton skeleton)
        {
            if (skeleton == null) return;

            if (head.Count >= skeletonsInHistory)
            { removeOldestFromHistory(); }

            head.Add(skeleton.Joints[JointType.Head]);
            
            rightShoulder.Add(skeleton.Joints[JointType.ShoulderRight]);
            leftShoulder.Add(skeleton.Joints[JointType.ShoulderLeft]);
            centerShoulder.Add(skeleton.Joints[JointType.ShoulderCenter]);

            rightElbow.Add(skeleton.Joints[JointType.ElbowRight]);
            leftElbow.Add(skeleton.Joints[JointType.ElbowLeft]);
            
            rightHand.Add(skeleton.Joints[JointType.HandRight]);
            leftHand.Add(skeleton.Joints[JointType.HandLeft]);
            
            rightWrist.Add(skeleton.Joints[JointType.WristRight]);
            leftWrist.Add(skeleton.Joints[JointType.WristLeft]);
            
            spine.Add(skeleton.Joints[JointType.Spine]);
            
            rightHip.Add(skeleton.Joints[JointType.HipRight]);
            centerHip.Add(skeleton.Joints[JointType.HipCenter]);
            leftHip.Add(skeleton.Joints[JointType.HipLeft]);

            rightKnee.Add(skeleton.Joints[JointType.KneeRight]);
            leftKnee.Add(skeleton.Joints[JointType.KneeLeft]);

            rightAnkle.Add(skeleton.Joints[JointType.AnkleRight]);
            leftAnkle.Add(skeleton.Joints[JointType.AnkleLeft]);

            rightFoot.Add(skeleton.Joints[JointType.FootRight]);
            leftFoot.Add(skeleton.Joints[JointType.FootLeft]);
        
        }

        private void removeOldestFromHistory()
        {
            foreach (KeyValuePair<JointNames.Joints, List<Joint>> jointList in jointHistoryList)
            {
                jointList.Value.RemoveAt(0);
            }
            //head.RemoveAt(0);
            //centerShoulder.RemoveAt(0);
            //rightShoulder.RemoveAt(0);
            //leftShoulder.RemoveAt(0);
            //rightElbow.RemoveAt(0);
            //leftElbow.RemoveAt(0);
            //rightHand.RemoveAt(0);
            //leftHand.RemoveAt(0);
            //rightWrist.RemoveAt(0);
            //leftWrist.RemoveAt(0);
            //spine.RemoveAt(0);
            //rightHip.RemoveAt(0);
            //leftHip.RemoveAt(0);
            //centerHip.RemoveAt(0);
            //rightKnee.RemoveAt(0);
            //leftKnee.RemoveAt(0);
            //rightAnkle.RemoveAt(0);
            //leftAnkle.RemoveAt(0);
            //rightFoot.RemoveAt(0);
            //leftFoot.RemoveAt(0);
        }
    }
}
