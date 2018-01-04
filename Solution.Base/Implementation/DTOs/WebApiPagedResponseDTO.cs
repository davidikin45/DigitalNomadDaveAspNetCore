using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solution.Base.Implementation.DTOs
{
    public class WebApiPagedResponseDTO<T>
    {
        public int Page
        { get; set; }

        public int PageSize
        { get; set; }

        [JsonIgnore]
        public int TotalItems
        {
            get { return Records; }
            set { Records = value; }
        }

        public int Records
        { get; set; }

        [JsonIgnore]
        public IEnumerable<T> Data
        {
            get { return Rows; }
        }

        public IList<T> Rows
        { get; set; }

        public int Total
        {
            get
            {
                return Pages;
            }
        }

        public string Search
        { get; set; }

        public string OrderColumn
        { get; set; }

        public string OrderType
        { get; set; }

        [JsonIgnore]
        public int Pages
        {
            get
            {
                return Math.Max((int)Math.Ceiling(Convert.ToDouble(Records) / PageSize), 0);
            }
        }
    }
}
