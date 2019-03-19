using Gma.QrCodeNet.Encoding;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;
using System.Drawing;
using System.Drawing.Imaging;
using Gma.QrCodeNet.Encoding.Windows.Render;
using MySql.Data.MySqlClient;
using Microsoft.Office.Interop.Excel;
using Excel = Microsoft.Office.Interop.Excel;

namespace MyQrCode
{
    class Program
    {
        public static string preNumber(int number)//输出前三个数字
        {
            if (number < 10)
            {
                return "00" + number;
            }
            else
            if (number < 100)
            {
                return "0" + number;
            }
            else
            {
                return number.ToString();
            }
        }
        public static List<string> readFromFile(string filePath)//读取文件并返回保存有每行数据的数组
        {
            //string filePath;
            //filePath = Console.ReadLine();//读取文件路径
            StreamReader fileStr = new StreamReader(filePath);//读取整个文件
            List<string> qrcodeStr = new List<string>();
            while (!fileStr.EndOfStream)
            {
                qrcodeStr.Add(fileStr.ReadLine());
                //将每行数据保存

            }
            //关闭文件
            fileStr.Close();
            return qrcodeStr;//返回保存有每行数据的数组
        }
        public static void printOneCode(string s) //生成一行数据的二维码
        {
            //string[] SampleText = new string[1000];
            //SampleText = ReadStr(filePath);//读取字符串
            //for (int i = 0; i < s.Length; i++)
            //{
            if (s.Length < 32 && s.Length > 0)
            {
                QrEncoder qrEncoder = new QrEncoder(ErrorCorrectionLevel.M);//构建编码器
                QrCode qrCode = qrEncoder.Encode(s);//生成码
                for (int j = 0; j < qrCode.Matrix.Width; j++)
                {
                    //生成二维码点阵
                    for (int k = 0; k < qrCode.Matrix.Width; k++)
                    {
                        char charToPrint = qrCode.Matrix[k, j] ? ' ' : '■';
                        Console.Write(charToPrint);
                    }
                    Console.WriteLine();
                }
                Console.WriteLine();
            }
            else
            {
                Console.WriteLine("Wrong Message");
            }
            // }
        }
        public static void printQrCodesInjpg(List<string> qrcodeStr)//输出全部二维码并以.jpg格式保存在指定目录下
        {
            //string filePath;
            //filePath = Console.ReadLine();
            string savePath;
            savePath = Environment.CurrentDirectory.ToString() + "图片";
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }
            //List<string> qrcodeStr = new List<string>();
            //qrcodeStr = ReadStr(filePath);
            if (qrcodeStr.Count > 0)
            {
                int numberOfQrCode = 0;
                foreach (var str in qrcodeStr)
                {
                    if (str == null)
                    {
                        break;//如果无数据则跳出
                    }
                    if (str.Length < 32 && str.Length > 4)
                    {
                        string fileName = savePath + "\\" + preNumber(numberOfQrCode) + str.Substring(0, 4) + ".jpg";
                        numberOfQrCode++;
                        var qrEncoder = new QrEncoder(ErrorCorrectionLevel.M);
                        var qrCode = qrEncoder.Encode(str);//生成二维码
                        GraphicsRenderer gRender = new GraphicsRenderer(new FixedModuleSize(30, QuietZoneModules.Four));
                        using (FileStream stream = new FileStream(fileName, FileMode.Create))
                        {
                            gRender.WriteToStream(qrCode.Matrix, ImageFormat.Bmp, stream, new System.Drawing.Point(1000, 1000));//生成图片
                        }
                    }
                }
            }
        }
        public static List<string> readFromMysql(string database, string table)//从数据库中读取字符串
        {
            //连接数据库
            //-m server=localhost;database=gy1;uid=root;pwd=a1102134015;
            MySqlConnection conn = new MySqlConnection(database);
            //创建储存字符串的容器
            List<string> qrcodeStr = new List<string>();
            try
            {
                //开启连接
                conn.Open();
                //与要查找数据的表相连接
                MySqlCommand cmd = new MySqlCommand("select * from " + table, conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                // 逐行读取数据并储存
                while (reader.Read())
                    qrcodeStr.Add(reader.GetString("code"));
                reader.Close();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                //关闭连接
                conn.Close();
            }
            return qrcodeStr;
        }
        public static List<string> readFromExcel(string readPath)//从Excel表中读取数据
        {
            _Application xlsApp = new Excel.Application();
            // string fullPath = System.IO.Path.GetFullPath(readPath);               //解决不能读取相对路径的问题
            Workbook xlsWb = xlsApp.Workbooks.Open(readPath);                       //读取excel文件到内存
            Worksheet xlsWs = xlsWb.Worksheets[1];                                  //选择Sheet1第一个表

            List<string> readStr = new List<string>();                             //保存每一行字符串的值
            for (int i = 1; i <= xlsWs.UsedRange.Rows.Count; i++)                   //逐行读取
            {
                readStr.Add(xlsWs.Cells[i, 1].Value2);                             //读取单元格
            }
            return readStr;
        }
        static void Main(string[] args)
        {
            args = Environment.GetCommandLineArgs();//获取命令行数组
            /*if (args.Length == 0)
                System.Console.WriteLine("out");*/
            if (args.Length == 1)//无参数输出帮助文档，并安全退出
            {
                Console.WriteLine("命令行可以是例如 -f D:\\Csharpexp\\MyQrCode\\MyQrCode\\bin\\Debug\\text.txt'，-f表示QrCode信息放在-f的后面的data\aqrcode.txt文件中。如果没有-f则在控制台输出QrCode。");
                Console.WriteLine("也可以从数据库中读取 例如 -m sever=host;database=gy1;uid=root;pwd=a1102134015 myTable");
                Console.WriteLine("还可以从excel表中读取 例如 -e D:\\Csharpexp\\MyQrCode\\MyQrCode\\bin\\Debug\\test.xls");
                Console.WriteLine("二维码保存在bin目录下的 图片文件夹下");
                Console.WriteLine("安全退出");
                return;
            }
            if (args.Length == 2 && (args[1] == "-f" || args[1] == "-m") || args[1] == "-e")//如果有两个参数而且第二个参数为 -f或-m或-e 输出帮助文档并且退出
            {
                Console.WriteLine("命令行可以是例如 -f D:\\Csharpexp\\MyQrCode\\MyQrCode\\bin\\Debug\\text.txt'，-f表示QrCode信息放在-f的后面的data\aqrcode.txt文件中。如果没有-f则在控制台输出QrCode。");
                Console.WriteLine("也可以从数据库中读取 例如 -m sever=host;database=gy1;uid=root;pwd=a1102134015 myTable");
                Console.WriteLine("还可以从excel表中读取 例如 -e D:\\Csharpexp\\MyQrCode\\MyQrCode\\bin\\Debug\\test.xls");
                Console.WriteLine("二维码保存在bin目录下的 图片文件夹下");
                Console.WriteLine("安全退出");
                return;
            }
            else if (args.Length == 3 && args[1] == "-f")//有两个参数且第二个参数为-f 则在.txt文件中读取数据，并且保存在在bin目录下的 图片文件夹下
            {
                if (args.Length > 3)
                    Console.WriteLine("Wrong");
                else
                {

                    //Console.WriteLine("11111");
                    printQrCodesInjpg(readFromFile(args[2]));
                    Console.WriteLine("输出完毕file");
                }
            }
            else if (args.Length == 3 && args[1] == "-e")//有两个参数且第二个参数为-e 则在.xls文件中读取数据，并且保存在在bin目录下的 图片文件夹下
            {
                if (args.Length > 3)
                    Console.WriteLine("Wrong");
                else
                {
                    printQrCodesInjpg(readFromExcel(args[2]));
                    Console.WriteLine("输出完毕excel");
                }
            }
            else if (args.Length == 4 && args[1] == "-m")//有两个参数且第二个参数为-m 则在数据库中读取数据，并且保存在在bin目录下的 图片文件夹下
            {
                //-m server=localhost;database=gy1;uid=root;pwd=a1102134015 myTable
                if (args.Length > 4)
                    Console.WriteLine("Wrong2");
                else
                {
                    printQrCodesInjpg(readFromMysql(args[2], args[3]));
                    Console.WriteLine("输出完毕mysql");
                }
            }
            else
            {
                string s = System.Console.ReadLine();
                //Console.WriteLine(s);
                printOneCode(s);
                Console.WriteLine("输出完毕");
            }
            //printQrCodesInbmp();
            Console.WriteLine("安全结束");
        }
    }
}