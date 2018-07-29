using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;



namespace ScreenCapture
{
 
    public partial class CaptureWindows : Form
    {
        #region 用户变量  
        //记录鼠标按下坐标，确定绘图起点  
        private Point DownPoint = Point.Empty;
        //截图完成  
        //private bool CatchFinished = false;
        //截图开始  
        private bool CatchStart = false;

        private bool BackGroundPicSeted = false;
        //保存原始图像  
        private Bitmap originBmp;
        //保存截图的矩形  
        private Rectangle CatchRect;

        private Screen lastScreen;
   
        MouseHook mh;

    
        #endregion
        public CaptureWindows()
        {
            InitializeComponent();
            //this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            //this.BackColor = Color.Transparent;
            //this.Opacity = 0.4;
            mh = new MouseHook();
            mh.SetHook();
            mh.MouseDownEvent += mh_MouseDownEvent;
            mh.MouseUpEvent += mh_MouseUpEvent;
            mh.MouseMoveEvent+=mh_MouseMoveEvent;
            //CaptureApi.mouse_event(CaptureApi.MouseEventFlag.LeftDown,0,0,)
        }

        private void mh_MouseMoveEvent(object sender, MouseEventArgs e)
        {
            this.label1.Location = new Point(e.X + 10, e.Y - 10);
            this.label1.Text = "Position：" + e.X + "," + e.Y;
            this.label1.Visible = true;
            if (CatchStart)
            {

                this.timer1.Enabled = false;
                //Bitmap destBmp = (Bitmap)originBmp.Clone();
               
                Point newPoint = new Point(DownPoint.X, DownPoint.Y);
                Point p2 = new Point(e.X, e.Y);
                Bitmap destBmp = CapRect(DownPoint, p2);
                if (destBmp == null)
                {
                   
                    return;
                }
                //Bitmap destBmp = CapActvieScreen();
                //Bitmap destBmp = new Bitmap(GetMouseScreen().Bounds.Width, GetMouseScreen().Bounds.Height);
                System.IntPtr DesktopHandle = Win32Api.GetDC(System.IntPtr.Zero);
                Graphics g = Graphics.FromHdc(DesktopHandle);
                destBmp.Save(@"d:\destbmp.png");
                //this.Opacity = 0.9;
                //g.DrawImage(destBmp, e.Location);
                
                //if (CatchRect != null)
                //{
                //    g.Clear(Color.Empty);
                //}
                //g.CopyFromScreen(new Point(0, 0), new Point(0, 0), GetMouseScreen().Bounds.Size);


                Pen pen = new Pen(Color.Blue, 4);

                //Brush brush = new Brush(Color.LightGray,);
                
                int width = Math.Abs(e.X - DownPoint.X);
                int height = Math.Abs(e.Y - DownPoint.Y);
                //判断矩形的起始坐标  
                if (e.X < DownPoint.X)
                    newPoint.X = e.X;
                if (e.Y < DownPoint.Y)
                    newPoint.Y = e.Y;
                //保存矩形  
              

                CatchRect = new Rectangle(DownPoint, new Size(width, height));
                
                //this.Opacity = 0;

                //将矩形画在这个画板上  
                g.DrawRectangle(pen, CatchRect);
                //释放这个画板  
                g.Dispose();
                //重新创建一个Graphics类  
                Graphics g1 = this.CreateGraphics();
                ////如果之前那个画板不释放，而直接g = this.CreateGraphics()这样的话无法释放掉第一次创建的g,因为只是把地址转到新的g了，如同string一样。  
                ////将刚才所画的图片画到这个窗体上
                g1.DrawRectangle(pen, CatchRect);
                //g1.FillRectangle(destBmp)
                //g1.DrawImage(destBmp, new Point(0, 0));
                //g1.DrawImage(destBmp, DownPoint);
                //destBmp.Save(@"d:\destbmp.png");
                ////这个也可以属于二次缓冲技术，如果直接将矩形画在窗体上，会造成图片抖动并且会有无数个矩形  
                ////释放这个画板  
                g1.Dispose();
                ////释放掉Bmp对象。  k
                //destBmp.Dispose();
                //要及时释放不会再次使用的对象，不然内存将会被大量消耗  

            }
            else
            {
                //Point p = Cursor.Position;
                //Screen s = GetMouseScreen();
                //if ((lastScreen != null) && (s.Equals(lastScreen)))
                //{
                //    return;
                //}
                //lastScreen = s;
                //DrawScreenRect(s);
                //this.Left = s.Bounds.Left;
                //this.Top = s.Bounds.Top;
                //this.Size = s.Bounds.Size;
                //this.label1.Location = new Point(p.X + 10, p.Y - 10);
                //this.label1.Text = "Location：" + p.X + "," + p.Y;
                //this.label1.Visible = true;
            }
        }

        private void mh_MouseUpEvent(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                //如果已经开始绘制  
                if (CatchStart)
                {
                    Point p2 = new Point(e.X, e.Y);
                    Bitmap bitmap = CapRect(DownPoint, p2);
                    if (bitmap == null)
                    {
                        CatchStart = false;
                        //完成绘制设为true  
                        //CatchFinished = true;
                        this.Close();
                        return;
                    }
                    bitmap.Save("d:\\test1.png");
                    //将开始绘制设为false  
                    CatchStart = false;
                    //完成绘制设为true  
                    //CatchFinished = true;
                    //this.Close();
                }
            }
            if (e.Button == MouseButtons.Right)
            {
                //MessageBox.Show("release Right Mouse");
            }
        }

        private void mh_MouseDownEvent(object sender, MouseEventArgs e)
        {
            
            if (e.Button == MouseButtons.Left)
            {
                if (e.Button == MouseButtons.Left)
                {
                    //如果捕捉没有开始  
                    if (!CatchStart)
                    {
                        CatchStart = true;
                        timer1.Enabled = false;
                        //this.BackgroundImage = CapActvieScreen();
                        ////this.Opacity = 0.5;
                        //this.Size = GetMouseScreen().Bounds.Size;
                        ////保存鼠标按下坐标  
                        //DownPoint = new Point(e.X, e.Y);
                        //Point newPoint = new Point(DownPoint.X, DownPoint.Y);
                        //originBmp = new Bitmap(GetMouseScreen().Bounds.Width, GetMouseScreen().Bounds.Height);
                        //Graphics g = Graphics.FromImage(originBmp);
                        //g.CopyFromScreen(new Point(0, 0), new Point(0, 0), GetMouseScreen().Bounds.Size);
                        //if (!BackGroundPicSeted)
                        //{
                        //    this.BackgroundImage = originBmp;
                        //    BackGroundPicSeted = true;
                        //    this.Opacity = 1;

                        //}
                        //g.Dispose();
                        //originBmp.Dispose();
                    }
                }
            }
            if (e.Button == MouseButtons.Right)
            {
                this.Close();
            }
            //p1 = e.Location;
        }

        private Screen GetMouseScreen()
        {
            Screen sc = Screen.AllScreens.FirstOrDefault(s => s.Bounds.Contains(Cursor.Position));
            if (!(sc ==null))
            {
                return sc;
            }
            return null;
        }




        private void timer1_Tick(object sender, EventArgs e)
        {

            //Point p = Cursor.Position;
            //Screen s = GetMouseScreen();
            //this.Left = s.Bounds.Left;
            //this.Top = s.Bounds.Top;
            //this.Size = s.Bounds.Size;
            //this.label1.Location = new Point(p.X + 10, p.Y - 10);
            //this.label1.Text = "当前坐标：" + p.X + "," + p.Y;
            //this.label1.Visible = true;
        }

   
        private void CaptureCreen()
        {
            this.Opacity = 0;
            Bitmap bitmap = new Bitmap(GetMouseScreen().Bounds.Width, GetMouseScreen().Bounds.Height);
            Graphics g = Graphics.FromImage(bitmap);
           g.CopyFromScreen(new Point(0, 0), new Point(0, 0), GetMouseScreen().Bounds.Size);
            bitmap.Save(@"d:\test.png");
        }

        private Bitmap CapActvieScreen()
        {
            this.Opacity = 0;
            Bitmap bitmap = new Bitmap(GetMouseScreen().Bounds.Width, GetMouseScreen().Bounds.Height);
            Graphics g = Graphics.FromImage(bitmap);
            g.CopyFromScreen(new Point(0, 0), new Point(0, 0), GetMouseScreen().Bounds.Size);
            return bitmap;
            //bitmap.Save(@"d:\test.png");
        }
        private void DrawScreenRect(Screen s)
        {
            System.IntPtr DesktopHandle = Win32Api.GetDC(System.IntPtr.Zero);
            
            Rectangle captureRectangle = new Rectangle(new Point(1,1),new Size(s.Bounds.Width-1,s.Bounds.Height-1));
            //Bitmap bitmap = new Bitmap(s.Bounds.Size.Width, s.Bounds.Size.Height);
            Graphics g = Graphics.FromHdc(DesktopHandle);
            Pen pen = new Pen(Color.Yellow,5);
            g.DrawRectangle(pen, captureRectangle);
            //SolidBrush sb = new SolidBrush(Color.FromArgb(60, 255, 255, 255));
            //Rectangle NewRec = new Rectangle(s.Bounds.Location.X+1,s.Bounds.Y+1,s.Bounds.Width-3,s.Bounds.Height-3);
            //g.FillRectangle(sb, NewRec);
            g.Dispose();

        }
        private Bitmap CapRect(Point start, Point end)
        {
            //this.Opacity = 0;
            int width = Math.Abs(start.X - end.X);
            int height = Math.Abs(start.Y - end.Y);

            //判断矩形的起始坐标  
            if (end.X < start.X)
            {
                int temp = end.X;
                end.X = start.X;
                start.X = temp;
            }
            if (end.Y < start.Y)
            {
                int temp = end.Y;
                end.Y = start.Y;
                start.Y = temp;
            }
            if (width == 0 || height == 0)
            {
                return null;
            }
            //Rectangle captureRectangle = GetMouseScreen().Bounds;
            Rectangle captureRectangle ;
             captureRectangle   = new Rectangle(start.X,start.Y,width,height); 
            
            //Rectangle captureRectangle = new captureRectangle(start,end,)
           

            Bitmap bitmap = new Bitmap(width,height);
            Graphics g = Graphics.FromImage(bitmap);
           // g.CopyFromScreen(end,start, new Size(width,height));
            g.CopyFromScreen(captureRectangle.Left, captureRectangle.Top, 0, 0, captureRectangle.Size);
            //bitmap.Save(@"d:\test22.png");
            g.Dispose();

            return bitmap;
        }




    private void button1_Click(object sender, EventArgs e)
        {
            System.IntPtr DesktopHandle = Win32Api.GetDC(System.IntPtr.Zero);
            Graphics g = Graphics.FromHdc(DesktopHandle);
            g.DrawRectangle(new Pen(Color.Red), new Rectangle(300, 300, 100, 100));
            g.Dispose();
        }
    }
}
