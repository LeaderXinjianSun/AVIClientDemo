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

        private string eStopToggleButtonVisibility;

        public string EStopToggleButtonVisibility
        {
            get { return eStopToggleButtonVisibility; }
            set
            {
                eStopToggleButtonVisibility = value;
                this.RaisePropertyChanged("EStopToggleButtonVisibility");
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
        #endregion
        #region 构造函数
        public MainWindowViewModel()
        {
            #region 初始化参数
            try
            {
                MessageStr = "";
                Version = "20201022";
                HomePageVisibility = "Visible";
                ParameterPageVisibility = "Collapsed";
                RemotePath = Inifile.INIGetStringValue(iniParameterPath, "System", "RemotePath", "D:\\");
                MachineID = Inifile.INIGetStringValue(iniParameterPath, "System", "MachineID", "Null");
                StationNo = 0;
                StationNo = int.Parse(Inifile.INIGetStringValue(iniParameterPath, "System", "StationNo", "1"));
                if (StationNo == 1)
                    EStopToggleButtonVisibility = "Visible";
                else
                    EStopToggleButtonVisibility = "Collapsed";
                cameraName = Inifile.INIGetStringValue(iniParameterPath, "System", "CameraName", "[0] Integrated Camera");
                cameraInterface = Inifile.INIGetStringValue(iniParameterPath, "System", "CameraInterface", "DirectShow");
                Version += StationNo.ToString();
                EStopIsChecked = false;
                CameraROIList = new ObservableCollection<ROI>();
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
                            await Task.Delay(800);
                            mysql = new Mysql();
                            if (await Task.Run<bool>(() => { return mysql.Connect(); }))
                            {
                                string stm = $"SELECT * FROM avilinestate LIMIT 1";
                                DataSet ds = mysql.Select(stm);
                                DataTable dt = ds.Tables["table0"];
                                if ((int)dt.Rows[0]["M1State"] == 0)//0：等待拍照 1：拍照中 2：放板
                                {
                                    var guid = Guid.NewGuid();
                                    stm = $"UPDATE avilinestate SET M1State = 1,M1BoardID = '{guid}'";
                                    result = mysql.executeQuery(stm);
                                    if (result > 0)
                                    {
                                        AddMessage($"板编号 {guid}_{MachineID} 更新成功");
                                        if (mycam.GrabImage(0, false))
                                        {
                                            AddMessage("拍照成功");
                                            CameraIamge = mycam.CurrentImage;
                                            if (await Task.Run<bool>(()=> { return mycam.SaveImage("bmp", Path.Combine(RemotePath, $"{guid}_{MachineID}.bmp")); }))
                                            {
                                                AddMessage("图片保存成功");
                                                stm = $"UPDATE avilinestate SET M1State = 2";
                                                result = mysql.executeQuery(stm);
                                                if (result > 0)
                                                {
                                                    AddMessage("1号机放板");
                                                }
                                            }
                                            else
                                            {
                                                AddMessage("图片保存失败");
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                AddMessage("数据库未连接");
                                StatusDataBase = false;
                            }
                            mysql.DisConnect();
                            #endregion
                            break;
                        case 2:
                            #region 2号机内容
                            mysql = new Mysql();
                            if (await Task.Run<bool>(() => { return mysql.Connect(); }))
                            {
                                string stm = $"SELECT * FROM avilinestate LIMIT 1";
                                DataSet ds = mysql.Select(stm);
                                DataTable dt = ds.Tables["table0"];
                                if ((int)dt.Rows[0]["M1State"] != -1)//0：等待拍照 1：拍照中 2：放板 -1：急停
                                {
                                    if ((int)dt.Rows[0]["M1State"] == 2 && (int)dt.Rows[0]["M2State"] == 0)//1号机放板，2号机等待拍照
                                    {
                                        stm = $"UPDATE avilinestate SET M2State = 1,M2BoardID = M1BoardID";
                                        result = mysql.executeQuery(stm);
                                        if (result > 0)
                                        {
                                            AddMessage("2号机拍照");
                                            if (mycam.GrabImage(0, false))
                                            {
                                                AddMessage("拍照成功");
                                                CameraIamge = mycam.CurrentImage;
                                                string boardID = (string)dt.Rows[0]["M1BoardID"];                                                                                               
                                                if (await Task.Run<bool>(() => { return mycam.SaveImage("bmp", Path.Combine(RemotePath, $"{boardID}_{MachineID}.bmp")); }))
                                                {
                                                    AddMessage("图片保存成功");
                                                    stm = $"SELECT * FROM avilinestate LIMIT 1";
                                                    ds = mysql.Select(stm);
                                                    dt = ds.Tables["table0"];
                                                    if ((int)dt.Rows[0]["M1State"] != -1)
                                                        stm = $"UPDATE avilinestate SET M2State = 2,M1State = 0,M1BoardID = NULL";
                                                    else
                                                        stm = $"UPDATE avilinestate SET M2State = 2,M1BoardID = NULL";
                                                    result = mysql.executeQuery(stm);
                                                    if (result > 0)
                                                    {
                                                        AddMessage("2号机放板");
                                                    }
                                                }
                                                else
                                                {
                                                    AddMessage("图片保存失败");
                                                }
                                            }
                                        }
                                    }
                                    
                                }
                            }
                            else
                            {
                                AddMessage("数据库未连接");
                                StatusDataBase = false;
                            }
                            mysql.DisConnect();
                            #endregion
                            break;
                        case 3:
                            #region 3号机内容
                            mysql = new Mysql();
                            if (await Task.Run<bool>(() => { return mysql.Connect(); }))
                            {
                                string stm = $"SELECT * FROM avilinestate LIMIT 1";
                                DataSet ds = mysql.Select(stm);
                                DataTable dt = ds.Tables["table0"];
                                if ((int)dt.Rows[0]["M1State"] != -1)//0：等待拍照 1：拍照中 2：放板 -1：急停
                                {
                                    if ((int)dt.Rows[0]["M2State"] == 2 && (int)dt.Rows[0]["M3State"] == 0)//1号机放板，2号机等待拍照
                                    {
                                        stm = $"UPDATE avilinestate SET M3State = 1,M3BoardID = M2BoardID";
                                        result = mysql.executeQuery(stm);
                                        if (result > 0)
                                        {
                                            AddMessage("3号机拍照");
                                            if (mycam.GrabImage(0, false))
                                            {
                                                AddMessage("拍照成功");
                                                CameraIamge = mycam.CurrentImage;
                                                string boardID = (string)dt.Rows[0]["M2BoardID"];
                                                if (await Task.Run<bool>(() => { return mycam.SaveImage("bmp", Path.Combine(RemotePath, $"{boardID}_{MachineID}.bmp")); }))
                                                {
                                                    AddMessage("图片保存成功");
                                                    stm = $"UPDATE avilinestate SET M3State = 2,M2State = 0,M2BoardID = NULL";
                                                    result = mysql.executeQuery(stm);
                                                    if (result > 0)
                                                    {
                                                        AddMessage("3号机放板");
                                                    }
                                                }
                                                else
                                                {
                                                    AddMessage("图片保存失败");
                                                }
                                            }
                                        }
                                    }

                                }
                            }
                            else
                            {
                                AddMessage("数据库未连接");
                                StatusDataBase = false;
                            }
                            mysql.DisConnect();
                            #endregion
                            break;
                        case 4:
                            #region 4号机内容
                            mysql = new Mysql();
                            if (await Task.Run<bool>(() => { return mysql.Connect(); }))
                            {
                                string stm = $"SELECT * FROM avilinestate LIMIT 1";
                                DataSet ds = mysql.Select(stm);
                                DataTable dt = ds.Tables["table0"];
                                if ((int)dt.Rows[0]["M1State"] != -1)//0：等待拍照 1：拍照中 2：放板 -1：急停
                                {
                                    if ((int)dt.Rows[0]["M3State"] == 2 && (int)dt.Rows[0]["M4State"] == 0)//1号机放板，2号机等待拍照
                                    {
                                        stm = $"UPDATE avilinestate SET M4State = 1,M4BoardID = M3BoardID";
                                        result = mysql.executeQuery(stm);
                                        if (result > 0)
                                        {
                                            AddMessage("4号机拍照");
                                            if (mycam.GrabImage(0, false))
                                            {
                                                AddMessage("拍照成功");
                                                CameraIamge = mycam.CurrentImage;
                                                string boardID = (string)dt.Rows[0]["M3BoardID"];
                                                if (await Task.Run<bool>(() => { return mycam.SaveImage("bmp", Path.Combine(RemotePath, $"{boardID}_{MachineID}.bmp")); }))
                                                {
                                                    AddMessage("图片保存成功");
                                                    stm = $"UPDATE avilinestate SET M4State = 0,M3State = 0,M3BoardID = NULL";
                                                    result = mysql.executeQuery(stm);
                                                    if (result > 0)
                                                    {
                                                        AddMessage("4号机放板");
                                                    }
                                                }
                                                else
                                                {
                                                    AddMessage("图片保存失败");
                                                }
                                            }
                                        }
                                    }

                                }
                            }
                            else
                            {
                                AddMessage("数据库未连接");
                                StatusDataBase = false;
                            }
                            mysql.DisConnect();
                            #endregion
                            break;
                        default:
                            break;
                    }
                }
                catch (Exception ex)
                {
                    AddMessage(ex.Message);
                }
                
                await Task.Delay(200);
            }
        }
        #endregion
    }
}
