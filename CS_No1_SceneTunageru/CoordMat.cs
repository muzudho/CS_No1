using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;
using System.Windows.Forms;
using System.Xml;

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

        /// <summary>
        /// マウスカーソルが合わさっています。
        /// </summary>
        private bool isMouseOvered;
        public bool IsMouseOvered
        {
            get
            {
                return this.isMouseOvered;
            }
            set
            {
                this.isMouseOvered = value;
            }
        }

        private string fileName;
        public string FileName
        {
            get
            {
                return fileName;
            }
            set
            {
                fileName = value;
            }
        }



        /// <summary>
        /// 表示の有無。
        /// </summary>
        private bool isVisible;
        public bool IsVisible
        {
            get
            {
                return this.isVisible;
            }
            set
            {
                this.isVisible = value;
            }
        }

        public void Clear()
        {
            this.sourceBounds = new Rectangle(100, 100, 20 * UiMain.CELL_SIZE, 20 * UiMain.CELL_SIZE);
            this.Movement = new Rectangle();
            this.fileName = "noname.png";
            this.IsVisible = true;
        }

        public CoordMat()
        {
            this.Clear();
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

            Pen borderPen;
            Pen gridPen;
            Brush brush;
            // 枠線の太さ
            float weight;
            if (this.isMouseOvered)
            {
                weight = 4.0f;
            }
            else
            {
                weight = 2.0f;
            }

            if (this.IsSelected)
            {
                borderPen = new Pen(Color.FromArgb(128, 0, 0, 255),weight);
                gridPen = new Pen(Color.FromArgb(128, 0, 0, 255));
                brush = new SolidBrush(Color.FromArgb(128, 0, 0, 255));
            }
            else
            {
                borderPen = new Pen(Color.FromArgb(128, 0, 0, 0),weight);
                gridPen = new Pen(Color.FromArgb(128, 0, 0, 0));
                brush = new SolidBrush(Color.FromArgb(128, 0, 0, 0));
            }

            // 縦線
            int e1 = bounds2.Height / cellSize;
            for (int l1 = 1; l1 < e1; l1++)
            {
                g.DrawLine(gridPen,
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
                    gridPen,
                    0 + bounds2.X,
                    l1 * cellSize + bounds2.Y,
                    bounds2.Width + bounds2.X,
                    l1 * cellSize + bounds2.Y
                    );
            }

            // 枠線
            g.DrawRectangle(borderPen, bounds2);

            // ファイル名
            g.DrawString( this.FileName, new Font("ＭＳ ゴシック", 12.0f), brush, bounds2.Location );

            //────────────────────────────────────────
            // 移動後
            //────────────────────────────────────────

            bounds2 = new Rectangle(
                this.SourceBounds.X + this.Movement.X,
                this.SourceBounds.Y + this.Movement.Y,
                this.SourceBounds.Width + this.Movement.Width,
                this.SourceBounds.Height + this.Movement.Height
                );

            // 枠線の太さ
            if (this.isMouseOvered)
            {
                weight = 4.0f;
            }
            else
            {
                weight = 2.0f;
            }

            if (this.IsSelected)
            {
                borderPen = new Pen(Color.Blue,weight);
                gridPen = new Pen(Color.Blue);
            }
            else
            {
                borderPen = new Pen(Color.Black, weight);
                gridPen = new Pen(Color.Black);
            }

            // 縦線
            e1 = bounds2.Height / cellSize;
            for (int l1 = 1; l1 < e1; l1++)
            {
                g.DrawLine(gridPen,
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
                    gridPen,
                    0 + bounds2.X,
                    l1 * cellSize + bounds2.Y,
                    bounds2.Width + bounds2.X,
                    l1 * cellSize + bounds2.Y
                    );
            }

            // 枠線
            g.DrawRectangle( borderPen, bounds2 );

            // ファイル名
            g.DrawString(this.FileName, new Font("ＭＳ ゴシック", 12.0f), brush, bounds2.Location);
        }

        /// <summary>
        /// 表示しているとき、指定座標が境界内なら真。
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public bool IsHit(Point location)
        {
            return this.IsVisible && this.Bounds.Contains(location);
        }

        public void Save(StringBuilder sb)
        {
            sb.Append("  <coord-mat x=\"" + this.SourceBounds.X + "\" y=\"" + this.SourceBounds.Y + "\" width=\"" + this.SourceBounds.Width + "\" height=\"" + this.SourceBounds.Height + "\" font-size=\"" + this.SourceBounds + "\" />");
            sb.Append(Environment.NewLine);
        }

        public void Load(XmlElement xe)
        {
            this.Clear();

            string s;
            int x;
            int y;
            int w;
            int h;

            s = xe.GetAttribute("x");
            int.TryParse(s, out x);
            s = xe.GetAttribute("y");
            int.TryParse(s, out y);
            s = xe.GetAttribute("width");
            int.TryParse(s, out w);
            s = xe.GetAttribute("height");
            int.TryParse(s, out h);
            this.SourceBounds = new Rectangle(x, y, w, h);
        }

    }
}
