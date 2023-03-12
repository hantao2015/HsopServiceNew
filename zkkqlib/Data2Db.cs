using System;
using System.Collections.Generic;
using System.Text;
using HS.Platform;
using System.IO;
using System.Collections;
namespace Machinelib
{
   public  class Data2Db
    {
       public event OnDeviceCommandExecuteEventHandler OnDeviceCmdExe;
       bool isStop = false;
        public bool Save1File2Db(ref CmsPassport pst, enrolldata aData, ref string errtext, string strFilename, string strFullname)
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
                        OnDeviceCmdExe("Cmd=Save1File2Db", "strFullname=" + strFullname + "Save2DbFailsCount" + lngErrorcount.ToString() + "ErrorList=" + errtext);
                    }
                    return false;
                }
                //移动这个文件到备份目录下
                if (!Directory.Exists(ParamConfig.ApplicationPath + "Bak\\"))
                {
                    Directory.CreateDirectory(ParamConfig.ApplicationPath + "Bak\\");
                }
                string BakFileName = ParamConfig.ApplicationPath + "Bak\\" + Convert.ToString(TimeId.CurrentMilliseconds()) + "-" + FileName;
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
                    OnDeviceCmdExe("Cmd=Save1File2Db", "Error:" + errtext);
                }
                return false;
            }
            return true;

        }
        public bool SaveAllFile2Db(CmsPassport pst, enrolldata aData, ref string errtext)
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

                        if (!Save1File2Db(ref pst, aData, ref  tmpText, fi.Name, fi.FullName))
                        {
                            strText = strText + tmpText;
                            errorcount++;
                            if (OnDeviceCmdExe != null)
                            {
                                OnDeviceCmdExe("Cmd=SaveAllFile2Db->Save1File2Db", "Error" + strText);
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
    }
}
