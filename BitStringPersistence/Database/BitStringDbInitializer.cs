namespace BitStringPersistence.Database
{
    public static class BitStringDatabaseInitializer
    {
        public static void Initialize(BitStringDbContext context)
        {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            BitStringDataSeeder.SeedData(context);
        }
    }
}