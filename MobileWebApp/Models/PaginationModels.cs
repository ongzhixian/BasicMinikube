namespace MobileWebApp.Models;

public interface IPageMetaData
{
    
    int EndRecordNumber { get; }
    bool IsFirstPage { get; }
    bool IsLastPage { get; }
    int LastPageNumber { get; }
    int NextPage { get; }
    int PageNumber { get; set; }
    int PageSize { get; set; }
    int PreviousPage { get; }
    int StartRecordNumber { get; }
    long TotalRecords { get; set; }
}

public class PageOf<T> : IPageMetaData
{
    public int PageNumber { get; set; }

    public int PageSize { get; set; }

    public long TotalRecords { get; set; }

    public IEnumerable<T> Data { get; set; } = [];

    public bool IsFirstPage
    {
        get
        {
            return PageNumber <= 1;
        }
    }

    public int LastPageNumber
    {
        get
        {
            //return ((int)(TotalRecords / PageSize)) + ((TotalRecords % PageSize > 0) ? 1 : 0);

            // TODO: BUG: crashes if PageSize is 0
            var lastPageNumber = (int)(TotalRecords + PageSize - 1) / PageSize;
            return lastPageNumber <= 0 ? 1 : lastPageNumber;
        }
    }

    public bool IsLastPage
    {
        get
        {
            return PageNumber >= LastPageNumber;
        }
    }

    public int PreviousPage
    {
        get
        {
            if (IsFirstPage) return 1;
            return PageNumber - 1;
        }
    }

    public int NextPage
    {
        get
        {
            if (IsLastPage) return PageNumber;
            return PageNumber + 1;
        }
    }

    public int StartRecordNumber
    {
        get
        {
            return (PageNumber - 1) * PageSize + 1;
        }
    }

    public int EndRecordNumber
    {
        get
        {
            return StartRecordNumber + Data.Count() - 1;
        }
    }
}