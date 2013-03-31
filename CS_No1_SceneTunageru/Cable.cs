using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;

namespace Gs_No1
{

    /// <summary>
    /// 接続線。
    /// </summary>
    public class Cable
    {

        /// <summary>
        /// 移動前の境界線。[0～1]
        /// </summary>
        private Rectangle[] sourceBounds;
        public Rectangle[] SourceBounds
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
        /// 動かした量。[0～1]
        /// </summary>
        private Rectangle[] movement;
        public Rectangle[] Movement
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
        /// 移動後の境界線。[0～1]
        /// </summary>
        public Rectangle[] Bounds
        {
            get
            {
                Rectangle[] bounds2 = new Rectangle[2];
                for(int i=0; i<2; i++)
                {
                    bounds2[i] = new Rectangle(
                    this.SourceBounds[i].X + this.Movement[i].X,
                    this.SourceBounds[i].Y + this.Movement[i].Y,
                    this.SourceBounds[i].Width + this.Movement[i].Width,
                    this.SourceBounds[i].Height + this.Movement[i].Height
                    );
                }

                return bounds2;
            }
        }

        /// <summary>
        /// 表示の有無。[0～1]
        /// </summary>
        private bool[] isVisible;
        public bool[] IsVisible
        {
            get
            {
                return isVisible;
            }
            set
            {
                isVisible = value;
            }
        }


        public Cable()
        {
            this.SourceBounds = new Rectangle[]{
                new Rectangle(100, 100, 10, 10),
                new Rectangle(100, 100, 10, 10)
            };

            this.Movement = new Rectangle[]{
                new Rectangle(),
                new Rectangle()
            };

            this.IsVisible = new bool[]{
                false,
                false
            };
        }

        public void Paint(Graphics g)
        {
            // ──────────
            // [0]起点　[1]終点
            // ──────────
            for (int i = 0; i < 2; i++)
            {
                if (this.IsVisible[i])
                {

                    //────────────────────────────────────────
                    // 移動前の残像
                    //────────────────────────────────────────

                    g.FillEllipse(
                        new SolidBrush(Color.FromArgb(128, 0, 0, 0)),
                        new Rectangle(
                            this.sourceBounds[i].X,
                            this.sourceBounds[i].Y,
                            this.sourceBounds[i].Width,
                            this.sourceBounds[i].Height
                            )
                        );

                    //────────────────────────────────────────
                    // 移動後
                    //────────────────────────────────────────

                    g.FillEllipse(Brushes.Black,
                        new Rectangle(
                            this.sourceBounds[i].X,
                            this.sourceBounds[i].Y,
                            this.sourceBounds[i].Width,
                            this.sourceBounds[i].Height
                            )
                        );
                }
            }

            // ──────────
            // [0]起点　[1]終点　の接続
            // ──────────
            if (this.IsVisible[0] && this.IsVisible[1])
            {
                float weight = 2.0f;

                Point[] points = new Point[3];

                // [0]起点
                points[0] = new Point(
                    (int)(this.Bounds[0].X - weight/2 + UiMain.CELL_SIZE/2),
                    (int)(this.Bounds[0].Y - weight / 2 + UiMain.CELL_SIZE / 2)
                    );
                // [1]中間点
                points[1] = new Point(
                    (int)(this.Bounds[1].X - weight / 2 + UiMain.CELL_SIZE / 2),
                    (int)(this.Bounds[0].Y - weight / 2 + UiMain.CELL_SIZE / 2)
                    );
                // [2]終点
                points[2] = new Point(
                    (int)(this.Bounds[1].X - weight / 2 + UiMain.CELL_SIZE / 2),
                    (int)(this.Bounds[1].Y - weight / 2 + UiMain.CELL_SIZE / 2)
                    );

                g.DrawLine(
                    new Pen(Color.Black, weight),
                    points[0],
                    points[1]
                    );
                g.DrawLine(
                    new Pen(Color.Black, weight),
                    points[1],
                    points[2]
                    );

            }
        }

        public void Save(StringBuilder sb)
        {
            sb.Append("  <cable x0=\"" + this.Bounds[0].X + "\" y0=\"" + this.Bounds[0].Y + "\" visible0=\"" + this.IsVisible[0] + "\" x1=\"" + this.Bounds[1].X + "\" y1=\"" + this.Bounds[1].Y + "\" visible1=\"" + this.IsVisible[1] + "\" />");
            sb.Append(Environment.NewLine);

            System.Console.WriteLine("接続線：　起点座標（" + this.Bounds[0].X + "," + this.Bounds[0].Y + "）　終点座標（" + this.Bounds[1].X + "," + this.Bounds[1].Y + "）");
        }

    }
}
