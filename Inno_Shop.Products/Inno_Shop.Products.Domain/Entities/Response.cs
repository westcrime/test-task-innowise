using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inno_Shop.Products.Domain.Entities
{
    public class Response<T>
    {
        public Response(T? data, Result result)
        {
            if (result.IsSuccess && data == null)
            {
                throw new ArgumentNullException("Wrong result", nameof(result));
            }
            this.Data = data;
            this.Result = result;
        }
        public T? Data { get; }
        public Result Result { get; }
    }
}
