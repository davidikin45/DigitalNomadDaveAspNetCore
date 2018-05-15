using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.Common.Implementation.Dtos
{
    public class WebApiPagedResponseDto<T>
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

        public string PreviousPageLink
        { get; set; }

        public string NextPageLink
        { get; set; }

        public bool HasPrevious
        {
            get
            {
                return (Page > 1);
            }
        }

        public bool HasNext
        {
            get
            {
                return (Page < Total);
            }
        }


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
