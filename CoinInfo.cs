namespace GrpcService
{
    public class CoinInfo
    {
        public long Id { get; set; }
        public List<string> History = new List<string>();
    }
}
