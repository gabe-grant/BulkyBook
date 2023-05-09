using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.Utility
{
    // static details class to store all the contants of the application
    public static class SD
    {
        // to avoid "magic strings" we place tall the roles here and then call them in the Register.cshtml file inside Identity
        // role contants
        public const string Role_User_Indi = "Individual";
        public const string Role_User_Comp = "Company";
        public const string Role_Admin = "Admin";
        public const string Role_Employee = "Employee";

        public const string StatusPending = "Pending"; // intial status when order is created
		public const string StatusApproved = "Approved"; // if customer, when payment is approved
		public const string StatusInProcess = "InProcess"; // updated by admin when processing the order
		public const string StatusShipped = "Shipped"; // will be final status 
		public const string StatusCancelled = "Cancelled"; // unless cancelled 
		public const string StatusRefunded = "Refunded"; // or refunded 

        public const string PaymentStatusPending = "Pending"; // intial staus
		public const string PaymentStatusApproved = "Approved"; // then approved
		public const string PaymentStatusDelayedPayment = "ApprovedForDelayedPayment"; // unless it's a company, where they have 30 days to make payment after ship
		public const string PaymentStatusRejected = "Rejected"; // if rejected

        public const string SessionCart = "SessionShoppingCart";
    }
}
