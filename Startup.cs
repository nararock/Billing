using Billing;

namespace GrpcService
{
    public static class Startup
    {
        public static List<User> Users = new List<User>()
            {
                new User()
                {
                    Name = "boris",
                    Rating = 5000,
                    Amount = 0,
                    Coins = new Stack<CoinInfo>()
    },
                new User()
                {
                    Name = "maria",
                    Rating = 1000,
                    Amount = 0,
                    Coins = new Stack<CoinInfo>()
                },
                new User()
                {
                    Name = "oleg",
                    Rating = 800,
                    Amount = 0,
                    Coins = new Stack<CoinInfo>()
                }
            };
        public static int LastIdCoin = -1;    
    }
}

