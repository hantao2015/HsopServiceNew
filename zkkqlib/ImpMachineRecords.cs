using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using hsopPlatform;
using HS.Platform;
using System.Collections;
namespace Machinelib
{
    public class ImpMachineRecords
    {
        public void Deal(ref CmsPassport pst)
        {
            Hashtable hf = new Hashtable();
            CmsTableParam rp = new CmsTableParam();
            long lngResid = 412431312761;
            CmsTableReturn rtncmstable = new CmsTableReturn();
            DataSet ds = new DataSet();
            CardDeal TransObject = new CardDeal();
            DataTable dt = new DataTable();
            string strSQl = "select * from ct412431312761 where C3_412431370485='Y'";
            Exception ex = null;
            SLog.Err("采集:strSQl=" + strSQl, ref ex, false);
            try
            {
                int lngReccount = 0;


                dt = CmsTable.GetDatasetForHostTable(ref pst, lngResid, false, "", "", strSQl, 0, 0, ref lngReccount, "", false).Tables[0];
                SLog.Err("采集信息:dt.Rows.Count=" + dt.Rows.Count.ToString(), ref ex, false);
                for (int i = 0; i < dt.Rows.Count; i++)
                {

                    long lngRecid = Convert.ToInt64(dt.Rows[i]["REC_ID"]);
                    hf = CmsTable.GetRecordHashtableByRecID(ref pst, lngResid, lngRecid, false);
                    DealAfterEdit(ref pst, lngResid, lngRecid, ref hf);

                }
            }
            catch (Exception exception1)
            {

                SLog.Err("采集错误Deal" + exception1.Message, ref  exception1, false);

            }

            /*
             * C3_412431333009 设备编号 整数 8  显示     必填项                   
            2 编辑 C3_412442230440 设备名称 文字 20  显示     必填项                   
            3 编辑 C3_412431345459 设备ip地址 文字 20  显示     必填项                   
            4 编辑 C3_412441957386 设备端口号 整数 8  显示     必填项                   
            5 编辑 C3_412432581536 设备类型 文字 20  显示   计算                    
            6 编辑 C3_412431450055 选择设备类型 整数 8    下拉字典   必填项                   
            7 编辑 C3_412431370485 是否启用 文字 1  显示 是否项(Y/)                      
            8 编辑 C3_412431355980 启用时间 
             C3_412431355980 启用时间 日期 8  显示                        
9 编辑 C3_412474013358 上次通信时间 时间 8  显示                        
10 编辑 C3_412474021868 是否在线 

             * 
             */
        }
        public void DealAfterEdit(ref CmsPassport pst, long lngResID, long lngRecID, ref System.Collections.Hashtable hashFieldVal)
        {


            Devicetype devicetype = Devicetype.unknown;
            if (!(hashFieldVal["C3_412431450055"] == null))
            {
                devicetype = (Devicetype)Convert.ToInt32(hashFieldVal["C3_412431450055"]);
                if (devicetype == Devicetype.zkmu260)
                {

                    try
                    {
                        Exception ex = null;

                        CmsTableParam rp = new CmsTableParam();
                        Hashtable hs = new Hashtable();
                        enrolldata newdata = new enrolldata();
                        string errtext = "";

                        newdata.strdeviceip = Convert.ToString(hashFieldVal["C3_412431345459"]);
                        newdata.lngdeviceport = Convert.ToInt32(hashFieldVal["C3_412441957386"]);
                        newdata.deviceid = Convert.ToInt32(hashFieldVal["C3_412431333009"]);
                       // zkmu260 zk = new zkmu260();
                        SLog.Err("采集数据:strdeviceip=" + newdata.strdeviceip , ref ex, false);
                        if (zkmu260.GetRecords(ref pst, newdata, ref errtext))
                        {

                            hs.Add("C3_412474021868", "Y");
                            hs.Add("C3_412474013358", DateTime.Now);
                            hs.Add("C3_412474229224", "");

                            DbStatement dbs = null;
                            CmsDbStatement.UpdateRows(ref pst.Dbc, ref hs, "ct412431312761", "C3_412431333009=" + newdata.deviceid.ToString(), ref dbs);

                        }
                        else
                        {
                            hs.Add("C3_412474021868", "N");
                            hs.Add("C3_412474013358", DateTime.Now);
                            hs.Add("C3_412474229224", errtext);

                            DbStatement dbs = null;
                            CmsDbStatement.UpdateRows(ref pst.Dbc, ref hs, "ct412431312761", "C3_412431333009=" + newdata.deviceid.ToString(), ref dbs);
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
        }
    }
}

