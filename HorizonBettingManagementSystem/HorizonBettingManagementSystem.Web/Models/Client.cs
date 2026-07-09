using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;

namespace HorizonBettingManagementSystem.Web.Models;

public class Client
{
    public int ClientId { get; set; }

    public string IdNumber { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public DateTimeOffset CreatedDate { get; set; }

    [ValidateNever]
    public List<Account> Accounts { get; set; } = new List<Account>();
}
