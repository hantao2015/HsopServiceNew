using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using hsopPlatform;
using HS.Platform;
using System.Collections;

namespace Machinelib
{
   enum  Devicetype
   {
       zkmu260=1,
       unknown=0

   }
   public class  enrolldata
   {

       public  long lngpnid;
       public long recid;
       public string strygno;
       public string strname;
       public string strisvalid;
       public string strpin;
       public long lngcardno;
       public string strcardguid;
       public string strdeviceip;
       public long lngdeviceport;
       public long deviceid;
       static public bool updatedb(ref CmsPassport pst,enrolldata aData,   string errtxt, bool isSucceed,ref string strerr )
       {
           try
           {
               long resid = 412431424724;
               CmsTableParam cp = new CmsTableParam();
               Hashtable ht = new Hashtable();
               if (isSucceed)
               {
                   ht.Add("C3_412431951771", "Y");

               }
               else
               {
                   ht.Add("C3_412431951771", "N");

               }
               ht.Add("C3_412445046801", errtxt);
               ht.Add(" C3_412431963339", DateTime.Now);
               CmsTable.EditRecord(ref pst, resid, aData.recid, ref ht, ref cp);
           }
           catch (Exception ex)

           {
               strerr = ex.Message.ToString();
               return false;

           }


           return true;
       }

       static  public bool getenroll(long lngpnid,long deviceid,ref CmsPassport pst,ref Hashtable  htdata,ref string error)
       {
           long resid = 412431424724;
           DataSet ds=new DataSet ();
           int rec=0;
          
            try
           { 
            string strsql="select *  from ct412431424724 where C3_412431503382="+lngpnid.ToString()+" and C3_412431485300="+deviceid.ToString()+"";

             ds=CmsTable.GetDatasetForHostTable(ref pst, resid, false, "", "", strsql, 0, 0, ref rec, "", false);
             htdata.Clear();
             for (int i = 0; i < ds.Tables[0].DefaultView.Count ;i++ )
             {
                 enrolldata tmpdata = new enrolldata();
                 tmpdata.deviceid = deviceid;
                 tmpdata.lngpnid = lngpnid;
                 tmpdata.lngcardno = Convert.ToInt32(ds.Tables[0].DefaultView[i].Row["C3_412431618896"]);
                 tmpdata.strcardguid = Convert.ToString(ds.Tables[0].DefaultView[i].Row["C3_412442531754"]);
                 tmpdata.strisvalid = Convert.ToString(ds.Tables[0].DefaultView[i].Row["C3_412431575705"]);
                 tmpdata.strname = Convert.ToString(ds.Tables[0].DefaultView[i].Row["C3_412431503253"]);
                 tmpdata.strygno = Convert.ToString(ds.Tables[0].DefaultView[i].Row["C3_412431503106"]);
                 tmpdata.recid = Convert.ToInt64(ds.Tables[0].DefaultView[i].Row["REC_ID"]);
                 htdata.Add(tmpdata.lngcardno, tmpdata);

             }
           }
           catch (Exception ex)
           {
               error=ex.Message.ToString();
               return false;
           }
           return true;
       }
       static public bool AddRecords2Db(ref  CmsPassport pst, int deviceid, string dwEnrollNumber, string RecordTime, ref string errtext)
       {
           try
           {
             
               long lngresid = 412433149535;
               CmsTableParam rp = new CmsTableParam();
               Hashtable hs = new Hashtable();
               hs.Clear();
               hs.Add("C3_412433161639", deviceid);
               hs.Add("C3_412433192211", dwEnrollNumber);
               hs.Add("C3_412433237587", DateTime.Parse(RecordTime));
               hs.Add("C3_412433223743", 1);
               CmsTable.AddRecord(ref pst, lngresid, ref hs, ref rp);


           }

           catch (Exception ex)
           {

               errtext = ex.Message;
               SLog.Err("实时数据采集错误："+errtext , ref ex);
               return false;

           }

           return true;
       }


   }
}
