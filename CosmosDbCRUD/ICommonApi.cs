using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CosmosDbCRUD
{
    public interface ICommonApi: IDisposable
    {
        Task CreateCollection();
        Task CreateDatabase();
        Task CreateItems();
        Task DeleteItem(string documentId, object partitionValue);
        Task<ICommonDocument> ReadItem(string documentId);
        List<ICommonDocument> ReadItemCollectionAcrossAllPartition();
        List<ICommonDocument> ReadItemCollectionInPartition();
        List<ICommonDocument> ReadItemCollectionParallelQuery();
        Task UpdateItem(ICommonDocument reading, string documentId);
    }
}