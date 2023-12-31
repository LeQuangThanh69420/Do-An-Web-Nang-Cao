using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BE.Model.Entity
{
    public class Card
    {
        public long CardId { get; set; }
        public string CardName { get; set; }
        public string CardImageURL { get; set; }
        public string CardTypeName { get; set; }
        public string? CardOriginName { get; set; }
        public string? CardElementName { get; set; }
        public string CardRarityName { get; set; }
    }
}