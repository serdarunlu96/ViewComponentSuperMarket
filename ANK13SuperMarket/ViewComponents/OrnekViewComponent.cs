using Microsoft.AspNetCore.Mvc;

namespace ANK13SuperMarket.ViewComponents
{
    public class OrnekViewComponent : ViewComponent
    {
        //View componentin yapılacaklarını yazacağız
        public OrnekViewComponent()
        {
            
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View();
        }
    }
}
