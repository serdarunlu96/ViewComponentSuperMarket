using ANK13SuperMarket.Context;
using Microsoft.AspNetCore.Mvc;

namespace ANK13SuperMarket.ViewComponents
{
    public class StoktaOlmayanlarViewComponent : ViewComponent
    {
        private readonly MarketDbContext _db;

        public StoktaOlmayanlarViewComponent(MarketDbContext db)
        {
            _db = db;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View(_db.Products.Where(p=>!p.IsInStock).ToList());
        }
    }
}
