using System.Collections.Generic;

namespace Rentler.Interview.Lib.Models;

public class PagedResponseDto<T>
{
    public IEnumerable<T> Results { get; set; }

    public int TotalPages => Size > 0 ? TotalRecords / Size + 1 : 0;

    public int TotalRecords { get; set; } 
    
    public int Size { get; set; }
    public int Page { get; set; }
}