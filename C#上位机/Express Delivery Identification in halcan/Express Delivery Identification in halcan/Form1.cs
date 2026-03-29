using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.IO.Ports;
using HalconDotNet;

namespace Express_Delivery_Identification_in_halcan
{
    public partial class Form1 : Form
    {
        // 全局变量：存放所有图片路径的列表
        private List<string> imagePaths = new List<string>();
        // 全局变量：当前处理到的图片索引
        private int currentImageIndex = 0;
        // 全局变量 ho_currentImage
        private HObject ho_currentImage = null;
        // 用于存放 Shape 模板句柄
        private HTuple hv_ModelID_3 = null;
        private HTuple hv_ModelID_fed = null;
        // 用于存放 OCR 字典句柄
        private HTuple hv_OCRHandle = null;
        // 存放模型轮廓用于显示
        private HObject ho_ModelContours_3 = null;
        private HObject ho_ModelContours_fed = null;
        // 串口对象
        private SerialPort serialPort1 = new SerialPort();
        // 接收缓冲区，按 \r\n 分包
        private StringBuilder receiveBuffer = new StringBuilder();
        // 保存最近一次识别结果
        private string lastRecognitionResult = "";


        //构造函数
        public Form1()
        {
            InitializeComponent();
            // 【修复】必须在窗体启动时初始化模型，否则识别时会报 #1401 错误
            InitHalconModels();

            InitSerialPort();
            InitSerialUI();
        }
        //串口界面初始化函数
        private void InitSerialUI()
        {
            cmbBaud.Items.Clear();
            cmbBaud.Items.Add("9600");
            cmbBaud.Items.Add("19200");
            cmbBaud.Items.Add("38400");
            cmbBaud.Items.Add("57600");
            cmbBaud.Items.Add("115200");
            cmbBaud.SelectedIndex = 0;

            RefreshPortList();

            serialPort1.DataReceived += SerialPort1_DataReceived;
            serialPort1.Encoding = Encoding.UTF8;

            lblPortStatus.Text = "未连接";
        }
        //刷新串口列表函数
        private void RefreshPortList()
        {
            cmbPort.Items.Clear();

            string[] ports = SerialPort.GetPortNames();
            cmbPort.Items.AddRange(ports);

            if (cmbPort.Items.Count > 0)
            {
                cmbPort.SelectedIndex = 0;
                LogMessage("已刷新串口列表。");
            }
            else
            {
                LogMessage("未检测到串口。");
            }
        }
        //打开/关闭串口函数
        private bool OpenSerialPort()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(cmbPort.Text))
                {
                    MessageBox.Show("请选择串口号！");
                    return false;
                }

                serialPort1.PortName = cmbPort.Text;
                serialPort1.BaudRate = int.Parse(cmbBaud.Text);
                serialPort1.DataBits = 8;
                serialPort1.StopBits = StopBits.One;
                serialPort1.Parity = Parity.None;
                serialPort1.Open();

                lblPortStatus.Text = "已连接";
                btnOpenPort.Text = "关闭串口";

                LogMessage($"串口已打开：{serialPort1.PortName}，波特率：{serialPort1.BaudRate}");
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("打开串口失败：" + ex.Message);
                LogMessage("打开串口失败：" + ex.Message);
                return false;
            }
        }

        //关闭函数
        private void CloseSerialPort()
        {
            try
            {
                if (serialPort1.IsOpen)
                {
                    serialPort1.Close();
                    lblPortStatus.Text = "未连接";
                    btnOpenPort.Text = "打开串口";
                    LogMessage("串口已关闭。");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("关闭串口失败：" + ex.Message);
                LogMessage("关闭串口失败：" + ex.Message);
            }
        }


        private void InitHalconModels()
        {
            try
            {
                // 1. 读取模板文件 (请确保此路径在你的电脑上真实有效)
                string modelPath3 = @"C:/Users/1/Desktop/Express Delivery Recognition  in halcan/模板文件/ShipTo_Model3.shm";
                string modelPathFed = @"C:/Users/1/Desktop/Express Delivery Recognition  in halcan/模板文件/ShipTo_Model_fed.shm";

                if (!File.Exists(modelPath3))
                {
                    LogMessage("错误：找不到 Model3 模板！");
                    return;
                }

                if (!File.Exists(modelPathFed))
                {
                    LogMessage("错误：找不到 Model_fed 模板！");
                    return;
                }

                // 加载 Model3
                HOperatorSet.ReadShapeModel(modelPath3, out hv_ModelID_3);
                HOperatorSet.GetShapeModelContours(out ho_ModelContours_3, hv_ModelID_3, 1);

                // 加载 FED
                HOperatorSet.ReadShapeModel(modelPathFed, out hv_ModelID_fed);
                HOperatorSet.GetShapeModelContours(out ho_ModelContours_fed, hv_ModelID_fed, 1);

                LogMessage("模板加载成功（Model3 + FED）");


                // 2. 读取 OCR 字典 (确保该文件在 bin/Debug 目录下或指定全路径)
                HOperatorSet.ReadOcrClassMlp("Document_0-9A-Z_NoRej.omc", out hv_OCRHandle);

                LogMessage("模型与OCR字典加载成功！");
            }
            catch (Exception ex)
            {
                LogMessage($"模型加载失败。错误信息: {ex.Message}");
            }
        }

        private void InitSerialPort()
        {
            try
            {
                // 这里只做事件绑定和一些默认设置
                serialPort1.Encoding = Encoding.UTF8;

                LogMessage("串口模块初始化完成。");
            }
            catch (Exception ex)
            {
                LogMessage("串口初始化失败：" + ex.Message);
            }
        }

        // 选择文件夹按钮
        private void btnSelectFolder_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                fbd.Description = "请选择包含测试图片的文件夹";
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    txtFolderPath.Text = fbd.SelectedPath;
                    LoadImagePaths(fbd.SelectedPath);
                }
            }
        }

        // 加载图片路径
        private void LoadImagePaths(string folderPath)
        {
            imagePaths.Clear();
            currentImageIndex = 0;

            string[] extensions = { "*.jpg", "*.png", "*.bmp", "*.tif" };
            foreach (string ext in extensions)
            {
                imagePaths.AddRange(Directory.GetFiles(folderPath, ext));
            }

            if (imagePaths.Count > 0)
            {
                LogMessage($"成功加载了 {imagePaths.Count} 张图片。");
                lblImageCount.Text = $"当前图片：1/{imagePaths.Count}";
                ShowImageToHalcon(imagePaths[0]);
            }
            else
            {
                LogMessage("警告：文件夹内无有效图片！");
            }
        }

        // 日志打印方法
        private void LogMessage(string msg)
        {
            // 1. 界面显示逻辑 (确保跨线程安全)
            if (txtLog.InvokeRequired)
            {
                txtLog.Invoke(new Action(() => LogMessage(msg)));
                return;
            }
            string logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {msg}";
            txtLog.AppendText(logEntry + Environment.NewLine);

            // 2. 写入本地文件逻辑
            try
            {
                // 文件会保存在项目 bin/Debug 文件夹下
                string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Recognition_Log.txt");

                // 使用 AppendAllText，如果文件不存在会自动创建，存在则追加
                // Encoding.UTF8 确保中文不乱码
                File.AppendAllText(logPath, logEntry + Environment.NewLine, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                // 如果文件写入失败，至少在界面提示一下
                txtLog.AppendText($"[系统错误] 无法写入日志文件: {ex.Message}" + Environment.NewLine);
            }
        }

        //发送函数
        private void SendData(string msg)
        {
            try
            {
                if (!serialPort1.IsOpen)
                {
                    LogMessage("发送失败：串口未打开。");
                    return;
                }

                string sendText = msg + "\r\n";
                byte[] data = Encoding.UTF8.GetBytes(sendText);
                serialPort1.Write(data, 0, data.Length);

                LogMessage("[发送] " + msg);
            }
            catch (Exception ex)
            {
                LogMessage("发送数据失败：" + ex.Message);
            }
        }
        //接收函数
        private void SerialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                int len = serialPort1.BytesToRead;
                byte[] data = new byte[len];
                serialPort1.Read(data, 0, len);

                string newText = Encoding.UTF8.GetString(data);
                receiveBuffer.Append(newText);

                int endIndex = receiveBuffer.ToString().IndexOf("\r\n");
                while (endIndex != -1)
                {
                    string fullMessage = receiveBuffer.ToString(0, endIndex);
                    receiveBuffer.Remove(0, endIndex + 2);

                    this.Invoke(new Action(() =>
                    {
                        txtReceive.AppendText(fullMessage + Environment.NewLine);
                        txtReceive.SelectionStart = txtReceive.Text.Length;
                        txtReceive.ScrollToCaret();

                        LogMessage("[接收] " + fullMessage);
                    }));

                    HandleReceivedCommand(fullMessage.Trim());

                    endIndex = receiveBuffer.ToString().IndexOf("\r\n");
                }
            }
            catch (Exception ex)
            {
                this.Invoke(new Action(() =>
                {
                    LogMessage("串口接收异常：" + ex.Message);
                }));
            }
        }
        //命令处理函数
        private void HandleReceivedCommand(string cmd)
        {
            cmd = cmd.Trim().ToLower();

            this.Invoke(new Action(() =>
            {
                if (cmd == "recognition")
                {
                    LogMessage("收到命令：recognition，开始识别当前图片。");

                    string result = ProcessCurrent_Image();

                    if (!string.IsNullOrWhiteSpace(result))
                    {
                        SendData("RESULT:" + result);
                    }
                    else
                    {
                        SendData("ERROR:NO_RESULT");
                    }
                }
                else if (cmd == "next")
                {
                    LogMessage("收到命令：next，切换到下一张图片。");

                    btnNext_Click(null, null);
                    SendData("OK:NEXT");
                }
                else
                {
                    LogMessage("未知命令：" + cmd);
                }
            }));
        }




        // 显示图片
        private void ShowImageToHalcon(string path)
        {
            try
            {
                if (ho_currentImage != null)
                {
                    ho_currentImage.Dispose();
                    ho_currentImage = null;
                }

                HOperatorSet.ReadImage(out ho_currentImage, path);

                HTuple width, height;
                HOperatorSet.GetImageSize(ho_currentImage, out width, out height);
                HOperatorSet.SetPart(hWindowControl1.HalconWindow, 0, 0, height - 1, width - 1);
                HOperatorSet.DispObj(ho_currentImage, hWindowControl1.HalconWindow);
            }
            catch (Exception ex)
            {
                LogMessage("显示图片出错：" + ex.Message);
            }
        }

        // 识别当前图片按钮
        private void btnProcess_Click(object sender, EventArgs e)
        {
            string result = ProcessCurrent_Image();

            if (!string.IsNullOrWhiteSpace(result))
            {
                SendData("RESULT:" + result);
            }
            else
            {
                LogMessage("本次识别没有有效结果，未发送。");
            }
        }

        // 下一张图按钮
        private void btnNext_Click(object sender, EventArgs e)
        {
            if (imagePaths.Count == 0) return;

            currentImageIndex++;
            if (currentImageIndex >= imagePaths.Count)
            {
                currentImageIndex = 0;
                LogMessage("已回到第一张图片。");
            }

            lblImageCount.Text = $"当前图片：{currentImageIndex + 1}/{imagePaths.Count}";
            ShowImageToHalcon(imagePaths[currentImageIndex]);
        }

        // 核心识别算法
        private string ProcessCurrent_Image()
        {
            string finalResult = "";

            if (ho_currentImage == null || !ho_currentImage.IsInitialized())
            {
                LogMessage("请先加载图片！");
                return "";
            }

            if (hv_ModelID_3 == null || hv_ModelID_fed == null)
            {
                LogMessage("错误：模板未加载完整，无法识别！");
                return "";
            }

            if (hv_OCRHandle == null)
            {
                LogMessage("错误：OCR字库未加载，无法识别！");
                return "";
            }

            HObject ho_ImageMedian = null, ho_ImageErosion = null, ho_ImageDifference = null;
            HObject ho_ImageDiffEmphasize = null, ho_EdgeRegionPixel = null, ho_RegionClosing = null;
            HObject ho_RegionFillUp = null, ho_RegionErosion = null, ho_ConnectedRegions = null;
            HObject ho_SelectedRegions1 = null, ho_RegionDilation = null, ho_RegionUnion = null;
            HObject ho_BoundingBox = null, ho_ImageReduced = null;

            try
            {
                LogMessage("开始预处理与 ROI 提取...");

                // 1. 预处理
                HOperatorSet.MedianImage(ho_currentImage, out ho_ImageMedian, "circle", 1, "mirrored");
                HOperatorSet.GrayErosionRect(ho_ImageMedian, out ho_ImageErosion, 7, 7);
                HOperatorSet.SubImage(ho_ImageMedian, ho_ImageErosion, out ho_ImageDifference, 1, 0);
                HOperatorSet.Emphasize(ho_ImageDifference, out ho_ImageDiffEmphasize, 7, 7, 1.5);
                HOperatorSet.Threshold(ho_ImageDiffEmphasize, out ho_EdgeRegionPixel, 30, 255);
                HOperatorSet.ClosingCircle(ho_EdgeRegionPixel, out ho_RegionClosing, 3.5);
                HOperatorSet.FillUp(ho_RegionClosing, out ho_RegionFillUp);

                HOperatorSet.ErosionCircle(ho_RegionFillUp, out ho_RegionErosion, 8.5);
                HOperatorSet.Connection(ho_RegionErosion, out ho_ConnectedRegions);
                HOperatorSet.SelectShape(ho_ConnectedRegions, out ho_SelectedRegions1, "area", "and", 298387, 1.26114e+06);
                HOperatorSet.DilationCircle(ho_SelectedRegions1, out ho_RegionDilation, 8.5);

                HOperatorSet.Union1(ho_RegionDilation, out ho_RegionUnion);
                HOperatorSet.SmallestRectangle2(ho_RegionUnion, out HTuple hv_Row, out HTuple hv_Column, out HTuple hv_Phi, out HTuple hv_Length1, out HTuple hv_Length2);
                HOperatorSet.GenRectangle2(out ho_BoundingBox, hv_Row, hv_Column, hv_Phi, hv_Length1, hv_Length2);

                HOperatorSet.SetColor(hWindowControl1.HalconWindow, "green");
                HOperatorSet.SetDraw(hWindowControl1.HalconWindow, "margin");
                HOperatorSet.DispObj(ho_currentImage, hWindowControl1.HalconWindow);
                HOperatorSet.DispObj(ho_BoundingBox, hWindowControl1.HalconWindow);

                HOperatorSet.ReduceDomain(ho_currentImage, ho_BoundingBox, out ho_ImageReduced);

                // 2. 双模板匹配
                HTuple hv_MinScore3 = 0.80;
                HTuple hv_MinScoreFed = 0.80;

                HTuple hv_RowUsed = new HTuple();
                HTuple hv_ColumnUsed = new HTuple();
                HTuple hv_AngleUsed = new HTuple();
                HTuple hv_ScaleUsed = new HTuple();
                HTuple hv_ScoreUsed = new HTuple();

                bool useFed = false;
                string modelName = "";

                LogMessage("先匹配主模板 ShipTo_Model3...");

                HOperatorSet.FindScaledShapeModel(
                    ho_ImageReduced, hv_ModelID_3,
                    new HTuple(0).TupleRad(), new HTuple(360).TupleRad(),
                    0.52, 1.0, 0.7, 0, 0.54,
                    "least_squares", 0, hv_MinScore3,
                    out HTuple hv_Row3, out HTuple hv_Column3, out HTuple hv_Angle3, out HTuple hv_Scale3, out HTuple hv_Score3);

                if (hv_Score3.Length > 0)
                {
                    hv_RowUsed = hv_Row3;
                    hv_ColumnUsed = hv_Column3;
                    hv_AngleUsed = hv_Angle3;
                    hv_ScaleUsed = hv_Scale3;
                    hv_ScoreUsed = hv_Score3;
                    useFed = false;
                    modelName = "Model3";
                    LogMessage($"主模板匹配成功，找到 {hv_Score3.Length} 个目标。");
                }
                else
                {
                    LogMessage("Model3未找到，切换到 ShipTo_Model_fed...");

                    HOperatorSet.FindScaledShapeModel(
                        ho_ImageReduced, hv_ModelID_fed,
                        new HTuple(0).TupleRad(), new HTuple(360).TupleRad(),
                        0.52, 1.0, 0.7, 0, 0.54,
                        "least_squares", 0, hv_MinScoreFed,
                        out HTuple hv_RowFed, out HTuple hv_ColumnFed, out HTuple hv_AngleFed, out HTuple hv_ScaleFed, out HTuple hv_ScoreFed);

                    if (hv_ScoreFed.Length > 0)
                    {
                        hv_RowUsed = hv_RowFed;
                        hv_ColumnUsed = hv_ColumnFed;
                        hv_AngleUsed = hv_AngleFed;
                        hv_ScaleUsed = hv_ScaleFed;
                        hv_ScoreUsed = hv_ScoreFed;
                        useFed = true;
                        modelName = "FED";
                        LogMessage($"FED模板匹配成功，找到 {hv_ScoreFed.Length} 个目标。");
                    }
                    else
                    {
                        LogMessage("两个模板都未找到匹配项！");
                        return "";
                    }
                }

                HOperatorSet.SetColor(hWindowControl1.HalconWindow, "green");
                HOperatorSet.SetLineWidth(hWindowControl1.HalconWindow, 2);

                // 3. 循环 OCR
                for (int i = 0; i < hv_ScoreUsed.Length; i++)
                {
                    HObject ho_ContoursAffineTrans = null;
                    HObject ho_ROI_OCR = null;
                    HObject ho_ImageStraightened = null;
                    HObject ho_RegionOCRStraightened = null;
                    HObject ho_ImageOCR_ROI = null;
                    HObject ho_ImageOCR_Final = null;
                    HObject ho_ImageOCREnhanced = null;
                    HObject ho_ImageMeanLocal = null;
                    HObject ho_RegionText = null;
                    HObject ho_ConnectedChars = null;
                    HObject ho_SelectedChars = null;
                    HObject ho_SortedChars = null;

                    try
                    {
                        HTuple row_i = hv_RowUsed.TupleSelect(i);
                        HTuple col_i = hv_ColumnUsed.TupleSelect(i);
                        HTuple angle_i = hv_AngleUsed.TupleSelect(i);
                        HTuple scale_i = hv_ScaleUsed.TupleSelect(i);
                        HTuple score_i = hv_ScoreUsed.TupleSelect(i);

                        // 轮廓变换
                        HOperatorSet.HomMat2dIdentity(out HTuple hv_HomMat2DIdentity);
                        HOperatorSet.HomMat2dTranslate(hv_HomMat2DIdentity, row_i, col_i, out HTuple hv_HomMat2DTranslate);
                        HOperatorSet.HomMat2dRotate(hv_HomMat2DTranslate, angle_i, row_i, col_i, out HTuple hv_HomMat2DRotate);
                        HOperatorSet.HomMat2dScale(hv_HomMat2DRotate, scale_i, scale_i, row_i, col_i, out HTuple hv_HomMat2DScale);

                        HTuple hv_OffsetX, hv_OffsetY, hv_Len1, hv_Len2, hv_AngleOCR;

                        if (!useFed)
                        {
                            HOperatorSet.AffineTransContourXld(ho_ModelContours_3, out ho_ContoursAffineTrans, hv_HomMat2DScale);

                            hv_OffsetX = 180;
                            hv_OffsetY = 10;
                            hv_Len1 = 150;
                            hv_Len2 = 40;
                            hv_AngleOCR = angle_i - new HTuple(90).TupleRad();
                        }
                        else
                        {
                            HOperatorSet.AffineTransContourXld(ho_ModelContours_fed, out ho_ContoursAffineTrans, hv_HomMat2DScale);

                            hv_OffsetX = -310;
                            hv_OffsetY = -135;
                            hv_Len1 = 140;
                            hv_Len2 = 80;
                            hv_AngleOCR = angle_i + new HTuple(78).TupleRad();
                        }

                        HOperatorSet.SetColor(hWindowControl1.HalconWindow, "green");
                        HOperatorSet.DispObj(ho_ContoursAffineTrans, hWindowControl1.HalconWindow);

                        // 计算 OCR 框
                        HTuple hv_RowOCR = row_i - hv_AngleOCR.TupleSin() * hv_OffsetX + hv_AngleOCR.TupleCos() * hv_OffsetY;
                        HTuple hv_ColOCR = col_i + hv_AngleOCR.TupleCos() * hv_OffsetX + hv_AngleOCR.TupleSin() * hv_OffsetY;

                        HOperatorSet.GenRectangle2(out ho_ROI_OCR, hv_RowOCR, hv_ColOCR, hv_AngleOCR, hv_Len1, hv_Len2);
                        HOperatorSet.SetColor(hWindowControl1.HalconWindow, "yellow");
                        HOperatorSet.DispObj(ho_ROI_OCR, hWindowControl1.HalconWindow);

                        // OCR 图像拉正
                        HOperatorSet.VectorAngleToRigid(hv_RowOCR, hv_ColOCR, hv_AngleOCR, hv_RowOCR, hv_ColOCR, 0, out HTuple hv_HomMat2D_OCR);
                        HOperatorSet.AffineTransImage(ho_ImageReduced, out ho_ImageStraightened, hv_HomMat2D_OCR, "constant", "false");
                        HOperatorSet.AffineTransRegion(ho_ROI_OCR, out ho_RegionOCRStraightened, hv_HomMat2D_OCR, "nearest_neighbor");
                        HOperatorSet.ReduceDomain(ho_ImageStraightened, ho_RegionOCRStraightened, out ho_ImageOCR_ROI);
                        HOperatorSet.CropDomain(ho_ImageOCR_ROI, out ho_ImageOCR_Final);

                        // OCR 前处理
                        HOperatorSet.Emphasize(ho_ImageOCR_Final, out ho_ImageOCREnhanced, 7, 7, 1.5);
                        HOperatorSet.MeanImage(ho_ImageOCREnhanced, out ho_ImageMeanLocal, 21, 21);
                        HOperatorSet.DynThreshold(ho_ImageOCREnhanced, ho_ImageMeanLocal, out ho_RegionText, 15, "dark");
                        HOperatorSet.Connection(ho_RegionText, out ho_ConnectedChars);
                        HOperatorSet.SelectShape(ho_ConnectedChars, out ho_SelectedChars,
                            new HTuple("area", "height"), "and",
                            new HTuple(20, 10), new HTuple(1000, 150));
                        HOperatorSet.SortRegion(ho_SelectedChars, out ho_SortedChars, "character", "true", "row");

                        // OCR
                        HOperatorSet.DoOcrMultiClassMlp(ho_SortedChars, ho_ImageOCR_Final, hv_OCRHandle,
                            out HTuple hv_Class, out HTuple hv_Confidence);
                        HOperatorSet.TupleSum(hv_Class, out HTuple hv_FinalString);

                        string fileName = (imagePaths != null && imagePaths.Count > 0 && currentImageIndex >= 0 && currentImageIndex < imagePaths.Count)
                            ? Path.GetFileName(imagePaths[currentImageIndex])
                            : "当前图片";

                        string resultText = hv_FinalString.S;
                        finalResult = resultText;
                        lastRecognitionResult = resultText;

                        LogMessage($"模板: {modelName} | 图片: {fileName} | 识别结果: {resultText} | 匹配分数: {score_i.D:F2}");
                        LogMessage($"[OCR] 目标 {i + 1} 结果: {resultText}");
                    }
                    finally
                    {
                        ho_ContoursAffineTrans?.Dispose();
                        ho_ROI_OCR?.Dispose();
                        ho_ImageStraightened?.Dispose();
                        ho_RegionOCRStraightened?.Dispose();
                        ho_ImageOCR_ROI?.Dispose();
                        ho_ImageOCR_Final?.Dispose();
                        ho_ImageOCREnhanced?.Dispose();
                        ho_ImageMeanLocal?.Dispose();
                        ho_RegionText?.Dispose();
                        ho_ConnectedChars?.Dispose();
                        ho_SelectedChars?.Dispose();
                        ho_SortedChars?.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                LogMessage($"识别过程发生异常: {ex.Message}");
            }
            finally
            {
                ho_ImageMedian?.Dispose();
                ho_ImageErosion?.Dispose();
                ho_ImageDifference?.Dispose();
                ho_ImageDiffEmphasize?.Dispose();
                ho_EdgeRegionPixel?.Dispose();
                ho_RegionClosing?.Dispose();
                ho_RegionFillUp?.Dispose();
                ho_RegionErosion?.Dispose();
                ho_ConnectedRegions?.Dispose();
                ho_SelectedRegions1?.Dispose();
                ho_RegionDilation?.Dispose();
                ho_RegionUnion?.Dispose();
                ho_BoundingBox?.Dispose();
                ho_ImageReduced?.Dispose();
            }

            return finalResult;
        }

        // --- 补回被删除的空事件处理方法，解决 Designer 报错 ---

        private void txtFolderPath_TextChanged(object sender, EventArgs e)
        {
            // 这里暂时不需要写逻辑，留空即可
        }

        private void lblImageCount_Click(object sender, EventArgs e)
        {
            // 这里暂时不需要写逻辑，留空即可
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            // 这里暂时不需要写逻辑，留空即可
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
        //刷新按钮事件写上
        private void btnRefreshPort_Click(object sender, EventArgs e)
        {
            RefreshPortList();
        }

        private void btnOpenPort_Click(object sender, EventArgs e)
        {
            if (!serialPort1.IsOpen)
            {
                OpenSerialPort();
            }
            else
            {
                CloseSerialPort();
            }
        }
        //发送按钮事件
        private void btnSend_Click(object sender, EventArgs e)
        {
            string msg = txtSend.Text.Trim();

            if (string.IsNullOrWhiteSpace(msg))
            {
                MessageBox.Show("请输入要发送的内容！");
                return;
            }

            SendData(msg);
            txtSend.Clear();
        }
        //点击窗体空白处
        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                if (serialPort1.IsOpen)
                {
                    serialPort1.Close();
                }
            }
            catch
            {
            }
        }

        private void txtSend_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtReceive_TextChanged(object sender, EventArgs e)
        {
            txtReceive.SelectionStart = txtReceive.Text.Length;
            txtReceive.ScrollToCaret();
        }
    }

}