using CrazyMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleTest
{
    public class PersonInfo
    {
        public string Name { get; set; }
        public int Age { get; set; }

    }
    public class CategoryInfo
    {
        public string Name { get; set; }
        public List<PersonInfo> Persons { get; set; } = new List<PersonInfo>();
    }


    public class NewPersonInfo
    {
        public string Name { get; set; }
        public int Age { get; set; }

    }
    public class NewCategoryInfo
    {
        public string Name { get; set; }
        public List<NewPersonInfo> Persons { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                CategoryInfo category = new CategoryInfo();
                category.Name = "ct1";
                category.Persons.Add(new PersonInfo() { Name = "ali", Age = 27 });
                category.Persons.Add(new PersonInfo() { Name = "reza", Age = 10 });

                var newCategory = Mapper.Map<NewCategoryInfo>(category);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            Console.ReadKey();
        }
    }
}
