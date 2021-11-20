using System;
using System.Security.Cryptography;
using Xunit;

namespace Blockchain.UnitTests
{
    public sealed class TransactionTests
    {
        private const string Alice = "Alice";
        private const string Bob = "Bob";

        [Fact]
        public void HashShouldBeDeterministic()
        {
            var t1 = new Transaction(Alice, Bob, 10m);
            var t2 = new Transaction(Alice, Bob, 10m);
            Assert.Equal(t1.CalculateHashCode(SHA256.Create()), t2.CalculateHashCode(SHA256.Create()));
        }

        [Fact]
        public void HashCodeShouldNotChangeWithMultipleExecutions()
        {
            var t1 = new Transaction(Alice, Bob, 10m);
            var hash = t1.CalculateHashCode(SHA256.Create());
            var result = ByteArrayToString(hash);
            Assert.Equal("DE44156113444CD37054ACFC5243D9CF618E07A3AD433E1CE05AE53DD9248895", result);
        }

        private static string ByteArrayToString(byte[] ba) 
            => BitConverter.ToString(ba).Replace("-", "");
    }
}
