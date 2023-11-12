using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BE.Context;
using BE.Model.Dto;
using BE.Model.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace BE.Controllers
{
    public class GachaController : BaseApiController
    {
        private readonly DataContext _context;
        public GachaController(DataContext context)
        {
            _context = context;
        }
        [HttpGet("Gacha")]
        public async Task<ActionResult<List<GachaOutputDto>>> Gacha([FromQuery] GachaInputDto input)
        {
            var user = await _context.User.SingleOrDefaultAsync(u => u.Username == input.Username);
            Card randomCard;
            List<Card> allCards = new List<Card>();
            int price = 1000;

            if(user.Money > price)
            {
                user.Money -= price;
                _context.SaveChanges();
            }
            else return BadRequest(new {message = "Tài khoản không đủ tiền"});

            for (int i = 0; i < 10; i++)
            {
                var rate = new Random().Next(1, 101);

                if (rate <= 40) randomCard = await GetRandomCardsByRarity("N");
                else if (rate <= 70) randomCard = await GetRandomCardsByRarity("R");
                else if (rate <= 95) randomCard = await GetRandomCardsByRarity("SR");
                else randomCard = await GetRandomCardsByRarity("UR");
                allCards.Add(randomCard);
            }

            for (int i = 0; i < 10; i++)
            {
                var card = allCards[i];

                //Tạo mới UserCard và thêm vào cơ sở dữ liệu
                var userCard = new UserCard
                {
                    UserId = user.UserId, // Sử dụng UserId từ bảng User
                    CardId = card.CardId,
                };

                _context.UserCard.Add(userCard);
                _context.SaveChanges();
            }

            return Ok(allCards);
        }

        private async Task<Card> GetRandomCardsByRarity(string rarity)
        {
            return await _context.Card
                .Where(c => c.CardRarityName == rarity)
                .OrderBy(c => Guid.NewGuid())
                .Take(1)
                .SingleOrDefaultAsync();
        }
    }
}