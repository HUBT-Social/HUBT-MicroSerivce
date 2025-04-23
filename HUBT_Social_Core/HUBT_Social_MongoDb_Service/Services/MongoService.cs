using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HUBT_Social_MongoDb_Service.Services
{
    internal class MongoService<Collection>(IMongoCollection<Collection> mongoCollection) : IMongoService<Collection>
        where Collection : class
    {
        private readonly IMongoCollection<Collection> _mongoCollection = mongoCollection;

        public async Task<bool> Create(Collection collection)
        {
            try
            {
                await _mongoCollection.InsertOneAsync(collection);
                Console.WriteLine("Insert thành công vào MongoDB.");
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> Delete(Collection collection)
        {
            try
            {
                var idProperty = typeof(Collection).GetProperties()
                    .FirstOrDefault(p => p.GetCustomAttributes(typeof(BsonIdAttribute), true).Length != 0)
                    ?? throw new InvalidOperationException("No BsonId property found");
                var id = idProperty.GetValue(collection)?.ToString();
                if (string.IsNullOrEmpty(id))
                    throw new InvalidOperationException("Id value is null or empty");

                var filter = BuildIdFilter<Collection>(id);
                var result = await _mongoCollection.DeleteOneAsync(filter);

                return result.DeletedCount > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<Collection?> GetById(string id)
        {
            try
            {
                var filter = BuildIdFilter<Collection>(id);
                return await _mongoCollection.Find(filter).FirstOrDefaultAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> Update(Collection collection)
        {
            try
            {
                var idProperty = typeof(Collection).GetProperties()
                    .FirstOrDefault(p => p.GetCustomAttributes(typeof(BsonIdAttribute), true).Length != 0)
                    ?? throw new InvalidOperationException("No BsonId property found");

                var id = idProperty.GetValue(collection) ?? throw new InvalidOperationException("Id value is null");
                var filter = BuildIdFilter<Collection>(id);
                var updateResult = await _mongoCollection.ReplaceOneAsync(filter, collection);
                return updateResult.ModifiedCount > 0;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Update failed: {ex.Message}");
                return false;
            }
        }
        public async Task<bool> UpdateByFilter(
            Expression<Func<Collection, bool>> filterExpression,
            UpdateDefinition<Collection> update)
        {
            try
            {
                if (_mongoCollection == null)
                {
                    Console.WriteLine("_mongoCollection is null.");
                    return false;
                }

                if (filterExpression == null || update == null)
                {
                    Console.WriteLine("Filter or update is null.");
                    return false;
                }

                // Chuyển đổi biểu thức LINQ thành FilterDefinition
                var filter = Builders<Collection>.Filter.Where(filterExpression);

                var updateResult = await _mongoCollection.UpdateOneAsync(filter, update);
                Console.WriteLine($"MatchedCount: {updateResult.MatchedCount}, ModifiedCount: {updateResult.ModifiedCount}");
                return updateResult.ModifiedCount > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Update failed: {ex.Message}");
                return false;
            }
        }



        public async Task<IEnumerable<Collection>> GetAll(int? limit = null)
        {
            try
            {
                var query = _mongoCollection.Find(_ => true);
                if (limit.HasValue)
                {
                    query = query.Limit(limit.Value);
                }
                return await query.ToListAsync();
            }
            catch (Exception)
            {
                return [];
            }
        }
        public async Task<IEnumerable<Collection>> GetSlide(int page, int pageSize = 10)
        {
            try
            {
                if (page < 1) page = 1;

                int skip = (page - 1) * pageSize;

                var query = _mongoCollection.Find(_ => true)
                                            .Skip(skip)
                                            .Limit(pageSize);

                return await query.ToListAsync();
            }
            catch (Exception)
            {
                return [];
            }
        }


        public async Task<IEnumerable<Collection>> Find(Expression<Func<Collection, bool>> predicate)
        {
            try
            {
                var filter = Builders<Collection>.Filter.Where(predicate);
                return await _mongoCollection.Find(filter).ToListAsync();
            }
            catch (Exception)
            {
                return new List<Collection>(); // Tránh dùng `[]`, dùng List<Collection>() để tránh lỗi
            }
        }

        public async Task<bool> Exists(string id)
        {
            try
            {
                var filter = BuildIdFilter<Collection>(id);
                return await _mongoCollection.Find(filter).AnyAsync();
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<long> Count()
        {
            try
            {
                return await _mongoCollection.CountDocumentsAsync(_ => true);
            }
            catch (Exception)
            {
                return 0;
            }
        }
        public async Task<List<TField>> GetSlide<TField>
        (
            string id,
            Expression<Func<Collection, IEnumerable<TField>>> fieldSelector,
            int startIndex,
            int count
        )
        {
            if (startIndex < 0 || count <= 0)
                return new List<TField>();

            try
            {
                // Tạo filter dựa trên ID với kiểu Collection
                var filter = BuildIdFilter<Collection>(id);

                // Lấy tên trường từ fieldSelector
                var fieldName = ((MemberExpression)fieldSelector.Body).Member.Name;
                var field = new ExpressionFieldDefinition<Collection>(fieldSelector);

                // Xây dựng projection với Slice
                var projection = Builders<Collection>.Projection
                    .Include(fieldName)
                    .Slice(fieldName, startIndex, count);

                // Thực thi truy vấn
                var result = await _mongoCollection
                    .Find(filter)
                    .Project<Collection>(projection)
                    .ToListAsync();

                // Nếu không tìm thấy document, trả về danh sách rỗng
                if (result == null || result.Count == 0)
                    return new List<TField>();

                // Trích xuất danh sách từ result
                var items = fieldSelector.Compile()(result.FirstOrDefault());
                return items?.ToList() ?? new List<TField>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi lấy dữ liệu: {ex.Message}");
                return new List<TField>();
            }
        }


        private static FilterDefinition<T> BuildIdFilter<T>(object id)
        {
            ArgumentNullException.ThrowIfNull(id);

            if (id is ObjectId objectId)
                return Builders<T>.Filter.Eq("_id", objectId);

            var idString = id.ToString();
            if (ObjectId.TryParse(idString, out ObjectId parsedId))
                return Builders<T>.Filter.Eq("_id", parsedId);

            return Builders<T>.Filter.Eq("_id", idString);
        }
    }

}