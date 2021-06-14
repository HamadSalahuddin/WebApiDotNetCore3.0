using Entities.Models;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace Contracts
{
    public interface IDataShaper<T>
    {
        IEnumerable<ShapeEntity> ShapeData(IEnumerable<T> entities, string fieldsString);
        ShapeEntity ShapeData(T entity, string fieldsString);
    }
}
