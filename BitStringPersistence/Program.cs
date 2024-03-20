// See https://aka.ms/new-console-template for more information
using BitStringPersistence;
using BitStringPersistence.Database;
using Microsoft.EntityFrameworkCore;
using NorseTechnologies.NorseLibrary.Data;

Console.WriteLine("BitString Persistence");

using (var context = new BitStringDbContext())
{
    // We comment out the Initializer if we want to test out reading the values from the SQL Server database from a previous run

    BitStringDatabaseInitializer.Initialize(context);

    var bitStringGuid1 = new Guid("12345678-1234-1234-1234-123456789001");
    var bitStringGuid2 = new Guid("12345678-1234-1234-1234-123456789002");
    var bitStringGuid3 = new Guid("12345678-1234-1234-1234-123456789003");

    // Create a BitString object and add 2 segments
    // with a value of 0000000000000000000000000000000000000000000000000000010000000000000000000000000000000000000000000000000000000000000000000000101
    var bitString = new BitString();
    bitString.SetBit(0).SetBit(2).SetBit(74);

    // Persist the BitString to the database
    context.BitStrings.Add(bitString);
    context.SaveChanges();

    Console.WriteLine(string.Format("Created BitString with ID {0} persisted to the database.", bitString.Id.ToString()));

    // Retrieve the BitStrings from the database

    var retrievedBitString1Eager = context.BitStrings.Where(x => x.Id == bitStringGuid1).Include(x => x.Segments).Single(); // Eager load the bitstring segments in the initial query since we will always use the bitstring segments anytime we work with a bitstring
    var retrievedBitString2 = context.BitStrings.Where(x => x.Id == bitStringGuid2).Single(); // Retrieves an entity by its primary key, but it does not automatically load related entities. To load related entities, you must use the Include method to specify related data to be included in query results.

    // The Find method on a DbSet retrieves an entity by its primary key, but it does not automatically load related entities.
    var retrievedBitString3 = context.BitStrings.Find(bitStringGuid3);
    if (null != retrievedBitString3)
    {
        // You can follow the Find with an explicit loading of the related data:
        context.Entry(retrievedBitString3).Collection(b => b.Segments).Load();
    }

    // Another way to load related entities is to use lazy loading. However, this requires some
    // additional setup. In lazy loading, the related entities are loaded automatically the
    // first time the navigation property is accessed, provided that the DbContext is still in
    // scope and has not been disposed.
    //
    // To enable lazy loading:
    // 
    //  1. Install the Microsoft.EntityFrameworkCore.Proxies package.
    //
    //  2. In the OnConfiguring method of your DbContext, use the UseLazyLoadingProxies method:
    //
    //      protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //      {
    //          optionsBuilder.UseLazyLoadingProxies().UseSqlServer(yourConnectionString);
    //      }
    //
    //  3. Ensure that your navigation properties are virtual:
    //
    //      public virtual Profile Profile { get; set; }
    //      public virtual ICollection<Order> Orders { get; set; }
    //
    // With the above setup, EF Core will automatically load related entities the first time
    // you access a navigation property. Do note, however, that this can lead to the "N+1 query
    // problem" if not used carefully.
    //
    // Remember, when working with related entities, it's essential to keep track of how many
    // database operations are being executed and when, to ensure efficient and performant data access.

    // Display the BitString and its segments
    Console.WriteLine("\nSeeded BitString #1:\n");
    retrievedBitString1Eager?.DebugDump();

    Console.WriteLine("\nSeeded BitString #2:\n");
    retrievedBitString2?.DebugDump();

    Console.WriteLine("\nSeeded BitString #3:\n");
    retrievedBitString3?.DebugDump();

    Console.WriteLine("\nBitString created in program.cs:\n");
    bitString.DebugDump();
}

Console.WriteLine("Press any key to exit.");
Console.ReadKey();
        