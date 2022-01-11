using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using WebApiPhoneBook.Models;

namespace WebApiPhoneBook.Data
{
    public class DataInitalizer
    {
        public static async Task InitalizeAsync(DataContext context, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            string adminLogin = "admin";
            string password = "admin";

            if (await roleManager.FindByNameAsync("admin") == null)
            {
                await roleManager.CreateAsync(new IdentityRole("admin"));
            }

            if (await roleManager.FindByNameAsync("user") == null)
            {
                await roleManager.CreateAsync(new IdentityRole("user"));
            }

            if (await userManager.FindByNameAsync(adminLogin) == null)
            {
                User admin = new User { UserName = adminLogin };
                IdentityResult result = await userManager.CreateAsync(admin, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "admin");
                }
            }

            await context.Database.EnsureCreatedAsync();

            if (context.PhoneBooks.Any()) return;

            var data = new List<PhoneBook>()
            {
                new PhoneBook(){ LastName = "Фамилия_1", FirstName = "Имя_1", MiddleName = "Отчество_1", NumberPhone = "8-999-999-99-99", Address = "Адрес_1", Desc = "Описание_1"},
                new PhoneBook(){ LastName = "Фамилия_2", FirstName = "Имя_2", MiddleName = "Отчество_2", NumberPhone = "8-888-888-88-88", Address = "Адрес_2", Desc = "Описание_2"}
            };

            foreach (var r in data)
            {
                await context.PhoneBooks.AddAsync(r);
            }
        }
    }
}
