using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading;
using CsvHelper.Configuration;

namespace DataProcessor
{
    public class ProcessedOrderMap: ClassMap<ProcessedOrder>
    {
        public ProcessedOrderMap()
        {
            AutoMap(CultureInfo.CurrentCulture);

            Map(map => map.Customer)
                .Name("CustomerNumber");

            Map(map => map.Amount)
                .Name("Quantity")
                .TypeConverter<RomanTypeConverter>();
        }
    }
}
