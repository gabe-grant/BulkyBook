using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repository.IRepository
{
    // We are specifying that when we use Category Repository the Model will be Category
    // so that means on the implementation of CategoryRepository if we call get all it will retrieve all the Categories
    // the update method is usually dynamic and so placing it in the specific repository of its type is best practice
    // as well as the save changes to the db
    public interface IShoppingCartRepository : IRepository<ShoppingCart>
    {
        int IncrementCount(ShoppingCart shoppingCart, int count);
        int DecrementCount(ShoppingCart shoppingCart, int count);
    }
}
