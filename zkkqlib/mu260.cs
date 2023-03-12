using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using hsopPlatform;
using HS.Platform;
using zkemkeeper;
using System.Collections;
using System.IO;
using System.Windows.Forms;
using System.Net.NetworkInformation;
using Timer=System.Timers.Timer;
using System.Net;
using System.Data;
namespace Machinelib
{
    public delegate void OnAttTransactionEventHandler(ref int DeviceID, ref string ip, int EnrollNumber, int IsInValid, int AttState, int VerifyMethod, int Year, int Month, int Day, int Hour, int Minute, int Second);
    public delegate void OnAttTransactionEXEventHandler(ref int DeviceID, ref string ip, string EnrollNumber, int IsInValid, int AttState, int VerifyMethod, int Year, int Month, int Day, int Hour, int Minute, int Second, int WorkCode);
    public delegate void OnConnectedEventHandler(mu260 aMu260);
    public delegate void OnDisConnectedEventHandler(mu260 aMu260);
    public delegate void OnConnectingEventHandler(mu260 aMu260);
    public delegate void OnConnectTaskOverEventHandler(mu260 aMu260);
    public delegate void OnAttReaRecordEventHandler(ref int DeviceID, ref string ip, int EnrollNumber, int IsInValid, int AttState, int VerifyMethod, int Year, int Month, int Day, int Hour, int Minute, int Second);
    public delegate void OnDeviceCommandExecuteEventHandler(string cmdname,string log);
    public class mu260
    {

        public zkemkeeper.CZKEMClass zkkq = new CZKEMClass();
      
        public string DeviceIp = "";
        public int  DeviceID = 0;
        public int Port = 4370;
         
        public bool isConnnectTaskOver=false;
        public event OnAttTransactionEXEventHandler OnAttTransactionEX;
        public event OnConnectedEventHandler OnConnected;
        public event OnDisConnectedEventHandler DisOnConnected;
        public event OnConnectingEventHandler OnConnecting;
        public event OnConnectTaskOverEventHandler OnConnectTaskOver;
        public event OnAttReaRecordEventHandler OnAttReaRecord;
        public event OnDeviceCommandExecuteEventHandler OnDeviceCmdExe;
        public bool isConTried = false;
        public delegate void  Connect2Net();
        public bool isStop = false;
        public Timer tm;
        public bool isConnected = false;
     
        public void raiseEvent_DisOnConnected()
        {
            if (DisOnConnected != null)
            {

                DisOnConnected(this);
            }
        }
        public mu260()
        {
 
        }
        public mu260(string strIp, int port, int lngDeviceid )
        {
           
            try
            {
                DeviceIp = strIp;
                Port = port;
                DeviceID = lngDeviceid;
            
                Common.GetApplicationPath();
            }

            catch (Exception ex)
            {

            }

        }

     
        public bool Settime()
        {
            try
            {
             
                return zkkq.SetDeviceTime(1);
            }

            catch (Exception ex)
            {
                return false;
            }

          
        }

       
       
        public bool BeginConnect_Net(ref string errtxt)
        {
            
            try
            {
             
                int Result = 0;
                
                isConnnectTaskOver = false;
                
               return  tryconnection(ref errtxt );

            }

            catch (Exception ex)
            {
                return false ;

            }
        }
     
    
        
       
        void zkkq_OnAttTransactionEx(string EnrollNumber, int IsInValid, int AttState, int VerifyMethod, int Year, int Month, int Day, int Hour, int Minute, int Second, int WorkCode)
        {
            if (OnAttTransactionEX != null) 
            {
                OnAttTransactionEX(ref this.DeviceID, ref this.DeviceIp, EnrollNumber, IsInValid, AttState, VerifyMethod, Year, Month, Day, Hour, Minute, Second, WorkCode);
            }

        }

        void zkkq_OnConnected()
        {
            isConnected = true;
            if (OnConnected != null)
            {

                OnConnected(this);
            }
        }

        void zkkq_OnDisConnected()
        {
            isConnected = false;
            DisOnConnected(this);
        }



        public bool GetEnrollInfo(ref string enrollnumber, ref string outstrname,ref string outps,ref int outprevilege,ref bool outisenable,ref string strcardnumber,ref string errtxt)
        {
           // zkkq.get_CardNumber
            string  strnames="";
            string strps="";
            int intpre=0;
            bool beanble=false;
            int cardnumber=0;
            UInt32 ucardnumber = 0;
            try
            {
                if (zkkq.SSR_GetUserInfo(1, enrollnumber, out strnames, out strps, out intpre, out beanble))
                {
                    outstrname = strnames;
                    outps = strps;
                    outprevilege = intpre;
                    outisenable = beanble;
                    zkkq.GetStrCardNumber(out strcardnumber);
                   
                }
                else
                {
                    outstrname = "";
                    outps = "";
                    outprevilege = 0;
                    outisenable = false;
                    return false;
                }
            }
            catch (Exception  ex)
            {
                errtxt = ex.Message.ToString();
                return false;
            }
           

            return true;
        }
        public void updateDeviceOnlineFlags2DB(ref CmsPassport  pst,long deviceid,string isOnline,string errtxt)
        {
            Hashtable hs = new Hashtable();
            hs.Add("C3_412474021868", isOnline);
            hs.Add("C3_412474013358", DateTime.Now);
            hs.Add("C3_412474229224", errtxt);

            DbStatement dbs = null;
            CmsDbStatement.UpdateRows(ref pst.Dbc, ref hs, "ct412431312761", "C3_412431333009=" + deviceid.ToString(), ref dbs);
        }
        public bool tryconnection(ref string errtext)
        {
            try
            {
                if (OnConnecting != null)
                {
                    OnConnecting(this);
                }
                if (!isConnected)
                {
                    zkkq.Disconnect();
                    if (!zkkq.Connect_Net(DeviceIp, Port))
                    {
                        errtext = "Device Not Connected,try to SetDeviceTime Failed";
                        isConnected = false;
                       
                        if (DisOnConnected != null)
                        {
                            DisOnConnected(this);

                        }
                        isConnnectTaskOver = false;
                        return false;
                    }
                    else
                    {
                        zkkq.RegEvent(1, 1);

                        if (!zkkq.SetDeviceTime(1))
                        {
                            errtext = "SetDeviceTime Failed!";
                            return false;
                        }
                        zkkq.OnAttTransactionEx -= zkkq_OnAttTransactionEx; 
                        zkkq.OnAttTransactionEx += zkkq_OnAttTransactionEx;
                        isConnected = true;
                        if (OnConnected != null)
                        {
                            OnConnected(this);

                            if (OnConnectTaskOver != null)
                            {
                                isConnnectTaskOver = true;
                                OnConnectTaskOver(this);
                            }
                        }
                        return true;
                    }

                }

                if (zkkq.SetDeviceTime(1) != true)
                {
                    isConnected = false;
                    zkkq.Disconnect();

                    if (!zkkq.Connect_Net(DeviceIp, Port))
                    {
                        errtext = "Connect_Net Failed and SetDeviceTime Failed!";
                        if (DisOnConnected != null)
                        {
                            DisOnConnected(this);

                        }
                        isConnected = false;
                        return false;
                    }
                    else
                    {
                        zkkq.RegEvent(1, 1);
                        zkkq.OnAttTransactionEx -= zkkq_OnAttTransactionEx;
                        zkkq.OnAttTransactionEx += zkkq_OnAttTransactionEx;
                        if (zkkq.SetDeviceTime(1) != true)
                        {
                            errtext = "Connect_Net suceess but SetDeviceTime Failed!";
                            return false;
                        }
                        if (OnConnected != null)
                        {
                            OnConnected(this);
                            if (OnConnectTaskOver != null)
                            {
                                isConnnectTaskOver = true;
                                OnConnectTaskOver(this);
                            }
                        }
                        isConnected = true;
                        return true;
                    }


                }
                else
                {
                    if (OnConnected != null)
                    {
                        OnConnected(this);

                        if (OnConnectTaskOver != null)
                        {
                            isConnnectTaskOver = true;
                            OnConnectTaskOver(this);
                        }
                    }
                }
                
            }
            catch  (Exception ex)
            {
                SLog.Err("tryconnection error", ref ex, false);
                isConnected = false;
                return false;
               
            }


            return isConnected;

        }
        public   bool Enroll(enrolldata aData, ref string errtext,bool istryconn)
        {
           
            try
            {
                if (istryconn)
                {
                    if (tryconnection(ref errtext) != true)
                    {
                        if (tryconnection(ref errtext) != true)
                        {

                            return false;
                        }
                    }
                }

                int errorcode = 0;
                if ((aData.strygno == "") || (aData.strygno == null))
                {
                    errtext = "工号不能为空";
                    return false;
                }
                 
                 long l_CardNumber = Convert.ToInt64(aData.strcardguid);
                 int CardNumber = (int)l_CardNumber;

               
                if (aData.strisvalid == "Y")
                {
                    zkkq.set_CardNumber(0, CardNumber);
                    if (!(zkkq.SSR_SetUserInfo(1, aData.strygno, aData.strname, "", 0, true)))
                    {

                        zkkq.GetLastError(ref errorcode);
                        errtext = "信息注册失败" + errorcode.ToString();
                        return false;
                    }
                    else
                    {
                        errtext = "注册成功";
                        return true;
                    }
                }
                else
                {
                    string name="";
                    string Password="";
                    int Privilege=0;
                    bool en=false;
                    bool outisenble=false;
                    string strcardnumber="";
                    string errtstring="";
                    CardNumber = (int)1234;
                    zkkq.set_CardNumber(0, CardNumber);
                    if (!(zkkq.SSR_SetUserInfo(1, aData.strygno, aData.strname, "", 0, true)))
                    {

                        zkkq.GetLastError(ref errorcode);
                        errtext = "信息注销失败1234--" + errorcode.ToString();
                        return false;

                    }
                    zkkq.RefreshData(1);

                    if (GetEnrollInfo(ref aData.strygno, ref name, ref Password, ref Privilege, ref outisenble, ref strcardnumber, ref errtstring))
                    {



                        aData.strcardguid = "1234";
                       
                        if (aData.strcardguid == strcardnumber) 
                        {
                            if (!(zkkq.SSR_DeleteEnrollDataExt(1, aData.strygno, 13)))
                            {
                                zkkq.GetLastError(ref errorcode);
                                errtext = "信息注销失败203--" + errorcode.ToString();
                                return false;
                            }
                            else
                            {
                                zkkq.RefreshData(1);
                                errtext = "更换卡号并注销成功删除原来的名单";
                                return true;
                            }
                        }
                        else
                        {
                            zkkq.RefreshData(1);
                            errtext = "注销成功已经有新卡号";
                            return true;
                        }

                    }
                    else
                    {
                        zkkq.RefreshData(1);
                        errtext = "注销成功设备中没有找到此员工";
                        return true;
                    }



                    //zkkq.SSR_SetUserInfo(1, aData.strygno, aData.strname, "", 0, true);
                   
              
                    
                    
                }
            }

            catch (Exception ex)
            {

                errtext = ex.Message;

            }

            
            return true;
        }
        public bool GetAllRecords2Txt(ref CmsPassport pst, enrolldata aData, ref string errtext,bool isClearlog,ref string filename,ref string filefullname, bool isClearlogSilence=false  )
        {
           long lngRecordcount1=0,lngRecordcount2=0;

            try
            {
                if (tryconnection(ref errtext) != true)
                {
                    if (tryconnection(ref errtext) != true)
                    {
                        updateDeviceOnlineFlags2DB(ref pst, this.DeviceID, "N", errtext);
                        return false;
                    }
                }
                int errorcode=0;
               
                if (OnDeviceCmdExe != null)
                {
                    OnDeviceCmdExe("Cmd=GetAllRecords2Txt->ReadAllGLogData","" );
                }
                //zkkq.SetDeviceTime(1);
                if (isClearlog)
                {
                    zkkq.EnableDevice(1, false);
                }
                 if (isClearlogSilence)
                 {
                     if (GetDeviceKqDataCount(ref pst, ref lngRecordcount1, ref errtext))
                     {
                         if (lngRecordcount1 == 0)
                         {
                             Exception e=null;
                             //e.Message = "记录已经采集完毕";
                             DateTime dt = DateTime.Now;

                             string str = "设备编号：" + DeviceID.ToString() + "采集时间：" + dt.ToString();
                             SLog.Err("记录已经采集完毕," + str, ref e);
                             return true;
                         }
                         zkkq.EnableDevice(1, false);
                     }
                     else
                     {
                         isClearlogSilence = false;
                     }
                 }
                if (zkkq.ReadAllGLogData(1))
                {
                    if (!Directory.Exists(ParamConfig.ApplicationPath + "Record\\"))
                    {
                        Directory.CreateDirectory(ParamConfig.ApplicationPath + "Record\\");
                    }
                    filename = DateTime.Now.ToString("yyyyMMddHH-")+Convert.ToString(TimeId.CurrentMilliseconds()) + "-" + DeviceID.ToString() + ".txt";
                    string FileName = ParamConfig.ApplicationPath + "Record\\" + filename;
                    filefullname = FileName;
                    StreamWriter sw = new StreamWriter(FileName, true);
                    int RecordIndex = 0;
                    string DeviceNo = DeviceID.ToString();// cks[Index].Tag.ToString();

                    if (OnDeviceCmdExe != null)
                    {
                        OnDeviceCmdExe("Cmd=GetAllRecords2Txt->WrittingDataToFile", filefullname);
                    }
                        string dwEnrollNumber = "";
                        int dwVerifyMode = 0;
                        int dwInOutMode = 0;
                        int dwYear = 0;
                        int dwMonth = 0;
                        int dwDay = 0;
                        int dwHour = 0;
                        int dwMinute = 0;
                        int dwSecond = 0;
                        int dwWorkCode = 0;
                        long lngresid = 412433149535;
                        CmsTableParam rp = new CmsTableParam();
                        Hashtable hs = new Hashtable();
                        while (zkkq.SSR_GetGeneralLogData(1, out dwEnrollNumber, out dwVerifyMode, out dwInOutMode, out dwYear, out dwMonth, out dwDay, out dwHour, out dwMinute, out dwSecond, ref dwWorkCode))
                        {
                            string RecordTime = dwYear.ToString("D4") + "-" + dwMonth.ToString("D2") + "-" + dwDay.ToString("D2") + " " + dwHour.ToString("D2") + ":" + dwMinute.ToString("D2") + ":" + dwSecond.ToString("D2");
                            string RecordDate = dwYear.ToString("D4") + dwMonth.ToString("D2") + dwDay.ToString("D2");
                            string log= RecordIndex.ToString() + "," + dwEnrollNumber.ToString() + "," + RecordDate + "," + RecordTime + "," + DeviceNo;
                            sw.WriteLine(RecordIndex.ToString() + "," + dwEnrollNumber.ToString() + "," + RecordDate + "," + RecordTime + "," + DeviceNo);
                            
                            RecordIndex = RecordIndex + 1;
                            if (isClearlogSilence)
                            {  //verify record
                                //insert record }
                                //isClearlogSilence = isClearlogSilence;
                                try
                                {
                                    DateTime.Parse(RecordTime);
                                }
                                catch (Exception ex2)
                                {
                                    isClearlogSilence = false;
                                    SLog.Err("数据校验错误，位于：FileName:" + FileName + "/RecordIndex=" + RecordIndex.ToString(), ref ex2);
                                }

                                long intCount = CmsTable.CountByWhere(ref pst, lngresid, "C3_412433192211", "C3_412433192211=" + dwEnrollNumber.ToString() + " and  C3_412433237587='" + RecordTime + "' and C3_412433161639=" + DeviceNo);
                                if (intCount == 0)
                                {
                                    hs.Clear();
                                    hs.Add("C3_412433161639", DeviceNo);
                                    hs.Add("C3_412433192211", dwEnrollNumber);
                                    hs.Add("C3_412433237587", DateTime.Parse(RecordTime));
                                    hs.Add("C3_412433223743", 1);
                                    try
                                    {
                                        CmsTable.AddRecord(ref pst, lngresid, ref hs, ref rp);
                                    }
                                    catch (Exception ex3)
                                    {
                                        isClearlogSilence = false;
                                        SLog.Err("数据写入数据库错误，位于：FileName:" + FileName + "/RecordIndex=" + RecordIndex.ToString(), ref ex3);

                                    }
                                }
                                 

                            }
                          
                        }
                        if (OnDeviceCmdExe != null)
                        {
                            OnDeviceCmdExe("Cmd=GetAllRecords2Txt->DataToFileWritten", filefullname);
                        }

                    sw.Close();
                    if (isClearlog)
                    {
                        DialogResult r = MessageBox.Show("是否真的删除设备上的数据？", "", MessageBoxButtons.OKCancel);
                       if (r==DialogResult.OK)  
                       {
                           zkkq.ClearGLog(1);}  //存文本成功后删除设备中的记录
                        
                        if (OnDeviceCmdExe != null)
                        {
                            OnDeviceCmdExe("Cmd=GetAllRecords2Txt->ClearGLog","");
                        }
                        
                       zkkq.EnableDevice(1, true);
                        
                    }
                    //判断记录前后是否一致
                    if (isClearlogSilence)
                    {
                        if (GetDeviceKqDataCount(ref pst, ref lngRecordcount2, ref errtext))
                        {
                            if ((lngRecordcount1 == lngRecordcount2) && (lngRecordcount2 > 0))
                            {
                                isClearlogSilence = true;
                            }
                            else
                            {
                                isClearlogSilence = false;
                                zkkq.EnableDevice(1, true);
                            }
                        }
                        else
                        {
                            isClearlogSilence = false;
                            zkkq.EnableDevice(1, true);
                        }

                    }
                    //开始删除记录
                    if (isClearlogSilence)
                    {
                        //删除前采集文件
                        if (!Directory.Exists(ParamConfig.ApplicationPath + "RecordFile\\"))
                        {
                            Directory.CreateDirectory(ParamConfig.ApplicationPath + "RecordFile\\");
                        }
                        filename = DateTime.Now.ToString("yyyyMMddHH-") + Convert.ToString(TimeId.CurrentMilliseconds()) + "-" + DeviceID.ToString() + ".txt";
                        string strfilename = ParamConfig.ApplicationPath + "RecordFile\\" + filename;
                        filefullname = strfilename;
                        
                       

                         if (zkkq.GetDataFile(1, 1, filefullname))
                        {
                            zkkq.ClearGLog(1);
                        }
                        
                      
                        zkkq.EnableDevice(1, true);
                    }
                    else
                    {
                        zkkq.EnableDevice(1, true);
                    }

                    isConnected = true;
                    updateDeviceOnlineFlags2DB(ref pst, this.DeviceID, "Y", "");
                    return true;
                }
                else
                {
                    zkkq.GetLastError(ref errorcode);
                    errtext = "errorcode=" + errorcode.ToString();
                    updateDeviceOnlineFlags2DB(ref pst, this.DeviceID, "N", errtext);
                    isConnected = false  ;
                  
                    return false;
                }
            }
            catch (Exception E)
            {
                zkkq.EnableDevice(1, true);
                errtext = E.Message.ToString();
                isConnected = false;
                SLog.Err("数据采集错误， " + errtext , ref E);
                return false;
            }
        }
        public bool GetDeviceKqDataCount(ref CmsPassport pst, ref long lngCount,ref string errtext)
        {
            try
            {
                if (tryconnection(ref errtext) != true)
                {
                    if (tryconnection(ref errtext) != true)
                    {
                        updateDeviceOnlineFlags2DB(ref pst, this.DeviceID, "N", errtext);
                        return false;
                    }
                }
                int errorcode = 0;

                if (OnDeviceCmdExe != null)
                {
                    OnDeviceCmdExe("Cmd=GetAllRecords2Txt->ReadAllGLogData", "");
                }

                if (zkkq.SetDeviceTime(1))
                {
                    int intValue=0;
                    if (zkkq.GetDeviceStatus(1, 6, ref intValue))
                    {
                        lngCount=intValue;
                        return true;
                    }
                    else
                    {
                        errtext = "获取记录数失败!";
                    }


                }
                else
                {
                    errtext = "校时失败!";
                }
            }
            catch (Exception E)
            {
                //zkkq.EnableDevice(1, true);
                errtext = E.Message.ToString();
                isConnected = false;

                return false;
            }

            return false;
        }
        public bool GetKqdatafile(ref CmsPassport pst, enrolldata aData, ref string errtext, bool isClearlog, ref string filename, ref string filefullname)
        {

            try
            {
                if (tryconnection(ref errtext) != true)
                {
                    if (tryconnection(ref errtext) != true)
                    {
                        updateDeviceOnlineFlags2DB(ref pst, this.DeviceID, "N", errtext);
                        return false;
                    }
                }
                int errorcode = 0;

                if (OnDeviceCmdExe != null)
                {
                    OnDeviceCmdExe("Cmd=GetAllRecords2Txt->ReadAllGLogData", "");
                }
               
                if (zkkq.SetDeviceTime(1))
                {
                    if (!Directory.Exists(ParamConfig.ApplicationPath + "RecordFile\\"))
                    {
                        Directory.CreateDirectory(ParamConfig.ApplicationPath + "RecordFile\\");
                    }
                    filename = DateTime.Now.ToString("yyyyMMddHH-") + Convert.ToString(TimeId.CurrentMilliseconds()) + "-" + DeviceID.ToString() + ".txt";
                    string FileName = ParamConfig.ApplicationPath + "RecordFile\\" + filename;
                    filefullname = FileName;
                    int RecordIndex = 0;
                    string DeviceNo = DeviceID.ToString();

                    if (OnDeviceCmdExe != null)
                    {
                        OnDeviceCmdExe("Cmd=GetAllRecords2Txt->WrittingDataToFile", filefullname);
                    }
                    if (zkkq.GetDataFile(1, 1, filefullname))
                    {

                    }
                    else
                    {
                    }

                   
                    if (OnDeviceCmdExe != null)
                    {
                        OnDeviceCmdExe("Cmd=GetAllRecordskqfile->DataToFileWritten", filefullname);
                    }


                    isConnected = true;
                    updateDeviceOnlineFlags2DB(ref pst, this.DeviceID, "Y", "");
                    return true;
                }
                else
                {
                    zkkq.GetLastError(ref errorcode);
                    errtext = "errorcode=" + errorcode.ToString();
                    updateDeviceOnlineFlags2DB(ref pst, this.DeviceID, "N", errtext);
                    isConnected = false;

                    return false;
                }
            }
            catch (Exception E)
            {
                //zkkq.EnableDevice(1, true);
                errtext = E.Message.ToString();
                isConnected = false;

                return false;
            }
        }
        public bool Uploadfile(string adress, string filename, string  savepath, ref string err)
        {
           
            try
            {
                WebClient wb = new WebClient();
                adress = adress + "?savepath=" + savepath;
                wb.UploadFile(adress, filename);

            }
            catch (Exception ex)
            {
                err = ex.Message.ToString();
                return false;
            }
           
            return true;
            
        }
        public bool SaveAllFile2WebServer(string adress,string savepath, ref string errtext)
        {
            string strText = "";
            string tmpText = "";
            try
            {
                DirectoryInfo Dir = new DirectoryInfo(ParamConfig.ApplicationPath + "Record\\");
                long errorcount = 0;

                if (Dir.Exists)
                {
                    foreach (FileInfo fi in Dir.GetFiles("*.txt"))
                    {

                       
                        Thread.Sleep(5000);
                        if (Uploadfile(adress, fi.FullName, savepath, ref errtext))
                        {

                            //移动这个文件到备份目录下
                            if (!Directory.Exists(ParamConfig.ApplicationPath + "Bak\\"))
                            {
                                Directory.CreateDirectory(ParamConfig.ApplicationPath + "Bak\\");
                            }
                            string BakFileName = ParamConfig.ApplicationPath + "Bak\\" + Convert.ToString(TimeId.CurrentMilliseconds()) + "-" + fi.Name ;
                            File.Move(fi.FullName, BakFileName);
                            if (OnDeviceCmdExe != null)
                            {
                                OnDeviceCmdExe("Cmd=Uploadfile Ok:", fi.FullName);
                            }

                        }

                    }
                    if (errorcount > 0)
                    {
                        if (OnDeviceCmdExe != null)
                        {
                            OnDeviceCmdExe("Cmd=Uploadingfile error:", strText);
                        }
                        errtext = strText;

                        return false;
                    }
                }
            }
            catch (Exception e)
            {
                errtext = e.Message.ToString();
                return false;
            }
            return true;
        }
        public bool SaveAllFile2Db(  CmsPassport pst, enrolldata aData, ref string errtext)
        {
            string strText="";
            string tmpText = "";
            try
            {
                DirectoryInfo Dir = new DirectoryInfo(ParamConfig.ApplicationPath + "Record\\");
                long errorcount = 0;

                if (Dir.Exists)
                {
                    foreach (FileInfo fi in Dir.GetFiles("*.txt"))
                    {

                        if (!Save1File2Db(ref pst, aData, ref  tmpText, fi.Name, fi.FullName))
                        {
                            strText = strText + tmpText;
                            errorcount++;
                            if (OnDeviceCmdExe != null)
                            {
                                OnDeviceCmdExe("Cmd=SaveAllFile2Db->Save1File2Db", "Error"+strText);
                            }

                        }
                        else
                        {
                            if (OnDeviceCmdExe != null)
                            {
                                OnDeviceCmdExe("Cmd=SaveAllFile2Db->Save1File2Db", fi.Name);
                            }
                        }
                      
                    }
                    if (errorcount > 0)
                    {
                        errtext = strText;

                        return false;
                    }
                }
            }
            catch (Exception e)
            {
                errtext = e.Message.ToString();
                return false;
            }
             return true;
        }
        public bool getalluserinfo(ref CmsPassport pst)
        {
            string errtext = "";
            long lngErrorcount=0;
            string enrollnumber="";
            string enrollname="";
            string enrollpass="";
            string strcardnumber = "";
            long lngresid=427038975683;
            int enrollpre=0;
            CmsTableParam rp = new CmsTableParam();
            Hashtable hs = new Hashtable();
            DataTable dt = new DataTable();
            string strSql = "";
            DbStatement dbs = null;
            bool enrollenable=false;
        
            try
            {
                if (tryconnection(ref errtext) != true)
                {
                    if (tryconnection(ref errtext) != true)
                    {
                        return false;
                    }
                }
                if (zkkq.ReadAllUserID(1))
                {

                

                    CmsDbBase.DelRowsByWhere(ref pst, "ct427038975683", "C3_427038993650 = " + this.DeviceID.ToString(),ref  dbs);

                    while (zkkq.SSR_GetAllUserInfo(1,out enrollnumber,out enrollname,out enrollpass,out enrollpre,out enrollenable))
                    {
                        zkkq.GetStrCardNumber(out strcardnumber);

                        hs.Clear();
                        hs.Add("C3_427038993650", this.DeviceID);
                        hs.Add("C3_427039004712", enrollnumber);
                        hs.Add("C3_427039010687", enrollname);
                        hs.Add("C3_427039023740", enrollpass);
                        hs.Add("C3_427039017374", strcardnumber);
                        if (enrollenable)
                        {
                            hs.Add("C3_427039031510", "Y");
                        }
                        else
                        {
                            hs.Add("C3_427039031510", "N");
                        }
                       
                        try
                        {
                            Application.DoEvents();

                            if (OnDeviceCmdExe != null)
                            {
                                OnDeviceCmdExe("Cmd=SSR_GetAllUserInfo Ok:Add new Record", "enrollnumber=" + enrollnumber + ";enrollname=" + enrollname + ";strcardnumber=" + strcardnumber);
                            }
                                CmsTable.AddRecord(ref pst, lngresid, ref hs, ref rp);
                    

                        }
                        catch (Exception e)
                        {
                            lngErrorcount++;
                            errtext = errtext + e.Message.ToString();
                            SLog.Err(errtext, ref e);

                        }
                    }

                }


            }
                 //zkkq.GetStrCardNumber(out strcardnumber);

            catch (Exception ex)
            {
                errtext = ex.Message.ToString();
                SLog.Err(errtext, ref ex);

                return false;
            }

            return true ;
        }
        public bool Save1File2Db(ref CmsPassport pst, enrolldata aData, ref string errtext, string strFilename,string strFullname)
        {
             string FileName = strFilename;
             long lngresid = 412433149535;
             long lngErrorcount = 0;
             CmsTableParam rp = new CmsTableParam();
             Hashtable hs = new Hashtable();
             StreamReader sr = new StreamReader(strFullname);
             string aLine;
        
             try
             {
                 while ((aLine = sr.ReadLine()) != null)
                 {
                     string[] str = aLine.Split(',');
                     if (str.Length == 5)
                     {
                         string dwEnrollNumber = str[1];
                         string RecordDate = str[2];
                         string RecordTime = str[3];
                         string DeviceNo = str[4];
                         string strWhere = "C3_412433192211='" + dwEnrollNumber + "'  and C3_412433237587=CONVERT(datetime,'" + RecordTime + "')";
                         hs.Clear();
                         hs.Add("C3_412433161639", DeviceNo);
                         hs.Add("C3_412433192211", dwEnrollNumber);
                         hs.Add("C3_412433237587", DateTime.Parse(RecordTime));
                         hs.Add("C3_412433223743", 1);
                         try
                         {
                             DbStatement dbs = null;
                             if (CmsDbStatement.Count(ref pst.Dbc, "ct412433149535", strWhere, ref dbs) == 0)
                             {
                                 CmsTable.AddRecord(ref pst, lngresid, ref hs, ref rp);
                             }
                              
                         }
                         catch (Exception e)
                         {
                             lngErrorcount++;
                             errtext = errtext + e.Message.ToString();
 
                         }

                        
                         if (OnDeviceCmdExe != null)
                         {
                             OnDeviceCmdExe("Cmd=Save1File2Db", "Save1Record2Db:" + "dwEnrollNumber=" + dwEnrollNumber + "Time=" + DateTime.Parse(RecordTime).ToString() + "Deviceid=" + DeviceNo);
                         }
                     }
                    //Application.DoEvents();
                    if (isStop)
                    {
                        errtext = "User Cancle Operation";
                        isStop = false;
                        return false;
                    }

                 }
                 sr.Close();
                 if (lngErrorcount > 0)
                 {
                     if (OnDeviceCmdExe != null)
                     {
                         OnDeviceCmdExe("Cmd=Save1File2Db", "strFullname=" + strFullname + "Save2DbFailsCount" + lngErrorcount.ToString()+"ErrorList="+errtext);
                     }
                     return false ;
                 }
                 //移动这个文件到备份目录下
                 if (!Directory.Exists(ParamConfig.ApplicationPath + "Bak\\"))
                 {
                     Directory.CreateDirectory(ParamConfig.ApplicationPath + "Bak\\");
                 }
                 string BakFileName = ParamConfig.ApplicationPath  + "Bak\\" + Convert.ToString(TimeId.CurrentMilliseconds())+"-"+FileName;
                 File.Move(strFullname, BakFileName);
                 if (OnDeviceCmdExe != null)
                 {
                     OnDeviceCmdExe("Cmd=Save1File2Db", "Save2Db Ok Then Move " + strFullname + "To" + BakFileName);
                 }
             }
             catch (Exception e)
             {
                 errtext = e.Message.ToString();
                 if (OnDeviceCmdExe != null)
                 {
                     OnDeviceCmdExe("Cmd=Save1File2Db", "Error:"+errtext);
                 }
                 return false;
             }
            return true;
                        
         }
           
        
    
   
       
    

       
    }

    }

