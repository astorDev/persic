using System.Text.RegularExpressions;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Driver.Search;

public class MongoCollection<T> : IMongoCollection<T>
{
    public static string ConventionalName 
    {
        get 
        {
            var name = typeof(T).Name;
            name = Regex.Replace(name, "([a-z0-9])([A-Z])", "$1-$2");
            name = Regex.Replace(name, "([A-Z])([A-Z][a-z])", "$1-$2");
            return name.ToLowerInvariant();
        }
    }

    private readonly IMongoCollection<T> _inner;

    public MongoCollection(IMongoDatabase database, string collectionName = null)
    {
        collectionName ??= ConventionalName;
        _inner = database.GetCollection<T>(collectionName);
    }

    public CollectionNamespace CollectionNamespace => _inner.CollectionNamespace;
    public IMongoDatabase Database => _inner.Database;
    public IBsonSerializer<T> DocumentSerializer => _inner.DocumentSerializer;
    public IMongoIndexManager<T> Indexes => _inner.Indexes;
    public IMongoSearchIndexManager SearchIndexes => _inner.SearchIndexes;
    public MongoCollectionSettings Settings => _inner.Settings;

    public IAsyncCursor<TResult> Aggregate<TResult>(PipelineDefinition<T, TResult> pipeline, AggregateOptions options = null, CancellationToken cancellationToken = default) => _inner.Aggregate(pipeline, options, cancellationToken);
    public IAsyncCursor<TResult> Aggregate<TResult>(IClientSessionHandle session, PipelineDefinition<T, TResult> pipeline, AggregateOptions options = null, CancellationToken cancellationToken = default) => _inner.Aggregate(session, pipeline, options, cancellationToken);
    public Task<IAsyncCursor<TResult>> AggregateAsync<TResult>(PipelineDefinition<T, TResult> pipeline, AggregateOptions options = null, CancellationToken cancellationToken = default) => _inner.AggregateAsync(pipeline, options, cancellationToken);
    public Task<IAsyncCursor<TResult>> AggregateAsync<TResult>(IClientSessionHandle session, PipelineDefinition<T, TResult> pipeline, AggregateOptions options = null, CancellationToken cancellationToken = default) => _inner.AggregateAsync(session, pipeline, options, cancellationToken);
    public void AggregateToCollection<TResult>(PipelineDefinition<T, TResult> pipeline, AggregateOptions options = null, CancellationToken cancellationToken = default) => _inner.AggregateToCollection(pipeline, options, cancellationToken);
    public void AggregateToCollection<TResult>(IClientSessionHandle session, PipelineDefinition<T, TResult> pipeline, AggregateOptions options = null, CancellationToken cancellationToken = default) => _inner.AggregateToCollection(session, pipeline, options, cancellationToken);
    public Task AggregateToCollectionAsync<TResult>(PipelineDefinition<T, TResult> pipeline, AggregateOptions options = null, CancellationToken cancellationToken = default) => _inner.AggregateToCollectionAsync(pipeline, options, cancellationToken);
    public Task AggregateToCollectionAsync<TResult>(IClientSessionHandle session, PipelineDefinition<T, TResult> pipeline, AggregateOptions options = null, CancellationToken cancellationToken = default) => _inner.AggregateToCollectionAsync(session, pipeline, options, cancellationToken);
    public BulkWriteResult<T> BulkWrite(IEnumerable<WriteModel<T>> requests, BulkWriteOptions options = null, CancellationToken cancellationToken = default) => _inner.BulkWrite(requests, options, cancellationToken);
    public BulkWriteResult<T> BulkWrite(IClientSessionHandle session, IEnumerable<WriteModel<T>> requests, BulkWriteOptions options = null, CancellationToken cancellationToken = default) => _inner.BulkWrite(session, requests, options, cancellationToken);
    public Task<BulkWriteResult<T>> BulkWriteAsync(IEnumerable<WriteModel<T>> requests, BulkWriteOptions options = null, CancellationToken cancellationToken = default) => _inner.BulkWriteAsync(requests, options, cancellationToken);
    public Task<BulkWriteResult<T>> BulkWriteAsync(IClientSessionHandle session, IEnumerable<WriteModel<T>> requests, BulkWriteOptions options = null, CancellationToken cancellationToken = default) => _inner.BulkWriteAsync(session, requests, options, cancellationToken);
    public long Count(FilterDefinition<T> filter, CountOptions options = null, CancellationToken cancellationToken = default) => _inner.Count(filter, options, cancellationToken);
    public long Count(IClientSessionHandle session, FilterDefinition<T> filter, CountOptions options = null, CancellationToken cancellationToken = default) => _inner.Count(session, filter, options, cancellationToken);
    public Task<long> CountAsync(FilterDefinition<T> filter, CountOptions options = null, CancellationToken cancellationToken = default) => _inner.CountAsync(filter, options, cancellationToken);
    public Task<long> CountAsync(IClientSessionHandle session, FilterDefinition<T> filter, CountOptions options = null, CancellationToken cancellationToken = default) => _inner.CountAsync(session, filter, options, cancellationToken);
    public long CountDocuments(FilterDefinition<T> filter, CountOptions options = null, CancellationToken cancellationToken = default) => _inner.CountDocuments(filter, options, cancellationToken);
    public long CountDocuments(IClientSessionHandle session, FilterDefinition<T> filter, CountOptions options = null, CancellationToken cancellationToken = default) => _inner.CountDocuments(session, filter, options, cancellationToken);
    public Task<long> CountDocumentsAsync(FilterDefinition<T> filter, CountOptions options = null, CancellationToken cancellationToken = default) => _inner.CountDocumentsAsync(filter, options, cancellationToken);
    public Task<long> CountDocumentsAsync(IClientSessionHandle session, FilterDefinition<T> filter, CountOptions options = null, CancellationToken cancellationToken = default) => _inner.CountDocumentsAsync(session, filter, options, cancellationToken);
    public DeleteResult DeleteMany(FilterDefinition<T> filter, CancellationToken cancellationToken = default) => _inner.DeleteMany(filter, cancellationToken);
    public DeleteResult DeleteMany(FilterDefinition<T> filter, DeleteOptions options, CancellationToken cancellationToken = default) => _inner.DeleteMany(filter, options, cancellationToken);
    public DeleteResult DeleteMany(IClientSessionHandle session, FilterDefinition<T> filter, DeleteOptions options = null, CancellationToken cancellationToken = default) => _inner.DeleteMany(session, filter, options, cancellationToken);
    public Task<DeleteResult> DeleteManyAsync(FilterDefinition<T> filter, CancellationToken cancellationToken = default) => _inner.DeleteManyAsync(filter, cancellationToken);
    public Task<DeleteResult> DeleteManyAsync(FilterDefinition<T> filter, DeleteOptions options, CancellationToken cancellationToken = default) => _inner.DeleteManyAsync(filter, options, cancellationToken);
    public Task<DeleteResult> DeleteManyAsync(IClientSessionHandle session, FilterDefinition<T> filter, DeleteOptions options = null, CancellationToken cancellationToken = default) => _inner.DeleteManyAsync(session, filter, options, cancellationToken);
    public DeleteResult DeleteOne(FilterDefinition<T> filter, CancellationToken cancellationToken = default) => _inner.DeleteOne(filter, cancellationToken);
    public DeleteResult DeleteOne(FilterDefinition<T> filter, DeleteOptions options, CancellationToken cancellationToken = default) => _inner.DeleteOne(filter, options, cancellationToken);
    public DeleteResult DeleteOne(IClientSessionHandle session, FilterDefinition<T> filter, DeleteOptions options = null, CancellationToken cancellationToken = default) => _inner.DeleteOne(session, filter, options, cancellationToken);
    public Task<DeleteResult> DeleteOneAsync(FilterDefinition<T> filter, CancellationToken cancellationToken = default) => _inner.DeleteOneAsync(filter, cancellationToken);
    public Task<DeleteResult> DeleteOneAsync(FilterDefinition<T> filter, DeleteOptions options, CancellationToken cancellationToken = default) => _inner.DeleteOneAsync(filter, options, cancellationToken);
    public Task<DeleteResult> DeleteOneAsync(IClientSessionHandle session, FilterDefinition<T> filter, DeleteOptions options = null, CancellationToken cancellationToken = default) => _inner.DeleteOneAsync(session, filter, options, cancellationToken);
    public IAsyncCursor<TField> Distinct<TField>(FieldDefinition<T, TField> field, FilterDefinition<T> filter, DistinctOptions options = null, CancellationToken cancellationToken = default) => _inner.Distinct(field, filter, options, cancellationToken);
    public IAsyncCursor<TField> Distinct<TField>(IClientSessionHandle session, FieldDefinition<T, TField> field, FilterDefinition<T> filter, DistinctOptions options = null, CancellationToken cancellationToken = default) => _inner.Distinct(session, field, filter, options, cancellationToken);
    public Task<IAsyncCursor<TField>> DistinctAsync<TField>(FieldDefinition<T, TField> field, FilterDefinition<T> filter, DistinctOptions options = null, CancellationToken cancellationToken = default) => _inner.DistinctAsync(field, filter, options, cancellationToken);
    public Task<IAsyncCursor<TField>> DistinctAsync<TField>(IClientSessionHandle session, FieldDefinition<T, TField> field, FilterDefinition<T> filter, DistinctOptions options = null, CancellationToken cancellationToken = default) => _inner.DistinctAsync(session, field, filter, options, cancellationToken);
    public IAsyncCursor<TItem> DistinctMany<TItem>(FieldDefinition<T, IEnumerable<TItem>> field, FilterDefinition<T> filter, DistinctOptions options = null, CancellationToken cancellationToken = default) => _inner.DistinctMany(field, filter, options, cancellationToken);
    public IAsyncCursor<TItem> DistinctMany<TItem>(IClientSessionHandle session, FieldDefinition<T, IEnumerable<TItem>> field, FilterDefinition<T> filter, DistinctOptions options = null, CancellationToken cancellationToken = default) => _inner.DistinctMany(session, field, filter, options, cancellationToken);
    public Task<IAsyncCursor<TItem>> DistinctManyAsync<TItem>(FieldDefinition<T, IEnumerable<TItem>> field, FilterDefinition<T> filter, DistinctOptions options = null, CancellationToken cancellationToken = default) => _inner.DistinctManyAsync(field, filter, options, cancellationToken);
    public Task<IAsyncCursor<TItem>> DistinctManyAsync<TItem>(IClientSessionHandle session, FieldDefinition<T, IEnumerable<TItem>> field, FilterDefinition<T> filter, DistinctOptions options = null, CancellationToken cancellationToken = default) => _inner.DistinctManyAsync(session, field, filter, options, cancellationToken);
    public long EstimatedDocumentCount(EstimatedDocumentCountOptions options = null, CancellationToken cancellationToken = default) => _inner.EstimatedDocumentCount(options, cancellationToken);
    public Task<long> EstimatedDocumentCountAsync(EstimatedDocumentCountOptions options = null, CancellationToken cancellationToken = default) => _inner.EstimatedDocumentCountAsync(options, cancellationToken);
    public Task<IAsyncCursor<TProjection>> FindAsync<TProjection>(FilterDefinition<T> filter, FindOptions<T, TProjection> options = null, CancellationToken cancellationToken = default) => _inner.FindAsync(filter, options, cancellationToken);
    public Task<IAsyncCursor<TProjection>> FindAsync<TProjection>(IClientSessionHandle session, FilterDefinition<T> filter, FindOptions<T, TProjection> options = null, CancellationToken cancellationToken = default) => _inner.FindAsync(session, filter, options, cancellationToken);
    public TProjection FindOneAndDelete<TProjection>(FilterDefinition<T> filter, FindOneAndDeleteOptions<T, TProjection> options = null, CancellationToken cancellationToken = default) => _inner.FindOneAndDelete(filter, options, cancellationToken);
    public TProjection FindOneAndDelete<TProjection>(IClientSessionHandle session, FilterDefinition<T> filter, FindOneAndDeleteOptions<T, TProjection> options = null, CancellationToken cancellationToken = default) => _inner.FindOneAndDelete(session, filter, options, cancellationToken);
    public Task<TProjection> FindOneAndDeleteAsync<TProjection>(FilterDefinition<T> filter, FindOneAndDeleteOptions<T, TProjection> options = null, CancellationToken cancellationToken = default) => _inner.FindOneAndDeleteAsync(filter, options, cancellationToken);
    public Task<TProjection> FindOneAndDeleteAsync<TProjection>(IClientSessionHandle session, FilterDefinition<T> filter, FindOneAndDeleteOptions<T, TProjection> options = null, CancellationToken cancellationToken = default) => _inner.FindOneAndDeleteAsync(session, filter, options, cancellationToken);
    public TProjection FindOneAndReplace<TProjection>(FilterDefinition<T> filter, T replacement, FindOneAndReplaceOptions<T, TProjection> options = null, CancellationToken cancellationToken = default) => _inner.FindOneAndReplace(filter, replacement, options, cancellationToken);
    public TProjection FindOneAndReplace<TProjection>(IClientSessionHandle session, FilterDefinition<T> filter, T replacement, FindOneAndReplaceOptions<T, TProjection> options = null, CancellationToken cancellationToken = default) => _inner.FindOneAndReplace(session, filter, replacement, options, cancellationToken);
    public Task<TProjection> FindOneAndReplaceAsync<TProjection>(FilterDefinition<T> filter, T replacement, FindOneAndReplaceOptions<T, TProjection> options = null, CancellationToken cancellationToken = default) => _inner.FindOneAndReplaceAsync(filter, replacement, options, cancellationToken);
    public Task<TProjection> FindOneAndReplaceAsync<TProjection>(IClientSessionHandle session, FilterDefinition<T> filter, T replacement, FindOneAndReplaceOptions<T, TProjection> options = null, CancellationToken cancellationToken = default) => _inner.FindOneAndReplaceAsync(session, filter, replacement, options, cancellationToken);
    public TProjection FindOneAndUpdate<TProjection>(FilterDefinition<T> filter, UpdateDefinition<T> update, FindOneAndUpdateOptions<T, TProjection> options = null, CancellationToken cancellationToken = default) => _inner.FindOneAndUpdate(filter, update, options, cancellationToken);
    public TProjection FindOneAndUpdate<TProjection>(IClientSessionHandle session, FilterDefinition<T> filter, UpdateDefinition<T> update, FindOneAndUpdateOptions<T, TProjection> options = null, CancellationToken cancellationToken = default) => _inner.FindOneAndUpdate(session, filter, update, options, cancellationToken);
    public Task<TProjection> FindOneAndUpdateAsync<TProjection>(FilterDefinition<T> filter, UpdateDefinition<T> update, FindOneAndUpdateOptions<T, TProjection> options = null, CancellationToken cancellationToken = default) => _inner.FindOneAndUpdateAsync(filter, update, options, cancellationToken);
    public Task<TProjection> FindOneAndUpdateAsync<TProjection>(IClientSessionHandle session, FilterDefinition<T> filter, UpdateDefinition<T> update, FindOneAndUpdateOptions<T, TProjection> options = null, CancellationToken cancellationToken = default) => _inner.FindOneAndUpdateAsync(session, filter, update, options, cancellationToken);
    public IAsyncCursor<TProjection> FindSync<TProjection>(FilterDefinition<T> filter, FindOptions<T, TProjection> options = null, CancellationToken cancellationToken = default) => _inner.FindSync(filter, options, cancellationToken);
    public IAsyncCursor<TProjection> FindSync<TProjection>(IClientSessionHandle session, FilterDefinition<T> filter, FindOptions<T, TProjection> options = null, CancellationToken cancellationToken = default) => _inner.FindSync(session, filter, options, cancellationToken);
    public void InsertMany(IEnumerable<T> documents, InsertManyOptions options = null, CancellationToken cancellationToken = default) => _inner.InsertMany(documents, options, cancellationToken);
    public void InsertMany(IClientSessionHandle session, IEnumerable<T> documents, InsertManyOptions options = null, CancellationToken cancellationToken = default) => _inner.InsertMany(session, documents, options, cancellationToken);
    public Task InsertManyAsync(IEnumerable<T> documents, InsertManyOptions options = null, CancellationToken cancellationToken = default) => _inner.InsertManyAsync(documents, options, cancellationToken);
    public Task InsertManyAsync(IClientSessionHandle session, IEnumerable<T> documents, InsertManyOptions options = null, CancellationToken cancellationToken = default) => _inner.InsertManyAsync(session, documents, options, cancellationToken);
    public void InsertOne(T document, InsertOneOptions options = null, CancellationToken cancellationToken = default) => _inner.InsertOne(document, options, cancellationToken);
    public void InsertOne(IClientSessionHandle session, T document, InsertOneOptions options = null, CancellationToken cancellationToken = default) => _inner.InsertOne(session, document, options, cancellationToken);
    public Task InsertOneAsync(T document, CancellationToken _cancellationToken) => _inner.InsertOneAsync(document, _cancellationToken);
    public Task InsertOneAsync(T document, InsertOneOptions options = null, CancellationToken cancellationToken = default) => _inner.InsertOneAsync(document, options, cancellationToken);
    public Task InsertOneAsync(IClientSessionHandle session, T document, InsertOneOptions options = null, CancellationToken cancellationToken = default) => _inner.InsertOneAsync(session, document, options, cancellationToken);
    public IAsyncCursor<TResult> MapReduce<TResult>(BsonJavaScript map, BsonJavaScript reduce, MapReduceOptions<T, TResult> options = null, CancellationToken cancellationToken = default) => _inner.MapReduce(map, reduce, options, cancellationToken);
    public IAsyncCursor<TResult> MapReduce<TResult>(IClientSessionHandle session, BsonJavaScript map, BsonJavaScript reduce, MapReduceOptions<T, TResult> options = null, CancellationToken cancellationToken = default) => _inner.MapReduce(session, map, reduce, options, cancellationToken);
    public Task<IAsyncCursor<TResult>> MapReduceAsync<TResult>(BsonJavaScript map, BsonJavaScript reduce, MapReduceOptions<T, TResult> options = null, CancellationToken cancellationToken = default) => _inner.MapReduceAsync(map, reduce, options, cancellationToken);
    public Task<IAsyncCursor<TResult>> MapReduceAsync<TResult>(IClientSessionHandle session, BsonJavaScript map, BsonJavaScript reduce, MapReduceOptions<T, TResult> options = null, CancellationToken cancellationToken = default) => _inner.MapReduceAsync(session, map, reduce, options, cancellationToken);
    public IFilteredMongoCollection<TDerivedDocument> OfType<TDerivedDocument>() where TDerivedDocument : T => _inner.OfType<TDerivedDocument>();
    public ReplaceOneResult ReplaceOne(FilterDefinition<T> filter, T replacement, ReplaceOptions options = null, CancellationToken cancellationToken = default) => _inner.ReplaceOne(filter, replacement, options, cancellationToken);
    public ReplaceOneResult ReplaceOne(FilterDefinition<T> filter, T replacement, UpdateOptions options, CancellationToken cancellationToken = default) => _inner.ReplaceOne(filter, replacement, options, cancellationToken);
    public ReplaceOneResult ReplaceOne(IClientSessionHandle session, FilterDefinition<T> filter, T replacement, ReplaceOptions options = null, CancellationToken cancellationToken = default) => _inner.ReplaceOne(session, filter, replacement, options, cancellationToken);
    public ReplaceOneResult ReplaceOne(IClientSessionHandle session, FilterDefinition<T> filter, T replacement, UpdateOptions options, CancellationToken cancellationToken = default) => _inner.ReplaceOne(session, filter, replacement, options, cancellationToken);
    public Task<ReplaceOneResult> ReplaceOneAsync(FilterDefinition<T> filter, T replacement, ReplaceOptions options = null, CancellationToken cancellationToken = default) => _inner.ReplaceOneAsync(filter, replacement, options, cancellationToken);
    public Task<ReplaceOneResult> ReplaceOneAsync(FilterDefinition<T> filter, T replacement, UpdateOptions options = null, CancellationToken cancellationToken = default) => _inner.ReplaceOneAsync(filter, replacement, options, cancellationToken);
    public Task<ReplaceOneResult> ReplaceOneAsync(IClientSessionHandle session, FilterDefinition<T> filter, T replacement, ReplaceOptions options = null, CancellationToken cancellationToken = default) => _inner.ReplaceOneAsync(session, filter, replacement, options, cancellationToken);
    public Task<ReplaceOneResult> ReplaceOneAsync(IClientSessionHandle session, FilterDefinition<T> filter, T replacement, UpdateOptions options = null, CancellationToken cancellationToken = default) => _inner.ReplaceOneAsync(session, filter, replacement, options, cancellationToken);
    public UpdateResult UpdateMany(FilterDefinition<T> filter, UpdateDefinition<T> update, UpdateOptions options = null, CancellationToken cancellationToken = default) => _inner.UpdateMany(filter, update, options, cancellationToken);
    public UpdateResult UpdateMany(IClientSessionHandle session, FilterDefinition<T> filter, UpdateDefinition<T> update, UpdateOptions options = null, CancellationToken cancellationToken = default) => _inner.UpdateMany(session, filter, update, options, cancellationToken);
    public Task<UpdateResult> UpdateManyAsync(FilterDefinition<T> filter, UpdateDefinition<T> update, UpdateOptions options = null, CancellationToken cancellationToken = default) => _inner.UpdateManyAsync(filter, update, options, cancellationToken);
    public Task<UpdateResult> UpdateManyAsync(IClientSessionHandle session, FilterDefinition<T> filter, UpdateDefinition<T> update, UpdateOptions options = null, CancellationToken cancellationToken = default) => _inner.UpdateManyAsync(session, filter, update, options, cancellationToken);
    public UpdateResult UpdateOne(FilterDefinition<T> filter, UpdateDefinition<T> update, UpdateOptions options = null, CancellationToken cancellationToken = default) => _inner.UpdateOne(filter, update, options, cancellationToken);
    public UpdateResult UpdateOne(IClientSessionHandle session, FilterDefinition<T> filter, UpdateDefinition<T> update, UpdateOptions options = null, CancellationToken cancellationToken = default) => _inner.UpdateOne(session, filter, update, options, cancellationToken);
    public Task<UpdateResult> UpdateOneAsync(FilterDefinition<T> filter, UpdateDefinition<T> update, UpdateOptions options = null, CancellationToken cancellationToken = default) => _inner.UpdateOneAsync(filter, update, options, cancellationToken);
    public Task<UpdateResult> UpdateOneAsync(IClientSessionHandle session, FilterDefinition<T> filter, UpdateDefinition<T> update, UpdateOptions options = null, CancellationToken cancellationToken = default) => _inner.UpdateOneAsync(session, filter, update, options, cancellationToken);
    public IChangeStreamCursor<TResult> Watch<TResult>(PipelineDefinition<ChangeStreamDocument<T>, TResult> pipeline, ChangeStreamOptions options = null, CancellationToken cancellationToken = default) => _inner.Watch(pipeline, options, cancellationToken);
    public IChangeStreamCursor<TResult> Watch<TResult>(IClientSessionHandle session, PipelineDefinition<ChangeStreamDocument<T>, TResult> pipeline, ChangeStreamOptions options = null, CancellationToken cancellationToken = default) => _inner.Watch(session, pipeline, options, cancellationToken);
    public Task<IChangeStreamCursor<TResult>> WatchAsync<TResult>(PipelineDefinition<ChangeStreamDocument<T>, TResult> pipeline, ChangeStreamOptions options = null, CancellationToken cancellationToken = default) => _inner.WatchAsync(pipeline, options, cancellationToken);
    public Task<IChangeStreamCursor<TResult>> WatchAsync<TResult>(IClientSessionHandle session, PipelineDefinition<ChangeStreamDocument<T>, TResult> pipeline, ChangeStreamOptions options = null, CancellationToken cancellationToken = default) => _inner.WatchAsync(session, pipeline, options, cancellationToken);
    public IMongoCollection<T> WithReadConcern(ReadConcern readConcern) => _inner.WithReadConcern(readConcern);
    public IMongoCollection<T> WithReadPreference(ReadPreference readPreference) => _inner.WithReadPreference(readPreference);
    public IMongoCollection<T> WithWriteConcern(WriteConcern writeConcern) => _inner.WithWriteConcern(writeConcern);
}