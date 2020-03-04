using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;
using DataSetSampleQueries;
using SampleQueries;
using SampleSupport;

namespace QuerySamples
{
    [Title("LINQ Query Samples")]
    [Prefix("Linq")]
    public class CustomSamples : SampleHarness
    {
        private readonly static string dataPath = Path.GetFullPath(Path.Combine(Application.StartupPath, @"..\..\Data\"));
        public class Customer
        {
            public string CustomerID { get; set; }
            public string CompanyName { get; set; }
            public string Address { get; set; }
            public string City { get; set; }
            public string Region { get; set; }
            public string PostalCode { get; set; }
            public string Country { get; set; }
            public string Phone { get; set; }
            public string Fax { get; set; }
            public LinqSamples.Order[] Orders { get; set; }
        }

        public class Order
        {
            public int OrderID { get; set; }
            public DateTime OrderDate { get; set; }
            public decimal Total { get; set; }
        }

        public class Product
        {
            public int ProductID { get; set; }
            public string ProductName { get; set; }
            public string Category { get; set; }
            public decimal UnitPrice { get; set; }
            public int UnitsInStock { get; set; }
        }

        public class Supplier
        {
            public string SupplierName { get; set; }
            public string Address { get; set; }
            public string City { get; set; }
            public string Country { get; set; }
        }

        private List<LinqSamples.Product> productList;
        private List<LinqSamples.Customer> customerList;
        private List<LinqSamples.Supplier> supplierList;

        [Category("Tasks")]
        [Title("Task 1")]
        [Description("This method returns the list of clients which orders' count is more than 5.")]
        public void LinqQuery01()
        {
            List<LinqSamples.Customer> customers = GetCustomerList();

            var ManyOrdersClients =
                from customer in customers
                where (customer.Orders.Length > 5)
                select customer;

            Console.WriteLine("List of clients with many orders:");
            foreach (var client in ManyOrdersClients)
            {
                Console.WriteLine($"Customer ID = {client.CustomerID}, Orders' count ={client.Orders.Length}");
            }
        }

        [Category("Tasks")]
        [Title("Task 2 without grouping")]
        [Description("This method returns the list of clients and the customers from the same city as them.")]
        public void LinqQuery02NoGrouping()
        {
            List<LinqSamples.Customer> customers = GetCustomerList();
            List<LinqSamples.Supplier> suppliers = GetSupplierList();

            var SameCityClientsAndSuppliers =
                from customer in customers
                select
                    new
                    {
                        customer,
                        Suppliers =
                            from supplier in suppliers
                            where (supplier.Country == customer.Country && supplier.City == customer.City)
                            select supplier
                    };

            Console.WriteLine("List of clients with many orders:");
            foreach (var client in SameCityClientsAndSuppliers)
            {
                Console.WriteLine($"Customer ID = {client.customer.CustomerID}, Customer Country = {client.customer.Country}, Customer City = {client.customer.City}");
                foreach (var supplier in client.Suppliers)
                {
                    Console.WriteLine($"    Supplier Name = {supplier.SupplierName}, Supplier Country = {supplier.Country}, Supplier City = {supplier.City}");
                }
            }
        }

        [Category("Tasks")]
        [Title("Task 2 with grouping")]
        [Description("This method returns the list of clients and the customers from the same city as them.")]
        public void LinqQuery02Grouping()
        {
            List<LinqSamples.Customer> customers = GetCustomerList();
            List<LinqSamples.Supplier> suppliers = GetSupplierList();

            var SameCityClientsAndSuppliersGroup =
                from customer in customers
                from supplier in suppliers
                group supplier by customer
                into g
                select new
                {
                    CustomerID = g.Key.CustomerID,
                    CustomerCountry = g.Key.Country,
                    CustomerCity = g.Key.City,
                    SupplierInfo =
                        from sup in g
                        where sup.Country == g.Key.Country && sup.City == g.Key.City
                        select sup
                };

            Console.WriteLine("List of clients with many orders:");
            foreach (var group in SameCityClientsAndSuppliersGroup)
            {
                Console.WriteLine($"Customer ID = {group.CustomerID}, Customer Country = {group.CustomerCountry}, Customer City = {group.CustomerCity}");
                foreach (var supplier in group.SupplierInfo)
                {
                    Console.WriteLine($"    Supplier Name = {supplier.SupplierName}, Supplier Country = {supplier.Country}, Supplier City = {supplier.City}");
                }
            }
        }

        [Category("Tasks")]
        [Title("Task 3")]
        [Description("This method returns the list of clients which orders' sum is more than specified value.")]
        public void LinqQuery03()
        {
            List<LinqSamples.Customer> customers = GetCustomerList();

            var ExpensiveOrdersClients =
                from customer in customers
                where (customer.Orders.Sum((order => order.Total)) > 2000)
                select customer;

            Console.WriteLine("List of clients with expensive orders:");
            foreach (var customer in ExpensiveOrdersClients)
            {
                Console.WriteLine($"Customer ID = {customer.CustomerID}");
                Console.WriteLine($"Customer's orders sum = {customer.Orders.Sum((order => order.Total))}");
            }
        }

        [Category("Tasks")]
        [Title("Task 4")]
        [Description("This method returns the list of clients sorted by year, month of first order, orders' sum and name.")]
        public void LinqQuery04()
        {
            List<LinqSamples.Customer> customers = GetCustomerList();

            var SortedClients =
                from customer in customers
                where (customer.Orders.Length>0)
                orderby customer.Orders.First().OrderDate.Year, customer.Orders.First().OrderDate.Month, customer.Orders?.Sum((order => order.Total)), customer.CompanyName
                select customer;

            Console.WriteLine("Sorted Clients:");
            foreach (var client in SortedClients)
            {
                Console.WriteLine($"Customer Name = {client.CompanyName}, First Order Year = {client.Orders.First().OrderDate.Year}, " +
                                  $"First Order Month = {client.Orders.First().OrderDate.Month}, Orders' Sum = {client.Orders.Sum((order => order.Total))}");
            }
        }

        [Category("Tasks")]
        [Title("Task 5")]
        [Description("This method returns the list of clients which postal code doesn't contain numbers or region is not filled or there is no operator code in the number.")]
        public void LinqQuery05()
        {
            List<LinqSamples.Customer> customers = GetCustomerList();

            var SortedClients =
                from customer in customers
                where ((customer.PostalCode?.All((c => !char.IsNumber(c))) ?? true) ||
                       (customer.Region is null) || (customer.Phone.StartsWith("(")))
                select customer;

            Console.WriteLine("Sorted Clients:");
            foreach (var client in SortedClients)
            {
                Console.WriteLine($"CustomerID = {client.CustomerID}, Postal Code = {client.PostalCode}, Region = {client.Region}, Phone = {client.Phone}");
            }
        }

        [Category("Tasks")]
        [Title("Task 6")]
        [Description("This method returns the list of products grouped by their category, then units, then price.")]
        public void LinqQuery06()
        {
            List<LinqSamples.Product> products = GetProductList();

            var GroupedProducts =
                from product in products
                group product by product.Category
                into categoryGroup
                select new
                {
                    Category = categoryGroup.Key,
                    StockGroup =
                        from product in categoryGroup
                        group product by product.UnitsInStock
                        into stockGroup
                        select new
                        {
                            UnitsInStock = stockGroup.Key,
                            PriceGroup =
                                from product in stockGroup
                                group product by product.UnitPrice
                                into priceGroup
                                select new
                                {
                                    Price = priceGroup.Key,
                                    Products = priceGroup
                                }
                        }
                };
            Console.WriteLine("Grouped up products:");
            foreach (var productGroup in GroupedProducts)
            {
                Console.WriteLine($"Category: {productGroup.Category}");
                foreach (var stockGroup in productGroup.StockGroup)
                {
                    Console.WriteLine($"    Units in stock: {stockGroup.UnitsInStock}");
                    foreach (var priceGroup in stockGroup.PriceGroup)
                    {
                        Console.WriteLine($"        Price = {priceGroup.Price}");
                        foreach (var product in priceGroup.Products)
                        {
                            Console.WriteLine($"             Product ID ={product.ProductID} Product Name = {product.ProductName}");
                        }
                    }
                }
            }
        }

        [Category("Tasks")]
        [Title("Task 7")]
        [Description("This method returns the list of products sorted by price.")]
        public void LinqQuery07()
        {
            List<LinqSamples.Product> products = GetProductList();

            var ProductGroups = new
            {
                CheapProducts = from product in products
                    where (product.UnitPrice < 10)
                    select product,
                MiddleRangeProducts =
                    from product in products
                    where (product.UnitPrice >= 10 && product.UnitPrice < 20)
                    select product,
                ExpensiveProducts =
                    from product in products
                    where (product.UnitPrice >= 20)
                    select product,
            };

            Console.WriteLine("Products:");
            Console.WriteLine(" Cheap Products: ");
            foreach (var prod in ProductGroups.CheapProducts)
            {
                Console.WriteLine($"    Product ID ={prod.ProductID}, Product Price = {prod.UnitPrice}");
            }

            Console.WriteLine(" Middle Range Products: ");
            foreach (var prod in ProductGroups.MiddleRangeProducts)
            {
                Console.WriteLine($"    Product ID ={prod.ProductID}, Product Price = {prod.UnitPrice}");
            }

            Console.WriteLine(" Expensive Products: ");
            foreach (var prod in ProductGroups.ExpensiveProducts)
            {
                Console.WriteLine($"    Product ID ={prod.ProductID}, Product Price = {prod.UnitPrice}");
            }
        }

        [Category("Tasks")]
        [Title("Task 8")]
        [Description("This method returns the list of cities and average values of orders and money spent by clients from this city.")]
        public void LinqQuery08()
        {
            List<LinqSamples.Customer> customers = GetCustomerList();

            var CityStatistics =
                from customer in customers
                group customer by customer.City
                into cityGroup
                select new
                {
                    City = cityGroup.Key,
                    AverageOrderPrice = cityGroup.Average((customer => customer.Orders.Sum((order => order.Total)))),
                    AverageOrderCount = (int)cityGroup.Average((customer => customer.Orders.Length))
                };

            foreach (var group in CityStatistics)
            {
                Console.WriteLine($"City = {group.City}");
                Console.WriteLine($"    Average Orders = {group.AverageOrderCount}");
                Console.WriteLine($"    Average Orders Price = {group.AverageOrderPrice}");
            }
        }

        [Category("Tasks")]
        [Title("Task 9-1")]
        [Description("This method returns the list of regions and companies from there.")]
        public void LinqQuery09()
        {
            List<LinqSamples.Customer> customers = GetCustomerList();

            var RegionCustomers =
                from customer in customers
                group customer by customer.Region
                into cityGroup
                select new
                {
                    Region = cityGroup.Key,
                    Names = 
                        from customer in cityGroup
                        select customer.CompanyName
                };

            foreach (var group in RegionCustomers)
            {
                Console.WriteLine($"Region = {group.Region}");
                foreach (var company in group.Names)
                {
                    Console.WriteLine($"    CompanyName = {company}");
                }
            }
        }

        [Category("Tasks")]
        [Title("Task 9-2")]
        [Description("This method returns the list of customers with their maximum price order.")]
        public void LinqQuery10()
        {
            List<LinqSamples.Customer> customers = GetCustomerList();

            var MaxCustomerOrder =
                from customer in customers
                where (customer.Orders.Length > 0)
                select new
                {
                    Customer = customer,
                    MaxOrderCost = customer.Orders.Max((order => order.Total))
                };

            foreach (var group in MaxCustomerOrder)
            {
                Console.WriteLine($"Customer = {group.Customer.CustomerID}, MaxOrder Price = {group.MaxOrderCost}");
            }
        }

        [Category("Tasks")]
        [Title("Task 9-3")]
        [Description("This method returns the suppliers grouped up by their city and country.")]
        public void LinqQuery11()
        {
            List<LinqSamples.Supplier> suppliers = GetSupplierList();

            var CountryGroup =
                from supplier in suppliers
                group supplier by supplier.Country
                into countryGroup
                select new
                {
                    Country = countryGroup.Key,
                    CityGroup =
                        from supplier in countryGroup
                        group supplier by supplier.City
                };

            foreach (var countryGroup in CountryGroup)
            {
                Console.WriteLine($"Country = {countryGroup.Country}");
                foreach (var cityGroup in countryGroup.CityGroup)
                {
                    Console.WriteLine($"    City = {cityGroup.Key}");
                    foreach (var supplier in cityGroup)
                    {
                        Console.WriteLine($"        Supplier Name = {supplier.SupplierName}");
                    }
                }
            }
        }

        [Category("Tasks")]
        [Title("Task 9-4")]
        [Description("This method returns three most expensive products.")]
        public void LinqQuery12()
        {
            List<LinqSamples.Product> products = GetProductList();

            var ExpensiveProducts = products.OrderByDescending((product => product.UnitPrice)).Take(3);

            foreach (var product in ExpensiveProducts)
            {
                Console.WriteLine($"Product Name = {product.ProductName}, Product Price = {product.UnitPrice}");
            }
        }

        [Category("Tasks")]
        [Title("Task 9-5")]
        [Description("This method returns the list of clients with valid phone ordered by name.")]
        public void LinqQuery13()
        {
            List<LinqSamples.Customer> customers = GetCustomerList();

            var OrderedClients =
                from customer in customers
                where (customer.Phone.All((c => char.IsNumber(c) || c == ')' || c == '(' || c=='-' || char.IsSeparator(c))))
                orderby customer.CompanyName
                select customer;

            foreach (var client in OrderedClients)
            {
                Console.WriteLine($"Company Name = {client.CompanyName}, Phone = {client.Phone}");
            }
        }

        public List<LinqSamples.Product> GetProductList()
        {
            if (productList == null)
                createLists();

            return productList;
        }

        public List<LinqSamples.Supplier> GetSupplierList()
        {
            if (supplierList == null)
                createLists();

            return supplierList;
        }

        public List<LinqSamples.Customer> GetCustomerList()
        {
            if (customerList == null)
                createLists();

            return customerList;
        }

        private void createLists()
        {
            // Product data created in-memory using collection initializer:
            productList =
                new List<LinqSamples.Product> {
                    new LinqSamples.Product { ProductID = 1, ProductName = "Chai", Category = "Beverages", UnitPrice = 18.0000M, UnitsInStock = 39 },
                    new LinqSamples.Product { ProductID = 2, ProductName = "Chang", Category = "Beverages", UnitPrice = 19.0000M, UnitsInStock = 17 },
                    new LinqSamples.Product { ProductID = 3, ProductName = "Aniseed Syrup", Category = "Condiments", UnitPrice = 10.0000M, UnitsInStock = 13 },
                    new LinqSamples.Product { ProductID = 4, ProductName = "Chef Anton's Cajun Seasoning", Category = "Condiments", UnitPrice = 22.0000M, UnitsInStock = 53 },
                    new LinqSamples.Product { ProductID = 5, ProductName = "Chef Anton's Gumbo Mix", Category = "Condiments", UnitPrice = 21.3500M, UnitsInStock = 0 },
                    new LinqSamples.Product { ProductID = 6, ProductName = "Grandma's Boysenberry Spread", Category = "Condiments", UnitPrice = 25.0000M, UnitsInStock = 120 },
                    new LinqSamples.Product { ProductID = 7, ProductName = "Uncle Bob's Organic Dried Pears", Category = "Produce", UnitPrice = 30.0000M, UnitsInStock = 15 },
                    new LinqSamples.Product { ProductID = 8, ProductName = "Northwoods Cranberry Sauce", Category = "Condiments", UnitPrice = 40.0000M, UnitsInStock = 6 },
                    new LinqSamples.Product { ProductID = 9, ProductName = "Mishi Kobe Niku", Category = "Meat/Poultry", UnitPrice = 97.0000M, UnitsInStock = 29 },
                    new LinqSamples.Product { ProductID = 10, ProductName = "Ikura", Category = "Seafood", UnitPrice = 31.0000M, UnitsInStock = 31 },
                    new LinqSamples.Product { ProductID = 11, ProductName = "Queso Cabrales", Category = "Dairy Products", UnitPrice = 21.0000M, UnitsInStock = 22 },
                    new LinqSamples.Product { ProductID = 12, ProductName = "Queso Manchego La Pastora", Category = "Dairy Products", UnitPrice = 38.0000M, UnitsInStock = 86 },
                    new LinqSamples.Product { ProductID = 13, ProductName = "Konbu", Category = "Seafood", UnitPrice = 6.0000M, UnitsInStock = 24 },
                    new LinqSamples.Product { ProductID = 14, ProductName = "Tofu", Category = "Produce", UnitPrice = 23.2500M, UnitsInStock = 35 },
                    new LinqSamples.Product { ProductID = 15, ProductName = "Genen Shouyu", Category = "Condiments", UnitPrice = 15.5000M, UnitsInStock = 39 },
                    new LinqSamples.Product { ProductID = 16, ProductName = "Pavlova", Category = "Confections", UnitPrice = 17.4500M, UnitsInStock = 29 },
                    new LinqSamples.Product { ProductID = 17, ProductName = "Alice Mutton", Category = "Meat/Poultry", UnitPrice = 39.0000M, UnitsInStock = 0 },
                    new LinqSamples.Product { ProductID = 18, ProductName = "Carnarvon Tigers", Category = "Seafood", UnitPrice = 62.5000M, UnitsInStock = 42 },
                    new LinqSamples.Product { ProductID = 19, ProductName = "Teatime Chocolate Biscuits", Category = "Confections", UnitPrice = 9.2000M, UnitsInStock = 25 },
                    new LinqSamples.Product { ProductID = 20, ProductName = "Sir Rodney's Marmalade", Category = "Confections", UnitPrice = 81.0000M, UnitsInStock = 40 },
                    new LinqSamples.Product { ProductID = 21, ProductName = "Sir Rodney's Scones", Category = "Confections", UnitPrice = 10.0000M, UnitsInStock = 3 },
                    new LinqSamples.Product { ProductID = 22, ProductName = "Gustaf's Knäckebröd", Category = "Grains/Cereals", UnitPrice = 21.0000M, UnitsInStock = 104 },
                    new LinqSamples.Product { ProductID = 23, ProductName = "Tunnbröd", Category = "Grains/Cereals", UnitPrice = 9.0000M, UnitsInStock = 61 },
                    new LinqSamples.Product { ProductID = 24, ProductName = "Guaraná Fantástica", Category = "Beverages", UnitPrice = 4.5000M, UnitsInStock = 20 },
                    new LinqSamples.Product { ProductID = 25, ProductName = "NuNuCa Nuß-Nougat-Creme", Category = "Confections", UnitPrice = 14.0000M, UnitsInStock = 76 },
                    new LinqSamples.Product { ProductID = 26, ProductName = "Gumbär Gummibärchen", Category = "Confections", UnitPrice = 31.2300M, UnitsInStock = 15 },
                    new LinqSamples.Product { ProductID = 27, ProductName = "Schoggi Schokolade", Category = "Confections", UnitPrice = 43.9000M, UnitsInStock = 49 },
                    new LinqSamples.Product { ProductID = 28, ProductName = "Rössle Sauerkraut", Category = "Produce", UnitPrice = 45.6000M, UnitsInStock = 26 },
                    new LinqSamples.Product { ProductID = 29, ProductName = "Thüringer Rostbratwurst", Category = "Meat/Poultry", UnitPrice = 123.7900M, UnitsInStock = 0 },
                    new LinqSamples.Product { ProductID = 30, ProductName = "Nord-Ost Matjeshering", Category = "Seafood", UnitPrice = 25.8900M, UnitsInStock = 10 },
                    new LinqSamples.Product { ProductID = 31, ProductName = "Gorgonzola Telino", Category = "Dairy Products", UnitPrice = 12.5000M, UnitsInStock = 0 },
                    new LinqSamples.Product { ProductID = 32, ProductName = "Mascarpone Fabioli", Category = "Dairy Products", UnitPrice = 32.0000M, UnitsInStock = 9 },
                    new LinqSamples.Product { ProductID = 33, ProductName = "Geitost", Category = "Dairy Products", UnitPrice = 2.5000M, UnitsInStock = 112 },
                    new LinqSamples.Product { ProductID = 34, ProductName = "Sasquatch Ale", Category = "Beverages", UnitPrice = 14.0000M, UnitsInStock = 111 },
                    new LinqSamples.Product { ProductID = 35, ProductName = "Steeleye Stout", Category = "Beverages", UnitPrice = 18.0000M, UnitsInStock = 20 },
                    new LinqSamples.Product { ProductID = 36, ProductName = "Inlagd Sill", Category = "Seafood", UnitPrice = 19.0000M, UnitsInStock = 112 },
                    new LinqSamples.Product { ProductID = 37, ProductName = "Gravad lax", Category = "Seafood", UnitPrice = 26.0000M, UnitsInStock = 11 },
                    new LinqSamples.Product { ProductID = 38, ProductName = "Côte de Blaye", Category = "Beverages", UnitPrice = 263.5000M, UnitsInStock = 17 },
                    new LinqSamples.Product { ProductID = 39, ProductName = "Chartreuse verte", Category = "Beverages", UnitPrice = 18.0000M, UnitsInStock = 69 },
                    new LinqSamples.Product { ProductID = 40, ProductName = "Boston Crab Meat", Category = "Seafood", UnitPrice = 18.4000M, UnitsInStock = 123 },
                    new LinqSamples.Product { ProductID = 41, ProductName = "Jack's New England Clam Chowder", Category = "Seafood", UnitPrice = 9.6500M, UnitsInStock = 85 },
                    new LinqSamples.Product { ProductID = 42, ProductName = "Singaporean Hokkien Fried Mee", Category = "Grains/Cereals", UnitPrice = 14.0000M, UnitsInStock = 26 },
                    new LinqSamples.Product { ProductID = 43, ProductName = "Ipoh Coffee", Category = "Beverages", UnitPrice = 46.0000M, UnitsInStock = 17 },
                    new LinqSamples.Product { ProductID = 44, ProductName = "Gula Malacca", Category = "Condiments", UnitPrice = 19.4500M, UnitsInStock = 27 },
                    new LinqSamples.Product { ProductID = 45, ProductName = "Rogede sild", Category = "Seafood", UnitPrice = 9.5000M, UnitsInStock = 5 },
                    new LinqSamples.Product { ProductID = 46, ProductName = "Spegesild", Category = "Seafood", UnitPrice = 12.0000M, UnitsInStock = 95 },
                    new LinqSamples.Product { ProductID = 47, ProductName = "Zaanse koeken", Category = "Confections", UnitPrice = 9.5000M, UnitsInStock = 36 },
                    new LinqSamples.Product { ProductID = 48, ProductName = "Chocolade", Category = "Confections", UnitPrice = 12.7500M, UnitsInStock = 15 },
                    new LinqSamples.Product { ProductID = 49, ProductName = "Maxilaku", Category = "Confections", UnitPrice = 20.0000M, UnitsInStock = 10 },
                    new LinqSamples.Product { ProductID = 50, ProductName = "Valkoinen suklaa", Category = "Confections", UnitPrice = 16.2500M, UnitsInStock = 65 },
                    new LinqSamples.Product { ProductID = 51, ProductName = "Manjimup Dried Apples", Category = "Produce", UnitPrice = 53.0000M, UnitsInStock = 20 },
                    new LinqSamples.Product { ProductID = 52, ProductName = "Filo Mix", Category = "Grains/Cereals", UnitPrice = 7.0000M, UnitsInStock = 38 },
                    new LinqSamples.Product { ProductID = 53, ProductName = "Perth Pasties", Category = "Meat/Poultry", UnitPrice = 32.8000M, UnitsInStock = 0 },
                    new LinqSamples.Product { ProductID = 54, ProductName = "Tourtière", Category = "Meat/Poultry", UnitPrice = 7.4500M, UnitsInStock = 21 },
                    new LinqSamples.Product { ProductID = 55, ProductName = "Pâté chinois", Category = "Meat/Poultry", UnitPrice = 24.0000M, UnitsInStock = 115 },
                    new LinqSamples.Product { ProductID = 56, ProductName = "Gnocchi di nonna Alice", Category = "Grains/Cereals", UnitPrice = 38.0000M, UnitsInStock = 21 },
                    new LinqSamples.Product { ProductID = 57, ProductName = "Ravioli Angelo", Category = "Grains/Cereals", UnitPrice = 19.5000M, UnitsInStock = 36 },
                    new LinqSamples.Product { ProductID = 58, ProductName = "Escargots de Bourgogne", Category = "Seafood", UnitPrice = 13.2500M, UnitsInStock = 62 },
                    new LinqSamples.Product { ProductID = 59, ProductName = "Raclette Courdavault", Category = "Dairy Products", UnitPrice = 55.0000M, UnitsInStock = 79 },
                    new LinqSamples.Product { ProductID = 60, ProductName = "Camembert Pierrot", Category = "Dairy Products", UnitPrice = 34.0000M, UnitsInStock = 19 },
                    new LinqSamples.Product { ProductID = 61, ProductName = "Sirop d'érable", Category = "Condiments", UnitPrice = 28.5000M, UnitsInStock = 113 },
                    new LinqSamples.Product { ProductID = 62, ProductName = "Tarte au sucre", Category = "Confections", UnitPrice = 49.3000M, UnitsInStock = 17 },
                    new LinqSamples.Product { ProductID = 63, ProductName = "Vegie-spread", Category = "Condiments", UnitPrice = 43.9000M, UnitsInStock = 24 },
                    new LinqSamples.Product { ProductID = 64, ProductName = "Wimmers gute Semmelknödel", Category = "Grains/Cereals", UnitPrice = 33.2500M, UnitsInStock = 22 },
                    new LinqSamples.Product { ProductID = 65, ProductName = "Louisiana Fiery Hot Pepper Sauce", Category = "Condiments", UnitPrice = 21.0500M, UnitsInStock = 76 },
                    new LinqSamples.Product { ProductID = 66, ProductName = "Louisiana Hot Spiced Okra", Category = "Condiments", UnitPrice = 17.0000M, UnitsInStock = 4 },
                    new LinqSamples.Product { ProductID = 67, ProductName = "Laughing Lumberjack Lager", Category = "Beverages", UnitPrice = 14.0000M, UnitsInStock = 52 },
                    new LinqSamples.Product { ProductID = 68, ProductName = "Scottish Longbreads", Category = "Confections", UnitPrice = 12.5000M, UnitsInStock = 6 },
                    new LinqSamples.Product { ProductID = 69, ProductName = "Gudbrandsdalsost", Category = "Dairy Products", UnitPrice = 36.0000M, UnitsInStock = 26 },
                    new LinqSamples.Product { ProductID = 70, ProductName = "Outback Lager", Category = "Beverages", UnitPrice = 15.0000M, UnitsInStock = 15 },
                    new LinqSamples.Product { ProductID = 71, ProductName = "Flotemysost", Category = "Dairy Products", UnitPrice = 21.5000M, UnitsInStock = 26 },
                    new LinqSamples.Product { ProductID = 72, ProductName = "Mozzarella di Giovanni", Category = "Dairy Products", UnitPrice = 34.8000M, UnitsInStock = 14 },
                    new LinqSamples.Product { ProductID = 73, ProductName = "Röd Kaviar", Category = "Seafood", UnitPrice = 15.0000M, UnitsInStock = 101 },
                    new LinqSamples.Product { ProductID = 74, ProductName = "Longlife Tofu", Category = "Produce", UnitPrice = 10.0000M, UnitsInStock = 4 },
                    new LinqSamples.Product { ProductID = 75, ProductName = "Rhönbräu Klosterbier", Category = "Beverages", UnitPrice = 7.7500M, UnitsInStock = 125 },
                    new LinqSamples.Product { ProductID = 76, ProductName = "Lakkalikööri", Category = "Beverages", UnitPrice = 18.0000M, UnitsInStock = 57 },
                    new LinqSamples.Product { ProductID = 77, ProductName = "Original Frankfurter grüne Soße", Category = "Condiments", UnitPrice = 13.0000M, UnitsInStock = 32 }
                };

            supplierList = new List<LinqSamples.Supplier>(){
                    new LinqSamples.Supplier {SupplierName = "Exotic Liquids", Address = "49 Gilbert St.", City = "London", Country = "UK"},
                    new LinqSamples.Supplier {SupplierName = "New Orleans Cajun Delights", Address = "P.O. Box 78934", City = "New Orleans", Country = "USA"},
                    new LinqSamples.Supplier {SupplierName = "Grandma Kelly's Homestead", Address = "707 Oxford Rd.", City = "Ann Arbor", Country = "USA"},
                    new LinqSamples.Supplier {SupplierName = "Tokyo Traders", Address = "9-8 Sekimai Musashino-shi", City = "Tokyo", Country = "Japan"},
                    new LinqSamples.Supplier {SupplierName = "Cooperativa de Quesos 'Las Cabras'", Address = "Calle del Rosal 4", City = "Oviedo", Country = "Spain"},
                    new LinqSamples.Supplier {SupplierName = "Mayumi's", Address = "92 Setsuko Chuo-ku", City = "Osaka", Country = "Japan"},
                    new LinqSamples.Supplier {SupplierName = "Pavlova, Ltd.", Address = "74 Rose St. Moonie Ponds", City = "Melbourne", Country = "Australia"},
                    new LinqSamples.Supplier {SupplierName = "Specialty Biscuits, Ltd.", Address = "29 King's Way", City = "Manchester", Country = "UK"},
                    new LinqSamples.Supplier {SupplierName = "PB Knäckebröd AB", Address = "Kaloadagatan 13", City = "Göteborg", Country = "Sweden"},
                    new LinqSamples.Supplier {SupplierName = "Refrescos Americanas LTDA", Address = "Av. das Americanas 12.890", City = "Sao Paulo", Country = "Brazil"},
                    new LinqSamples.Supplier {SupplierName = "Heli Süßwaren GmbH & Co. KG", Address = "Tiergartenstraße 5", City = "Berlin", Country = "Germany"},
                    new LinqSamples.Supplier {SupplierName = "Plutzer Lebensmittelgroßmärkte AG", Address = "Bogenallee 51", City = "Frankfurt", Country = "Germany"},
                    new LinqSamples.Supplier {SupplierName = "Nord-Ost-Fisch Handelsgesellschaft mbH", Address = "Frahmredder 112a", City = "Cuxhaven", Country = "Germany"},
                    new LinqSamples.Supplier {SupplierName = "Formaggi Fortini s.r.l.", Address = "Viale Dante, 75", City = "Ravenna", Country = "Italy"},
                    new LinqSamples.Supplier {SupplierName = "Norske Meierier", Address = "Hatlevegen 5", City = "Sandvika", Country = "Norway"},
                    new LinqSamples.Supplier {SupplierName = "Bigfoot Breweries", Address = "3400 - 8th Avenue Suite 210", City = "Bend", Country = "USA"},
                    new LinqSamples.Supplier {SupplierName = "Svensk Sjöföda AB", Address = "Brovallavägen 231", City = "Stockholm", Country = "Sweden"},
                    new LinqSamples.Supplier {SupplierName = "Aux joyeux ecclésiastiques", Address = "203, Rue des Francs-Bourgeois", City = "Paris", Country = "France"},
                    new LinqSamples.Supplier {SupplierName = "New England Seafood Cannery", Address = "Order Processing Dept. 2100 Paul Revere Blvd.", City = "Boston", Country = "USA"},
                    new LinqSamples.Supplier {SupplierName = "Leka Trading", Address = "471 Serangoon Loop, Suite #402", City = "Singapore", Country = "Singapore"},
                    new LinqSamples.Supplier {SupplierName = "Lyngbysild", Address = "Lyngbysild Fiskebakken 10", City = "Lyngby", Country = "Denmark"},
                    new LinqSamples.Supplier {SupplierName = "Zaanse Snoepfabriek", Address = "Verkoop Rijnweg 22", City = "Zaandam", Country = "Netherlands"},
                    new LinqSamples.Supplier {SupplierName = "Karkki Oy", Address = "Valtakatu 12", City = "Lappeenranta", Country = "Finland"},
                    new LinqSamples.Supplier {SupplierName = "G'day, Mate", Address = "170 Prince Edward Parade Hunter's Hill", City = "Sydney", Country = "Australia"},
                    new LinqSamples.Supplier {SupplierName = "Ma Maison", Address = "2960 Rue St. Laurent", City = "Montréal", Country = "Canada"},
                    new LinqSamples.Supplier {SupplierName = "Pasta Buttini s.r.l.", Address = "Via dei Gelsomini, 153", City = "Salerno", Country = "Italy"},
                    new LinqSamples.Supplier {SupplierName = "Escargots Nouveaux", Address = "22, rue H. Voiron", City = "Montceau", Country = "France"},
                    new LinqSamples.Supplier {SupplierName = "Gai pâturage", Address = "Bat. B 3, rue des Alpes", City = "Annecy", Country = "France"},
                    new LinqSamples.Supplier {SupplierName = "Forêts d'érables", Address = "148 rue Chasseur", City = "Ste-Hyacinthe", Country = "Canada"},
                };


            // Customer/order data read into memory from XML file using XLinq:
            string customerListPath = Path.GetFullPath(Path.Combine(dataPath, "customers.xml"));

            customerList = (
                from e in XDocument.Load(customerListPath).
                          Root.Elements("customer")
                select new LinqSamples.Customer
                {
                    CustomerID = (string)e.Element("id"),
                    CompanyName = (string)e.Element("name"),
                    Address = (string)e.Element("address"),
                    City = (string)e.Element("city"),
                    Region = (string)e.Element("region"),
                    PostalCode = (string)e.Element("postalcode"),
                    Country = (string)e.Element("country"),
                    Phone = (string)e.Element("phone"),
                    Fax = (string)e.Element("fax"),
                    Orders = (
                        from o in e.Elements("orders").Elements("order")
                        select new LinqSamples.Order
                        {
                            OrderID = (int)o.Element("id"),
                            OrderDate = (DateTime)o.Element("orderdate"),
                            Total = (decimal)o.Element("total")
                        })
                        .ToArray()
                })
                .ToList();
        }
    }
}
