using AVIClientDemo.Model;
using HalconDotNet;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewROI;

namespace AVIClientDemo.ViewModel
{
    class MainWindowViewModel : NotificationObject
    {
        #region 属性绑定
        private string messageStr;

        public string MessageStr
        {
            get { return messageStr; }
            set
            {
                messageStr = value;
                this.RaisePropertyChanged("MessageStr");
            }
        }
        private string version;

        public string Version
        {
            get { return version; }
            set
            {
                version = value;
                this.RaisePropertyChanged("Version");
            }
        }

        private bool statusDataBase;

        public bool StatusDataBase
        {
            get { return statusDataBase; }
            set
            {
                statusDataBase = value;
                this.RaisePropertyChanged("StatusDataBase");
            }
        }
        private string homePageVisibility;

        public string HomePageVisibility
        {
            get { return homePageVisibility; }
            set
            {
                homePageVisibility = value;
                this.RaisePropertyChanged("HomePageVisibility");
            }
        }
        private string parameterPageVisibility;

        public string ParameterPageVisibility
        {
            get { return parameterPageVisibility; }
            set
            {
                parameterPageVisibility = value;
                this.RaisePropertyChanged("ParameterPageVisibility");
            }
        }
        private string machineID;

        public string MachineID
        {
            get { return machineID; }
            set
            {
                machineID = value;
                this.RaisePropertyChanged("MachineID");
            }
        }
        private string remotePath;

        public string RemotePath
        {
            get { return remotePath; }
            set
            {
                remotePath = value;
                this.RaisePropertyChanged("RemotePath");
            }
        }
        private bool eStopIsChecked;

        public bool EStopIsChecked
        {
            get { return eStopIsChecked; }
            set
            {
                eStopIsChecked = value;
                this.RaisePropertyChanged("EStopIsChecked");
            }
        }
        private int stationNo;

        public int StationNo
        {
            get { return stationNo; }
            set
            {
                stationNo = value;
                this.RaisePropertyChanged("StationNo");
            }
        }
        private HImage cameraIamge;

        public HImage CameraIamge
        {
            get { return cameraIamge; }
            set
            {
                cameraIamge = value;
                this.RaisePropertyChanged("CameraIamge");
            }
        }
        private bool cameraRepaint;

        public bool CameraRepaint
        {
            get { return cameraRepaint; }
            set
            {
                cameraRepaint = value;
                this.RaisePropertyChanged("CameraRepaint");
            }
        }
        private ObservableCollection<ROI> cameraROIList;

        public ObservableCollection<ROI> CameraROIList
        {
            get { return cameraROIList; }
            set
            {
                cameraROIList = value;
                this.RaisePropertyChanged("CameraROIList");
            }
        }
        private HObject cameraAppendHObject;

        public HObject CameraAppendHObject
        {
            get { return cameraAppendHObject; }
            set
            {
                cameraAppendHObject = value;
                this.RaisePropertyChanged("CameraAppendHObject");
            }
        }
        private Tuple<string, object> cameraGCStyle;

        public Tuple<string, object> CameraGCStyle
        {
            get { return cameraGCStyle; }
            set
            {
                cameraGCStyle = value;
                this.RaisePropertyChanged("CameraGCStyle");
            }
        }
        private bool statusPLC;

        public bool StatusPLC
        {
            get { return statusPLC; }
            set
            {
                statusPLC = value;
                this.RaisePropertyChanged("StatusPLC");
            }
        }
        private string curBoardID;

        public string CurBoardID
        {
            get { return curBoardID; }
            set
            {
                curBoardID = value;
                this.RaisePropertyChanged("CurBoardID");
            }
        }
        private string remoteIP;

        public string RemoteIP
        {
            get { return remoteIP; }
            set
            {
                remoteIP = value;
                this.RaisePropertyChanged("RemoteIP");
            }
        }
        private int remotePort;

        public int RemotePort
        {
            get { return remotePort; }
            set
            {
                remotePort = value;
                this.RaisePropertyChanged("RemotePort");
            }
        }
        private bool statusTCP;

        public bool StatusTCP
        {
            get { return statusTCP; }
            set
            {
                statusTCP = value;
                this.RaisePropertyChanged("StatusTCP");
            }
        }
        private string localIP;

        public string LocalIP
        {
            get { return localIP; }
            set
            {
                localIP = value;
                this.RaisePropertyChanged("LocalIP");
            }
        }
        private int localPort;

        public int LocalPort
        {
            get { return localPort; }
            set
            {
                localPort = value;
                this.RaisePropertyChanged("LocalPort");
            }
        }

        #endregion
        #region 方法绑定
        public DelegateCommand AppLoadedEventCommand { get; set; }
        public DelegateCommand<object> MenuActionCommand { get; set; }
        public DelegateCommand FolderBrowserDialogCommand { get; set; }
        public DelegateCommand ParameterSaveCommand { get; set; }
        public DelegateCommand<object> OperateButtonCommand { get; set; }        
        #endregion
        #region 变量
        private string iniParameterPath = System.Environment.CurrentDirectory + "\\Parameter.ini";
        CameraOperate mycam = new CameraOperate();string cameraName = "", cameraInterface = "";
        InovanceH3UModbusTCP H3u;
        DXH.Net.DXHTCPClient tcpNet;
        #endregion
        #region 构造函数
        public MainWindowViewModel()
        {
            #region 初始化参数
            try
            {
                MessageStr = "";
                Version = "20201103";
                HomePageVisibility = "Visible";
                ParameterPageVisibility = "Collapsed";
                RemotePath = Inifile.INIGetStringValue(iniParameterPath, "System", "RemotePath", "D:\\");
                MachineID = Inifile.INIGetStringValue(iniParameterPath, "System", "MachineID", "Null");
                StationNo = 0;
                StationNo = int.Parse(Inifile.INIGetStringValue(iniParameterPath, "System", "StationNo", "1"));

                cameraName = Inifile.INIGetStringValue(iniParameterPath, "System", "CameraName", "[0] Integrated Camera");
                cameraInterface = Inifile.INIGetStringValue(iniParameterPath, "System", "CameraInterface", "DirectShow");
                Version += StationNo.ToString();
                EStopIsChecked = false;
                CameraROIList = new ObservableCollection<ROI>();

                string ip = Inifile.INIGetStringValue(iniParameterPath, "PLC", "IP", "192.168.0.100");
                H3u = new InovanceH3UModbusTCP(ip);

                RemoteIP = Inifile.INIGetStringValue(iniParameterPath, "Remote", "IP", "192.168.0.11");
                RemotePort = int.Parse(Inifile.INIGetStringValue(iniParameterPath, "Remote", "PORT", "3000"));
                LocalIP = Inifile.INIGetStringValue(iniParameterPath, "Local", "IP", "192.168.0.11");
                LocalPort = int.Parse(Inifile.INIGetStringValue(iniParameterPath, "Local", "PORT", "5000"));
                tcpNet = new DXH.Net.DXHTCPClient(RemoteIP, RemotePort, LocalIP, LocalPort);
                tcpNet.ConnectStateChanged += TcpNet_ConnectStateChanged;
            }
            catch (Exception ex)
            {
                AddMessage(ex.Message);
            }

            #endregion

            AppLoadedEventCommand = new DelegateCommand(new Action(this.AppLoadedEventCommandExecute));
            MenuActionCommand = new DelegateCommand<object>(new Action<object>(this.MenuActionCommandExecute));
            FolderBrowserDialogCommand = new DelegateCommand(new Action(this.FolderBrowserDialogCommandExecute));
            ParameterSaveCommand = new DelegateCommand(new Action(this.ParameterSaveCommandExecute));
            OperateButtonCommand = new DelegateCommand<object>(new Action<object>(this.OperateButtonCommandExecute));
        }

        private void TcpNet_ConnectStateChanged(object sender, string e)
        {
            StatusTCP = e == "Connected";
        }

        private void OperateButtonCommandExecute(object obj)
        {
            switch (obj.ToString())
            {
                case "0":
                    if (EStopIsChecked)
                    {
                        //急停按钮按下
                        AddMessage("急停按钮按下");
                        try
                        {
                            Mysql mysql = new Mysql();
                            if (mysql.Connect())
                            {
                                string stm = $"UPDATE avilinestate SET M1State = -1";
                                int result = mysql.executeQuery(stm);
                                if (result < 1)
                                {
                                    AddMessage("数据库更新失败");
                                    StatusDataBase = false;
                                }
                                else
                                    StatusDataBase = true;
                            }
                            else
                            {
                                AddMessage("数据库未连接");
                                StatusDataBase = false;
                            }
                            mysql.DisConnect();
                        }
                        catch (Exception ex)
                        {
                            AddMessage($"数据库连接失败{ex.Message}");
                            StatusDataBase = false;
                        }

                    }
                    else
                    {
                        //急停按钮抬起
                        AddMessage("急停按钮抬起");
                        try
                        {
                            Mysql mysql = new Mysql();
                            if (mysql.Connect())
                            {
                                string stm = $"UPDATE avilinestate SET M1State = 0,M2State = 0,M3State = 0,M4State = 0";
                                int result = mysql.executeQuery(stm);
                                if (result < 1)
                                {
                                    AddMessage("数据库更新失败");
                                    StatusDataBase = false;
                                }
                                else
                                    StatusDataBase = true;
                            }
                            else
                            {
                                AddMessage("数据库未连接");
                                StatusDataBase = false;
                            }
                            mysql.DisConnect();
                        }
                        catch (Exception ex)
                        {
                            AddMessage($"数据库连接失败{ex.Message}");
                            StatusDataBase = false;
                        }
                    }
                    break;
                case "1":
                    //Inifile.INIWriteValue(iniParameterPath, "System", "CameraName", "[0] Integrated Camera");
                    //Inifile.INIWriteValue(iniParameterPath, "System", "CameraInterface", "DirectShow");
                    //AddMessage("待添加内容");
                    if (mycam.GrabImage(0, false))
                    {
                        AddMessage("拍照成功");
                        CameraIamge = mycam.CurrentImage;
                    }
                    else
                    {
                        AddMessage("拍照失败");
                    }
                    break;
                case "2":
                    break;
                default:
                    break;
            }
        }

        private void ParameterSaveCommandExecute()
        {
            Inifile.INIWriteValue(iniParameterPath, "System", "RemotePath", RemotePath);
            Inifile.INIWriteValue(iniParameterPath, "System", "MachineID", MachineID);
            Inifile.INIWriteValue(iniParameterPath, "Remote", "IP", RemoteIP);
            Inifile.INIWriteValue(iniParameterPath, "Remote", "PORT", RemotePort.ToString());
            Inifile.INIWriteValue(iniParameterPath, "Local", "IP", LocalIP);
            Inifile.INIWriteValue(iniParameterPath, "Local", "PORT", LocalPort.ToString());
            AddMessage("保存参数");
        }

        private void FolderBrowserDialogCommandExecute()
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    RemotePath = dialog.SelectedPath;
                }
            }
        }

        private void MenuActionCommandExecute(object obj)
        {
            switch (obj.ToString())
            {
                case "0":
                    HomePageVisibility = "Visible";
                    ParameterPageVisibility = "Collapsed";
                    break;
                case "1":
                    HomePageVisibility = "Collapsed";
                    ParameterPageVisibility = "Visible";
                    break;
                default:
                    break;
            }
        }

        private void AppLoadedEventCommandExecute()
        {
            AddMessage("软件加载完成");
            try
            {
                Mysql mysql = new Mysql();
                if (mysql.Connect())
                {
                    string stm = $"SELECT NOW()";
                    DataSet ds = mysql.Select(stm);
                    AddMessage($"数据库连接成功{ ds.Tables["table0"].Rows[0][0]}");
                    StatusDataBase = true;
                }
                else
                {
                    AddMessage("数据库未连接");
                    StatusDataBase = false;
                }
                mysql.DisConnect();
            }
            catch (Exception ex)
            {
                AddMessage($"数据库连接失败{ex.Message}");
                StatusDataBase = false;
            }
            #region 初始化相机
            if (mycam.OpenCamera(cameraName, cameraInterface))
            {
                AddMessage("相机打开成功");
            }
            else
            {
                AddMessage("相机打开失败");
            }
            #endregion
            tcpNet.StartTCPConnect();
            Run();
        }
        #endregion
        #region 自定义函数
        private void AddMessage(string str)
        {
            string[] s = MessageStr.Split('\n');
            if (s.Length > 1000)
            {
                MessageStr = "";
            }
            if (MessageStr != "")
            {
                MessageStr += "\n";
            }
            MessageStr += System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + " " + str;
        }
        private async void Run()
        {
            Mysql mysql; int result;
            await Task.Delay(1000);
            while (true)
            {
                try
                {

                    switch (StationNo)
                    {
                        case 1:
                            #region 1号机内容


                            if (H3u.ReadM("M2000"))
                            {
                                AddMessage("进料位-等待进板");
                                H3u.SetM("M2000", false);
                                short[] newBoard = new short[70];
                                for (int i = 0; i < 70; i++)
                                {
                                    Random rd = new Random();
                                    if (rd.Next(100) < 80)
                                    {
                                        newBoard[i] = 1;
                                    }
                                    else
                                    {
                                        newBoard[i] = 0;
                                    }
                                }
                                H3u.WriteMultD("D4000", newBoard);
                                await Task.Delay(500);
                                H3u.SetM("M2100", true);
                                AddMessage("进料位-进板完成");
                            }

                            if (H3u.ReadM("M2011"))
                            {
                                AddMessage("工位1-进板完成");
                                H3u.SetM("M2011", false);
                                mysql = new Mysql();
                                if (await Task.Run<bool>(() => { return mysql.Connect(); }))
                                {
                                    var guid = Guid.NewGuid();
                                    CurBoardID = guid.ToString();
                                    string stm = $"UPDATE avilinestate SET M1BoardID = '{guid}'";
                                    result = mysql.executeQuery(stm);
                                    if (result > 0)
                                    {
                                        AddMessage($"板编号 {guid}_{MachineID} 更新成功");
                                    }
                                }
                                else
                                {
                                    AddMessage("数据库未连接");
                                    StatusDataBase = false;
                                }
                                mysql.DisConnect();
                            }



                            if (H3u.ReadM("M2010"))
                            {
                                int d2010 = H3u.ReadW("D2010");
                                AddMessage($"工位1-{d2010}-开始拍照");
                                H3u.SetM("M2010", false);
                                if (mycam.GrabImage(0, false))
                                {
                                    AddMessage("拍照成功");
                                    CameraIamge = mycam.CurrentImage;
                                    if (await Task.Run<bool>(() => { return mycam.SaveImage("bmp", Path.Combine(RemotePath, $"{CurBoardID}_{MachineID}_{d2010}.bmp")); }))
                                    {
                                        AddMessage("图片保存成功");
                                        tcpNet.TCPSend($"{CurBoardID}_{MachineID}_{d2010}.bmp");
                                    }
                                    else
                                    {
                                        AddMessage("图片保存失败");
                                    }
                                    H3u.SetM("M2110", true);
                                }
                                
                            }
                            
                            #endregion
                            break;

                        case 2:
                            #region 2号机内容
                            if (H3u.ReadM("M2021"))
                            {
                                AddMessage("工位2-进板完成");
                                H3u.SetM("M2021", false);
                                mysql = new Mysql();
                                if (await Task.Run<bool>(() => { return mysql.Connect(); }))
                                {
                                    string stm = $"SELECT * FROM avilinestate LIMIT 1";
                                    DataSet ds = mysql.Select(stm);
                                    DataTable dt = ds.Tables["table0"];

                                    CurBoardID = (string)dt.Rows[0]["M1BoardID"];

                                    stm = $"UPDATE avilinestate SET M2BoardID = '{CurBoardID}'";
                                    result = mysql.executeQuery(stm);
                                    if (result > 0)
                                    {
                                        AddMessage($"板编号 {CurBoardID}_{MachineID} 更新成功");
                                    }
                                }
                                else
                                {
                                    AddMessage("数据库未连接");
                                    StatusDataBase = false;
                                }
                                mysql.DisConnect();
                            }



                            if (H3u.ReadM("M2020"))
                            {
                                int d2020 = H3u.ReadW("D2020");
                                AddMessage($"工位2-{d2020}-开始拍照");
                                H3u.SetM("M2020", false);
                                if (mycam.GrabImage(0, false))
                                {
                                    AddMessage("拍照成功");
                                    CameraIamge = mycam.CurrentImage;
                                    if (await Task.Run<bool>(() => { return mycam.SaveImage("bmp", Path.Combine(RemotePath, $"{CurBoardID}_{MachineID}_{d2020}.bmp")); }))
                                    {
                                        AddMessage("图片保存成功");
                                        tcpNet.TCPSend($"{CurBoardID}_{MachineID}_{d2020}.bmp");
                                    }
                                    else
                                    {
                                        AddMessage("图片保存失败");
                                    }
                                    H3u.SetM("M2120", true);
                                }

                            }
                            #endregion
                            break;
                        case 3:
                            #region 3号机内容
                            if (H3u.ReadM("M2031"))
                            {
                                AddMessage("工位3-进板完成");
                                H3u.SetM("M2031", false);
                                mysql = new Mysql();
                                if (await Task.Run<bool>(() => { return mysql.Connect(); }))
                                {
                                    string stm = $"SELECT * FROM avilinestate LIMIT 1";
                                    DataSet ds = mysql.Select(stm);
                                    DataTable dt = ds.Tables["table0"];

                                    CurBoardID = (string)dt.Rows[0]["M2BoardID"];

                                    stm = $"UPDATE avilinestate SET M3BoardID = '{CurBoardID}'";
                                    result = mysql.executeQuery(stm);
                                    if (result > 0)
                                    {
                                        AddMessage($"板编号 {CurBoardID}_{MachineID} 更新成功");
                                    }
                                }
                                else
                                {
                                    AddMessage("数据库未连接");
                                    StatusDataBase = false;
                                }
                                mysql.DisConnect();
                            }



                            if (H3u.ReadM("M2030"))
                            {
                                int d2030 = H3u.ReadW("D2030");
                                AddMessage($"工位3-{d2030}-开始拍照");
                                H3u.SetM("M2030", false);
                                if (mycam.GrabImage(0, false))
                                {
                                    AddMessage("拍照成功");
                                    CameraIamge = mycam.CurrentImage;
                                    if (await Task.Run<bool>(() => { return mycam.SaveImage("bmp", Path.Combine(RemotePath, $"{CurBoardID}_{MachineID}_{d2030}.bmp")); }))
                                    {
                                        AddMessage("图片保存成功");
                                        tcpNet.TCPSend($"{CurBoardID}_{MachineID}_{d2030}.bmp");
                                    }
                                    else
                                    {
                                        AddMessage("图片保存失败");
                                    }
                                    H3u.SetM("M2130", true);
                                }

                            }
                            #endregion
                            break;
                        case 4:
                            #region 4号机内容
                            if (H3u.ReadM("M2041"))
                            {
                                AddMessage("工位4-进板完成");
                                H3u.SetM("M2041", false);
                                mysql = new Mysql();
                                if (await Task.Run<bool>(() => { return mysql.Connect(); }))
                                {
                                    string stm = $"SELECT * FROM avilinestate LIMIT 1";
                                    DataSet ds = mysql.Select(stm);
                                    DataTable dt = ds.Tables["table0"];

                                    CurBoardID = (string)dt.Rows[0]["M3BoardID"];

                                    stm = $"UPDATE avilinestate SET M4BoardID = '{CurBoardID}'";
                                    result = mysql.executeQuery(stm);
                                    if (result > 0)
                                    {
                                        AddMessage($"板编号 {CurBoardID}_{MachineID} 更新成功");
                                    }
                                }
                                else
                                {
                                    AddMessage("数据库未连接");
                                    StatusDataBase = false;
                                }
                                mysql.DisConnect();
                            }

                            if (H3u.ReadM("M2040"))
                            {
                                int d2040 = H3u.ReadW("D2040");
                                AddMessage($"工位4-{d2040}-开始拍照");
                                H3u.SetM("M2040", false);
                                if (mycam.GrabImage(0, false))
                                {
                                    AddMessage("拍照成功");
                                    CameraIamge = mycam.CurrentImage;
                                    if (await Task.Run<bool>(() => { return mycam.SaveImage("bmp", Path.Combine(RemotePath, $"{CurBoardID}_{MachineID}_{d2040}.bmp")); }))
                                    {
                                        AddMessage("图片保存成功");
                                        tcpNet.TCPSend($"{CurBoardID}_{MachineID}_{d2040}.bmp");
                                    }
                                    else
                                    {
                                        AddMessage("图片保存失败");
                                    }
                                    H3u.SetM("M2140", true);
                                }

                            }
                            #endregion
                            break;
                        default:
                            break;
                    }


                    StatusPLC = H3u.ConnectState;
                }
                catch (Exception ex)
                {
                    AddMessage(ex.Message);
                }
                
                await Task.Delay(100);
            }
        }
        #endregion
    }
}
