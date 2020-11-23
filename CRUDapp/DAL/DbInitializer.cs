using CRUDapp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CRUDapp.DAL
{
    public static class DbInitializer
    {
        /// <summary>
        /// Initialise DB with test data.
        /// If the database is not found:
        ///    It is created and loaded with test data.
        ///    It loads test data into arrays rather than List<T> collections to optimize performance.
        /// If the database if found, it takes no action.
        /// </summary>
        /// <param name="context"></param>
        public static void Initialize(ApplicationDbContext context)
        {
            // The EnsureCreated method is used to automatically create the database. 
            context.Database.EnsureCreated();

            // Look for any contacts.
            if (context.Contacts.Any())
            {
                return;   // DB has been seeded
            }

            var contacts = new Contact[]
            {
                new Contact{Name="James Pluck",Email="james.pluck@whoisvisiting.com",Phone="02045 115 445",Domain="whoisvisiting.com",Notes="Manager"}, // "Marketing Manager of Whoisvisiting."
                new Contact{Name="Mike Nourse",Email="mike@thedeveloperlink.io",Phone="02045 115 445",Domain="thedeveloperlink.io",Notes="Recruiter"}, // "The Developer Link recruiter."
                new Contact{Name="Gabriella Balbino",Email="gabriella.balbino@whoisvisiting.com",Phone="408-290-0181",Domain="whoisvisiting.com",Notes="Marketing"}, // "Marketing team member of Whoisvisiting."
                new Contact{Name="Ant Musker",Email="ant.musker@whoisvisiting.com",Phone="0800 088 5775",Domain="whoisvisiting.com",Notes="Sales"}, // "Sales team member of Whoisvisiting."
                new Contact{Name="Marek Kubis",Email="marekakubis@yahoo.com",Phone="07927 44 8883",Domain="attractionworld.com",
                            Notes="Candidate" } // "Candidate for the position of a software developer (Whois Backend Developer JD)."
            };
            foreach (Contact s in contacts)
            {
                context.Contacts.Add(s);
            }
            context.SaveChanges();
        }
    }


}
