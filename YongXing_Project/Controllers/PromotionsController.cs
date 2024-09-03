using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using YongXing_Project.Models;

[Route("api/[controller]")]
[ApiController]
public class PromotionsController : ControllerBase
{
    private static readonly List<Promotion> Promotions = new List<Promotion>
    {
        new Promotion { Id = 1, ImagePath = "/images/promotion1.jpeg", Text = "Amazing Summer Sale!" },
        new Promotion { Id = 2, ImagePath = "/images/promotion2.jpeg", Text = "Limited Time Offer!" },
        new Promotion { Id = 3, ImagePath = "/images/promotion3.jpeg", Text = "Exclusive Deals Just for You!" }
    };

    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(Promotions);
    }
}
