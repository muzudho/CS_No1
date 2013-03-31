using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;


namespace Gs_No1
{
    public partial class UiMain : UserControl
    {
        public static readonly int CELL_SIZE = 20;

        /// <summary>
        /// 座標マット。
        /// </summary>
        private CoordMat coordMat;

        /// <summary>
        /// クリックした始点。
        /// </summary>
        private Point clickedLocation;
        private bool isClickedLocationVisible;

        /// <summary>
        /// マウスボタンを離した地点。
        /// </summary>
        private Point releaseMouseButtonLocation;
        private bool isReleaseMouseButtonLocationVisible;

        /// <summary>
        /// 保存ボタン
        /// </summary>
        private GraphicButton saveGbtn;

        /// <summary>
        /// 読込ボタン
        /// </summary>
        private GraphicButton loadGbtn;

        /// <summary>
        /// 座標マット・ボタン
        /// </summary>
        private GraphicButton coordMatGbtn;

        /// <summary>
        /// シーン編集・ボタン
        /// </summary>
        private GraphicButton sceneGbtn;

        /// <summary>
        /// ムーブ・ボタン
        /// </summary>
        private GraphicButton moveGbtn;

        /// <summary>
        /// テキスト・ボタン
        /// </summary>
        private GraphicButton textGbtn;

        /// <summary>
        /// 作成ボタン
        /// </summary>
        private GraphicButton createGbtn;

        /// <summary>
        /// 削除ボタン
        /// </summary>
        private GraphicButton deleteGbtn;

        /// <summary>
        /// 接続ボタン
        /// </summary>
        private GraphicButton connectGbtn;

        /// <summary>
        /// 切断ボタン
        /// </summary>
        private GraphicButton disconnectGbtn;

        /// <summary>
        /// シーンボックス一覧。
        /// </summary>
        private List<SceneBox> sceneBoxList;
        public List<SceneBox> SceneBoxList
        {
            get
            {
                return this.sceneBoxList;
            }
            set
            {
                this.sceneBoxList = value;
            }
        }

        /// <summary>
        /// 接続線一覧
        /// </summary>
        private List<Cable> cableList;
        public List<Cable> CableList
        {
            get
            {
                return this.cableList;
            }
            set
            {
                this.cableList = value;
            }
        }


        /// <summary>
        /// シーン作成回数。0スタート。
        /// </summary>
        private int sceneCreateCounter;

        /// <summary>
        /// マウス・ドラッグの移動量を測ります。
        /// </summary>
        private Point movement;
        public Point Movement
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



        public UiMain()
        {
            this.clickedLocation = new Point();
            // 座標マット
            this.coordMat = new CoordMat();
            this.coordMat.IsSelected = true;
            // セーブボタン
            this.saveGbtn = new GraphicButton();
            this.saveGbtn.Id = "save";
            this.saveGbtn.FilePath = "img/btn_Save.png";
            this.saveGbtn.Bounds = new Rectangle(0 * 50, 0, 50, 50);
            this.saveGbtn.SwitchOnAction = () =>
            {

                StringBuilder sb = new StringBuilder();
                sb.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                sb.Append(Environment.NewLine);
                sb.Append("<scene-tunageru>");
                sb.Append(Environment.NewLine);

                // ──────────
                // 全シーン
                // ──────────
                foreach (SceneBox scene in this.SceneBoxList)
                {
                    scene.Save(sb);
                }

                // ──────────
                // 全接続線
                // ──────────
                foreach (Cable cable in this.CableList)
                {
                    cable.Save(sb);
                }

                sb.Append("</scene-tunageru>");
                sb.Append(Environment.NewLine);
                string xml = sb.ToString();
                System.Console.WriteLine("保存したい。xml="+xml);

                // テキストファイルとして上書き。
                System.IO.File.WriteAllText("save.xml", xml, Encoding.UTF8);
            };
            // ロードボタン
            this.loadGbtn = new GraphicButton();
            this.loadGbtn.Id = "load";
            this.loadGbtn.FilePath = "img/btn_Load.png";
            this.loadGbtn.Bounds = new Rectangle(0 * 50, 1*50, 50, 50);
            this.loadGbtn.SwitchOnAction = () =>
            {
                System.Console.WriteLine("ロードしたい。");

                // ──────────
                // クリアー
                // ──────────
                this.SceneBoxList.Clear();
                this.CableList.Clear();


                XmlDocument doc = new XmlDocument();
                doc.Load("save.xml");
                System.Console.WriteLine("要素数="+doc.DocumentElement.ChildNodes.Count);

                foreach (XmlNode xn in doc.DocumentElement.ChildNodes)
                {
                    if (xn.NodeType == XmlNodeType.Element)
                    {
                        XmlElement xe = (XmlElement)xn;
                        string s;
                        bool b;
                        int x;
                        int y;
                        int w;
                        int h;
                        switch (xe.Name)
                        {
                            case "scene":
                                SceneBox scene = new SceneBox();
                                scene.Title = xe.GetAttribute("title");
                                s = xe.GetAttribute("x");
                                int.TryParse(s, out x);
                                s = xe.GetAttribute("y");
                                int.TryParse(s, out y);
                                s = xe.GetAttribute("width");
                                int.TryParse(s, out w);
                                s = xe.GetAttribute("height");
                                int.TryParse(s, out h);
                                scene.SourceBounds = new Rectangle(x, y, w, h);
                                this.SceneBoxList.Add(scene);
                                break;
                            case "cable":
                                Cable cable = new Cable();

                                s = xe.GetAttribute("x0");
                                int.TryParse(s, out x);
                                s = xe.GetAttribute("y0");
                                int.TryParse(s, out y);
                                cable.SourceBounds[0] = new Rectangle(x,y,UiMain.CELL_SIZE, UiMain.CELL_SIZE);
                                s = xe.GetAttribute("visible0");
                                bool.TryParse(s,out b);
                                cable.IsVisible[0] = b;

                                s = xe.GetAttribute("x1");
                                int.TryParse(s, out x);
                                s = xe.GetAttribute("y1");
                                int.TryParse(s, out y);
                                cable.SourceBounds[1] = new Rectangle(x, y, UiMain.CELL_SIZE, UiMain.CELL_SIZE);
                                s = xe.GetAttribute("visible1");
                                bool.TryParse(s,out b);
                                cable.IsVisible[1] = b;

                                this.CableList.Add(cable);
                                break;
                        }
                    }
                }

                this.Refresh();
            };
            // 座標マットボタン
            this.coordMatGbtn = new GraphicButton();
            this.coordMatGbtn.Id = "coordMat";
            this.coordMatGbtn.FilePath = "img/btn_CoordMat.png";
            this.coordMatGbtn.Bounds = new Rectangle(2*50, 0, 50, 50);
            this.coordMatGbtn.IsSelected = true;
            this.coordMatGbtn.SwitchOnAction = () => { this.coordMat.IsSelected = true; };
            this.coordMatGbtn.SwitchOffAction = () => { this.coordMat.IsSelected = false; };
            // シーンボタン
            this.sceneGbtn = new GraphicButton();
            this.sceneGbtn.Id = "scene";
            this.sceneGbtn.FilePath = "img/btn_Scene.png";
            this.sceneGbtn.Bounds = new Rectangle(3*50, 0, 50, 50);
            this.sceneGbtn.SwitchOnAction = () => {
                this.textGbtn.IsVisible = true;
                this.createGbtn.IsVisible = true;
                this.deleteGbtn.IsVisible = true;
                this.connectGbtn.IsVisible = true;
                this.disconnectGbtn.IsVisible = true;
            };
            this.sceneGbtn.SwitchOffAction = () =>
            {
                this.textGbtn.IsVisible = false;
                this.createGbtn.IsVisible = false;
                this.deleteGbtn.IsVisible = false;
                this.connectGbtn.IsVisible = false;
                this.disconnectGbtn.IsVisible = false;
            };
            // 移動ボタン
            this.moveGbtn = new GraphicButton();
            this.moveGbtn.Id = "move";
            this.moveGbtn.FilePath = "img/btn_Move.png";
            this.moveGbtn.Bounds = new Rectangle(2*50, 50, 50, 50);
            this.moveGbtn.IsSelected = true;
            // テキストボタン
            this.textGbtn = new GraphicButton();
            this.textGbtn.Id = "text";
            this.textGbtn.FilePath = "img/btn_Text.png";
            this.textGbtn.Bounds = new Rectangle(3*50, 50, 50, 50);
            this.textGbtn.IsVisible = false;
            // 作成ボタン
            this.createGbtn = new GraphicButton();
            this.createGbtn.Id = "create";
            this.createGbtn.FilePath = "img/btn_Create.png";
            this.createGbtn.Bounds = new Rectangle(4*50, 50, 50, 50);
            this.createGbtn.IsVisible = false;
            this.createGbtn.SwitchOnAction = () =>
                {
                    //既存シーンボックスの選択解除
                    foreach (SceneBox scene2 in this.SceneBoxList)
                    {
                        scene2.IsSelected = false;
                    }

                    //シーンボックス作成
                    SceneBox scene = new SceneBox();
                    scene.Title = "シーン"+this.sceneCreateCounter;
                    scene.SourceBounds = new Rectangle(
                        this.coordMat.SourceBounds.X,
                        this.coordMat.SourceBounds.Y,
                        6 * UiMain.CELL_SIZE,
                        3*UiMain.CELL_SIZE
                        );
                    scene.FontSize = 12.0f;
                    scene.IsSelected = true;
                    this.SceneBoxList.Add(scene);

                    this.sceneCreateCounter++;
                };
            // 削除ボタン
            this.deleteGbtn = new GraphicButton();
            this.deleteGbtn.Id = "delete";
            this.deleteGbtn.FilePath = "img/btn_Delete.png";
            this.deleteGbtn.Bounds = new Rectangle(5*50, 50, 50, 50);
            this.deleteGbtn.IsVisible = false;
            // 接続ボタン
            this.connectGbtn = new GraphicButton();
            this.connectGbtn.Id = "connect";
            this.connectGbtn.FilePath = "img/btn_Connect.png";
            this.connectGbtn.Bounds = new Rectangle(6 * 50, 1 * 50, 50, 50);
            this.connectGbtn.IsVisible = false;
            this.connectGbtn.SwitchOnAction = () =>
            {
                // 全シーン選択解除
                foreach (SceneBox scene in this.SceneBoxList)
                {
                    scene.IsSelected = false;
                }
            };
            // 切断ボタン
            this.disconnectGbtn = new GraphicButton();
            this.disconnectGbtn.Id = "disconnect";
            this.disconnectGbtn.FilePath = "img/btn_Disconnect.png";
            this.disconnectGbtn.Bounds = new Rectangle(7 * 50, 1 * 50, 50, 50);
            this.disconnectGbtn.IsVisible = false;
            this.disconnectGbtn.SwitchOnAction = () =>
            {
                // 全シーン選択解除
                foreach (SceneBox scene in this.SceneBoxList)
                {
                    scene.IsSelected = false;
                }
            };

            this.sceneBoxList = new List<SceneBox>();
            this.cableList = new List<Cable>();
            this.Movement = new Point();
            InitializeComponent();
            this.textBox1.Font = new Font("ＭＳ ゴシック", 12.0f);
        }

        /// <summary>
        /// クリックした地点と、マウスボタンを放した地点に丸を描き、直線で結びます。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UiMain_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            // 枠線
            g.DrawRectangle(Pens.Red, 0, 0, this.Width-1, this.Height-1);

            // 座標マット
            this.coordMat.Paint(g);

            // 各種ボタン
            this.saveGbtn.Paint(g);
            this.loadGbtn.Paint(g);
            this.coordMatGbtn.Paint(g);
            this.sceneGbtn.Paint(g);
            this.moveGbtn.Paint(g);
            this.textGbtn.Paint(g);
            this.createGbtn.Paint(g);
            this.deleteGbtn.Paint(g);
            this.connectGbtn.Paint(g);
            this.disconnectGbtn.Paint(g);

            // シーンボックス
            foreach (SceneBox scene in this.SceneBoxList)
            {
                scene.Paint(g);
            }

            // ケーブル
            foreach (Cable cable in this.CableList)
            {
                cable.Paint(g);
            }

            // クリック地点
            if(this.isClickedLocationVisible)
            {
                g.FillEllipse(
                    new SolidBrush( Color.FromArgb(128,0,0,255)),
                    this.clickedLocation.X-5,
                    this.clickedLocation.Y-5,
                    10,
                    10
                    );
            }

            // マウスボタンを放した地点
            if(this.isReleaseMouseButtonLocationVisible)
            {
                g.FillEllipse(
                    Brushes.Blue,
                    this.releaseMouseButtonLocation.X - 5,
                    this.releaseMouseButtonLocation.Y - 5,
                    10,
                    10
                    );

                // 線を引きます。
                g.DrawLine(
                    Pens.Blue,
                    this.clickedLocation,
                    this.releaseMouseButtonLocation
                    );

                // 構成部品の移動量をクリアーします。
                this.coordMat.Movement = new Rectangle();
                foreach (SceneBox scene in this.SceneBoxList)
                {
                    scene.Movement = new Rectangle();
                }
            }
        }

        private void UiMain_Resize(object sender, EventArgs e)
        {
            this.Refresh();
        }

        private void UiMain_MouseUp(object sender, MouseEventArgs e)
        {
            //────────────────────────────────────────
            // 何もないところでマウスボタンを放したかどうか
            //────────────────────────────────────────
            bool actorReleased = false;

            // セーブ
            if (this.saveGbtn.Bounds.Contains(e.Location))
            {
                this.saveGbtn.PerformSwitchOn(sender, e);
                actorReleased = true;
            }

            // ロード
            if (this.loadGbtn.Bounds.Contains(e.Location))
            {
                this.loadGbtn.PerformSwitchOn(sender, e);
                actorReleased = true;
            }

            // 座標マット
            if (this.coordMatGbtn.Bounds.Contains(e.Location))
            {
                this.coordMatGbtn.PerformSwitchOn(sender, e);
                actorReleased = true;
            }

            // シーン
            if (this.sceneGbtn.Bounds.Contains(e.Location))
            {
                this.sceneGbtn.PerformSwitchOn(sender, e);
                actorReleased = true;
            }

            // 移動
            if (this.moveGbtn.Bounds.Contains(e.Location))
            {
                this.moveGbtn.PerformSwitchOn(sender, e);
                actorReleased = true;
            }

            // テキスト
            if (this.textGbtn.Bounds.Contains(e.Location))
            {
                this.textGbtn.PerformSwitchOn(sender, e);
                actorReleased = true;
            }

            // 作成
            if (this.createGbtn.Bounds.Contains(e.Location))
            {
                this.createGbtn.PerformSwitchOn(sender, e);
                actorReleased = true;
            }

            // 削除
            if (this.deleteGbtn.Bounds.Contains(e.Location))
            {
                this.deleteGbtn.PerformSwitchOn(sender, e);
                actorReleased = true;
            }

            // 接続
            if (this.connectGbtn.Bounds.Contains(e.Location))
            {
                this.connectGbtn.PerformSwitchOn(sender, e);
                actorReleased = true;
            }

            // 切断
            if (this.disconnectGbtn.Bounds.Contains(e.Location))
            {
                this.disconnectGbtn.PerformSwitchOn(sender, e);
                actorReleased = true;
            }

            foreach (SceneBox scene in this.SceneBoxList)
            {
                if (scene.Bounds.Contains(e.Location))
                {
                    actorReleased = true;
                }
            }

            // 何もないところの上で放したとき
            if (!actorReleased)
            {
            }





            // マウスボタンを放した地点を設定。
            this.releaseMouseButtonLocation = e.Location;

            bool isMoveCoordMat = false;
            int moveSceneFlg = 0;// 1:全シーン移動。2:選択シーンのみ移動。
            bool moveCableFlg = false;
            // 座標マットモードの場合
            if (this.coordMatGbtn.IsSelected)
            {
                isMoveCoordMat = true;
                moveSceneFlg = 1;
                moveCableFlg = true;
            }
            // シーンモードの場合
            if (this.sceneGbtn.IsSelected)
            {
                moveSceneFlg = 2;
            }


            // 選択されている構成部品があれば、移動量分だけ移動します。
            if (isMoveCoordMat)
            {
                this.coordMat.SourceBounds = new Rectangle(
                    this.coordMat.SourceBounds.X + this.Movement.X,
                    this.coordMat.SourceBounds.Y + this.Movement.Y,
                    this.coordMat.SourceBounds.Width,
                    this.coordMat.SourceBounds.Height
                    );
                this.coordMat.Movement = new Rectangle();
            }

            if (moveSceneFlg == 1)
            {
                // ──────────
                // 全シーン　移動量セット
                // ──────────
                foreach (SceneBox scene in this.SceneBoxList)
                {
                    int x;
                    int y;
                    if (isMoveCoordMat)
                    {
                        x = scene.SourceBounds.X + this.Movement.X;
                        y = scene.SourceBounds.Y + this.Movement.Y;
                    }
                    else
                    {
                        x = scene.SourceBounds.X + this.Movement.X - this.Movement.X % UiMain.CELL_SIZE;
                        y = scene.SourceBounds.Y + this.Movement.Y - this.Movement.Y % UiMain.CELL_SIZE;
                    }
                    scene.SourceBounds = new Rectangle(
                        x,
                        y,
                        scene.SourceBounds.Width,
                        scene.SourceBounds.Height
                        );
                    scene.Movement = new Rectangle();
                }
            }
            else if (moveSceneFlg == 2)
            {
                // ──────────
                // 選択シーン　移動量セット
                // ──────────
                foreach (SceneBox scene in this.SceneBoxList)
                {
                    if (scene.IsSelected)
                    {
                        scene.SourceBounds = new Rectangle(
                            scene.SourceBounds.X + this.Movement.X - this.Movement.X % UiMain.CELL_SIZE,
                            scene.SourceBounds.Y + this.Movement.Y - this.Movement.Y % UiMain.CELL_SIZE,
                            scene.SourceBounds.Width,
                            scene.SourceBounds.Height
                            );
                        scene.Movement = new Rectangle();
                    }
                }
            }

            if (moveCableFlg)
            {
                // 全ての接続線を、移動量分だけ移動します。
                foreach (Cable cable in this.CableList)
                {
                    // ──────────
                    // [0]起点　[1]終点
                    // ──────────
                    for (int i = 0; i < 2; i++)
                    {
                        int x;
                        int y;
                        if (isMoveCoordMat)
                        {
                            x = cable.SourceBounds[i].X + this.Movement.X;
                            y = cable.SourceBounds[i].Y + this.Movement.Y;
                        }
                        else
                        {
                            x = cable.SourceBounds[i].X + this.Movement.X - this.Movement.X % UiMain.CELL_SIZE;
                            y = cable.SourceBounds[i].Y + this.Movement.Y - this.Movement.Y % UiMain.CELL_SIZE;
                        }
                        cable.SourceBounds[i] = new Rectangle(
                            x,
                            y,
                            cable.SourceBounds[i].Width,
                            cable.SourceBounds[i].Height
                            );
                        cable.Movement[i] = new Rectangle();
                    }

                }
            }

            this.isReleaseMouseButtonLocationVisible = true;


            this.Refresh();
        }

        private void UiMain_MouseDown(object sender, MouseEventArgs e)
        {

            //────────────────────────────────────────
            // テキストボックス→シーン名
            //────────────────────────────────────────
            if (this.textBox1.Visible)
            {
                foreach (SceneBox scene in this.SceneBoxList)
                {
                    if (scene.IsSelected)
                    {
                        scene.Title = this.textBox1.Text;
                    }
                }

                // フォーカスを外します。
                this.ActiveControl = null;
            }

            //────────────────────────────────────────
            // 何もないところでマウスボタン押下したかどうか
            //────────────────────────────────────────
            bool actorPressed = false;

            // セーブ
            if (this.saveGbtn.Bounds.Contains(e.Location))
            {
                actorPressed = true;
            }

            // ロード
            if (this.loadGbtn.Bounds.Contains(e.Location))
            {
                actorPressed = true;
            }

            // 座標マット
            if (this.coordMatGbtn.Bounds.Contains(e.Location))
            {
                actorPressed = true;
            }

            // シーン
            if (this.sceneGbtn.Bounds.Contains(e.Location))
            {
                actorPressed = true;
            }

            // 移動
            if (this.moveGbtn.Bounds.Contains(e.Location))
            {
                actorPressed = true;
            }

            // テキスト
            if (this.textGbtn.Bounds.Contains(e.Location))
            {
                actorPressed = true;
            }

            // 作成
            if (this.createGbtn.Bounds.Contains(e.Location))
            {
                actorPressed = true;
            }

            // 削除
            if (this.deleteGbtn.Bounds.Contains(e.Location))
            {
                actorPressed = true;
            }

            // 接続
            if (this.connectGbtn.Bounds.Contains(e.Location))
            {
                actorPressed = true;
            }

            // 切断
            if (this.disconnectGbtn.Bounds.Contains(e.Location))
            {
                actorPressed = true;
            }

            foreach (SceneBox scene in this.SceneBoxList)
            {
                if (scene.Bounds.Contains(e.Location))
                {
                    actorPressed = true;
                }
            }

            // 何もないところでマウスボタン押下したとき
            if (!actorPressed)
            {
                // シーンボックスの選択を解除
                foreach (SceneBox scene in this.SceneBoxList)
                {
                    scene.IsSelected = false;
                }
            }



            // マウスボタンを押した地点を設定。
            this.clickedLocation = e.Location;
            this.isClickedLocationVisible = true;

            // マウスボタンを放した地点を消す。
            this.isReleaseMouseButtonLocationVisible = false;

            // ラジオボタンのように。
            // 今回、マウスボタンで押されたボタン
            string pressedBtn="";
            if(this.coordMatGbtn.Bounds.Contains(e.Location))
            {
                pressedBtn = this.coordMatGbtn.Id;
            }
            else if(this.sceneGbtn.Bounds.Contains(e.Location))
            {
                pressedBtn = this.sceneGbtn.Id;
            }

            if ("" != pressedBtn)
            {
                // 選択されていればオン、選択されていなければオフ。
                if (this.coordMatGbtn.Id == pressedBtn)
                {
                    this.coordMatGbtn.IsSelected = true;
                }
                else
                {
                    this.coordMatGbtn.IsSelected = false;
                    this.coordMatGbtn.SwitchOffAction();
                }

                if (this.sceneGbtn.Id == pressedBtn)
                {
                    this.sceneGbtn.IsSelected = true;
                }
                else
                {
                    this.sceneGbtn.IsSelected = false;
                    this.sceneGbtn.SwitchOffAction();
                }
            }

            // ラジオボタンのように。
            // 今回、マウスボタンで押されたボタン
            pressedBtn = "";
            if (this.moveGbtn.Bounds.Contains(e.Location))
            {
                pressedBtn = this.moveGbtn.Id;
            }
            else if (this.textGbtn.Bounds.Contains(e.Location))
            {
                pressedBtn = this.textGbtn.Id;
            }
            else if (this.createGbtn.Bounds.Contains(e.Location))
            {
                pressedBtn = this.createGbtn.Id;
            }
            else if (this.deleteGbtn.Bounds.Contains(e.Location))
            {
                pressedBtn = this.deleteGbtn.Id;
            }
            else if (this.connectGbtn.Bounds.Contains(e.Location))
            {
                pressedBtn = this.connectGbtn.Id;
            }
            else if (this.disconnectGbtn.Bounds.Contains(e.Location))
            {
                pressedBtn = this.disconnectGbtn.Id;
            }

            // 選択されていればオン、選択されていなければオフ。
            if ("" != pressedBtn)
            {
                // 移動
                if (this.moveGbtn.Id == pressedBtn)
                {
                    this.moveGbtn.IsSelected = true;
                }
                else
                {
                    this.moveGbtn.IsSelected = false;
                    this.moveGbtn.SwitchOffAction();
                }

                // テキスト
                if (this.textGbtn.Id == pressedBtn)
                {
                    this.textGbtn.IsSelected = true;
                }
                else
                {
                    this.textGbtn.IsSelected = false;
                    this.textGbtn.SwitchOffAction();
                }

                // 作成
                if (this.createGbtn.Id == pressedBtn)
                {
                    this.createGbtn.IsSelected = true;
                }
                else
                {
                    this.createGbtn.IsSelected = false;
                    this.createGbtn.SwitchOffAction();
                }

                // 削除
                if (this.deleteGbtn.Id == pressedBtn)
                {
                    this.deleteGbtn.IsSelected = true;
                }
                else
                {
                    this.deleteGbtn.IsSelected = false;
                    this.deleteGbtn.SwitchOffAction();
                }

                // 接続
                if (this.connectGbtn.Id == pressedBtn)
                {
                    this.connectGbtn.IsSelected = true;
                }
                else
                {
                    this.connectGbtn.IsSelected = false;
                    this.connectGbtn.SwitchOffAction();
                }

                // 切断
                if (this.disconnectGbtn.Id == pressedBtn)
                {
                    this.disconnectGbtn.IsSelected = true;
                }
                else
                {
                    this.disconnectGbtn.IsSelected = false;
                    this.disconnectGbtn.SwitchOffAction();
                }
            }

            if (this.sceneGbtn.IsSelected)
            {
                if (!this.connectGbtn.IsSelected && !this.disconnectGbtn.IsSelected)
                {
                    //────────────────────────────────────────
                    // シーンモード　ただし、接続、切断モードでない
                    //────────────────────────────────────────

                    // シーンボックスの選択
                    SceneBox scene2 = null;
                    foreach (SceneBox scene in this.SceneBoxList)
                    {
                        if (scene.Bounds.Contains(e.Location))
                        {
                            scene2 = scene;
                            // 最初の１件のみ
                            break;
                        }
                    }

                    if (null != scene2)
                    {
                        if (this.textGbtn.IsSelected)
                        {
                            //────────────────────────────────────────
                            // テキストモード
                            //────────────────────────────────────────

                            //他のシーンを選択解除
                            foreach (SceneBox scene in this.SceneBoxList)
                            {
                                scene.IsSelected = false;
                            }

                            scene2.IsSelected = true;
                            this.textBox1.Visible = true;
                            this.textBox1.Bounds = new Rectangle(
                                scene2.Bounds.X,
                                scene2.Bounds.Y,
                                scene2.Bounds.Width,
                                scene2.Bounds.Height
                                );
                            this.textBox1.Text = scene2.Title;
                            this.textBox1.Focus();
                            this.textBox1.SelectAll();
                        }
                        else if (this.deleteGbtn.IsSelected)
                        {
                            //────────────────────────────────────────
                            // 削除モード
                            //────────────────────────────────────────
                            this.SceneBoxList.Remove(scene2);
                        }
                        else
                        {
                            scene2.IsSelected = true;
                        }

                    }
                }
                else if (this.connectGbtn.IsSelected)
                {
                    //────────────────────────────────────────
                    // シーンモード　ただし、接続モード
                    //────────────────────────────────────────

                    Cable cable = null;
                    if (0 < this.CableList.Count)
                    {
                        // 既存ケーブルの最終要素
                        cable = this.CableList[this.CableList.Count-1];

                        // [0]起点　かつ　[1]終点　の両方が表示されている場合、新しく追加。
                        if (cable.IsVisible[0] && cable.IsVisible[1])
                        {
                            cable = new Cable();
                            this.CableList.Add(cable);
                        }
                    }
                    else
                    {
                        cable = new Cable();
                        this.CableList.Add(cable);
                    }

                    // ──────────
                    // [0]起点　[1]終点
                    // ──────────
                    int i;
                    if (!cable.IsVisible[0])
                    {
                        i = 0;
                    }
                    else
                    {
                        i = 1;
                    }

                    cable.IsVisible[i] = true;
                    cable.SourceBounds[i] = new Rectangle(
                        (e.Location.X) - (e.Location.X - this.coordMat.Bounds.X) % UiMain.CELL_SIZE,
                        (e.Location.Y) - (e.Location.Y - this.coordMat.Bounds.Y) % UiMain.CELL_SIZE,
                        UiMain.CELL_SIZE,
                        UiMain.CELL_SIZE
                        );
                }
                else
                {
                    //────────────────────────────────────────
                    // シーンモード　ただし、切断モード
                    //────────────────────────────────────────

                    // 削除するケーブルの一覧
                    List<Cable> delete = new List<Cable>();

                    foreach (Cable cable in this.CableList)
                    {
                        if (cable.Bounds[0].Contains(e.Location))
                        {
                            delete.Add(cable);
                        }
                        else if (cable.Bounds[1].Contains(e.Location))
                        {
                            delete.Add(cable);
                        }
                    }

                    foreach (Cable cable in delete)
                    {
                        this.CableList.Remove(cable);
                    }

                }
            }

            this.Refresh();
        }

        private void UiMain_Load(object sender, EventArgs e)
        {
            this.saveGbtn.Load();
            this.loadGbtn.Load();
            this.coordMatGbtn.Load();
            this.sceneGbtn.Load();
            this.moveGbtn.Load();
            this.textGbtn.Load();
            this.createGbtn.Load();
            this.deleteGbtn.Load();
            this.connectGbtn.Load();
            this.disconnectGbtn.Load();
        }

        private void UiMain_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.textGbtn.IsSelected)
            {
                //────────────────────────────────────────
                // テキストモード
                //────────────────────────────────────────

                // 移動モード禁止。
            }
            else
            {
                if (this.isClickedLocationVisible && !this.isReleaseMouseButtonLocationVisible)
                {
                    // マウス・ドラッグの移動量を測ります。
                    this.Movement = new Point(
                        e.Location.X - this.clickedLocation.X,
                        e.Location.Y - this.clickedLocation.Y
                        );
                    //System.Console.WriteLine("ドラッグ中 e(" + e.Location.X + "," + e.Location.Y + ") click(" + this.clickedLocation.X + "," + this.clickedLocation.Y + ") move(" + this.Movement.X + "," + this.Movement.Y + ")");

                    bool isMoveCoordMat = false;
                    int moveSceneFlg = 0;// 1:全シーン移動。2:選択シーンのみ移動。
                    bool moveCableFlg = false;
                    // 座標マットモードの場合
                    if (this.coordMatGbtn.IsSelected)
                    {
                        isMoveCoordMat = true;
                        moveSceneFlg = 1;
                        moveCableFlg = true;
                    }
                    // シーンモードの場合
                    if (this.sceneGbtn.IsSelected)
                    {
                        moveSceneFlg = 2;
                    }

                    // 選択している構成部品に、移動量をセットします。
                    if (isMoveCoordMat)
                    {
                        this.coordMat.Movement = new Rectangle(this.Movement.X, this.Movement.Y, 0, 0);
                    }

                    if (moveSceneFlg == 1)
                    {
                        // ──────────
                        // 全シーン　移動量セット
                        // ──────────
                        foreach (SceneBox scene in this.SceneBoxList)
                        {
                            int x;
                            int y;
                            if (isMoveCoordMat)
                            {
                                x = this.Movement.X;
                                y = this.Movement.Y;
                            }
                            else
                            {
                                x = this.Movement.X - this.Movement.X % UiMain.CELL_SIZE;
                                y = this.Movement.Y - this.Movement.Y % UiMain.CELL_SIZE;
                            }

                            scene.Movement = new Rectangle(
                                x,
                                y,
                                0, 0);
                        }
                    }
                    else if (moveSceneFlg == 2)
                    {
                        // ──────────
                        // 選択シーン　移動量セット
                        // ──────────
                        foreach (SceneBox scene in this.SceneBoxList)
                        {
                            if (scene.IsSelected)
                            {
                                scene.Movement = new Rectangle(
                                    this.Movement.X - this.Movement.X % UiMain.CELL_SIZE,
                                    this.Movement.Y - this.Movement.Y % UiMain.CELL_SIZE,
                                    0, 0);
                            }
                        }
                    }

                    if (moveCableFlg)
                    {
                        // ──────────
                        // 全接続線　移動量セット
                        // ──────────
                        foreach (Cable cable in this.CableList)
                        {
                            // ──────────
                            // [0]起点　[1]終点
                            // ──────────
                            for (int i = 0; i < 2; i++)
                            {
                                int x;
                                int y;
                                if (isMoveCoordMat)
                                {
                                    x = this.Movement.X;
                                    y = this.Movement.Y;
                                }
                                else
                                {
                                    x = this.Movement.X - this.Movement.X % UiMain.CELL_SIZE;
                                    y = this.Movement.Y - this.Movement.Y % UiMain.CELL_SIZE;
                                }

                                cable.Movement[i] = new Rectangle(
                                    x,
                                    y,
                                    0, 0);
                            }

                        }
                    }
                }
                else
                {
                    this.Movement = new Point();
                }
            }

            this.Refresh();
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            this.textBox1.Visible = false;
        }
    }
}
