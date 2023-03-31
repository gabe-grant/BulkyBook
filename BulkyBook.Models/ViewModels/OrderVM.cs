using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.Models.ViewModels
{
	public class OrderVM
	{
        public OrderHeader OrderHeader { get; set; }
        // an order can have multiple order details
        public IEnumerable<OrderDetail> OrderDetails{ get; set; }
    }
}
