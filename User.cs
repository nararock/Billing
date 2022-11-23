namespace GrpcService
{
    public class User
    {
        public string Name { get; set; }
        public int Rating { get; set; }
        public long Amount { get; set; }
        public Stack<CoinInfo> Coins { get; set; }
    }
}
