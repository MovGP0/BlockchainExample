using System.Security.Cryptography;
using System.Text;

namespace Blockchain
{
	public sealed class Transaction
	{
		public Transaction(string from, string to, decimal amount)
		{
			ArgumentNullException.ThrowIfNull(from);
			ArgumentNullException.ThrowIfNull(to);

			From = from;
			To = to;
			Amount = amount;
		}

		public string From { get; }
		public string To { get; }
		public decimal Amount { get; }

		#region Hashing
		private byte[] _hash = { };
		public byte[] CalculateHashCode(SHA256 sha256)
		{
			if (_hash.Length > 0) return _hash;

			var utf8 = Encoding.UTF8;

			var rawData = Array.Empty<byte>()
				.Concat(utf8.GetBytes(From))
				.Concat(utf8.GetBytes(To))
				.Concat(decimal.GetBits(Amount).SelectMany(BitConverter.GetBytes))
				.ToArray();

			_hash = sha256.ComputeHash(rawData);
			return _hash;
		}
		#endregion
	}
}