using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BE.Context;
using BE.InterfaceController;
using BE.Model.Dto;
using BE.Model.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BE.Controllers
{
    public class DealController : BaseApiController, IDealController
    {
        private readonly DataContext _context;
        private readonly UserCardController _userCard;
        public DealController(DataContext context, UserCardController userCard)
        {
            _context = context;
            _userCard = userCard;
        }

        [HttpGet("SearchDeal")]
        public async Task<ActionResult<List<DealSearchOutputDto>>> SearchDeal([FromQuery] DealSearchInputDto input)
        {
            var deal = from Deal in _context.Deal 
            join UserCard in _context.UserCard on Deal.UserCardId equals UserCard.UserCardId 
            join Card in _context.Card on UserCard.CardId equals Card.CardId 
            join User in _context.User on UserCard.UserId equals User.UserId
            where (string.IsNullOrWhiteSpace(input.SellUsername) || User.Username.Contains(input.SellUsername))
            && (string.IsNullOrWhiteSpace(input.CardName) || Card.CardName.Contains(input.CardName))
            && (string.IsNullOrWhiteSpace(input.CardTypeName) || Card.CardTypeName == input.CardTypeName)
            && (string.IsNullOrWhiteSpace(input.CardOriginName) || Card.CardOriginName == input.CardOriginName)
            && (string.IsNullOrWhiteSpace(input.CardElementName) || Card.CardElementName == input.CardElementName)
            && (string.IsNullOrWhiteSpace(input.CardRarityName) || Card.CardRarityName == input.CardRarityName)
            && (input.PriceFrom == null || input.PriceFrom == 0 || Deal.Price >= input.PriceFrom) && (input.PriceTo == null || Deal.Price <= input.PriceTo)
            && (input.DateFrom == null || Deal.CreateDate >= input.DateFrom) && (input.DateTo == null || Deal.CreateDate <= input.DateTo)
            && (Deal.BuyUserId == null)
            select new DealSearchOutputDto() {
                DealId = Deal.DealId,
                CardId = Card.CardId,
                SellUsername = User.Username,
                CardName = Card.CardName,
                CardImageURL = Card.CardImageURL,
                CardTypeName = Card.CardTypeName,
                CardOriginName = Card.CardOriginName,
                CardElementName = Card.CardElementName,
                CardRarityName = Card.CardRarityName,
                Price = Deal.Price,
                CreateDate = Deal.CreateDate,
            };
            return await deal.ToListAsync();
        }

        //[Authorize]
        [HttpGet("GetBoughtDeal")]
        public async Task<ActionResult<List<DealGetBuyedOutputDto>>> GetBoughtDeal([FromQuery] string Username)
        {
            var user = await _context.User.SingleOrDefaultAsync(u => u.Username == Username);
            if (user == null) return BadRequest(new {message = "User not found!"});
            var deal = from Deal in _context.Deal 
            join User in _context.User on Deal.SellUserId equals User.UserId 
            join UserCard in _context.UserCard on Deal.UserCardId equals UserCard.UserCardId 
            join Card in _context.Card on UserCard.CardId equals Card.CardId 
            orderby Deal.CreateDate descending
            where (Deal.BuyUserId != null)
            && (Deal.BuyUserId == user.UserId)
            select new DealGetBuyedOutputDto() {
                DealId = Deal.DealId,
                SellUsername = User.Username,
                CardId = Card.CardId,
                CardName = Card.CardName,
                CardImageURL = Card.CardImageURL,
                CardTypeName = Card.CardTypeName,
                CardOriginName = Card.CardOriginName,
                CardElementName = Card.CardElementName,
                CardRarityName = Card.CardRarityName,
                Price = Deal.Price,
                CreateDate = Deal.CreateDate,
            };
            return await deal.ToListAsync();
        }

        //[Authorize]
        [HttpGet("GetSoldDeal")]
        public async Task<ActionResult<List<DealGetSelledOutputDto>>> GetSoldDeal([FromQuery] string Username)
        {
            var user = await _context.User.SingleOrDefaultAsync(u => u.Username == Username);
            if (user == null) return BadRequest(new {message = "User not found!"});
            var deal = from Deal in _context.Deal 
            join User in _context.User on Deal.BuyUserId equals User.UserId 
            join UserCard in _context.UserCard on Deal.UserCardId equals UserCard.UserCardId 
            join Card in _context.Card on UserCard.CardId equals Card.CardId 
            where (Deal.BuyUserId != null)
            && (Deal.SellUserId == user.UserId)
            orderby Deal.CreateDate descending
            select new DealGetSelledOutputDto() {
                DealId = Deal.DealId,
                BuyUsername = User.Username,
                CardId = Card.CardId,
                CardName = Card.CardName,
                CardImageURL = Card.CardImageURL,
                CardTypeName = Card.CardTypeName,
                CardOriginName = Card.CardOriginName,
                CardElementName = Card.CardElementName,
                CardRarityName = Card.CardRarityName,
                Price = Deal.Price,
                CreateDate = Deal.CreateDate,
            };
            return await deal.ToListAsync();
        }

        //[Authorize]
        [HttpPost("CreateDeal")]
        public async Task<ActionResult> CreateDeal([FromBody] DealCreateInputDto input)
        {
            TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            DateTime vietnamTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone);
            var selluser = await _context.User.SingleOrDefaultAsync(u => u.Username == input.SellUsername);
            if (selluser == null) return BadRequest(new {message = "User not found!"});
            var usercard = await _context.UserCard.SingleOrDefaultAsync(uc => uc.UserCardId == input.UserCardId);
            if (usercard == null) return BadRequest(new {message = "Your Card not found!"});
            if (usercard.UserId != selluser.UserId) return BadRequest(new {message = "You don't owned this Card!"});
            if (usercard.OnDeal == true) return BadRequest(new {message = "Card already on another Deal!"});
            if (input.Price <= 0) return BadRequest(new {message = "Price invalid!"});
            else 
            {
                var newdeal = new Deal 
                {
                    SellUserId = selluser.UserId,
                    BuyUserId = null,
                    UserCardId = usercard.UserCardId,
                    Price = input.Price,
                    CreateDate = vietnamTime,
                };
                _context.Deal.Add(newdeal);
                await _userCard.MakeOnDeal(usercard.UserCardId);
                await _context.SaveChangesAsync();
            }
            return Ok(new {message = "Create Deal successfully!"});
        }

        //[Authorize]
        [HttpPut("EditDeal")]
        public async Task<ActionResult> EditDeal([FromBody] DealEditInputDto input)
        {
            var selluser = await _context.User.SingleOrDefaultAsync(u => u.Username == input.SellUsername);
            if (selluser == null) return BadRequest(new {message = "User not found!"});
            var deal = await _context.Deal.SingleOrDefaultAsync(d => d.DealId == input.DealId);
            if (deal == null) return NotFound(new {message = "Deal not found!"});
            if (deal.SellUserId != selluser.UserId) return NotFound(new {message = "You don't owned this Deal!"});
            var usercard = await _context.UserCard.SingleOrDefaultAsync(uc => uc.UserCardId == input.UserCardId);
            if (usercard == null) return BadRequest(new {message = "Your Card not found!"});
            if (usercard.UserId != selluser.UserId) return BadRequest(new {message = "You don't owned this Card!"});
            if (usercard.OnDeal == true) return BadRequest(new {message = "Card already on another Deal!"});
            if (input.Price <= 0) return BadRequest(new {message = "Price invalid!"});
            else
            {
                await _userCard.RemoveOnDeal(deal.UserCardId);
                await _userCard.MakeOnDeal(input.UserCardId);
                deal.UserCardId = input.UserCardId;
                deal.Price = input.Price;
                await _context.SaveChangesAsync();
            }
            return Ok(new {message = "Edit Deal successfully!"});
        }

        //[Authorize]
        [HttpDelete("DeleteDeal")]
        public async Task<ActionResult> DeleteDeal([FromBody] DealDeleteInputDto input)
        {
            var selluser = await _context.User.SingleOrDefaultAsync(u => u.Username == input.SellUsername);
            if (selluser == null) return BadRequest(new {message = "User not found!"});
            var deal = await _context.Deal.SingleOrDefaultAsync(d => d.DealId == input.DealId);
            if (deal == null) return NotFound(new {message = "Deal not found!"});
            if (deal.SellUserId != selluser.UserId) return NotFound(new {message = "You don't owned this Deal!"});
            else
            {
                _context.Deal.Remove(deal);
                await _userCard.RemoveOnDeal(deal.UserCardId);
                await _context.SaveChangesAsync();
            }
            return Ok(new {message = "Delete Deal successfully!"});
        }
        
        //[Authorize]
        [HttpPost("AcceptDeal")]
        public async Task<ActionResult> AcceptDeal([FromBody] DealAcceptInputDto input)
        {
            var buyuser = await _context.User.SingleOrDefaultAsync(u => u.Username == input.BuyUsername);
            if (buyuser == null) return BadRequest(new {message = "User not found!"});
            var deal = await _context.Deal.SingleOrDefaultAsync(d => d.DealId == input.DealId);
            if (deal == null) return NotFound(new {message = "Deal not found!"});
            if (buyuser.UserId == deal.SellUserId) return BadRequest(new {message = "Can't buy your own card!"});
            if (buyuser.Money < deal.Price) return BadRequest(new {message = "Account don't have enough money!"});
            if (deal.BuyUserId != null) return BadRequest(new {message = "Deal does not exist!"});
            else
            {
                deal.BuyUserId = buyuser.UserId;
                await _userCard.ChangeOwner(deal.UserCardId, buyuser.UserId);
                buyuser.Money -= deal.Price;
                var selluser = await _context.User.SingleOrDefaultAsync(u => u.UserId == deal.SellUserId);
                if (selluser != null)
                {
                    selluser.Money += deal.Price;
                }
                await _context.SaveChangesAsync();
            }
            return Ok(new {message = "Purchase successfully!"});
        }
    }
}