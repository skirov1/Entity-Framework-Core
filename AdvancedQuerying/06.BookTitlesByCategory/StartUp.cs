namespace BookShop
{
    using BookShop.Models.Enums;
    using Data;
    using Initializer;
    using Microsoft.EntityFrameworkCore;
    using System.Linq;
    using System.Text;

    public class StartUp
    {
        public static void Main()
        {
            using var db = new BookShopContext();
            //DbInitializer.ResetDatabase(db);

            var input = Console.ReadLine();

            Console.WriteLine(GetBooksByCategory(db, input));

        }

        public static string GetBooksByAgeRestriction(BookShopContext context, string command)
        {
            AgeRestriction ageRestriction;

            bool isParsed = Enum.TryParse(command, true, out ageRestriction);

            if (!isParsed)
            {
                return string.Empty;
            }

            var books = context
                .Books
                .Where(b => b.AgeRestriction == ageRestriction)
                .OrderBy(b => b.Title)
                .ToList();

            var sb = new StringBuilder();

            foreach (var book in books)
            {
                sb.AppendLine(book.Title);
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetGoldenBooks(BookShopContext context)
        {
            EditionType editionType = EditionType.Gold;

            var books = context
               .Books
               .Where(b => b.EditionType == editionType)
               .Where(b => b.Copies < 5000)
               .OrderBy(b => b.BookId)
               .Select(b => b.Title)
               .ToList();

            var sb = new StringBuilder();

            foreach (var book in books)
            {
                sb.AppendLine(book);
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetBooksByPrice(BookShopContext context)
        {
            var books = context
                .Books
                .Where(b => b.Price > 40)
                .Select(b => new
                {
                    b.Title,
                    b.Price
                })
                .OrderByDescending(b => b.Price)
                .ToList();

            var sb = new StringBuilder();

            foreach (var book in books)
            {
                sb.AppendLine($"{book.Title} - ${book.Price:f2}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetBooksNotReleasedIn(BookShopContext context, int year)
        {
            var books = context
               .Books
               .Where(b => b.ReleaseDate.Value.Year != year)
               .OrderBy(b => b.BookId)
               .Select(b => b.Title)
               .ToList();

            var sb = new StringBuilder();

            foreach (var book in books)
            {
                sb.AppendLine($"{book}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetBooksByCategory(BookShopContext context, string input)
        {
            var categories = input.ToLower().Split().ToList();

            var books = context
                .Books
                .Include(b => b.BookCategories)
                .Select(b => new
                {
                    b.Title,
                    BookCategory = b.BookCategories
                        .Select(bc => bc.Category.Name)
                        .FirstOrDefault()
                })
                .Where(b => categories.Contains(b.BookCategory.ToLower()))
                .OrderBy(b => b.Title)
                .ToList();


            var sb = new StringBuilder();

            foreach (var book in books)
            {
                sb.AppendLine($"{book.Title}");
            }

            return sb.ToString().TrimEnd();
        }
    }
}


