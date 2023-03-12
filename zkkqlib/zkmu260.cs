using System;
using System.Collections.Generic;

using System.Text;

using hsopPlatform;
using HS.Platform;
using zkemkeeper;
using System.Collections;
namespace Machinelib
{
   public  class zkmu260 
    {
        public static  zkemkeeper.CZKEMClass zkkq = new zkemkeeper.CZKEMClass();
        public static bool Enroll(enrolldata aData, ref string errtext)
        {
           

            if ((aData.strygno == "") || (aData.strygno == null))
            {
                errtext = "工号不能为空";
                return false;
            }
            try
            {
                try
                {
                    string strVer = "";

                    zkkq.GetSDKVersion(ref strVer);
                    if (!zkkq.Connect_Net(aData.strdeviceip, (int)aData.lngdeviceport))
                    {
                        errtext = "设备连接失败";
                        return false;

                    }
                }
                catch
                {
                    int errcode = 0;
                    if (!zkkq.Connect_Net(aData.strdeviceip, (int)aData.lngdeviceport))
                    {
                        zkkq.GetLastError(ref errcode);
                        errtext = "设备连接失败";
                        return false;

                    }
                }

                //zkkq.set_CardNumber(0, (int)(Convert.ToInt64(aData.strcardguid)));

                if (aData.strisvalid == "Y")
                {
                    zkkq.set_STR_CardNumber(0, aData.strcardguid);
                    if (!(zkkq.SSR_SetUserInfo(1, aData.strygno, aData.strname, "", 0, true)))
                    {

                        errtext = "信息注册失败";
                        return false;
                    }
                }
                else
                {
                    if (!(zkkq.SSR_DeleteEnrollData(1, aData.strygno, 0)))
                    {
                        errtext = "信息注销失败";
                        return false;
                    }
                }
            }

            catch (Exception ex)
            {

                errtext = ex.Message;

            }
            finally
            {
                zkkq.Disconnect();
                
            }
           
            return true;
        }
        public static bool GetRecords(ref  CmsPassport pst, enrolldata aData, ref string errtext)
        {
            try
            {
                try
                {
                    string strVer = "";

                    zkkq.GetSDKVersion(ref strVer);
                    if (!zkkq.Connect_Net(aData.strdeviceip, (int)aData.lngdeviceport))
                    {
                        errtext = "设备连接失败";
                        return false;

                    }
                }
                catch
                {
                    int errcode = 0;
                    if (!zkkq.Connect_Net(aData.strdeviceip, (int)aData.lngdeviceport))
                    {
                        zkkq.GetLastError(ref errcode);
                        errtext = "设备连接失败";
                        return false;

                    }
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
                long lngresid=412433149535;
                CmsTableParam rp = new CmsTableParam();
                Hashtable hs = new Hashtable();
                //zkkq.SetDeviceTime(1);
 
                while (zkkq.SSR_GetGeneralLogData(1, out dwEnrollNumber, out dwVerifyMode, out dwInOutMode, out dwYear, out dwMonth, out dwDay, out dwHour, out dwMinute, out dwSecond, ref dwWorkCode))
                {
                    string RecordTime = dwYear.ToString("D4") + "-" + dwMonth.ToString("D2") + "-" + dwDay.ToString("D2") + " " + dwHour.ToString("D2") + ":" + dwMinute.ToString("D2") + ":" + dwSecond.ToString("D2");
                    string RecordDate = dwYear.ToString("D4") + dwMonth.ToString("D2") + dwDay.ToString("D2");
                    hs.Clear();
                    hs.Add("C3_412433161639", aData.deviceid);
                    hs.Add("C3_412433192211", dwEnrollNumber);
                    hs.Add("C3_412433237587", DateTime.Parse(RecordTime));
                    hs.Add("C3_412433223743",1);
                    try
                    {
                        CmsTable.AddRecord(ref pst, lngresid, ref hs, ref rp);
                    }
                    catch
                    { 

                    }

                   
                }

               
            }

            catch (Exception ex)
            {

                errtext = ex.Message;
                return false;

            }
           
            return true;
        }

       
    }
}
/*C3_412433161639 设备编号 整数 8  显示                        
2 编辑 C3_412433192109 人员编号 整数 8  显示                        
3 编辑 C3_412433192211 员工工号 文字 20  显示                        
4 编辑 C3_412433192388 员工姓名 文字 30  显示                        
5 编辑 C3_412433192527 卡号 文字 20  显示                        
6 编辑 C3_412433213827 记录类型 文字 20  显示                        
7 编辑 C3_412433223743 记录类型编号 整数 8  显示                        
8 编辑 C3_412433237587 记录时间 时间 8  显示                        
9 编辑 C3_412433269542 记录类型扩展1 文字 20  显示                        
10 编辑 C3_412433276772 记录类型扩展2 文字 20  显示                        
11 编辑 C3_412433282612 记录类型扩展3 文字 20  显示                        
12 编辑 C3_412433433249 记录类型扩展4 文字 20  显示                        
13 编辑 C3_412433433346 记录类型扩展5 文字 20  显示                        
14 编辑 C3_412433433445 记录类型扩展6 
*/
