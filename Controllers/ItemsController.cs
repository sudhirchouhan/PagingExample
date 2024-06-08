using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using PagingExample.Models;

namespace PagingExample.Controllers
{
    public class ItemsController : Controller
    {
        private readonly ApplicationDBContext _db;
        private readonly IWebHostEnvironment webHostEnvironment;
        public ItemsController(ApplicationDBContext db, IWebHostEnvironment environment)
        {
            _db = db;
            webHostEnvironment = environment;

        }
        public async Task<IActionResult> GetDataFromWebAPI()
        {
            // Below code is written for consuming the Web API
            List<Item> items = new List<Item>();
            using (HttpClient client = new HttpClient())
            {
                using (var response = client.GetAsync("https://localhost:7214/api/Items/Index"))
                {
                    string apiResponse = await response.Result.Content.ReadAsStringAsync();
                    items = JsonConvert.DeserializeObject<List<Item>>(apiResponse);
                }
            }
            return View(items);
            //Code ended here
            // return View();
        }
        public IActionResult Index(int pg = 1)
        {
            var data = _db.Items.ToList();
            const int pageSize = 3;
            if (pg < 1)
                pg = 1;
            int recsCount = data.Count;

            var pager = new Pager(recsCount, pg, pageSize);
            int recSkip = (pg - 1) * pageSize;
            var dataList = data.Skip(recSkip).Take(pager.PageSize).ToList();
            ViewBag.Pager = pager;
            return View(dataList);
        }
        public IActionResult CreateItem()
        {
            return View();
        }
        [HttpPost]
        public IActionResult CreateItem(Item item)
        {
            string UniqueFileName = UploadImage(item);
            var data = new Item
            {
                Name = item.Name,
                Brand = item.Brand,
                Description = item.Description,
                Path = UniqueFileName,
            };
            _db.Items.Add(data);
            _db.SaveChanges();
            TempData["Success"] = "Data Saved Successfully";
            return RedirectToAction("Index");
        }
        public string UploadImage(Item item)
        {
            string UniqueFileName = string.Empty;
            if (item.ImagePath != null)
            {
                string ImageFolder = Path.Combine(webHostEnvironment.WebRootPath, "images/");

                UniqueFileName = Guid.NewGuid().ToString() + "_" + item.ImagePath.FileName;
                string filePath = Path.Combine(ImageFolder, UniqueFileName);
                using (var filestream = new FileStream(filePath, FileMode.Create))
                {
                    item.ImagePath.CopyTo(filestream);
                }
            }
            return UniqueFileName;
        }

        public IActionResult InsertItems()
        {
            return View();
        }
        [HttpPost]
        public IActionResult InsertItems(Item item)
        {
            string NewFileName = string.Empty;
            if (item.ImagePath != null)
            {
                string Folder_for_Image = Path.Combine(webHostEnvironment.WebRootPath, "images/");
                NewFileName = Guid.NewGuid().ToString() + "_" + item.ImagePath.FileName;
                string path_of_file = Path.Combine(Folder_for_Image, NewFileName);
                using (var filestream = new FileStream(path_of_file, FileMode.Create))
                {
                    item.ImagePath.CopyTo(filestream);
                }
            }
            var data = new Item
            {
                Name = item.Name,
                Brand = item.Brand,
                Description = item.Description,
                Path = NewFileName
            };
            _db.Items.Add(data);
            _db.SaveChanges();
            TempData["Success"] = "Data Saved Successfully";
            return RedirectToAction("Index");
        }
    }
}
