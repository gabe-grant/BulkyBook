using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.Utility
{
	public class StripeSettings
	{
        // these properties must be the exact as the variables inside of appsettings
        // we can map these properties in the program.cs file
        public string SecretKey { get; set; }
        public string PublishableKey { get; set; }
    }
}
