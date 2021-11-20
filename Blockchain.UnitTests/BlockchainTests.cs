using System.Linq;
using Xunit;

namespace Blockchain.UnitTests
{
    public sealed class BlockchainTests
    {
        private const string Miner = "Miner";
        private const string Alice = "Alice";
        private const string Bob = "Bob";
        private const int ProofOfWorkDifficulty = 2;
        private const decimal MiningReward = 10m;

        [Fact]
        public void CreateBlockchain()
        {
            var blockChain = new BlockChain(ProofOfWorkDifficulty, MiningReward);
            blockChain.Append(new Transaction(Alice, Bob, 200m));
            blockChain.Append(new Transaction(Bob, Alice, 10m));

            Assert.Equal(1, blockChain.Chain.SelectMany(c => c.Transactions).Count());
            Assert.Equal(0m, blockChain.GetBalance(Alice));
            Assert.Equal(0m, blockChain.GetBalance(Bob));
            Assert.True(blockChain.IsValid(), "Blockchain was invalid");
        }

        [Fact]
        public void MinerShouldGetReward()
        {
            var blockChain = new BlockChain(ProofOfWorkDifficulty, MiningReward);
            blockChain.Append(new Transaction(Alice, Bob, 200m));
            blockChain.Append(new Transaction(Bob, Alice, 10m));
            blockChain.MineBlock(Miner);

            Assert.Equal(4, blockChain.Chain.SelectMany(c => c.Transactions).Count());
            Assert.Equal(10m, blockChain.GetBalance(Miner));
            Assert.Equal(-190m, blockChain.GetBalance(Alice));
            Assert.Equal(190m, blockChain.GetBalance(Bob));
            Assert.True(blockChain.IsValid(), "Blockchain was invalid");
        }
    }
}