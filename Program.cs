using System;
using System.Collections.Generic;
using System.Linq;

namespace InventorySystem
{
    /// <summary>
    /// Represents a product in the store inventory.
    /// </summary>
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public int ReorderThreshold { get; set; }

        public Product(int id, string name, decimal price, int quantity, int reorderThreshold)
        {
            Id = id;
            Name = name;
            Price = price;
            Quantity = quantity;
            ReorderThreshold = reorderThreshold;
        }

        public bool ReduceStock(int amount)
        {
            if (amount <= 0 || Quantity < amount) return false;
            Quantity -= amount;
            return true;
        }

        public void AddStock(int amount)
        {
            if (amount > 0) Quantity += amount;
        }

        public bool NeedsReorder()
        {
            return Quantity <= ReorderThreshold;
        }

        public override string ToString()
        {
            string lowStockAlert = NeedsReorder() ? " [LOW STOCK]" : "";
            return $"ID:{Id,-3} | {Name,-20} | ${Price,-6:F2} | Stock: {Quantity,-3}{lowStockAlert}";
        }
    }

    /// <summary>
    /// Manages all inventory operations.
    /// </summary>
    public class InventoryManager
    {
        private List<Product> _products = new List<Product>();
        private int _nextId = 1;

        public void AddProduct(string name, decimal price, int quantity, int reorderThreshold)
        {
            var product = new Product(_nextId++, name, price, quantity, reorderThreshold);
            _products.Add(product);
            Console.WriteLine($"\n[SUCCESS] Product '{name}' added with ID {product.Id}");
        }

        public void RemoveProduct(int id)
        {
            var product = FindProduct(id);
            if (product != null)
            {
                _products.Remove(product);
                Console.WriteLine($"\n[SUCCESS] Product '{product.Name}' removed.");
            }
            else
            {
                Console.WriteLine($"\n[ERROR] Product ID {id} not found.");
            }
        }

        public Product FindProduct(int id)
        {
            return _products.FirstOrDefault(p => p.Id == id);
        }

        public List<Product> GetAllProducts()
        {
            return _products.ToList();
        }

        public bool PurchaseProduct(int id, int quantity)
        {
            var product = FindProduct(id);
            if (product == null) return false;
            if (!product.ReduceStock(quantity)) return false;
            return true;
        }

        public void DisplayInventory()
        {
            Console.WriteLine("\n+--------------------------------------------------+");
            Console.WriteLine("|               CURRENT INVENTORY                 |");
            Console.WriteLine("+--------------------------------------------------+");

            if (_products.Count == 0)
            {
                Console.WriteLine("|         No products in inventory              |");
            }
            else
            {
                foreach (var p in _products)
                {
                    Console.WriteLine($"| {p.ToString(),-48} |");
                }
            }
            Console.WriteLine("+--------------------------------------------------+");
        }

        public void ShowLowStockAlerts()
        {
            var lowStock = _products.Where(p => p.NeedsReorder()).ToList();
            if (lowStock.Any())
            {
                Console.WriteLine("\n[LOW STOCK ALERTS]");
                foreach (var p in lowStock)
                {
                    Console.WriteLine($"   WARNING: {p.Name} - Only {p.Quantity} left (Threshold: {p.ReorderThreshold})");
                }
            }
            else
            {
                Console.WriteLine("\n[OK] All products have sufficient stock.");
            }
        }
    }

    /// <summary>
    /// Main program entry point.
    /// </summary>
    class Program
    {
        static InventoryManager manager = new InventoryManager();

        static void Main(string[] args)
        {
            // Seed initial inventory
            manager.AddProduct("Coffee Beans", 12.99m, 50, 10);
            manager.AddProduct("Paper Towels", 4.49m, 8, 5);
            manager.AddProduct("Dish Soap", 3.99m, 3, 5);

            while (true)
            {
                Console.Clear();
                Console.WriteLine("+--------------------------------------------------+");
                Console.WriteLine("|         INVENTORY MANAGEMENT SYSTEM             |");
                Console.WriteLine("+--------------------------------------------------+");
                Console.WriteLine("|    1. REGISTER (Make a purchase)                 |");
                Console.WriteLine("|    2. ORDER TAB (View inventory)                |");
                Console.WriteLine("|    3. EDIT INVENTORY (Add/Remove products)       |");
                Console.WriteLine("|    4. EXIT                                       |");
                Console.WriteLine("+--------------------------------------------------+");
                Console.Write("\nChoose option (1-4): ");

                string choice = Console.ReadLine();

                if (choice == "1")
                    RegisterMenu();
                else if (choice == "2")
                    OrderTabMenu();
                else if (choice == "3")
                    EditInventoryMenu();
                else if (choice == "4")
                    break;
                else
                    Console.WriteLine("\n[ERROR] Invalid option. Press ENTER to try again.");
            }
        }

        static void RegisterMenu()
        {
            while (true)
            {
                Console.Clear();
                manager.DisplayInventory();
                Console.Write("\nEnter Product ID to purchase (0 to exit): ");

                if (!int.TryParse(Console.ReadLine(), out int id) || id == 0)
                    break;

                var product = manager.FindProduct(id);
                if (product == null)
                {
                    Console.WriteLine("\n[ERROR] Product not found. Press ENTER.");
                    Console.ReadLine();
                    continue;
                }

                Console.Write($"How many {product.Name}? ");
                if (!int.TryParse(Console.ReadLine(), out int qty) || qty <= 0)
                {
                    Console.WriteLine("\n[ERROR] Invalid quantity. Press ENTER.");
                    Console.ReadLine();
                    continue;
                }

                if (manager.PurchaseProduct(id, qty))
                {
                    decimal total = product.Price * qty;
                    Console.WriteLine($"\n[SUCCESS] Purchased {qty}x {product.Name}");
                    Console.WriteLine($"   Total: ${total:F2}");
                    Console.WriteLine($"   Remaining stock: {product.Quantity}");
                }
                else
                {
                    Console.WriteLine("\n[ERROR] Insufficient stock!");
                }

                Console.WriteLine("\nPress ENTER to continue...");
                Console.ReadLine();
            }
        }

        static void OrderTabMenu()
        {
            Console.Clear();
            manager.DisplayInventory();
            manager.ShowLowStockAlerts();
            Console.WriteLine("\nPress ENTER to return...");
            Console.ReadLine();
        }

        static void EditInventoryMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("+--------------------------------------------------+");
                Console.WriteLine("|                EDIT INVENTORY                   |");
                Console.WriteLine("+--------------------------------------------------+");
                Console.WriteLine("|    1. Add Product                               |");
                Console.WriteLine("|    2. Remove Product                            |");
                Console.WriteLine("|    3. Back to Main Menu                         |");
                Console.WriteLine("+--------------------------------------------------+");
                Console.Write("\nChoose option (1-3): ");

                string choice = Console.ReadLine();

                if (choice == "1")
                {
                    Console.Write("\nProduct Name: ");
                    string name = Console.ReadLine();

                    Console.Write("Price: $");
                    if (!decimal.TryParse(Console.ReadLine(), out decimal price))
                    {
                        Console.WriteLine("[ERROR] Invalid price.");
                        Console.ReadLine();
                        continue;
                    }

                    Console.Write("Initial Quantity: ");
                    if (!int.TryParse(Console.ReadLine(), out int qty))
                    {
                        Console.WriteLine("[ERROR] Invalid quantity.");
                        Console.ReadLine();
                        continue;
                    }

                    Console.Write("Reorder Threshold: ");
                    if (!int.TryParse(Console.ReadLine(), out int threshold))
                    {
                        Console.WriteLine("[ERROR] Invalid threshold.");
                        Console.ReadLine();
                        continue;
                    }

                    manager.AddProduct(name, price, qty, threshold);
                    Console.WriteLine("\nPress ENTER to continue...");
                    Console.ReadLine();
                }
                else if (choice == "2")
                {
                    manager.DisplayInventory();
                    Console.Write("\nEnter Product ID to remove (0 to cancel): ");
                    if (int.TryParse(Console.ReadLine(), out int id) && id != 0)
                    {
                        manager.RemoveProduct(id);
                    }
                    Console.WriteLine("\nPress ENTER to continue...");
                    Console.ReadLine();
                }
                else if (choice == "3")
                {
                    break;
                }
                else
                {
                    Console.WriteLine("\n[ERROR] Invalid option. Press ENTER.");
                    Console.ReadLine();
                }
            }
        }
    }
}