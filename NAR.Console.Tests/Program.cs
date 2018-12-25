using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NAR.Console.Tests
{
    class Program
    {
        static void initWorldCoords(int TAB_SIZE_Y, int TAB_SIZE_X, ref double[] _wx, ref double[] _wy)
        {
            int i, j;


            for (i = 0; i < TAB_SIZE_Y; i++)
                for (j = 0; j < TAB_SIZE_X; j++)
                {
                    _wx[TAB_SIZE_X * i + j] = 1.65 * (((double)j + 1));//) - TAB_SIZE_X / 2.0 + 0.5); //
                    _wy[TAB_SIZE_X * i + j] = 1.65 * (((double)i + 1));// ) - TAB_SIZE_Y / 2.0 + 0.5);//+ 1) );
                }
        }

        static void Main(string[] args)
        {

            Execution exe = new Execution();
            exe.Run();


            int np = 8;
            //double[] xw = new double[]   {-30, -30, 30, 30, -15, -15, 15, 15};
            //double[] yw = new double[]   {-30, -30, 30, 30, -15, -15, 15, 15};

            //xw[0] = 29.5; xw[1] = -30.5; xw[2] = -30.5; xw[3] = 29.5; xw[4] = 10.5; xw[5] = -19.5; xw[6] = -19.5; xw[7] = 10.5;
            //yw[0] = -34.5; yw[1] = 35.5; yw[2] = -34.5; yw[3] = 35.5; yw[4] = -22.5; yw[5] = 12.5; yw[6] = -22.5; yw[7] = 12.5;
            
            //double[] xf = new double[]  {-30, -30, 30, 30, -15, -15, 15, 15};
            //double[] yf = new double[] { -30, -30, 30, 30, -15, -15, 15, 15 };


            //xf[0] = 29.5; xf[1] = -30.5; xf[2] = -30.5; xf[3] = 29.5; xf[4] = 10.5; xf[5] = -19.5; xf[6] = -19.5; xf[7] = 10.5;
            //yf[0] = -34.5; yf[1] = 35.5; yf[2] = -34.5; yf[3] = 35.5; yf[4] = -22.5; yf[5] = 12.5; yf[6] = -22.5; yf[7] = 12.5;

            
            //xf[0] = 19.5; xf[1] = -3.5; xf[2] = -3.5; xf[3] = 19.5; xf[4] = 10.5; xf[5] = -9.5; xf[6] = -9.5; xf[7] = 10.5;
            //yf[0] = -34.5; yf[1] = 35.5; yf[2] = -34.5; yf[3] = 35.5; yf[4] = -22.5; yf[5] = 12.5; yf[6] = -22.5; yf[7] = 12.5;

            int count = 6 * 8;

            double[] xw = new double[count];
            double[] yw = new double[count];

            double[] xf = new double[count];
            double[] yf = new double[count];

            initWorldCoords(6, 8, ref xw, ref yw);
            initWorldCoords(6, 8, ref xf, ref yf);

            
            for (int i = 0; i < count; i++)
            {
                xf[i] = xf[i] - 640 / 2;
                yf[i] = yf[i] - 320 / 2;
            }






            NAR.ImageProcessing.CameraCalibration.Tsai.TsaiCameraCalibration cal = new ImageProcessing.CameraCalibration.Tsai.TsaiCameraCalibration();
            cal.Calibrate(count, xw, yw, xf, yf);


            Execution exec = new Execution();
            exec.Run();
            


           
            //IList<NAR.Capture.Drivers.WebCamWinAPI.API.Device> list = NAR.Capture.Drivers.WebCamWinAPI.Wrapper.GetDevices();
                



            //NAR.Capture.Drivers.IDriver driver = new NAR.Capture.Drivers.WebCamWinAPI.WebCamWinDriver (new Model.ImageConfig(640,480));            
            //NAR.Capture.Management.ICamera camera = new NAR.Capture.Management.CameraManager(driver);
            
            
            //camera.Connect();

            //Model.IImage image = camera.GrabFrame();


            //System.Console.ReadLine();

            //camera.Disconnect();

            //System.Console.ReadLine();
        }

    }
}
