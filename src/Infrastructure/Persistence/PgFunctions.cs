namespace Infrastructure.Persistence;

// general: CREATE EXTENSION IF NOT EXISTS pg_trgm;
// CREATE EXTENSION IF NOT EXISTS fuzzystrmatch;

// genre: CREATE INDEX trgm_idx ON genres USING gin (name gin_trgm_ops);

public static class PgFunctions
{
    public static double PgSimilarity(string source, string target) => throw new NotImplementedException();
    public static bool PgTrgmMatch(string source, string target) => throw new NotImplementedException();
    public static int PgFuzzyMatch(string source, string target) => throw new NotImplementedException();
}
