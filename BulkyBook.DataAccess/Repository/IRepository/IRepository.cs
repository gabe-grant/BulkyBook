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
        T GetFirstOrDefault(Expression<Func<T,bool>> filter, string? includeProperties = null, bool tracked = true);
        // above and below we are including null properties from the models in the database and including them to manipulate in the repository
        IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter=null, string? includeProperties = null);
        void Add(T entity);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entity);

    }
}


/* 
 * This interface should be able to handle all the Repository Classes, so it will be Generic Typed '<T>'
 * 
 */                                                                                                                                                                                                                                                        