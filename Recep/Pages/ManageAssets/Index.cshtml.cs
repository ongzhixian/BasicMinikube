using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Distributed;

namespace Recep.Pages.ManageAssets;

public class IndexModel : PageModel
{
    private readonly IDistributedCache distributedCache;

    public IndexModel(IDistributedCache distributedCache)
    {
        this.distributedCache = distributedCache;
    }

    public void OnGet()
    {
        //distributedCache.SetString("gaga", "booboo");

        var val = distributedCache.GetString("gaga");

    }
}
