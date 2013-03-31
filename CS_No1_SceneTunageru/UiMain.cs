using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Gs_No1
{
    public partial class UiMain : UserControl
    {

        /// <summary>
        /// 座標マット。
        /// </summary>
        private CoordinateMat coordMat;

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
            this.coordMat = new CoordinateMat();
            this.coordMat.IsSelected = true;
            this.clickedLocation = new Point();
            this.coordMatGbtn = new GraphicButton();
            this.coordMatGbtn.Id = "coordMat";
            this.coordMatGbtn.FilePath = "img/btn_CoordMat.png";
            this.coordMatGbtn.Bounds = new Rectangle(50, 0, 50, 50);
            this.coordMatGbtn.IsSelected = true;
            this.coordMatGbtn.SwitchOnAction = () => { this.coordMat.IsSelected = true; };
            this.coordMatGbtn.SwitchOffAction = () => { this.coordMat.IsSelected = false; };
            this.sceneGbtn = new GraphicButton();
            this.sceneGbtn.Id = "scene";
            this.sceneGbtn.FilePath = "img/btn_Scene.png";
            this.sceneGbtn.Bounds = new Rectangle(100, 0, 50, 50);
            this.sceneGbtn.SwitchOnAction = () => {
                this.textGbtn.IsVisible = true;
                this.createGbtn.IsVisible = true;
                this.deleteGbtn.IsVisible = true;
            };
            this.sceneGbtn.SwitchOffAction = () =>
            {
                this.textGbtn.IsVisible = false;
                this.createGbtn.IsVisible = false;
                this.deleteGbtn.IsVisible = false;
            };
            this.moveGbtn = new GraphicButton();
            this.moveGbtn.Id = "move";
            this.moveGbtn.FilePath = "img/btn_Move.png";
            this.moveGbtn.Bounds = new Rectangle( 50, 50, 50, 50);
            this.moveGbtn.IsSelected = true;
            this.textGbtn = new GraphicButton();
            this.textGbtn.Id = "text";
            this.textGbtn.FilePath = "img/btn_Text.png";
            this.textGbtn.Bounds = new Rectangle(100, 50, 50, 50);
            this.textGbtn.IsVisible = false;
            this.createGbtn = new GraphicButton();
            this.createGbtn.Id = "create";
            this.createGbtn.FilePath = "img/btn_Create.png";
            this.createGbtn.Bounds = new Rectangle(150, 50, 50, 50);
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
                        100,
                        100,
                        scene.SourceBounds.Width,
                        scene.SourceBounds.Height
                        );
                    scene.IsSelected = true;
                    this.SceneBoxList.Add(scene);

                    this.sceneCreateCounter++;
                };
            this.deleteGbtn = new GraphicButton();
            this.deleteGbtn.Id = "delete";
            this.deleteGbtn.FilePath = "img/btn_Delete.png";
            this.deleteGbtn.Bounds = new Rectangle(200, 50, 50, 50);
            this.deleteGbtn.IsVisible = false;
            this.sceneBoxList = new List<SceneBox>();
            this.Movement = new Point();
            InitializeComponent();
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
            this.coordMatGbtn.Paint(g);
            this.sceneGbtn.Paint(g);
            this.moveGbtn.Paint(g);
            this.textGbtn.Paint(g);
            this.createGbtn.Paint(g);
            this.deleteGbtn.Paint(g);

            // シーンボックス
            foreach (SceneBox scene in this.SceneBoxList)
            {
                scene.Paint(g);
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

            //座標マットボタンの上で放したとき
            if (this.coordMatGbtn.Bounds.Contains(e.Location))
            {
                this.coordMatGbtn.PerformSwitchOn(sender, e);
                actorReleased = true;
            }

            //シーンボタンの上で放したとき
            if (this.sceneGbtn.Bounds.Contains(e.Location))
            {
                this.sceneGbtn.PerformSwitchOn(sender, e);
                actorReleased = true;
            }

            //移動ボタンの上で放したとき
            if (this.moveGbtn.Bounds.Contains(e.Location))
            {
                this.moveGbtn.PerformSwitchOn(sender, e);
                actorReleased = true;
            }

            //テキストボタンの上で放したとき
            if (this.textGbtn.Bounds.Contains(e.Location))
            {
                this.textGbtn.PerformSwitchOn(sender, e);
                actorReleased = true;
            }

            //作成ボタンの上で放したとき
            if (this.createGbtn.Bounds.Contains(e.Location))
            {
                this.createGbtn.PerformSwitchOn(sender, e);
                actorReleased = true;
            }

            //削除ボタンの上で放したとき
            if (this.deleteGbtn.Bounds.Contains(e.Location))
            {
                this.deleteGbtn.PerformSwitchOn(sender, e);
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
            // 座標マットモードの場合
            if (this.coordMatGbtn.IsSelected)
            {
                isMoveCoordMat = true;
                moveSceneFlg = 1;
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
                // 全てのシーンを、移動量分だけ移動します。
                foreach (SceneBox scene in this.SceneBoxList)
                {
                    scene.SourceBounds = new Rectangle(
                        scene.SourceBounds.X + this.Movement.X,
                        scene.SourceBounds.Y + this.Movement.Y,
                        scene.SourceBounds.Width,
                        scene.SourceBounds.Height
                        );
                    scene.Movement = new Rectangle();
                }
            }
            else if (moveSceneFlg == 2)
            {
                // 選択されているシーンがあれば、移動量分だけ移動します。
                foreach (SceneBox scene in this.SceneBoxList)
                {
                    if (scene.IsSelected)
                    {
                        scene.SourceBounds = new Rectangle(
                            scene.SourceBounds.X + this.Movement.X,
                            scene.SourceBounds.Y + this.Movement.Y,
                            scene.SourceBounds.Width,
                            scene.SourceBounds.Height
                            );
                        scene.Movement = new Rectangle();
                    }
                }
            }

            this.isReleaseMouseButtonLocationVisible = true;


            this.Refresh();
        }

        private void UiMain_MouseDown(object sender, MouseEventArgs e)
        {
            //────────────────────────────────────────
            // 何もないところでマウスボタン押下したかどうか
            //────────────────────────────────────────
            bool actorPressed = false;

            //座標マットボタンを押したとき
            if (this.coordMatGbtn.Bounds.Contains(e.Location))
            {
                actorPressed = true;
            }

            //シーンボタンを押したとき
            if (this.sceneGbtn.Bounds.Contains(e.Location))
            {
                actorPressed = true;
            }

            //移動ボタンを押したとき
            if (this.moveGbtn.Bounds.Contains(e.Location))
            {
                actorPressed = true;
            }

            //テキストボタンを押したとき
            if (this.textGbtn.Bounds.Contains(e.Location))
            {
                actorPressed = true;
            }

            //作成ボタンを押したとき
            if (this.createGbtn.Bounds.Contains(e.Location))
            {
                actorPressed = true;
            }

            //削除ボタンを押したとき
            if (this.deleteGbtn.Bounds.Contains(e.Location))
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

            // 選択されていればオン、選択されていなければオフ。
            if ("" != pressedBtn)
            {
                if (this.moveGbtn.Id == pressedBtn)
                {
                    this.moveGbtn.IsSelected = true;
                }
                else
                {
                    this.moveGbtn.IsSelected = false;
                    this.moveGbtn.SwitchOffAction();
                }

                if (this.textGbtn.Id == pressedBtn)
                {
                    this.textGbtn.IsSelected = true;
                }
                else
                {
                    this.textGbtn.IsSelected = false;
                    this.textGbtn.SwitchOffAction();
                }

                if (this.createGbtn.Id == pressedBtn)
                {
                    this.createGbtn.IsSelected = true;
                }
                else
                {
                    this.createGbtn.IsSelected = false;
                    this.createGbtn.SwitchOffAction();
                }

                if (this.deleteGbtn.Id == pressedBtn)
                {
                    this.deleteGbtn.IsSelected = true;
                }
                else
                {
                    this.deleteGbtn.IsSelected = false;
                    this.deleteGbtn.SwitchOffAction();
                }
            }

            if (this.sceneGbtn.IsSelected)
            {
                //────────────────────────────────────────
                // シーンモード
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
                        // テキスト・モードのとき

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
                    else
                    {
                        scene2.IsSelected = true;
                    }

                }
            }

            this.Refresh();
        }

        private void UiMain_Load(object sender, EventArgs e)
        {
            this.coordMatGbtn.Load();
            this.sceneGbtn.Load();
            this.moveGbtn.Load();
            this.textGbtn.Load();
            this.createGbtn.Load();
            this.deleteGbtn.Load();
        }

        private void UiMain_MouseMove(object sender, MouseEventArgs e)
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
                // 座標マットモードの場合
                if (this.coordMatGbtn.IsSelected)
                {
                    isMoveCoordMat = true;
                    moveSceneFlg = 1;
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
                    // 全てのシーンに、移動量をセットします。
                    foreach (SceneBox scene in this.SceneBoxList)
                    {
                        scene.Movement = new Rectangle(this.Movement.X, this.Movement.Y, 0, 0);
                    }
                }
                else if (moveSceneFlg == 2)
                {
                    // 選択しているシーンに、移動量をセットします。
                    foreach (SceneBox scene in this.SceneBoxList)
                    {
                        if (scene.IsSelected)
                        {
                            scene.Movement = new Rectangle(this.Movement.X, this.Movement.Y, 0, 0);
                        }
                    }
                }
            }
            else
            {
                this.Movement = new Point();
            }

            this.Refresh();
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            this.textBox1.Visible = false;
        }
    }
}
