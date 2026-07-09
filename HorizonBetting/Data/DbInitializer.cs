using HorizonBetting.Models;
using System;
using System.Linq;
using System.Collections.Generic;

namespace HorizonBetting.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();

            // Look for any users.
            if (context.Users.Any())
            {
                return;   // DB has been seeded
            }

            var users = new User[]
            {
                new User{FirstName="James",Surname="Holden",IdNumber="8501015000080",Email="james.holden@roci.com"},
                new User{FirstName="Amos",Surname="Burton",IdNumber="8005125000081",Email="amos@roci.com"},
                new User{FirstName="Naomi",Surname="Nagata",IdNumber="8609205000082",Email="naomi@roci.com"},
                new User{FirstName="Chrisjen",Surname="Avasarala",IdNumber="5503145000083",Email="chrisjen@un.gov"}
            };
            foreach (User u in users)
            {
                context.Users.Add(u);
            }
            context.SaveChanges();

            var accounts = new Account[]
            {
                new Account{UserId=users[0].Id, AccountNumber="ACC-001001", OutstandingBalance=0},
                new Account{UserId=users[0].Id, AccountNumber="ACC-001002", OutstandingBalance=0},
                new Account{UserId=users[1].Id, AccountNumber="ACC-002001", OutstandingBalance=0},
                new Account{UserId=users[2].Id, AccountNumber="ACC-003001", OutstandingBalance=0},
                new Account{UserId=users[3].Id, AccountNumber="ACC-004001", OutstandingBalance=0}
            };
            foreach (Account a in accounts)
            {
                context.Accounts.Add(a);
            }
            context.SaveChanges();

            var random = new Random();
            var transactions = new List<Transaction>();

            foreach (var account in accounts)
            {
                // Generate 5-15 transactions per account
                int txCount = random.Next(5, 16);
                decimal currentBalance = 0;

                for (int i = 0; i < txCount; i++)
                {
                    // Random date in the past 30 days
                    var date = DateTime.Now.AddDays(-random.Next(1, 30)).AddHours(-random.Next(1, 24));
                    var isCredit = random.Next(100) > 40; // 60% chance of credit (deposit)
                    decimal amount = Math.Round((decimal)(random.NextDouble() * 5000 + 50), 2);

                    transactions.Add(new Transaction
                    {
                        AccountId = account.Id,
                        Amount = amount,
                        Type = isCredit ? TransactionType.Credit : TransactionType.Debit,
                        TransactionDate = date
                    });

                    if (isCredit)
                        currentBalance += amount;
                    else
                        currentBalance -= amount;
                }

                // Update account balance
                account.OutstandingBalance = currentBalance;
            }

            // sort chronologically before saving just in case
            foreach (var t in transactions.OrderBy(x => x.TransactionDate))
            {
                context.Transactions.Add(t);
            }
            
            context.SaveChanges();
        }
    }
}
