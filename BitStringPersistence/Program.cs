// See https://aka.ms/new-console-template for more information
using BitStringPersistence;
using NorseTechnologies.NorseLibrary.Data;
using static NorseTechnologies.NorseLibrary.Data.BitString;

Console.WriteLine("BitString Persistence");

using (var context = new BitStringDbContext())
{
    context.Database.EnsureDeleted();
    context.Database.EnsureCreated();

    // Create a BitString object and add segments
    var bitString = new BitString(64);
    bitString.SetBit(0).SetBit(2).SetBit(74);

    // Persist the BitString to the database
    context.BitStrings.Add(bitString);
    context.SaveChanges();

    Console.WriteLine("BitString persisted to the database.");

    // Retrieve the BitString from the database
    var retrievedBitString = context.BitStrings.Find(bitString.Id);

    // Display the BitString and its segments
    Console.WriteLine($"Retrieved BitString: Id={retrievedBitString.Id}");
    Console.WriteLine("BitString Value:" +  retrievedBitString.ToString());
    int iSegment = 0;
    foreach (var segment in retrievedBitString.Segments)
    {
        Console.WriteLine($"Segment[{iSegment++}]: Id={segment.Id}, Value={segment.BitMask}");
    }
}

Console.WriteLine("Press any key to exit.");
Console.ReadKey();
        