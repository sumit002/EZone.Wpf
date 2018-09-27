using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectronicZone.Wpf.Utility
{
    public class SaleManager
    {
        public SaleManager()
        {

        }

        /// Save Sales [consists of 5 steps]
        /// 1. Create Sale Person If New
        /// 2. Add SaleMaster
        /// 3. Add Pending Payment if any
        /// 4. Add payment transaction
        /// 5. Update Stock
        public bool CreateSale() {


            return true;
        }

        /// Reverse Sale

        /// 1. Remove Sale from SaleMaster
        /// 3. Reverse Pending Payment if any
        /// 4. Reverse payment transaction
        /// 5. Reverse Stock
        public bool ReverseSale() {


            return true;
        }
    }
}
