using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp.Extensions;
using OpenCvSharp;
using System.Drawing;

namespace ConsoleApp2
{
    class Program
    {
        private const uint WM_RBUTTONDOWN = 0x201;
        private const uint WM_RBUTTONUP = 0x202;


        // HANDLE ---------------------------------------------------------------------------------------------------------------------
        // PostMessage. 다른 윈도우로  특정 문자를 전송하는 구문. 
        [DllImport("user32.dll")] // System32 - User32에서 랜더링 윈도우 권한을 따와야 한다.
        public static extern bool PostMessage(IntPtr hWnd, UInt32 Msg, int wParam, IntPtr lParam);

        // FindWindow. 윈도우의 핸들러 네임을 가져온다.
        [DllImport("User32.dll")]
        public static extern IntPtr FindWindow(String lpClassName, String lpWindowName);

        // FindWindowEx. 가져온 메인 핸들러에서, 자식 윈도우의 핸들러 네임을 가져온다.
        [DllImport("user32.dll")]
        public static extern IntPtr FindWindowEx(IntPtr Parent, IntPtr Child, string lpszClass, string lpszWindows);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        internal static extern bool PrintWindow(IntPtr hWnd, IntPtr hdcBlt, int nFlags);

        // Static -----------------------------------------------------------------------------------------------------------------------
        public static IntPtr findwindow = FindWindow(null, "MOMO");
        public static IntPtr hwnd_child = FindWindowEx(findwindow, IntPtr.Zero, "RenderWindow", "TheRender");
        public static int TF;



        // Func ---------------------------------------------------------------------------------------------------------------------------
        public static IntPtr lparam(int x, int y)
        {
            y -= 30;
            IntPtr lparam = new IntPtr(x | y << 16);
            return lparam;
        }
        public static void Click(int x, int y)
        {
            PostMessage(hwnd_child, WM_RBUTTONDOWN, 1, lparam(x, y));
            PostMessage(hwnd_child, WM_RBUTTONUP, 0, lparam(x, y));
        }

        public static int searchIMG(Bitmap Search_Img)
        {
            Mat ScreenMat = OpenCvSharp.Extensions.BitmapConverter.ToMat(Capture());    
            Mat TargetMat = OpenCvSharp.Extensions.BitmapConverter.ToMat(Search_Img);
            using (Mat res = ScreenMat.MatchTemplate(TargetMat, TemplateMatchModes.CCoeffNormed))
            {
                //찾은 이미지의 유사도를 담을 더블형 최대 최소 값을 선언합니다.
                double minval, maxval = 0;
                //찾은 이미지의 위치를 담을 포인트형을 선업합니다.
                OpenCvSharp.Point minloc, maxloc;
                //찾은 이미지의 유사도 및 위치 값을 받습니다. 
                Cv2.MinMaxLoc(res, out minval, out maxval, out minloc, out maxloc);
                //Console.WriteLine("찾은 이미지의 유사도 : " + maxval);
                //Console.WriteLine("찾은 이미지의 좌표 : X " + maxloc.X + " Y " + maxloc.Y);
                if( maxval > 0.8)
                {
                    Click(maxloc.X, maxloc.Y);
                    return 1;
                }
                return 0;
            }
        }
        public static int searchNImg(Bitmap Search_Img)
        {
            Mat ScreenMat = OpenCvSharp.Extensions.BitmapConverter.ToMat(Capture());
            Mat TargetMat = OpenCvSharp.Extensions.BitmapConverter.ToMat(Search_Img);
            using (Mat res = ScreenMat.MatchTemplate(TargetMat, TemplateMatchModes.CCoeffNormed))
            {
                //찾은 이미지의 유사도를 담을 더블형 최대 최소 값을 선언합니다.
                double minval, maxval = 0;
                //찾은 이미지의 위치를 담을 포인트형을 선업합니다.
                OpenCvSharp.Point minloc, maxloc;
                //찾은 이미지의 유사도 및 위치 값을 받습니다. 
                Cv2.MinMaxLoc(res, out minval, out maxval, out minloc, out maxloc);
                if (maxval > 0.8) return 1;
                return 0;
            }
        }

        public static Bitmap Capture()
        {
            Graphics Graphicsdata = Graphics.FromHwnd(findwindow);

            //찾은 플레이어 창 크기 및 위치를 가져옵니다. 
            Rectangle rect = Rectangle.Round(Graphicsdata.VisibleClipBounds);

            //플레이어 창 크기 만큼의 비트맵을 선언해줍니다.
            Bitmap bmp = new Bitmap(rect.Width, rect.Height);

            //비트맵을 바탕으로 그래픽스 함수로 선언해줍니다.
            using (Graphics g = Graphics.FromImage(bmp))
            {
                //찾은 플레이어의 크기만큼 화면을 캡쳐합니다.
                IntPtr hdc = g.GetHdc();
                PrintWindow(findwindow, hdc, 0x2);
                g.ReleaseHdc(hdc);
            }
            return bmp;
        }


        public static void search(string path)
        {
            Bitmap a = new Bitmap(path);
            TF = searchIMG(a);
        }

        public static void searchN(string path)
        {
            Bitmap a = new Bitmap(path);
            TF = searchNImg(a);
        }
        


        // 클릭을 하고 싶으면 Click(X좌표, Y좌표);
        // 이미지를 찾아서 클릭하고싶으면 search(이미지 경로);
        // 이미지를 찾기만 하고 싶으면 searchN(이미지 경로); ==검사를 할 때, Boolean 변수를 생성하여, 검사하여야 함.
        // TF = search(이미지 경로); 
        static void Main(string[] args)
        {
            search(@"img\Icon.png");
            Console.WriteLine("아이콘을 찾았습니다.");

            int count = 0;
            do
            {
                searchN(@"img\MainC.png");
                if (TF == 1) Click(400, 480);
                count += 1;
                Console.WriteLine(count);
                if (count == 500) Click(940, 50);
            } while (TF == 0);

            Console.WriteLine("메인화면을 찾았습니다.");

            Console.Read();
        }
    }
}
