using System;
using System.Collections.Generic;

using System.Text;
using System.Data ;
using hsopPlatform;
using HS.Platform;
using System.Collections;
namespace Machinelib
{
    
    public class ImpMachineNameList 

    {
        public event OnDeviceCommandExecuteEventHandler OnDeviceCmdExe;
       
        public bool Deal(  CmsPassport pst,  Hashtable hsmu260,Int32 predeviceno,ref int errorlevel )
        {

            int intErorCount = 0;
            try

            {


                Hashtable hf = new Hashtable();
                CmsTableParam rp = new CmsTableParam();
                long lngResid = 412431424724;
                CmsTableReturn rtncmstable = new CmsTableReturn();
                DataSet ds = new DataSet();
                CardDeal TransObject = new CardDeal();
                DataTable dt = new DataTable();
                string strSQl = "select * from ct412431424724 where C3_412431951771<>'Y'  and C3_412442950775=1 and C3_412431503106 <>'' and C3_412442531754<>'' and C3_412431485300  in (select C3_412431333009  from CT412431312761 where SERVERNO=" + predeviceno.ToString() + ") order by C3_412431963339";
                Exception ex = null;
                int lngReccount = 0;
                string err="";
                dt = CmsTable.GetDatasetForHostTable(ref pst, lngResid, false, "", "", strSQl, 0, 0, ref lngReccount, "", false).Tables[0];
               // SLog.Err("同步信息:dt.Rows.Count=" + dt.Rows.Count.ToString(), ref ex, false);
                if (OnDeviceCmdExe != null)
                {
                  //  OnDeviceCmdExe("ImpMachineNameList.Deal->同步信息:dt.Rows.Count=", dt.Rows.Count.ToString());
                }
                for (int i = 0; i < dt.Rows.Count; i++)
                {

                    long lngRecid = Convert.ToInt64(dt.Rows[i]["REC_ID"]);
                    hf = CmsTable.GetRecordHashtableByRecID(ref pst, lngResid, lngRecid, false);
                    int deviceid = Convert.ToInt32(hf["C3_412431485300"]);
                    if (hsmu260.ContainsKey(deviceid))
                    {

                        mu260 amu=(mu260)hsmu260[deviceid];
                        if (amu.isConTried == false)
                        {
                            if (amu.tryconnection(ref err))
                            {
                                amu.isConnected = true;
                                amu.isConTried = true;
                                hsmu260[deviceid] = amu;

                            }
                        }
                    }
                    if (hsmu260.ContainsKey(deviceid))
                    {
                        mu260 amu = (mu260)hsmu260[deviceid];

                        if (!DealAfterEdit(ref pst, lngResid, lngRecid, ref hf, ref hsmu260, !(amu.isConnected)))
                        {

                            intErorCount++;

                            if (!DealAfterEdit(ref pst, lngResid, lngRecid, ref hf, ref hsmu260, true))
                            { intErorCount++; }
                        }
                        else
                        {
                            intErorCount = 0;
                        }
                        if ((intErorCount > 20))
                        {
                           
                            if (OnDeviceCmdExe != null)
                            {
                                OnDeviceCmdExe("设备名单同步失败大于20次","");
                            }
                            errorlevel = -100;
                            return false;
                        }
                    }


               
                }
            }
            catch (Exception exception1)
            {

                SLog.Err("设备名单同步错误Deal" + exception1.Message, ref  exception1, false);
                if (OnDeviceCmdExe != null)
                {
                    OnDeviceCmdExe("ImpMachineNameList.Deal->设备名单同步错误Deal=", exception1.Message);
                }
                errorlevel = 0;
                return false;

            }
            return true;

        }
       

        public bool DealAfterEdit(ref CmsPassport pst, long lngResID, long lngRecID, ref System.Collections.Hashtable hashFieldVal,ref Hashtable hsmu260,bool istrycon)
        {
            string isSyncro = Convert.ToString(hashFieldVal["C3_412431951771"]);
            if (isSyncro == "Y") return true;

            Devicetype devicetype = Devicetype.unknown;
            if (!(hashFieldVal["C3_412442950775"] == null))
            {
                devicetype = (Devicetype)Convert.ToInt32(hashFieldVal["C3_412442950775"]);
                if (devicetype == Devicetype.zkmu260)
                {

                    try
                    {
                        Exception ex = null;

                        CmsTableParam rp = new CmsTableParam();
                        Hashtable hs = new Hashtable();
                        enrolldata newdata = new enrolldata();
                        mu260 amu260;
                        string errtext = "";
                        int deviceid = Convert.ToInt32(hashFieldVal["C3_412431485300"]);
                        newdata.lngpnid = Convert.ToInt32(hashFieldVal["C3_412442950775"]);
                        newdata.strygno = Convert.ToString(hashFieldVal["C3_412431503106"]);
                        newdata.strname = Convert.ToString(hashFieldVal["C3_412431503253"]);
                        newdata.strisvalid = Convert.ToString(hashFieldVal["C3_412431575705"]);
                        newdata.strpin = Convert.ToString(hashFieldVal["C3_412442073123"]);
                        newdata.lngcardno = Convert.ToInt32(hashFieldVal["C3_412431618896"]);
                        newdata.strcardguid = Convert.ToString(hashFieldVal["C3_412442531754"]);
                        newdata.strdeviceip = Convert.ToString(hashFieldVal["C3_412441936482"]);
                        newdata.lngdeviceport = Convert.ToInt32(hashFieldVal["C3_412441979964"]);
                        //  zkmu260 zk = new zkmu260();
                        // SLog.Err("同步信息:ygno=" + newdata.strygno + "strdeviceip=" + newdata.strdeviceip + "strisvalid" + newdata.strisvalid, ref ex, false);
                        if (OnDeviceCmdExe != null)
                        {
                            OnDeviceCmdExe("ImpMachineNameList.DealAfterEdit->设备名单同步Deal=", newdata.strygno + "strdeviceip=" + newdata.strdeviceip + "strisvalid" + newdata.strisvalid);
                        }
                        if (hsmu260.ContainsKey(deviceid))
                        {
                            amu260 = (mu260)hsmu260[deviceid];
                        }
                        else
                        {
                            return true ;
                        }
                        string err = "";

                        if (amu260.Enroll(newdata, ref errtext, istrycon))
                        {

                            hs.Add("C3_412431951771", "Y");
                            hs.Add("C3_412431963339", DateTime.Now);
                            hs.Add("C3_412445046801", "同步成功" + errtext);

                            DbStatement dbs = null;
                            CmsDbStatement.UpdateRows(ref pst.Dbc, ref hs, "ct412431424724", "rec_id=" + lngRecID.ToString(), ref dbs);
                            if (OnDeviceCmdExe != null)
                            {
                                OnDeviceCmdExe("ImpMachineNameList.DealAfterEdit->Enroll:", "Success");
                            }
                            amu260.updateDeviceOnlineFlags2DB(ref pst, deviceid, "Y", "");
                            return true;
                        }
                        else
                        {
                            hs.Add("C3_412431951771", "N");
                            hs.Add("C3_412431963339", DateTime.Now);
                            hs.Add("C3_412445046801", errtext);

                            DbStatement dbs = null;
                            CmsDbStatement.UpdateRows(ref pst.Dbc, ref hs, "ct412431424724", "rec_id=" + lngRecID.ToString(), ref dbs);
                            if (OnDeviceCmdExe != null)
                            {
                                OnDeviceCmdExe("ImpMachineNameList.DealAfterEdit->Enroll:", "Fail" + errtext);
                            }
                            amu260.updateDeviceOnlineFlags2DB(ref pst, deviceid, "N", errtext);
                            return false;
                        }



                    }
                    catch (Exception ex)
                    {
                        SLog.Err("设备名单同步错误DealAfterEdit" + ex.Message, ref  ex, false);
                        if (OnDeviceCmdExe != null)
                        {
                            OnDeviceCmdExe("ImpMachineNameList.DealAfterEdit", "Exception:" + ex.Message.ToString());
                        }
                        return  false ;
                    }

                }
                else
                {
                    OnDeviceCmdExe("ImpMachineNameList.DealAfterEdit", "devicetype unkown");
                    return true;
                }
               
            }
            return true;

        }
       
     
       
    }
}
