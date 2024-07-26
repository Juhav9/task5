using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MVCPhones.Data;
using System.Collections;

namespace MVCPhones.Controllers
{
    public class PhonesController : Controller
    {
        private readonly PhonesContext _phonesContext;
        public IEnumerable<Phone>? Phone { get; set; }
        public PhonesController (PhonesContext phonesContext)
        {
            _phonesContext = phonesContext;
        }
        public async  Task<IActionResult> Index()
        {
            Phone = await _phonesContext.Phones
                                        .OrderBy(p => p.Make)
                                        .ThenBy(p => p.Model)
                                        .ToListAsync();
            return View(Phone);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) 
            {
                NotFound(); 
            }
            var phone = await _phonesContext.Phones
                                            .SingleOrDefaultAsync(p => p.Id == id);
            if(phone == null)
            {
                NotFound(nameof(phone));
            }
            return View(phone);
        }
        [HttpPost]
        public async Task<IActionResult> Edit([Bind("Id,Make, Model, RAM, PublishDate")] Phone phone)
        {
            if (!ModelState.IsValid)
            {
                return View(phone);
            }
            if(phone!=null) _phonesContext.Update(phone);
            await _phonesContext.SaveChangesAsync();
            return RedirectToAction("Index");
		}
		public async Task<IActionResult> Delete(int? id)
		{
            if (!ModelState.IsValid)
            {
                return NotFound();
            }
            if(id==null)
            {
                return NotFound();
            }
            var phone = await _phonesContext.Phones
                                            .FirstOrDefaultAsync(p => p.Id == id);
            if(phone==null)
            {
                return NotFound();
            }
			return View(phone);
		}
		[HttpPost, ActionName("Delete")]
		public async Task<IActionResult> DeleteCon(int? id)
        {
            var phone = await _phonesContext.Phones.SingleOrDefaultAsync(p => p.Id == id);
            _phonesContext.Phones.Remove(phone);
            await _phonesContext.SaveChangesAsync();
            return RedirectToAction("Index");

        }
		public IActionResult Add()
		{
			return View();
		}

        [HttpPost]
        public async Task<IActionResult> Add([Bind("Make, Model, RAM, PublishDate")] Phone phone)
        {
            if(ModelState.IsValid)
            {
                _phonesContext.Add(phone);
                await _phonesContext.SaveChangesAsync();    
				return RedirectToAction("Index");
			}
            return View();
        }
	}
}
