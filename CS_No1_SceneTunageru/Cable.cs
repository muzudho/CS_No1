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
        /// 中間点2つ。[0～1]
        /// </summary>
        private Point[] sourceBoundsNode;
        public Point[] SourceBoundsNode
        {
            get
            {
                return this.sourceBoundsNode;
            }
            set
            {
                this.sourceBoundsNode = value;
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
        /// 動かした量。[0～1]
        /// </summary>
        private Point[] movementNode;
        public Point[] MovementNode
        {
            get
            {
                return movementNode;
            }
            set
            {
                movementNode = value;
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
        /// 移動後の中間点2つ。[0～1]
        /// </summary>
        public Point[] BoundsNode
        {
            get
            {
                Point[] node2 = new Point[2];
                for (int i = 0; i < 2; i++)
                {
                    node2[i] = new Point(
                    this.SourceBoundsNode[i].X + this.MovementNode[i].X,
                    this.SourceBoundsNode[i].Y + this.MovementNode[i].Y
                    );
                }

                return node2;
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
        /// 中間点2つの表示の有無。[0～1]
        /// </summary>
        private bool[] isVisibleNode;
        public bool[] IsVisibleNode
        {
            get
            {
                return isVisibleNode;
            }
            set
            {
                isVisibleNode = value;
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
        /// 中間点2つ。選択中。
        /// </summary>
        private bool[] isSelectedNode;
        public bool[] IsSelectedNode
        {
            get
            {
                return isSelectedNode;
            }
            set
            {
                isSelectedNode = value;
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

        /// <summary>
        /// 中間点2つ。マウスカーソルが合わさっています。
        /// </summary>
        private bool[] isMouseOveredNode;
        public bool[] IsMouseOveredNode
        {
            get
            {
                return this.isMouseOveredNode;
            }
            set
            {
                this.isMouseOveredNode = value;
            }
        }

        public void Clear()
        {
            this.SourceBounds = new Rectangle[]{
                new Rectangle(100, 100, 10, 10),
                new Rectangle(100, 100, 10, 10)
            };

            this.SourceBoundsNode = new Point[]{
                new Point(100, 100),
                new Point(100, 100)
            };

            this.Movement = new Rectangle[]{
                new Rectangle(),
                new Rectangle()
            };

            this.MovementNode = new Point[]{
                new Point(),
                new Point()
            };

            this.IsVisible = new bool[]{
                false,
                false
            };

            this.IsVisibleNode = new bool[]{
                false,
                false
            };

            this.IsSelected = new bool[]{
                false,
                false
            };
            this.IsSelectedNode = new bool[]{
                false,
                false
            };

            this.IsMouseOvered = new bool[]{
                false,
                false
            };
            this.IsMouseOveredNode = new bool[]{
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
            // ──────────
            // [0]起点　[1]終点
            // ──────────
            #region 起点と終点
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
                        (int)(this.SourceBounds[i].X + this.Movement[i].X),// + weight / 2f
                        (int)(this.SourceBounds[i].Y + this.Movement[i].Y + weight / 2f),
                        (int)(this.SourceBounds[i].Width + this.Movement[i].Width),// - weight / 2f
                        (int)(this.SourceBounds[i].Height + this.Movement[i].Height)// - weight / 2f
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
            #endregion

            // ──────────
            // 中間点2つ[0～1]
            // ──────────
            #region 中間点2つ
            for (int i = 0; i < 2; i++)
            {
                if (this.IsVisibleNode[i])
                {
                    // 線の太さ
                    float weight;
                    if (this.isMouseOveredNode[i])
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

                    Point bounds2 = new Point(
                        (int)(this.sourceBoundsNode[i].X + this.MovementNode[i].X),
                        (int)(this.sourceBoundsNode[i].Y + this.MovementNode[i].Y + weight / 2f)
                        );

                    // 背景色
                    Brush backBrush;
                    if (this.IsSelectedNode[i])
                    {
                        backBrush = new SolidBrush(Color.FromArgb(128, 0, 255, 0));// 半透明の緑色
                    }
                    else
                    {
                        backBrush = new SolidBrush(Color.FromArgb(128, 255, 255, 255));// 半透明の白色
                    }

                    // 輪っか
                    g.FillEllipse(
                        backBrush,
                        new Rectangle(
                            bounds2.X,
                            bounds2.Y,
                            UiMain.CELL_SIZE,
                            UiMain.CELL_SIZE
                            )
                        );
                }
            }
            #endregion

        }

        /// <summary>
        /// 線　描画
        /// </summary>
        /// <param name="g"></param>
        public void PaintLine(Graphics g)
        {
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
                Point[] points = new Point[0];

                //
                // ５パターンある。
                //
                // （１）表示なし
                // （２）[0]起点
                // （３）[0]起点 - [1]折れ1 - [2]終点
                // （４）[0]起点 - [1]折れ1 - [2]中間点1 - [3]折れ2 - [4]終点
                // （５）[0]起点 - [1]折れ1 - [2]中間点1 - [3]折れ2 - [4]中間点2 - [5]終点

                // （５～１）
                // （５）
                if (this.IsVisible[0] && this.IsVisibleNode[0] && this.IsVisibleNode[1] && this.IsVisible[1])
                {
                    points = new Point[6];
                    // [0]起点
                    points[0] = new Point(
                        (int)(this.SourceBounds[0].X - weight / 2 + UiMain.CELL_SIZE / 2),
                        (int)(this.SourceBounds[0].Y - weight / 2 + UiMain.CELL_SIZE / 2)
                        );
                    // [1]折れ1
                    points[1] = new Point(
                        (int)(this.SourceBoundsNode[0].X - weight / 2 + UiMain.CELL_SIZE / 2),
                        (int)(this.SourceBounds[0].Y - weight / 2 + UiMain.CELL_SIZE / 2)
                        );
                    // [2]中間点1
                    points[2] = new Point(
                        (int)(this.SourceBoundsNode[0].X - weight / 2 + UiMain.CELL_SIZE / 2),
                        (int)(this.SourceBoundsNode[0].Y - weight / 2 + UiMain.CELL_SIZE / 2)
                        );
                    // [3]折れ2
                    points[3] = new Point(
                        (int)(this.SourceBoundsNode[1].X - weight / 2 + UiMain.CELL_SIZE / 2),
                        (int)(this.SourceBoundsNode[0].Y - weight / 2 + UiMain.CELL_SIZE / 2)
                        );
                    // [4]中間点2
                    points[4] = new Point(
                        (int)(this.SourceBoundsNode[1].X - weight / 2 + UiMain.CELL_SIZE / 2),
                        (int)(this.SourceBoundsNode[1].Y - weight / 2 + UiMain.CELL_SIZE / 2)
                        );
                    // [5]終点
                    points[5] = new Point(
                        (int)(this.SourceBounds[1].X - weight / 2 + UiMain.CELL_SIZE / 2),
                        (int)(this.SourceBounds[1].Y - weight / 2 + UiMain.CELL_SIZE / 2)
                        );
                }
                // （３）
                else if (this.IsVisible[0] && this.IsVisible[1])
                {
                    points = new Point[3];
                    // [0]起点
                    points[0] = new Point(
                        (int)(this.SourceBounds[0].X - weight / 2 + UiMain.CELL_SIZE / 2),
                        (int)(this.SourceBounds[0].Y - weight / 2 + UiMain.CELL_SIZE / 2)
                        );
                    // [1]折れ1
                    points[1] = new Point(
                        (int)(this.SourceBounds[1].X - weight / 2 + UiMain.CELL_SIZE / 2),
                        (int)(this.SourceBounds[0].Y - weight / 2 + UiMain.CELL_SIZE / 2)
                        );
                    // [2]終点
                    points[2] = new Point(
                        (int)(this.SourceBounds[1].X - weight / 2 + UiMain.CELL_SIZE / 2),
                        (int)(this.SourceBounds[1].Y - weight / 2 + UiMain.CELL_SIZE / 2)
                        );
                }

                // 折れ線の色
                Pen pen = new Pen(Color.FromArgb(160, 192, 192, 192), weight);//半透明の灰色
                int between = UiMain.CELL_SIZE / 2;

                // 折れ線
                for (int i = 0; i < points.Length - 1;i++ )
                {
                    // （５）
                    // 矢印頭。
                    if (i == 4 && this.IsVisible[0] && this.IsVisibleNode[0] && this.IsVisibleNode[1] && this.IsVisible[1])
                    {
                        g.DrawLine(
                            pen,
                            points[i].X,
                            points[i].Y,
                            points[i + 1].X - between,
                            points[i + 1].Y
                            );
                    }
                    // （３）
                    // 矢印頭
                    else if (i == 0 && this.IsVisible[0] && this.IsVisible[1] && this.Bounds[1].Y < this.Bounds[0].Y)
                    {
                        g.DrawLine(
                            pen,
                            points[i].X + between,
                            points[i].Y,
                            points[i + 1].X,
                            points[i + 1].Y
                            );
                    }
                    else
                    {
                        g.DrawLine(
                            pen,
                            points[i],
                            points[i + 1]
                            );
                    }
                }

                // （５）
                // 矢印頭。
                if (this.IsVisible[0] && this.IsVisibleNode[0] && this.IsVisibleNode[1] && this.IsVisible[1])
                {
                    //　＼
                    g.DrawLine(
                        pen,
                        points[5].X - UiMain.CELL_SIZE / 2 - between,
                        points[5].Y - UiMain.CELL_SIZE / 2,
                        points[5].X - between,
                        points[5].Y
                        );

                    //　／
                    g.DrawLine(
                        pen,
                        points[5].X - between,
                        points[5].Y,
                        points[5].X - UiMain.CELL_SIZE / 2 - between,
                        points[5].Y + UiMain.CELL_SIZE / 2
                        );
                }
                // （３）
                // 矢印頭
                else if (this.IsVisible[0] && this.IsVisible[1] && this.Bounds[1].Y < this.Bounds[0].Y)
                {
                    //　／
                    g.DrawLine(
                        pen,
                        points[0].X + UiMain.CELL_SIZE / 2 + between,
                        points[0].Y + UiMain.CELL_SIZE / 2,
                        points[0].X + between,
                        points[0].Y
                        );

                    //　＼
                    g.DrawLine(
                        pen,
                        points[0].X + between,
                        points[0].Y,
                        points[0].X + UiMain.CELL_SIZE / 2 + between,
                        points[0].Y - UiMain.CELL_SIZE / 2
                        );
                }

                //────────────────────────────────────────
                // 移動後
                //────────────────────────────────────────
                // 線の太さ
                weight = 2.0f;
                points = new Point[0];

                //
                // ５パターンある。
                //
                // （１）表示なし
                // （２）[0]起点
                // （３）[0]起点 - [1]折れ1 - [2]終点
                // （４）[0]起点 - [1]折れ1 - [2]中間点1 - [3]折れ2 - [4]終点
                // （５）[0]起点 - [1]折れ1 - [2]中間点1 - [3]折れ2 - [4]中間点2 - [5]終点

                // （５～１）
                // （５）
                if (this.IsVisible[0] && this.IsVisibleNode[0] && this.IsVisibleNode[1] && this.IsVisible[1])
                {
                    points = new Point[6];
                    // [0]起点
                    points[0] = new Point(
                        (int)(this.SourceBounds[0].X + this.Movement[0].X - weight / 2 + UiMain.CELL_SIZE / 2),
                        (int)(this.SourceBounds[0].Y + this.Movement[0].Y - weight / 2 + UiMain.CELL_SIZE / 2)
                        );
                    // [1]折れ1
                    points[1] = new Point(
                        (int)(this.SourceBoundsNode[0].X + this.MovementNode[0].X - weight / 2 + UiMain.CELL_SIZE / 2),
                        (int)(this.SourceBounds[0].Y + this.Movement[0].Y - weight / 2 + UiMain.CELL_SIZE / 2)
                        );
                    // [2]中間点1
                    points[2] = new Point(
                        (int)(this.SourceBoundsNode[0].X + this.MovementNode[0].X - weight / 2 + UiMain.CELL_SIZE / 2),
                        (int)(this.SourceBoundsNode[0].Y + this.MovementNode[0].Y - weight / 2 + UiMain.CELL_SIZE / 2)
                        );
                    // [3]折れ2
                    points[3] = new Point(
                        (int)(this.SourceBoundsNode[1].X + this.MovementNode[1].X - weight / 2 + UiMain.CELL_SIZE / 2),
                        (int)(this.SourceBoundsNode[0].Y + this.MovementNode[0].Y - weight / 2 + UiMain.CELL_SIZE / 2)
                        );
                    // [4]中間点2
                    points[4] = new Point(
                        (int)(this.SourceBoundsNode[1].X + this.MovementNode[1].X - weight / 2 + UiMain.CELL_SIZE / 2),
                        (int)(this.SourceBoundsNode[1].Y + this.MovementNode[1].Y - weight / 2 + UiMain.CELL_SIZE / 2)
                        );
                    // [5]終点
                    points[5] = new Point(
                        (int)(this.SourceBounds[1].X + this.Movement[1].X - weight / 2 + UiMain.CELL_SIZE / 2),
                        (int)(this.SourceBounds[1].Y + this.Movement[1].Y - weight / 2 + UiMain.CELL_SIZE / 2)
                        );
                }
                // （３）
                else if (this.IsVisible[0] && this.IsVisible[1])
                {
                    points = new Point[3];
                    // [0]起点
                    points[0] = new Point(
                        (int)(this.SourceBounds[0].X + this.Movement[0].X - weight / 2 + UiMain.CELL_SIZE / 2),
                        (int)(this.SourceBounds[0].Y + this.Movement[0].Y - weight / 2 + UiMain.CELL_SIZE / 2)
                        );
                    // [1]折れ1
                    points[1] = new Point(
                        (int)(this.SourceBounds[1].X + this.Movement[1].X - weight / 2 + UiMain.CELL_SIZE / 2),
                        (int)(this.SourceBounds[0].Y + this.Movement[0].Y - weight / 2 + UiMain.CELL_SIZE / 2)
                        );
                    // [2]終点
                    points[2] = new Point(
                        (int)(this.SourceBounds[1].X + this.Movement[1].X - weight / 2 + UiMain.CELL_SIZE / 2),
                        (int)(this.SourceBounds[1].Y + this.Movement[1].Y - weight / 2 + UiMain.CELL_SIZE / 2)
                        );
                }

                // 折れ線の色
                pen = new Pen(Color.Black, weight);//黒
                between = UiMain.CELL_SIZE / 2;

                // 折れ線
                for (int i = 0; i < points.Length - 1; i++)
                {

                    // （５）
                    // 矢印頭。
                    if (i == 4 && this.IsVisible[0] && this.IsVisibleNode[0] && this.IsVisibleNode[1] && this.IsVisible[1])
                    {
                        g.DrawLine(
                            pen,
                            points[i].X,
                            points[i].Y,
                            points[i + 1].X - between,
                            points[i + 1].Y
                            );
                    }
                    // （３）
                    // 矢印頭
                    else if (i == 0 && this.IsVisible[0] && this.IsVisible[1] && this.Bounds[1].Y < this.Bounds[0].Y)
                    {
                        g.DrawLine(
                            pen,
                            points[i].X + between,
                            points[i].Y,
                            points[i + 1].X,
                            points[i + 1].Y
                            );
                    }
                    else
                    {
                        g.DrawLine(
                            pen,
                            points[i],
                            points[i + 1]
                            );
                    }

                }


                // （５）
                // 矢印頭。
                if (this.IsVisible[0] && this.IsVisibleNode[0] && this.IsVisibleNode[1] && this.IsVisible[1])
                {
                    //　＼
                    g.DrawLine(
                        pen,
                        points[5].X - UiMain.CELL_SIZE / 2 - between,
                        points[5].Y - UiMain.CELL_SIZE / 2,
                        points[5].X - between,
                        points[5].Y
                        );

                    //　／
                    g.DrawLine(
                        pen,
                        points[5].X - between,
                        points[5].Y,
                        points[5].X - UiMain.CELL_SIZE / 2 - between,
                        points[5].Y + UiMain.CELL_SIZE / 2
                        );
                }
                // （３）
                // 矢印頭
                else if (this.IsVisible[0] && this.IsVisible[1] && this.Bounds[1].Y < this.Bounds[0].Y)
                {
                    //　／
                    g.DrawLine(
                        pen,
                        points[0].X + UiMain.CELL_SIZE / 2 + between,
                        points[0].Y + UiMain.CELL_SIZE / 2,
                        points[0].X + between,
                        points[0].Y
                        );

                    //　＼
                    g.DrawLine(
                        pen,
                        points[0].X + between,
                        points[0].Y,
                        points[0].X + UiMain.CELL_SIZE / 2 + between,
                        points[0].Y - UiMain.CELL_SIZE / 2
                        );
                }

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
                        (int)(this.SourceBounds[i].X),// + weight / 2f
                        (int)(this.SourceBounds[i].Y + weight / 2f),
                        (int)(this.SourceBounds[i].Width),// - weight / 2f
                        (int)(this.SourceBounds[i].Height)// - weight / 2f
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
                        (int)(this.SourceBounds[i].X + this.Movement[i].X),// + weight / 2f
                        (int)(this.SourceBounds[i].Y + this.Movement[i].Y + weight / 2f),
                        (int)(this.SourceBounds[i].Width + this.Movement[i].Width),// - weight / 2f
                        (int)(this.SourceBounds[i].Height + this.Movement[i].Height)// - weight / 2f
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
            sb.Append("  <cable");
            sb.Append(" x0=\"" + this.SourceBounds[0].X + "\" y0=\"" + this.SourceBounds[0].Y + "\"");
            sb.Append(" visible0=\"" + this.IsVisible[0] + "\"");
            sb.Append(" x1=\"" + this.SourceBounds[1].X + "\" y1=\"" + this.SourceBounds[1].Y + "\"");
            sb.Append(" visible1=\"" + this.IsVisible[1] + "\"");
            sb.Append(" node0x=\"" + this.SourceBoundsNode[0].X + "\" node0y=\"" + this.SourceBoundsNode[0].Y + "\"");
            sb.Append(" node0visible=\"" + this.IsVisibleNode[0] + "\"");
            sb.Append(" node1x=\"" + this.SourceBoundsNode[1].X + "\" node1y=\"" + this.SourceBoundsNode[1].Y + "\"");
            sb.Append(" node1visible=\"" + this.IsVisibleNode[1] + "\"");
            sb.Append(" />");
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

            // ──────────
            // 中間点
            // ──────────
            s = xe.GetAttribute("node0x");
            int.TryParse(s, out x);
            s = xe.GetAttribute("node0y");
            int.TryParse(s, out y);
            this.SourceBoundsNode[0] = new Point(
                x,
                y
                );
            s = xe.GetAttribute("node0visible");
            bool.TryParse(s, out b);
            this.IsVisibleNode[0] = b;

            s = xe.GetAttribute("node1x");
            int.TryParse(s, out x);
            s = xe.GetAttribute("node1y");
            int.TryParse(s, out y);
            this.SourceBoundsNode[1] = new Point(
                x,
                y
                );
            s = xe.GetAttribute("node1visible");
            bool.TryParse(s, out b);
            this.IsVisibleNode[1] = b;
        }

        /// <summary>
        /// 表示しているとき、指定座標が境界内なら真。
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public bool IsHit(int index, Point location)
        {
            return this.IsVisible[index] && this.Bounds[index].Contains(location);
        }

        /// <summary>
        /// 表示しているとき、指定座標が境界内なら真。
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public bool IsHitNode(int index, Point location)
        {
            return this.IsVisibleNode[index] && new Rectangle(
                this.BoundsNode[index].X,
                this.BoundsNode[index].Y,
                UiMain.CELL_SIZE,
                UiMain.CELL_SIZE
                ).Contains(location);
        }


        /// <summary>
        /// マウスが合わさっているかどうかを判定し、状態変更します。
        /// </summary>
        /// <param name="location"></param>
        public bool CheckMouseOver(int index, Point location, ref bool forcedOff)
        {
            if (forcedOff)
            {
                this.IsMouseOvered[index] = false;
            }
            else
            {
                if (this.IsHit(index, location))
                {
                    this.IsMouseOvered[index] = true;
                    forcedOff = true;
                }
                else
                {
                    this.IsMouseOvered[index] = false;
                }
            }

            return this.IsMouseOvered[index];
        }

        /// <summary>
        /// マウスが合わさっているかどうかを判定し、状態変更します。
        /// </summary>
        /// <param name="location"></param>
        public bool CheckMouseOverNode(int index, Point location, ref bool forcedOff)
        {
            if (forcedOff)
            {
                this.IsMouseOveredNode[index] = false;
            }
            else
            {
                if (this.IsHitNode(index, location))
                {
                    this.IsMouseOveredNode[index] = true;
                    forcedOff = true;
                }
                else
                {
                    this.IsMouseOveredNode[index] = false;
                }
            }

            return this.IsMouseOveredNode[index];
        }

    }
}
