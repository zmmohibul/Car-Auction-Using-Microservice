namespace SearchService.RequestHelpers;

public class QueryParams
{
    public string SearchTerm { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 4;
    public string Seller { get; set; }
    public string Winner { get; set; }
    public string OrderBy { get; set; } = string.Empty;
    public string FilterBy { get; set; }
}