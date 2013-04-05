using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;
using System.Windows.Forms;

namespace Gs_No1
{
    public class GraphicButton
    {
        /// <summary>
        /// 後景色。
        /// </summary>
        private Color backColor;

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
        /// 画像ファイルパス。
        /// </summary>
        private string filePath;
        public string FilePath
        {
            get
            {
                return this.filePath;
            }
            set
            {
                this.filePath = value;
            }
        }

        /// <summary>
        /// 画像。
        /// </summary>
        private Image image;
        public Image Image
        {
            get
            {
                return this.image;
            }
        }

        /// <summary>
        /// 実行プログラム。スイッチオン時。
        /// </summary>
        public delegate void ButtonAction();
        private ButtonAction switchOnAction;
        public ButtonAction SwitchOnAction
        {
            get
            {
                return this.switchOnAction;
            }
            set
            {
                this.switchOnAction = value;
            }
        }

        /// <summary>
        /// 実行プログラム。スイッチオフ時。
        /// </summary>
        private ButtonAction switchOffAction;
        public ButtonAction SwitchOffAction
        {
            get
            {
                return this.switchOffAction;
            }
            set
            {
                this.switchOffAction = value;
            }
        }

        /// <summary>
        /// 他のボタンと識別するために使う名前を入れます。
        /// </summary>
        private string id;
        public string Id
        {
            get
            {
                return this.id;
            }
            set
            {
                this.id = value;
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

        public GraphicButton()
        {
            this.id = "";
            this.backColor = Color.White;
            this.bounds = new Rectangle(0,0,50,50);
            this.switchOnAction = () => {};
            this.switchOffAction = () => {};
            this.filePath = "";
            this.isVisible = true;
        }

        public void Load()
        {
            this.image = System.Drawing.Image.FromFile(this.filePath);
        }

        public void Paint(Graphics g)
        {
            if (this.isVisible)
            {
                // 画像
                if (null != this.image)
                {
                    g.DrawImage(this.image, this.Bounds);
                }

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

                // 枠線の色
                Pen pen;
                if (this.isSelected)
                {
                    pen = new Pen(Color.Green, weight);
                }
                else
                {
                    pen = new Pen(Color.Black, weight);
                }

                // 枠線
                Rectangle bounds2 = new Rectangle(
                        this.bounds.X,
                        this.bounds.Y,
                        this.bounds.Width - 2,
                        this.bounds.Height - 2
                        );
                g.DrawRectangle(pen, bounds2);

                // 半透明の緑色で塗りつぶし
                if (this.isSelected)
                {
                    g.FillRectangle(new SolidBrush(Color.FromArgb(128, 0, 255, 0)), bounds2);
                }
            }
        }

        public void PerformSwitchOn(object sender, MouseEventArgs e)
        {
            this.switchOnAction();
        }

        public void PerformSwitchOff(object sender, MouseEventArgs e)
        {
            this.switchOffAction();
        }

        public void MouseDown(object sender, MouseEventArgs e)
        {
            if (this.isVisible)
            {
                if (this.Bounds.Contains(e.Location))
                {
                    //ystem.Console.WriteLine("範囲内。mouse(" + e.X + "," + e.Y + ") bounds(" + this.Bounds.X + "," + this.Bounds.Y + "," + this.Bounds.Width + "," + this.Bounds.Height + ")");
                    this.isSelected = true;
                }
                else
                {
                    //ystem.Console.WriteLine("境界外。");
                    this.isSelected = false;
                }
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
