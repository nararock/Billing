using Billing;
using Grpc.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.IIS.Core;
using System.Security.AccessControl;

namespace GrpcService.Services
{
    public class BillingService: Billing.Billing.BillingBase
    {
        private readonly ILogger<BillingService> _logger;
        public BillingService(ILogger<BillingService> logger)
        {
            _logger = logger;
        }
        public override async Task ListUsers(None request, IServerStreamWriter<UserProfile> responseStream, ServerCallContext context)
        {
            List<UserProfile> users = new List<UserProfile>();

            foreach(var element in Startup.Users)
            {
                UserProfile user = new UserProfile() { Name = element.Name, Amount = element.Amount};
                users.Add(user);
            }

            foreach (var user in users)
            {
                await responseStream.WriteAsync(user);
            }
        }
       
        public override Task<Response> CoinsEmission(EmissionAmount request, ServerCallContext context)
        {
            int length = Startup.Users.Count;
            if (request.Amount < length) return Task.FromResult(new Response() { Status = Response.Types.Status.Failed, Comment = "not enough coins" });
            int sumRating = Startup.Users.Sum(u => u.Rating);
            long amountCoin = 0;
            for(int i = 0; i < length; i++)
            {
                long temp = (long)(Math.Ceiling(((double)request.Amount * (double)Startup.Users[i].Rating) / (double)sumRating));
                Startup.Users[i].Amount += temp;
                amountCoin += temp;
                if (request.Amount - amountCoin < length - 1 - i)
                {
                    Startup.Users[i].Amount -= (length - 1 - i) - (request.Amount - amountCoin);
                    amountCoin -= (length - 1 - i) - (request.Amount - amountCoin);
                    temp -= (length - 1 - i) - (request.Amount - amountCoin);
                }
                for (int j = 0; j < temp; j++)
                {
                    CoinInfo coin = new CoinInfo();
                    Startup.LastIdCoin++;
                    coin.Id = Startup.LastIdCoin;
                    coin.History.Add(Startup.Users[i].Name);
                    Startup.Users[i].Coins.Push(coin);
                }
            }

            return Task.FromResult(new Response() { Status = Response.Types.Status.Ok, Comment = "emission of coins was successful" });
        }
        public override Task<Response> MoveCoins(MoveCoinsTransaction request, ServerCallContext context)
        {
            User userSrc = new User();
            User userDst = new User();
            foreach(var element in Startup.Users)
            {
                if (element.Name == request.SrcUser) userSrc = element;
                else if (element.Name == request.DstUser) userDst = element;
            }
            if (userDst == null || userSrc == null) return Task.FromResult(new Response() { Status = Response.Types.Status.Unspecified, Comment = "there are no such users" });
            if (userSrc.Amount < request.Amount) return Task.FromResult(new Response { Status = Response.Types.Status.Failed, Comment = "not enough coins" });
            for (int i = 0; i < request.Amount; i++)
            {
                var coin = userSrc.Coins.Pop();
                coin.History.Add(userDst.Name);
                userDst.Coins.Push(coin);
                userSrc.Amount--;
                userDst.Amount++;
            }
            return Task.FromResult(new Response() { Status = Response.Types.Status.Ok, Comment = "move coins was successful" });
        }
        public override Task<Coin> LongestHistoryCoin(None request, ServerCallContext context)
        {
            int maxHistory = 0;
            CoinInfo coinMaxHistory = new CoinInfo();
            foreach(var user in Startup.Users)
            {
                foreach(var coin in user.Coins)
                {
                    if (coin.History.Count > maxHistory)
                    {
                        maxHistory = coin.History.Count;
                        coinMaxHistory = coin;
                    }
                }
            }
            string coinHistory = "";
            foreach (var element in coinMaxHistory.History)
            {
                coinHistory += element.ToString();
            }
            Coin coinResult = new Coin() { Id = coinMaxHistory.Id, History = coinHistory };
            return Task.FromResult(coinResult);
        }
    }
}
