using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Collections;
using System.Data;
using DbUnity;
using Oracle.DataAccess.Client;

namespace BusinessSer
{
    public class BusinessControler
    {
        /// <summary>
        /// 客户端
        /// </summary>
        public static string client = ConfigurationManager.AppSettings["client"];
        /// <summary>
        /// 配置公司
        /// </summary>
        public static string companyNum = ConfigurationManager.AppSettings["companyNum"];
        /// <summary>
        /// 配置工厂
        /// </summary>
        public static string siteNum = ConfigurationManager.AppSettings["siteNum"];
        /// <summary>
        /// xaml文件名默认为库位库区
        /// </summary>
        public static string area = ConfigurationManager.AppSettings["area"];
        /// <summary>
        /// 获取库位的详细信息
        /// </summary>
        /// <param name="locationNum">库位绑定tag</param>
        /// <returns></returns>
        public static Location GetLocationInfo(string locationBind)
        {
            Location loc = new Location();
            try
            {
                string sqlStr = string.Format(@"select t.*,decode(t.lgpos_capacity_status,'0','Empty','1','Full','Partly') as statusDesc,
                                        NVL((SELECT SUM(L.TYRE_NUMS)
                                         FROM BASE_LGPOS_LIMIT L
                                        WHERE L.COMPANY_NUM = T.COMPANY_NUM AND
                                              L.LGPOS_ID = T.LGPOS_ID),
                                       0) AS HASIN,
                                   NVL(TO_NUMBER(NVL((SELECT R.LIMITE_MAXVALUE
                                                       FROM BASE_LGPOS_RULE R
                                                      WHERE R.COMPANY_NUM = T.COMPANY_NUM AND
                                                            R.LGPOS_ID = T.LGPOS_ID AND
                                                            R.LIMITE_TYPE = '3' AND
                                                            ROWNUM = 1),
                                                     '0')) *
                                       GET_MATERIAL_CAPACITY((SELECT M.LIMITE_MAXVALUE
                                                               FROM BASE_LGPOS_LIMIT M
                                                              WHERE M.COMPANY_NUM = T.COMPANY_NUM AND
                                                                    M.LGPOS_ID = T.LGPOS_ID AND
                                                                    ROWNUM = 1),
                                                             '{3}',
                                                             '{4}'),
                                       0) AS CAP 
                            from base_lgpos t where t.company_num='{0}' and t.sa_id='{1}' and t.lgpos_location='{2}' and rownum=1 ",
                                companyNum, area, locationBind,siteNum,client);
                using (IDataReader reader = DbControler.ExecuteReader(sqlStr))
                {
                    if (reader != null)
                    {
                        if (reader.Read())
                        {
                            loc.LocationNum = reader["lgpos_id"].ToString().Trim();
                            loc.LocationStatus = reader["lgpos_capacity_status"].ToString().Trim();
                            loc.LocationStatusDesc = reader["statusDesc"].ToString().Trim();
                            loc.HasIn = reader["HASIN"].ToString().Trim();
                            loc.CanIn = (int.Parse(reader["CAP"].ToString().Trim()) - int.Parse(reader["HASIN"].ToString().Trim())).ToString();
                            if (int.Parse(loc.CanIn) < 0)
                            {
                                loc.CanIn = "0";
                            }
                            if (loc.HasIn=="0" && loc.CanIn=="0")
                            {
                                loc.CanIn = "9999";
                            }

                            sqlStr = string.Format(@"select * from base_pallet t
                                where t.company_num='{0}' and t.bind_lgpos_id='{1}'", companyNum, loc.LocationNum);
                            using (IDataReader palletReader = DbControler.ExecuteReader(sqlStr))
                            {
                                if (reader != null)
                                {
                                    List<Pallet> palletList = new List<Pallet>();
                                    while (palletReader.Read())
                                    {
                                        Pallet p = new Pallet();
                                        p.PalletNum = palletReader["PALLET_NUM"].ToString().Trim();
                                        p.PalletMater = palletReader["BIND_MATERIAL_NUMBER"].ToString().Trim();
                                        p.PalletSpec = palletReader["BIND_MATERIAL_DESC"].ToString().Trim();
                                        p.PalletGrade = palletReader["BIND_MATERIAL_CHARGE"].ToString().Trim();
                                        p.PalletAge = palletReader["BIND_MIN_HLES"].ToString().Trim();
                                        p.PalletQuantiy = palletReader["BIND_QUANTITY"].ToString().Trim();
                                        palletList.Add(p);
                                    }
                                    loc.LocationMater = palletList;
                                    reader.Close();
                                }
                            }
                        }
                        else
                        {
                            loc = null;
                        }
                        reader.Close();
                    }
                }
            }
            catch
            {
                loc = null;
            }
            return loc;
        }

        /// <summary>
        /// 获取库位及其库存信息
        /// </summary>
        /// <param name="locationNum">库位绑定tag</param>
        /// <returns></returns>
        public static Location GetLocationStockInfo(string locationBind)
        {
            Location loc = new Location();
            try
            {
                string sqlStr = string.Format(@"select t.*,decode(t.lgpos_capacity_status,'0','Empty','1','Full','Partly') as statusDesc 
                            from base_lgpos t where t.company_num='{0}' and t.sa_id='{1}' and t.lgpos_location='{2}' and rownum=1 ",
                                companyNum, area, locationBind);
                using (IDataReader reader = DbControler.ExecuteReader(sqlStr))
                {
                    if (reader != null)
                    {
                        if (reader.Read())
                        {
                            loc.LocationNum = reader["lgpos_id"].ToString().Trim();
                            loc.LocationStatus = reader["lgpos_capacity_status"].ToString().Trim();
                            loc.LocationStatusDesc = reader["statusDesc"].ToString().Trim();

                            sqlStr = string.Format(@"select * from base_lgpos_limit t
                                where t.company_num='{0}' and t.lgpos_id='{1}' and t.limite_type='1'", companyNum, loc.LocationNum);
                            using (IDataReader stockReader = DbControler.ExecuteReader(sqlStr))
                            {
                                if (reader != null)
                                {
                                    List<Stock> stockList = new List<Stock>();
                                    while (stockReader.Read())
                                    {
                                        Stock p = new Stock();
                                        p.StockMater = stockReader["LIMITE_MAXVALUE"].ToString().Trim();
                                        p.StockSpec = stockReader["LIMIT_MATERIAL_DESC"].ToString().Trim();
                                        p.StockPallet = stockReader["PALLET_NUMS"].ToString().Trim();
                                        p.StockTyre = stockReader["TYRE_NUMS"].ToString().Trim();
                                        stockList.Add(p);
                                    }
                                    loc.LocationStock = stockList;
                                    reader.Close();
                                }
                            }
                        }
                        else
                        {
                            loc = null;
                        }
                        reader.Close();
                    }
                }
            }
            catch
            {
                loc = null;
            }
            return loc;
        }

        public static string GetLoctionBind(string bindTag)
        {
            string returnStr = "";
            try
            {
                string sqlStr = string.Format(@"select t.lgpos_id from base_lgpos t
                            where t.company_num='{0}' and t.lgpos_location='{1}' and rownum=1", companyNum, bindTag);
                using (IDataReader reader = DbControler.ExecuteReader(sqlStr))
                {
                    if (reader != null)
                    {
                        if (reader.Read())
                        {
                            returnStr = reader["lgpos_id"].ToString().Trim();
                        }
                        reader.Close();
                    }
                }
            }
            catch (Exception)
            {
                returnStr = "";
            }
            return returnStr;
        }

        /// <summary>
        /// 设置库位绑定图形控件
        /// </summary>
        /// <param name="locationNum">库位编号</param>
        /// <param name="bindTag">绑定控件tag</param>
        /// <returns></returns>
        public static string SetLoctionBind(string locationNum, string bindTag)
        {
            string returnStr = "";
            try
            {
                string sqlStr = string.Format(@"update base_lgpos t
                            set t.lgpos_location='{2}'
                            where t.company_num='{0}' and t.lgpos_id='{1}'", companyNum, locationNum, bindTag);
                int x = DbControler.ExecuteSql(sqlStr);
                if (x != 1)
                {
                    throw new Exception("location not exists!");
                }
                returnStr = "S";
            }
            catch (Exception ex)
            {
                returnStr = ex.Message;
            }
            return returnStr;
        }

        /// <summary>
        /// 获取库位列表--包含库位库容状态
        /// </summary>
        /// <returns></returns>
        public static IList<Location> GetLocations()
        {
            IList<Location> locs = new List<Location>();
            try
            {
                string sqlStr = string.Format(@"select t.*
                            from base_lgpos t where t.company_num='{0}' and t.sa_id='{1}'",
                                companyNum, area);
                using (IDataReader reader = DbControler.ExecuteReader(sqlStr))
                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            Location loc = new Location();
                            loc.LocationNum = reader["lgpos_id"].ToString().Trim();
                            loc.LocationStatus = reader["lgpos_capacity_status"].ToString().Trim();
                            loc.LocationBind = reader["lgpos_location"].ToString().Trim();
                            locs.Add(loc);
                        }
                        reader.Close();
                    }
                }
            }
            catch
            {
                locs.Clear();
            }
            return locs;
        }

        /// <summary>
        /// 计算当前库位的库容状态--0,1/3,2/3,1   暂时只支持4种库容状态
        /// </summary>
        /// <param name="locationNum"></param>
        /// <returns></returns>
        public static int GetLocationCapacity(string locationNum)
        {
            return 1;
        }


        /// <summary>
        /// 获取库位库容状态汇总分布
        /// </summary>
        /// <param name="list1"></param>
        /// <param name="list2"></param>
        public static void GetLocationDistribution(out List<string> list1, out List<string> list2)
        {
            list1 = new List<string>();
            list2 = new List<string>();
            try
            {
                string sqlStr = string.Format(@"SELECT DECODE(T.LGPOS_CAPACITY_STATUS,
                                                  '0',
                                                  'empty',
                                                  '1',
                                                  'full',
                                                  '2',
                                                  'partly') AS sta,
                                           COUNT(1) AS cou
                                      FROM BASE_LGPOS T where t.company_num='{0}' and t.sa_id='{1}'
                                     GROUP BY T.LGPOS_CAPACITY_STATUS
                                     ORDER BY T.LGPOS_CAPACITY_STATUS",
                                companyNum, area);
                using (IDataReader reader = DbControler.ExecuteReader(sqlStr))
                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            list1.Add(reader["sta"].ToString().Trim());
                            list2.Add(reader["cou"].ToString().Trim());
                        }
                        reader.Close();
                    }
                }
            }
            catch
            {
                list1.Clear();
                list2.Clear();
            }
        }

        /// <summary>
        /// 获取已经设置过的库位
        /// </summary>
        /// <returns></returns>
        public static List<string> GetHasSetList()
        {
            List<string> list = new List<string>();
            try
            {
                string sqlStr = string.Format(@"SELECT  distinct t.lgpos_location
                                      FROM BASE_LGPOS T where t.company_num='{0}' and t.sa_id='{1}'",
                                companyNum, area);
                using (IDataReader reader = DbControler.ExecuteReader(sqlStr))
                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            list.Add(reader["lgpos_location"].ToString().Trim());
                        }
                        reader.Close();
                    }
                }
            }
            catch
            {
                list.Clear();
            }
            return list;
        }
    }
}
