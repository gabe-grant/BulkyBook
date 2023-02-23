using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;



namespace BulkyBook.DataAccess.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        // Returning an IEnumerable and T can be any class and this is return a list
        T GetFirstOrDefault(Expression<Func<T,bool>> filter);
        IEnumerable<T> GetAll();
        void Add(T entity);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entity);

    }
}


/* 
 * This interface should be able to handle all the Repository Classes, so it will be Generic Typed '<T>'
 * 
 */                                                                                                                                                                                                                                                        