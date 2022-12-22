﻿using NetFlex.DAL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetFlex.DAL.Entities
{
    public class Subscription
    {
        public Guid Id { get; set; }
        public SubscriptionType Name { get; set; }
        public float Cost { get; set; }
    }
}
