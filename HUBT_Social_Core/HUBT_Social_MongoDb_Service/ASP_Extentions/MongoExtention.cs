using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUBT_Social_MongoDb_Service.ASP_Extentions
{
    public static class MongoExtention
    {
        public static async Task<T?> FirstOrDefaultAsync<T>(this Task<IEnumerable<T>> source)
        {
            var result = await source;
            return result.FirstOrDefault();
        }

        public static async Task<T?> LastOrDefaultAsync<T>(this Task<IEnumerable<T>> source)
        {
            var result = await source;
            return result.LastOrDefault();
        }

        public static async Task<T?> SingleOrDefaultAsync<T>(this Task<IEnumerable<T>> source)
        {
            var result = await source;
            return result.SingleOrDefault();
        }

        public static async Task<bool> AnyAsync<T>(this Task<IEnumerable<T>> source)
        {
            var result = await source;
            return result.Any();
        }

        public static async Task<int> CountAsync<T>(this Task<IEnumerable<T>> source)
        {
            var result = await source;
            return result.Count();
        }

        public static async Task<List<T>> ToListAsync<T>(this Task<IEnumerable<T>> source)
        {
            var result = await source;
            return result.ToList();
        }
    }
}
