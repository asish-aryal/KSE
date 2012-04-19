using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinect_Explorer
{
    public class Field_ : Variable_
    {
        
        private bool method_is_static = true;
        private Member_Level_Visibility method_visibility = Member_Level_Visibility.PRIVATE;
        private bool isSynchronized = false;


        public Boolean IsStatic
        {
            get { return this.method_is_static; }
            set { this.method_is_static = value; }
        }

        public Member_Level_Visibility Visibility
        {
            get { return method_visibility; }
            set { method_visibility = value; }
        }



        public bool IsSynchronized
        {
            get { return isSynchronized; }
            set { isSynchronized = value; }
        }


        public Field_(String name, String type)
            : base(name, type)
        {   }


        public Field_(String name, String type, Member_Level_Visibility visibility)
            : base(name, type)
        {
            this.method_visibility = visibility;
        }


    }
}
