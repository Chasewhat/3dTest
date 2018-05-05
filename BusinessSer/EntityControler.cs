using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusinessSer
{
    public class EntityControler
    {
    }
    /// <summary>
    /// 库位实体类
    /// </summary>
    public class Location
    {
        private string locationNum;

        public string LocationNum
        {
            get { return locationNum; }
            set { locationNum = value; }
        }
        private string locationStatusDesc;

        public string LocationStatusDesc
        {
            get { return locationStatusDesc; }
            set { locationStatusDesc = value; }
        }

        private string locationStatus;

        public string LocationStatus
        {
            get { return locationStatus; }
            set { locationStatus = value; }
        }
        private List<Pallet> locationMater;

        public List<Pallet> LocationMater
        {
            get { return locationMater; }
            set { locationMater = value; }
        }

        private List<Stock> locationStock;

        public List<Stock> LocationStock
        {
            get { return locationStock; }
            set { locationStock = value; }
        }

        private string locationBind;

        public string LocationBind
        {
            get { return locationBind; }
            set { locationBind = value; }
        }

        private string hasIn;

        public string HasIn
        {
            get { return hasIn; }
            set { hasIn = value; }
        }
        private string canIn;

        public string CanIn
        {
            get { return canIn; }
            set { canIn = value; }
        }
    }

    /// <summary>
    /// 货架实体类
    /// </summary>
    public class Pallet
    {
        private string palletNum;

        public string PalletNum
        {
            get { return palletNum; }
            set { palletNum = value; }
        }
        private string palletMater;

        public string PalletMater
        {
            get { return palletMater; }
            set { palletMater = value; }
        }
        private string palletSpec;

        public string PalletSpec
        {
            get { return palletSpec; }
            set { palletSpec = value; }
        }
        private string palletGrade;

        public string PalletGrade
        {
            get { return palletGrade; }
            set { palletGrade = value; }
        }
        private string palletAge;

        public string PalletAge
        {
            get { return palletAge; }
            set { palletAge = value; }
        }
        private string palletQuantiy;

        public string PalletQuantiy
        {
            get { return palletQuantiy; }
            set { palletQuantiy = value; }
        }
    }

    /// <summary>
    /// 库位库存实体
    /// </summary>
    public class Stock
    {
        private string stockMater;

        public string StockMater
        {
            get { return stockMater; }
            set { stockMater = value; }
        }
        private string stockSpec;

        public string StockSpec
        {
            get { return stockSpec; }
            set { stockSpec = value; }
        }

        private string stockPallet;

        public string StockPallet
        {
            get { return stockPallet; }
            set { stockPallet = value; }
        }

        private string stockTyre;

        public string StockTyre
        {
            get { return stockTyre; }
            set { stockTyre = value; }
        }
    }
}
