using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;

namespace HorizonBettingManagementSystem.Web.Models;

public class Account
{
    public int AccountId { get; set; }

    public string AccountNumber { get; set; }

    public int ClientId { get; set; }

    public decimal Balance { get; set; }

    public bool IsClosed { get; set; }

    public DateTimeOffset CreatedDate { get; set; }

    [ValidateNever]
    public Client Client { get; set; }

    [ValidateNever]
    public List<Transaction> Transactions { get; set; } = new();
}