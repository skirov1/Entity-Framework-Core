namespace BookShop
{
    using BookShop.Models;
    using BookShop.Models.Enums;
    using Data;
    using Initializer;
    using Microsoft.EntityFrameworkCore;
    using System.Linq;
    using System.Text;
    using static System.Reflection.Metadata.BlobBuilder;

    public class StartUp
    {
        public static void Main()
        {
            using var db = new BookShopContext();
            //DbInitializer.ResetDatabase(db);

            Console.WriteLine(GetMostRecentBooks(db));
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

        public static string GetBooksReleasedBefore(BookShopContext context, string date)
        {
            var books = context
                .Books
                .Where(b => b.ReleaseDate < DateTime.ParseExact(date, "dd-MM-yyyy", System.Globalization.CultureInfo.InvariantCulture))
                .OrderByDescending(b => b.ReleaseDate)
                .Select(b => new
                {
                    b.Title,
                    b.EditionType,
                    b.Price
                })
                .ToList();

            var sb = new StringBuilder();

            foreach (var book in books)
            {
                sb.AppendLine($"{book.Title} - {book.EditionType.ToString()} - ${book.Price:f2}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
        {
            var authors = context
                .Authors
                .Where(a => a.FirstName.EndsWith(input))
                .Select(a => new
                {
                    a.FirstName,
                    a.LastName
                })
                .OrderBy(a => a.FirstName)
                .ThenBy(a => a.LastName)
                .ToList();

            var sb = new StringBuilder();

            foreach (var author in authors)
            {
                sb.AppendLine($"{author.FirstName} {author.LastName}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetBookTitlesContaining(BookShopContext context, string input)
        {
            var books = context
                .Books
                .Where(b => b.Title.ToLower().Contains(input.ToLower()))
                .Select(b => b.Title)
                .OrderBy(b => b)
                .ToList();

            var sb = new StringBuilder();

            foreach (var book in books)
            {
                sb.AppendLine($"{book}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetBooksByAuthor(BookShopContext context, string input)
        {
            var books = context
                .Books
                .Include(b => b.Author)
                .Where(b => b.Author.LastName.ToLower().StartsWith(input.ToLower()))
                .OrderBy(b => b.BookId)
                .Select(b => new
                {
                    b.Title,
                    b.Author.FirstName,
                    b.Author.LastName
                })
                .ToList();

            var sb = new StringBuilder();

            foreach (var book in books)
            {
                sb.AppendLine($"{book.Title} ({book.FirstName} {book.LastName})");
            }

            return sb.ToString().TrimEnd();
        }

        public static int CountBooks(BookShopContext context, int lengthCheck)
        {
            var books = context
                .Books
                .Where(b => b.Title.Length > lengthCheck)
                .ToList();

            return books.Count;
        }

        public static string CountCopiesByAuthor(BookShopContext context)
        {
            var copiesByAuthor = context
                .Authors
                .Select(a => new
                {
                    AuthorName = $"{a.FirstName} {a.LastName}",
                    TotalCopies = a.Books.Sum(x => x.Copies)
                })
                .OrderByDescending(x => x.TotalCopies)
                .ToList();

            var sb = new StringBuilder();

            foreach (var item in copiesByAuthor)
            {
                sb.AppendLine($"{item.AuthorName} - {item.TotalCopies}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetTotalProfitByCategory(BookShopContext context)
        {
            var profitByCategory = context
                .Categories
                .Select(c => new
                {
                    c.Name,
                    TotalProfit = c.CategoryBooks.Sum(cb => cb.Book.Price * cb.Book.Copies)
                })
                .OrderByDescending(c => c.TotalProfit)
                .ThenBy(c => c.Name)
                .ToList();

            var sb = new StringBuilder();

            foreach (var item in profitByCategory)
            {
                sb.AppendLine($"{item.Name} ${item.TotalProfit:f2}");
            }

            return sb.ToString().TrimEnd(); 
        }

        public static string GetMostRecentBooks(BookShopContext context)
        {
            var recentBooks = context
               .Categories
               .Select(c => new
               {
                   c.Name,
                   TotalProfit = c.CategoryBooks.Sum(cb => cb.Book.Price * cb.Book.Copies),
                   Books = c.CategoryBooks.Select(cb => cb.Book).OrderByDescending(b => b.ReleaseDate).Take(3)
               })
               .OrderBy(c => c.Name)
               .ToList();

            var sb = new StringBuilder();

            foreach (var category in recentBooks)
            {
                sb.AppendLine($"--{category.Name}");
                foreach (var book in category.Books)
                {
                    sb.AppendLine($"{book.Title} ({book.ReleaseDate.Value.Year})");
                }
            }

            return sb.ToString().TrimEnd();
        }
    }
}


