using SWD63APFC2023.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StackExchange.Redis;
using Newtonsoft.Json;

namespace SWD63APFC2023.DataAccess
{
    public class CacheMenusRepository
    {

        IDatabase myDb;
        public CacheMenusRepository(string connectionString)
        {
            var cm = ConnectionMultiplexer.Connect(connectionString);
            myDb = cm.GetDatabase();
        }

        public async Task<List<Menu>> GetMenusAsync()
        {
            string menusAsStr = await myDb.StringGetAsync("menus");
            if (String.IsNullOrEmpty(menusAsStr))
            {
                return new List<Menu>();
            }

            var list = JsonConvert.DeserializeObject<List<Menu>>(menusAsStr);
            return list;
        }

        public async Task AddMenu(Menu m)
        {
            var list = await GetMenusAsync();
            list.Add(m);
            string menusAsStr =  JsonConvert.SerializeObject(list);

            await myDb.StringSetAsync("menus", menusAsStr);
        }

        public async Task Delete(string link)
        {
            var list = await GetMenusAsync();
            var menuToDelete = list.SingleOrDefault(x => x.Link == link);
            list.Remove(menuToDelete);

            string menusAsStr = JsonConvert.SerializeObject(list);

            await myDb.StringSetAsync("menus", menusAsStr);
        }
    }
}
