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
    /// シーン・ボックス。
    /// </summary>
    public class SceneBox
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
        /// フォント名。
        /// </summary>
        private string fontName;
        public string FontName
        {
            get
            {
                return fontName;
            }
            set
            {
                fontName = value;
            }
        }

        /// <summary>
        /// フォントサイズ。
        /// </summary>
        private float fontSize;
        public float FontSize
        {
            get
            {
                return fontSize;
            }
            set
            {
                fontSize = value;
            }
        }




        /// <summary>
        /// 表示文字列。
        /// </summary>
        private string title;
        public string Title
        {
            get
            {
                return title;
            }
            set
            {
                title = value;
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

        /// <summary>
        /// 形状。0:シーン。1:分岐。2:説明。
        /// </summary>
        private int shape;
        public int Shape
        {
            get
            {
                return shape;
            }
            set
            {
                shape = value;
            }
        }


        public void Clear()
        {
            this.title = "名無し";
            this.sourceBounds = new Rectangle(0, 0, 100, 100);
            this.Movement = new Rectangle();
            this.FontName = "ＭＳ ゴシック";
            this.fontSize = 12.0f;
            this.IsVisible = true;
            this.Shape = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        public SceneBox()
        {
            this.Clear();
        }

        public SceneBox Clone()
        {
            SceneBox scene = new SceneBox();

            scene.FontSize = this.FontSize;

            scene.IsMouseOvered = this.IsMouseOvered;

            scene.IsSelected = this.IsSelected;

            scene.IsVisible = this.IsVisible;

            scene.Movement = new Rectangle(
                this.Movement.X,
                this.Movement.Y,
                this.Movement.Width,
                this.Movement.Height
                );

            scene.Shape = this.Shape;

            scene.SourceBounds = new Rectangle(
                this.SourceBounds.X,
                this.SourceBounds.Y,
                this.SourceBounds.Width,
                this.SourceBounds.Height
                );

            scene.Title = this.Title;

            return scene;
        }


        public void Paint(Graphics g)
        {
            // 線の太さ
            float weight;
            if (this.isMouseOvered)
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
            Rectangle bounds2;
            Pen pen;
            if (this.Movement.X != 0 || this.Movement.Y != 0)
            {
                // 少しでも移動しているなら。

                bounds2 = new Rectangle(
                    (int)(this.sourceBounds.X),
                    (int)(this.sourceBounds.Y + weight),
                    (int)(this.sourceBounds.Width),
                    (int)(this.sourceBounds.Height)
                    );

                pen = new Pen(Color.FromArgb(160, 192, 192, 192), weight);

                if (this.Shape == 1)
                {
                    // ひし形
                    Point[] points = new Point[]{
                        new Point( bounds2.X+bounds2.Width/2, bounds2.Y-bounds2.Height/3 ),//上
                        new Point( bounds2.X+bounds2.Width+bounds2.Width/3, bounds2.Y+bounds2.Height/2 ),//右
                        new Point( bounds2.X+bounds2.Width/2, bounds2.Y+bounds2.Height+bounds2.Height/3 ),//下
                        new Point( bounds2.X-bounds2.Width/3, bounds2.Y+bounds2.Height/2 ),//左
                        new Point( bounds2.X+bounds2.Width/2, bounds2.Y-bounds2.Height/3 ),//上
                    };
                    g.DrawLines(pen,points);
                }
                else if (this.Shape == 2)
                {
                    // 解説用
                    g.DrawEllipse(pen, new Rectangle(
                        bounds2.X - 4,
                        bounds2.Y - 4,
                        8,
                        8
                        ));
                }
                else
                {
                    // 枠線
                    g.DrawRectangle(pen, bounds2);
                }

                // タイトル
                g.DrawString(
                    this.title,
                    new Font(this.FontName, this.FontSize),
                    new SolidBrush(Color.FromArgb(160, 192, 192, 192)),
                    new Point(
                        (int)(bounds2.X + weight),
                        (int)(bounds2.Y + weight)
                        )
                    );
            }

            //────────────────────────────────────────
            // 移動後
            //────────────────────────────────────────

            bounds2 = new Rectangle(
                (int)(this.sourceBounds.X + this.Movement.X),// + weight/2f
                (int)(this.sourceBounds.Y + this.Movement.Y + weight), //  / 2f
                (int)(this.sourceBounds.Width + this.Movement.Width),
                (int)(this.sourceBounds.Height + this.Movement.Height)
                );
            pen = new Pen(Color.Black, weight);
            
            // 背景色
            Brush backBrush;
            if (this.IsSelected)
            {
                backBrush = Brushes.Lime;
            }
            else
            {
                backBrush = Brushes.White;
            }

            if (this.Shape == 1)
            {
                // ひし形
                Point[] points = new Point[]{
                        new Point( bounds2.X+bounds2.Width/2, bounds2.Y-bounds2.Height/3 ),//上
                        new Point( bounds2.X+bounds2.Width+bounds2.Width/3, bounds2.Y+bounds2.Height/2 ),//右
                        new Point( bounds2.X+bounds2.Width/2, bounds2.Y+bounds2.Height+bounds2.Height/3 ),//下
                        new Point( bounds2.X-bounds2.Width/3, bounds2.Y+bounds2.Height/2 ),//左
                        new Point( bounds2.X+bounds2.Width/2, bounds2.Y-bounds2.Height/3 ),//上
                    };
                g.FillPolygon(backBrush,points);
                g.DrawLines(pen, points);
            }
            else if (this.Shape == 2)
            {
                // 解説用
                g.DrawEllipse(pen, new Rectangle(
                    bounds2.X - 4,
                    bounds2.Y - 4,
                    8,
                    8
                    ));
            }
            else
            {
                // 枠線
                g.FillRectangle(backBrush, bounds2);
                g.DrawRectangle(pen, bounds2);
            }

            // タイトル
            g.DrawString(
                this.title,
                new Font(this.FontName, this.FontSize),
                Brushes.Black,
                new Point(
                    (int)(bounds2.X + weight),
                    (int)(bounds2.Y + weight)
                    )
                );

        }

        public void Save(StringBuilder sb)
        {
            sb.Append("  <scene");
            sb.Append(" title=\"" + this.Title + "\"");
            sb.Append(" x=\"" + this.SourceBounds.X + "\" y=\"" + this.SourceBounds.Y + "\" width=\"" + this.SourceBounds.Width + "\" height=\"" + this.SourceBounds.Height + "\"");
            sb.Append(" font-name=\"" + this.FontName + "\" font-size=\"" + this.FontSize + "\"");
            sb.Append(" shape=\"" + this.shape + "\"");
            sb.Append(" />");
            sb.Append(Environment.NewLine);

            //ystem.Console.WriteLine("シーン：　座標（" + this.Bounds.X + "," + this.Bounds.Y + "）　タイトル（" + this.Title + "）");
        }

        public void Load(XmlElement xe)
        {
            this.Clear();

            string s;
            int x;
            int y;
            int w;
            int h;
            float fontSize;

            this.Title = xe.GetAttribute("title");
            s = xe.GetAttribute("x");
            int.TryParse(s, out x);
            s = xe.GetAttribute("y");
            int.TryParse(s, out y);
            s = xe.GetAttribute("width");
            int.TryParse(s, out w);
            s = xe.GetAttribute("height");
            int.TryParse(s, out h);

            this.SourceBounds = new Rectangle(
                x,
                y,
                w,
                h
                );

            this.FontName = xe.GetAttribute("font-name");

            s = xe.GetAttribute("font-size");
            if (float.TryParse(s, out fontSize))
            {
                if (0 < fontSize)
                {
                    this.FontSize = fontSize;
                }
                else
                {
                    this.FontSize = 8.0f;// TODO:フォントサイズ
                }
            }
            //ystem.Console.WriteLine("fontSize=" + fontSize + "　this.FontSize=" + this.FontSize);

            int n;
            s = xe.GetAttribute("shape");
            if (int.TryParse(s, out n))
            {
                this.Shape = n;
            }
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

        /// <summary>
        /// マウスが合わさっているかどうかを判定し、状態変更します。
        /// </summary>
        /// <param name="location"></param>
        public void CheckMouseOver(Point location, ref bool forcedOff)
        {
            if (forcedOff)
            {
                this.IsMouseOvered = false;
            }
            else
            {
                if (this.IsHit(location))
                {
                    this.IsMouseOvered = true;
                    forcedOff = true;
                }
                else
                {
                    this.IsMouseOvered = false;
                }
            }
        }

    }
}
