using System.Security.Cryptography;

namespace Blockchain;

public sealed class BlockChain
{
    private int ProofOfWorkDifficulty { get; }
    private decimal MiningReward { get; }
    private List<Transaction> PendingTransactions { get; } = new();
    public List<Block> Chain { get; set; }

    public BlockChain(int proofOfWorkDifficulty, decimal miningReward)
    {
        ProofOfWorkDifficulty = proofOfWorkDifficulty;
        MiningReward = miningReward;
        Chain = new List<Block> { CreateGenesisBlock() };
    }

    public void Append(Transaction transaction)
    {
        PendingTransactions.Add(transaction);
    }

    public void MineBlock(string minerAddress)
    {
        var minerRewardTransaction = new Transaction("", minerAddress, MiningReward);
        PendingTransactions.Add(minerRewardTransaction);
        var block = new Block(DateTime.UtcNow, PendingTransactions, Chain[^1].Hash);
        block.MineBlock(ProofOfWorkDifficulty);
        Chain.Add(block);

        PendingTransactions.Clear();
    }

    public bool IsValid()
    {
        using var sha256 = SHA256.Create();
        for (var i = 1; i < Chain.Count; i++)
        {
            var previousBlock = Chain[i - 1];
            var currentBlock = Chain[i];

            if (!currentBlock.Hash.SequenceEqual(currentBlock.CreateHash(sha256)))
                return false;
				
            if (!currentBlock.PreviousHash.SequenceEqual(previousBlock.Hash))
                return false;
        }

        return true;
    }

    public decimal GetBalance(string address)
    {
        var balance = 0m;

        foreach (var transaction in Chain.SelectMany(block => block.Transactions))
        {
            if (transaction.From == address)
            {
                balance -= transaction.Amount;
            }

            if (transaction.To == address)
            {
                balance += transaction.Amount;
            }
        }

        return balance;
    }

    private static Block CreateGenesisBlock()
    {
        var transactions = new List<Transaction> { new("", "", 0m) };
        return new Block(DateTime.UtcNow, transactions, Array.Empty<byte>());
    }
}