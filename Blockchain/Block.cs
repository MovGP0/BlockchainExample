using System.Diagnostics;
using System.Security.Cryptography;

namespace Blockchain;

public sealed class Block
{
    /// <summary>time of block creation</summary>
    private DateTime TimeStamp { get; }

    private long Nonce { set; get; }

    /// <summary>contains the hash of the previous block in the chain</summary>
    public byte[] PreviousHash { get; }

    /// <summary>hash of the block calculated based on all the properties of the block</summary>
    public byte[] Hash { get; private set; }

    /// <summary>data stored in the block</summary>
    public Transaction[] Transactions { get; }

    public Block(DateTime timeStamp, IEnumerable<Transaction> transactions, byte[] previousHash)
    {
        if (timeStamp.Kind != DateTimeKind.Utc) 
            throw new ArgumentException("Must be in UTC.", nameof(timeStamp));

        TimeStamp = timeStamp;
        Transactions = transactions.ToArray();
        PreviousHash = previousHash;

        using var sha256 = SHA256.Create();
        Hash = CreateHash(sha256);
    }

    public void MineBlock(int proofOfWorkDifficulty)
    {
        if (proofOfWorkDifficulty < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(proofOfWorkDifficulty), proofOfWorkDifficulty, "Difficulty must be at least 1.");
        }

        static string ByteArrayToString(byte[] ba) => BitConverter.ToString(ba).Replace("-", "");

        var hashValidationTemplate = new string('0', proofOfWorkDifficulty);
        var stopwatch = Stopwatch.StartNew();

        using var sha256 = SHA256.Create();
        while (ByteArrayToString(Hash)[..proofOfWorkDifficulty] != hashValidationTemplate)
        {
            Nonce += 1;
            Hash = CreateHash(sha256);
        }

        Debug.Assert(Hash.SequenceEqual(CreateHash(sha256)));

        stopwatch.Stop();
        Console.WriteLine($"Block with Hash '{ByteArrayToString(Hash)}' successfully mined within {stopwatch.ElapsedMilliseconds} ms.");
    }
    
    public byte[] CreateHash(SHA256 sha256)
    {
        var rawData = PreviousHash
            .Concat(BitConverter.GetBytes(TimeStamp.Ticks))
            .Concat(Transactions.SelectMany(e => e.CalculateHashCode(sha256)))
            .Concat(BitConverter.GetBytes(Nonce))
            .ToArray();

        return sha256.ComputeHash(rawData);
    }
}