﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using Gma.QrCodeNet.Encoding;
using System.Windows.Forms;

partial class frmMain
{
    /// <summary>
    /// 网卡字典
    /// </summary>
    /// <remarks></remarks>
    Dictionary<string, string> networkDict = new Dictionary<string, string>();
    /// <summary>
    /// Smobiler服务
    /// </summary>
    /// <remarks></remarks>
    Smobiler.Core.MobileServer server;

    private void frmMain_Load(object sender, EventArgs e)
    {
        try
        {
            InitialNetworkInterfaces();
            //定义Smobiler服务
            server = new Smobiler.Core.MobileServer();
            server.SessionStart += new Smobiler.Core.SmobilerSessionEventHandler(this.server_SessionStart);
            //设置默认网卡
            if (networkDict.Count > 0)
            {
                string[] combvalues = new string[networkDict.Count];
                networkDict.Keys.CopyTo(combvalues, 0);
                this.combNets.Items.AddRange(combvalues);
                this.combNets.SelectedIndex = 0;
            }
            //模拟Framework初始化代码
            InitFramework();
            //启动Smobiler服务
            StartServer();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
            Application.Exit();
        }
    }

    /// <summary>
    /// 获取当前可用的网卡信息
    /// </summary>
    /// <remarks></remarks>
    private void InitialNetworkInterfaces()
    {
        int i = 1;
        NetworkInterface[] NetworkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
        foreach (NetworkInterface NetworkIntf in NetworkInterfaces)
        {
            if (NetworkIntf.OperationalStatus == OperationalStatus.Up && NetworkIntf.NetworkInterfaceType != NetworkInterfaceType.Loopback)
            {
                IPInterfaceProperties IPInterfaceProperties = NetworkIntf.GetIPProperties();
                UnicastIPAddressInformationCollection UnicastIPAddressInformationCollection = IPInterfaceProperties.UnicastAddresses;
                foreach (UnicastIPAddressInformation UnicastIPAddressInformation in UnicastIPAddressInformationCollection)
                {
                    if (UnicastIPAddressInformation.Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        networkDict.Add(i.ToString() + ":" + NetworkIntf.Name + "/" + NetworkIntf.Description, UnicastIPAddressInformation.Address.ToString());
                        i += 1;
                    }
                }
            }
        }
    }

    private void StartServer()
    {
        try
        {
            //服务起始界面
            server.StartUpForm = typeof(test.SmobilerForm1);
            //服务TCP端口，默认为2323   
            this.txtTcpPort.Text = server.Setting.TcpServerPort.ToString();
            //服务HTTP端口，默认为2324  
            this.txtHTTPPort.Text = server.Setting.HttpServerPort.ToString();
            //启用服务
            server.StartServer();
        }
        catch (System.Net.HttpListenerException ex)
        {
            //遇到HTTP监听无法添加的异常时，需要注册HTTP监听，一般原因是由于当前用户没有管理员权限
            ProcessStartInfo psi = new ProcessStartInfo("netsh", "http delete urlacl url=http://+:" + server.Setting.HttpServerPort.ToString() + "/");
            psi.Verb = "runas";
            psi.CreateNoWindow = true;
            psi.WindowStyle = ProcessWindowStyle.Hidden;
            psi.UseShellExecute = true;
            Process.Start(psi).WaitForExit();
            psi = new ProcessStartInfo("netsh", "http add urlacl url=http://+:" + server.Setting.HttpServerPort.ToString() + "/ user=" + Environment.UserDomainName + "\\" + Environment.UserName);
            psi.Verb = "runas";
            psi.CreateNoWindow = true;
            psi.WindowStyle = ProcessWindowStyle.Hidden;
            psi.UseShellExecute = true;
            Process.Start(psi).WaitForExit();
            server.StartServer();
        }
    }

    /// <summary>
    /// 在下拉框选择变更事件重新生成二维码
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <remarks></remarks>
    private void combNets_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (networkDict.Count > 0)
        {
            this.txtNetAddress.Text = networkDict[this.combNets.SelectedItem.ToString()];
            qrcodeControl.Text = "Smobiler://" + this.txtNetAddress.Text + ":" + server.Setting.TcpServerPort.ToString() + ":" + server.Setting.HttpServerPort.ToString() + ":" + this.txtNetAddress.Text;
        }
    }

    /// <summary>
    /// 打开设置界面
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSetting_Click(object sender, EventArgs e)
    {
        try
        {
            frmSetting setting = new frmSetting();
            if (setting.ShowDialog() == DialogResult.Yes)
            {
                server.StopServer();
                server.Setting.InitialData();
                StartServer();
                qrcodeControl.Text = "Smobiler://" + this.txtNetAddress.Text + ":" + server.Setting.TcpServerPort.ToString() + ":" + server.Setting.HttpServerPort.ToString() + ":" + this.txtNetAddress.Text;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
            Application.Exit();
        }
    }

    #region "Framework Develop Code"
    //在Framework开发方式下，取消以下代码注释

    /// <summary>
    /// 初始化Framework
    /// </summary>
    /// <remarks></remarks>
    private void InitFramework()
    {
        //Searching.Core.DataBaseConnector co = null;
        //co = new Searching.Core.SQLServerConnector();
        ////数据库地址
        //co.DatabaseSource = "127.0.0.1";
        ////数据库类型
        //co.CurrentDataBase = "FrameworkDB";
        ////用户名
        //co.User = "sa";
        ////密码
        //co.Password = "smobiler";
        ////将连接给全局连接对象
        //Searching.Core.SystemSettings.DataConnector = co;
        ////数据库连接超时时间，单位为秒
        //Searching.Core.SystemSettings.CommandTimeout = 60;

        ////在SmobilerForm 中可以通过 Searching.Core.SystemSettings.DataConnector.ConnectionString 获取默认连接数据库字符串
    }

    private void server_SessionStart(object sender, Smobiler.Core.SmobilerSessionEventArgs e)
    {
        ////模拟用户登录注册
        //Searching.Core.SessionObject sobj = Searching.Core.SessionObject.Register("SomeOne", 0, new List<string>(), e.Client.SessionID, "");
        //e.Client.SetUserSessionID(sobj.ID);
        ////在SmobilerForm中可以通过SmobilerFramework.SmobilerDefaultValue.Session.UserID 获取登录用户名
    }
    #endregion
}