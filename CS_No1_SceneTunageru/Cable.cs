using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;
using System.Xml;

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

        /// <summary>
        /// 選択中。
        /// </summary>
        private bool[] isSelected;
        public bool[] IsSelected
        {
            get
            {
                return isSelected;
            }
            set
            {
                isSelected = value;
            }
        }

        /// <summary>
        /// マウスカーソルが合わさっています。
        /// </summary>
        private bool[] isMouseOvered;
        public bool[] IsMouseOvered
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

        public void Clear()
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

            this.IsSelected = new bool[]{
                false,
                false
            };

            this.IsMouseOvered = new bool[]{
                false,
                false
            };
        }

        public Cable()
        {
            this.Clear();
        }

        /// <summary>
        /// 円の背景　描画
        /// </summary>
        /// <param name="g"></param>
        public void PaintBackCircle(Graphics g)
        {
            //return;

            // ──────────
            // [0]起点　[1]終点
            // ──────────
            for (int i = 0; i < 2; i++)
            {
                if (this.IsVisible[i])
                {
                    // 線の太さ
                    float weight;
                    if (this.isMouseOvered[i])
                    {
                        weight = 4.0f;
                    }
                    else
                    {
                        weight = 2.0f;
                    }

                    //────────────────────────────────────────
                    // 移動前の残像
                    //────────────────────────────────────────

                    // 描画なし

                    //────────────────────────────────────────
                    // 移動後
                    //────────────────────────────────────────

                    Rectangle bounds2 = new Rectangle(
                        (int)(this.sourceBounds[i].X + this.Movement[i].X),// + weight / 2f
                        (int)(this.sourceBounds[i].Y + this.Movement[i].Y + weight / 2f),
                        (int)(this.sourceBounds[i].Width + this.Movement[i].Width),// - weight / 2f
                        (int)(this.sourceBounds[i].Height + this.Movement[i].Height)// - weight / 2f
                        );

                    // 背景色
                    Brush backBrush;
                    if (this.IsSelected[i])
                    {
                        backBrush = new SolidBrush(Color.FromArgb(128, 0, 255, 0));// Brushes.Lime;
                    }
                    else
                    {
                        backBrush = new SolidBrush(Color.FromArgb(128, 255, 255, 255));// Brushes.White;
                    }

                    // 輪っか
                    g.FillEllipse(
                        backBrush,
                        new Rectangle(
                            bounds2.X,
                            bounds2.Y,
                            bounds2.Width,
                            bounds2.Height
                            )
                        );
                }
            }
        }

        /// <summary>
        /// 線　描画
        /// </summary>
        /// <param name="g"></param>
        public void PaintLine(Graphics g)
        {
            //return;
            // 輪っかは無し。

            // ──────────
            // [0]起点　[1]終点　の接続
            // ──────────
            //
            // 起点と終点が共に表示されている場合。
            //
            if (this.IsVisible[0] && this.IsVisible[1])
            {
                //────────────────────────────────────────
                // 移動前の残像
                //────────────────────────────────────────

                // 線の太さ
                float weight = 2.0f;


                Point[] points = new Point[3];

                // [0]起点
                points[0] = new Point(
                    (int)(this.SourceBounds[0].X - weight / 2 + UiMain.CELL_SIZE / 2),
                    (int)(this.SourceBounds[0].Y - weight / 2 + UiMain.CELL_SIZE / 2)
                    );
                // [1]中間点
                points[1] = new Point(
                    (int)(this.SourceBounds[1].X - weight / 2 + UiMain.CELL_SIZE / 2),
                    (int)(this.SourceBounds[0].Y - weight / 2 + UiMain.CELL_SIZE / 2)
                    );
                // [2]終点
                points[2] = new Point(
                    (int)(this.SourceBounds[1].X - weight / 2 + UiMain.CELL_SIZE / 2),
                    (int)(this.SourceBounds[1].Y - weight / 2 + UiMain.CELL_SIZE / 2)
                    );

                // 折れ線の色
                Pen pen = new Pen(Color.FromArgb(160, 192, 192, 192), weight);

                // 折れ線１、２
                g.DrawLine(
                    pen,
                    points[0],
                    points[1]
                    );
                g.DrawLine(
                    pen,
                    points[1],
                    points[2]
                    );

                //────────────────────────────────────────
                // 移動後
                //────────────────────────────────────────

                // [0]起点
                points[0] = new Point(
                    (int)(this.Bounds[0].X - weight / 2 + UiMain.CELL_SIZE / 2),
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

                // 折れ線の色
                pen = new Pen(Color.Black, weight);

                // 折れ線１、２
                g.DrawLine(
                    pen,
                    points[0],
                    points[1]
                    );
                g.DrawLine(
                    pen,
                    points[1],
                    points[2]
                    );
            }
        }

        /// <summary>
        /// サークル
        /// </summary>
        /// <param name="g"></param>
        public void PaintMouseMark(Graphics g)
        {
            // ──────────
            // [0]起点　[1]終点
            // ──────────
            for (int i = 0; i < 2; i++)
            {
                if (this.IsVisible[i])
                {
                    // 線の太さ
                    float weight;
                    if (this.isMouseOvered[i])
                    {
                        weight = 4.0f;
                    }
                    else
                    {
                        weight = 2.0f;
                    }

                    //────────────────────────────────────────
                    // 移動前の残像
                    //────────────────────────────────────────

                    Rectangle bounds2 = new Rectangle(
                        (int)(this.sourceBounds[i].X),// + weight / 2f
                        (int)(this.sourceBounds[i].Y + weight / 2f),
                        (int)(this.sourceBounds[i].Width),// - weight / 2f
                        (int)(this.sourceBounds[i].Height)// - weight / 2f
                        );

                    Pen circlePen = new Pen(Color.FromArgb(160, 192, 192, 192), weight);

                    g.DrawEllipse(
                        circlePen,
                        new Rectangle(
                            bounds2.X,
                            bounds2.Y,
                            bounds2.Width,
                            bounds2.Height
                            )
                        );

                    //────────────────────────────────────────
                    // 移動後
                    //────────────────────────────────────────

                    bounds2 = new Rectangle(
                        (int)(this.sourceBounds[i].X + this.Movement[i].X),// + weight / 2f
                        (int)(this.sourceBounds[i].Y + this.Movement[i].Y + weight / 2f),
                        (int)(this.sourceBounds[i].Width + this.Movement[i].Width),// - weight / 2f
                        (int)(this.sourceBounds[i].Height + this.Movement[i].Height)// - weight / 2f
                        );

                    // ○の色
                    if (this.IsSelected[i])
                    {
                        // 半透明の緑
                        circlePen = new Pen(Color.FromArgb(128, 0, 255, 0), weight);
                    }
                    else
                    {
                        // 半透明の灰色
                        circlePen = new Pen(Color.FromArgb(128, 192, 192, 192), weight);
                    }

                    // 輪っか
                    g.DrawEllipse(
                        circlePen,
                        new Rectangle(
                            bounds2.X,
                            bounds2.Y,
                            bounds2.Width,
                            bounds2.Height
                            )
                        );
                }
            }
        }


        public void Save(StringBuilder sb)
        {
            sb.Append("  <cable x0=\"" + this.SourceBounds[0].X + "\" y0=\"" + this.SourceBounds[0].Y + "\" visible0=\"" + this.IsVisible[0] + "\" x1=\"" + this.SourceBounds[1].X + "\" y1=\"" + this.SourceBounds[1].Y + "\" visible1=\"" + this.IsVisible[1] + "\" />");
            sb.Append(Environment.NewLine);

            //ystem.Console.WriteLine("接続線：　起点座標（" + this.Bounds[0].X + "," + this.Bounds[0].Y + "）　終点座標（" + this.Bounds[1].X + "," + this.Bounds[1].Y + "）");
        }

        public void Load(XmlElement xe)
        {
            this.Clear();

            string s;
            int x;
            int y;
            bool b;

            s = xe.GetAttribute("x0");
            int.TryParse(s, out x);
            s = xe.GetAttribute("y0");
            int.TryParse(s, out y);
            this.SourceBounds[0] = new Rectangle(
                x,
                y,
                UiMain.CELL_SIZE,
                UiMain.CELL_SIZE
                );
            s = xe.GetAttribute("visible0");
            bool.TryParse(s, out b);
            this.IsVisible[0] = b;

            s = xe.GetAttribute("x1");
            int.TryParse(s, out x);
            s = xe.GetAttribute("y1");
            int.TryParse(s, out y);
            this.SourceBounds[1] = new Rectangle(
                x,
                y,
                UiMain.CELL_SIZE,
                UiMain.CELL_SIZE
                );
            s = xe.GetAttribute("visible1");
            bool.TryParse(s, out b);
            this.IsVisible[1] = b;
        }

        /// <summary>
        /// 表示しているとき、指定座標が境界内なら真。
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public bool IsHit0(Point location)
        {
            return this.IsVisible[0] && this.Bounds[0].Contains(location);
        }

        /// <summary>
        /// 表示しているとき、指定座標が境界内なら真。
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public bool IsHit1(Point location)
        {
            return this.IsVisible[1] && this.Bounds[1].Contains(location);
        }


        /// <summary>
        /// マウスが合わさっているかどうかを判定し、状態変更します。
        /// </summary>
        /// <param name="location"></param>
        public bool CheckMouseOver0(Point location, ref bool forcedOff)
        {
            if (forcedOff)
            {
                this.IsMouseOvered[0] = false;
            }
            else
            {
                if (this.IsHit0(location))
                {
                    this.IsMouseOvered[0] = true;
                    forcedOff = true;
                }
                else
                {
                    this.IsMouseOvered[0] = false;
                }
            }

            return this.IsMouseOvered[0];
        }

        /// <summary>
        /// マウスが合わさっているかどうかを判定し、状態変更します。
        /// </summary>
        /// <param name="location"></param>
        public void CheckMouseOver1(Point location, ref bool forcedOff)
        {
            if (forcedOff)
            {
                this.IsMouseOvered[1] = false;
            }
            else
            {
                if (this.IsHit1(location))
                {
                    this.IsMouseOvered[1] = true;
                    forcedOff = true;
                }
                else
                {
                    this.IsMouseOvered[1] = false;
                }
            }
        }

    }
}
