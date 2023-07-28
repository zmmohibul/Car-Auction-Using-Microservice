using Microsoft.AspNetCore.Mvc;
using MongoDB.Entities;
using SearchService.Models;
using SearchService.RequestHelpers;

namespace SearchService.Controllers;

[ApiController]
[Route("api/search")]
public class SearchController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> SearchItems([FromQuery] QueryParams queryParams)
    {
        var query = DB.PagedSearch<Item, Item>();
        if (!string.IsNullOrEmpty(queryParams.SearchTerm))
        {
            query.Match(Search.Full, queryParams.SearchTerm).SortByTextScore(); 
        }
        
        query = queryParams.OrderBy switch {
            "make" => query.Sort(sdb => sdb.Ascending(item => item.Make)),
            "new" => query.Sort(sdb => sdb.Descending(item => item.CreatedAt)),
            _ => query.Sort(sdb => sdb.Ascending(item => item.AuctionEnd)),
        };

        // query = queryParams.FilterBy switch
        // {
        //     "finished" => query.Match(x => x.AuctionEnd < DateTime.UtcNow),
        //     "endingSoon" => query.Match(x => x.AuctionEnd < DateTime.UtcNow.AddHours(6)
        //                                      && x.AuctionEnd > DateTime.UtcNow),
        //     _ => query.Match(x => x.AuctionEnd > DateTime.UtcNow)
        // };

        if (!string.IsNullOrEmpty(queryParams.Seller))
        {
            query.Match(item => item.Seller.Equals(queryParams.Seller));
        }
        
        if (!string.IsNullOrEmpty(queryParams.Winner))
        {
            query.Match(item => item.Seller.Equals(queryParams.Winner));
        }

        query.PageNumber(queryParams.PageNumber);
        query.PageSize(queryParams.PageSize);

        var result = await query.ExecuteAsync();
        
        return Ok(new
        {
            results = result.Results,
            pageCount = result.PageCount,
            totalCount = result.TotalCount
        });
    }
}