﻿using System;
using ClientData;

namespace ClientLogic
{
    class StoreItem : IStoreItem
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public LogicItemType Type { get; private set; }
        public float Price { get; private set; }
        public bool IsSold { get; set; }

        public StoreItem(IItem item)
        {
            Id = item.Id;
            Name = item.Name;
            Description = item.Description;
            Type = (LogicItemType)item.Type;
            Price = item.Price;
            IsSold = item.IsSold;
        }
    }
}
