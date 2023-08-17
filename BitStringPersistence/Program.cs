// See https://aka.ms/new-console-template for more information
using BitStringPersistence;
using BitStringPersistence.Database;
using Microsoft.EntityFrameworkCore;
using NorseTechnologies.NorseLibrary.Data;

Console.WriteLine("BitString Persistence");

using (var context = new BitStringDbContext())
{
    BitStringDatabaseInitializer.Initialize(context);

    var bitStringGuid1 = new Guid("12345678-1234-1234-1234-123456789001");
    var bitStringGuid2 = new Guid("12345678-1234-1234-1234-123456789002");

    // Create a BitString object and add 2 segments
    // with a value of 0000000000000000000000000000000000000000000000000000010000000000000000000000000000000000000000000000000000000000000000000000101
    var bitString = new BitString(64);
    bitString.SetBit(0).SetBit(2).SetBit(74);

    // Persist the BitString to the database
    context.BitStrings.Add(bitString);
    context.SaveChanges();

    Console.WriteLine(string.Format("Created BitString with ID {0} persisted to the database.", bitString.Id.ToString()));

    // Retrieve the BitString from the database
    var retrievedBitString1Eager = context.BitStrings.Where(x => x.Id == bitStringGuid1).Include(x => x.Segments).Single(); // Eager load the bitstring segments in the initial query since we will always use the bitstring segements anytime we work with a bitstring
    var retrievedBitString2 = context.BitStrings.Find(bitStringGuid2);

    // Display the BitString and its segments
    Console.WriteLine("\nSeeded BitString #1:\n");
    retrievedBitString1Eager?.DebugDump();

    Console.WriteLine("\nSeeded BitString #2:\n");
    retrievedBitString2?.DebugDump();

    Console.WriteLine("\nBitString created in program.cs:\n");
    bitString.DebugDump();
}

Console.WriteLine("Press any key to exit.");
Console.ReadKey();
        