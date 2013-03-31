using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;
using System.Windows.Forms;

namespace Gs_No1
{

    /// <summary>
    /// 座標マット
    /// </summary>
    public class CoordinateMat
    {

        /// <summary>
        /// 境界線。
        /// </summary>
        private Rectangle bounds;
        public Rectangle Bounds
        {
            get
            {
                return this.bounds;
            }
            set
            {
                this.bounds = value;
            }
        }


        /// <summary>
        /// 選択中。
        /// </summary>
        private bool isSelected;
        public bool IsSelected
        {
            get
            {
                return this.isSelected;
            }
            set
            {
                this.isSelected = value;
            }
        }


        public CoordinateMat()
        {
            this.bounds = new Rectangle(100,100,320,320);
        }

        public void Paint(Graphics g)
        {
            // セルサイズ
            int cellSize = 32;

            Pen pen;
            if (this.IsSelected)
            {
                pen = new Pen(Color.Blue);
            }
            else
            {
                pen = new Pen(Color.Black);
            }

            // 縦線
            int e1 = this.bounds.Height / cellSize;
            for (int l1 = 1; l1 < e1; l1++)
            {
                g.DrawLine(pen,
                    l1 * cellSize + this.bounds.X,
                    0 + this.bounds.Y,
                    l1 * cellSize + this.bounds.X,
                    this.bounds.Height + this.bounds.Y);
            }

            // 横線
            e1 = this.bounds.Width / cellSize;
            for (int l1 = 1; l1 < e1; l1++)
            {
                g.DrawLine(
                    pen,
                    0 + this.bounds.X,
                    l1 * cellSize + this.bounds.Y,
                    this.bounds.Width + this.bounds.X,
                    l1 * cellSize + this.bounds.Y
                    );
            }

            // 枠線
            g.DrawRectangle(
                pen,
                this.bounds
                );
        }

        public void MouseDown(object sender, MouseEventArgs e)
        {
            if (this.Bounds.Contains(e.Location))
            {
                System.Console.WriteLine("範囲内。mouse(" + e.X + "," + e.Y + ") bounds(" + this.Bounds.X + "," + this.Bounds.Y + "," + this.Bounds.Width + "," + this.Bounds.Height + ")");
                //this.isSelected = true;
            }
            else
            {
                System.Console.WriteLine("境界外。");
                //this.isSelected = false;
            }
        }

    }
}
