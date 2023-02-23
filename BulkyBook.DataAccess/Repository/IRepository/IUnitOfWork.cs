using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repository.IRepository
{
    // The Unit of Work acts as a parent wrapper of all the repositories we create
    public interface IUnitOfWork
    {
        ICategoryRepository Category { get; }
        void Save();
    }
}
