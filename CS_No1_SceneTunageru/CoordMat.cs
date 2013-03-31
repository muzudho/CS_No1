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
    public class CoordMat
    {

        /// <summary>
        /// 移動前の境界線。
        /// </summary>
        private Rectangle sourceBounds;
        public Rectangle SourceBounds
        {
            get
            {
                return this.sourceBounds;
            }
            set
            {
                this.sourceBounds = value;
            }
        }

        /// <summary>
        /// 動かした量。
        /// </summary>
        private Rectangle movement;
        public Rectangle Movement
        {
            get
            {
                return movement;
            }
            set
            {
                movement = value;
            }
        }

        /// <summary>
        /// 移動後の境界線。
        /// </summary>
        public Rectangle Bounds
        {
            get
            {
                return new Rectangle(
                    this.SourceBounds.X + this.Movement.X,
                    this.SourceBounds.Y + this.Movement.Y,
                    this.SourceBounds.Width + this.Movement.Width,
                    this.SourceBounds.Height + this.Movement.Height
                    );
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


        public CoordMat()
        {
            this.sourceBounds = new Rectangle(100,100,320,320);
            this.Movement = new Rectangle();
        }

        public void Paint(Graphics g)
        {

            // セルサイズ
            int cellSize = UiMain.CELL_SIZE;

            //────────────────────────────────────────
            // 移動前の残像
            //────────────────────────────────────────

            Rectangle bounds2 = new Rectangle(
                this.SourceBounds.X,
                this.SourceBounds.Y,
                this.SourceBounds.Width,
                this.SourceBounds.Height
                );

            Pen pen;
            if (this.IsSelected)
            {
                pen = new Pen(Color.FromArgb(128,0,0,255));
            }
            else
            {
                pen = new Pen(Color.FromArgb(128, 0, 0, 0));
            }

            // 縦線
            int e1 = bounds2.Height / cellSize;
            for (int l1 = 1; l1 < e1; l1++)
            {
                g.DrawLine(pen,
                    l1 * cellSize + bounds2.X,
                    0 + bounds2.Y,
                    l1 * cellSize + bounds2.X,
                    bounds2.Height + bounds2.Y);
            }

            // 横線
            e1 = bounds2.Width / cellSize;
            for (int l1 = 1; l1 < e1; l1++)
            {
                g.DrawLine(
                    pen,
                    0 + bounds2.X,
                    l1 * cellSize + bounds2.Y,
                    bounds2.Width + bounds2.X,
                    l1 * cellSize + bounds2.Y
                    );
            }

            // 枠線
            g.DrawRectangle(pen, bounds2);

            //────────────────────────────────────────
            // 移動後
            //────────────────────────────────────────

            bounds2 = new Rectangle(
                this.SourceBounds.X + this.Movement.X,
                this.SourceBounds.Y + this.Movement.Y,
                this.SourceBounds.Width + this.Movement.Width,
                this.SourceBounds.Height + this.Movement.Height
                );

            if (this.IsSelected)
            {
                pen = new Pen(Color.Blue);
            }
            else
            {
                pen = new Pen(Color.Black);
            }

            // 縦線
            e1 = bounds2.Height / cellSize;
            for (int l1 = 1; l1 < e1; l1++)
            {
                g.DrawLine(pen,
                    l1 * cellSize + bounds2.X,
                    0 + bounds2.Y,
                    l1 * cellSize + bounds2.X,
                    bounds2.Height + bounds2.Y);
            }

            // 横線
            e1 = bounds2.Width / cellSize;
            for (int l1 = 1; l1 < e1; l1++)
            {
                g.DrawLine(
                    pen,
                    0 + bounds2.X,
                    l1 * cellSize + bounds2.Y,
                    bounds2.Width + bounds2.X,
                    l1 * cellSize + bounds2.Y
                    );
            }

            // 枠線
            g.DrawRectangle( pen, bounds2 );
        }

    }
}
