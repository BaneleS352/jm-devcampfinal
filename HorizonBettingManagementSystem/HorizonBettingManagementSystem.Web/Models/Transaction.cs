using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;

namespace HorizonBettingManagementSystem.Web.Models;

public class Transaction
{
    public int TransactionId { get; set; }

    public int AccountId { get; set; }

    public decimal Amount { get; set; }

    public string TransactionType { get; set; }

    public DateTimeOffset TransactionDate { get; set; }

    public DateTimeOffset CaptureDate { get; set; }

    [ValidateNever]
    public Account Account { get; set; }
}