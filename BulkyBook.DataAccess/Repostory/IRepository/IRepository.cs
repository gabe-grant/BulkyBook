using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace BulkyBook.DataAccess.Repostory.IRepository
{
    internal interface IRepository<T> where T : class
    {
        // Assume that this T is for a Category, so what are the common methods to implement?
        IEnumerable<T> GetAll();
    }
}


/* 
 * This interface should be able to handle all the Repository Classes, so it will be Generic Typed '<T>'
 * 
 */