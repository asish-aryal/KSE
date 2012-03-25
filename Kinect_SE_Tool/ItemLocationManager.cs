using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace Kinect_SE_Tool
{
    public class ItemLocationManager
    {
		#region Fields (7) 

        private int current_page;
        private int items_per_column;
        private int items_per_page;
        private int items_per_row;
        private PointCollection points_in_a_page;
        private int selected_element;
        private int total_pages;

		#endregion Fields 

		#region Constructors (1) 

        public ItemLocationManager()
        {
            current_page = 1;
            selected_element = 1;
        }

		#endregion Constructors 

		#region Properties (6) 

        public int CURRENT_PAGE
        {
            get { return current_page; }
            set { current_page = value; }
        }

        public int ITEMS_PER_COLUMN
        {
            get { return items_per_column; }
        }

        public int ITEMS_PER_PAGE
        {
            get { return items_per_page; }
        }

        public int ITEMS_PER_ROW
        {
            get { return items_per_row; }
        }

        public int SELECTED_ITEM
        {
            get { return selected_element; }
            set { selected_element = value; }
        }

        public int TOTAL_PAGES
        {
            get { return total_pages; }
        }

		#endregion Properties 

		#region Methods (3) 

		// Public Methods (2) 

        public Point get_point(int item_number)
        {
            int nth_item = (item_number-1) % items_per_page;
            return points_in_a_page[nth_item];
        }

        public void recalcuate (int Canvas_Width, int Canvas_Height, int item_width, int item_height, int no_of_items)
        {
            points_in_a_page = new PointCollection();
            List<PointCollection> pages = new List<PointCollection>();
            int vertical_separation = get_separation_distance(item_height, Canvas_Height);
            int horizontal_separation = get_separation_distance(item_width, Canvas_Width);


            int cur_x;
            int cur_y;


            items_per_row = 0;                
            cur_y = vertical_separation;
            while ((cur_y + item_height) <= Canvas_Height)
            {
                items_per_row++;
                cur_x = horizontal_separation;
                while ((cur_x + item_width) <= Canvas_Width)
                {
                    points_in_a_page.Add(new Point((double)cur_x, (double)cur_y));
                    cur_x = cur_x + item_width + horizontal_separation;
                }

                cur_y = cur_y + item_height + vertical_separation;
            }

            if ((no_of_items % points_in_a_page.Count) == 0)
            { total_pages = no_of_items / points_in_a_page.Count; }
            else
            { total_pages = (no_of_items / points_in_a_page.Count) + 1;}

            if ((selected_element % points_in_a_page.Count) == 0)
            { current_page = selected_element / points_in_a_page.Count; }
            else
            {
                current_page = selected_element / points_in_a_page.Count + 1;
            }

            items_per_page = points_in_a_page.Count;
            items_per_column = items_per_page / items_per_row;
            
        }
		// Private Methods (1) 

        private int get_separation_distance(int space_taken_by_frame, int total_space)
        {
            int separation;
            int free_space = total_space % space_taken_by_frame;
            int no_of_gaps = (total_space / space_taken_by_frame) + 1;

            separation = free_space / no_of_gaps;

            while ((separation < 20) && (no_of_gaps > 2))
            {
                free_space = free_space + space_taken_by_frame;
                no_of_gaps--;
                separation = free_space / no_of_gaps;
            }

            return separation;
        }

		#endregion Methods 
    }
}
