using System;
using System.ComponentModel.DataAnnotations;

namespace PetStore.Models.Attributes
{
    public class NotExpiredCardAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                return true; // Let Required attribute handle empty values

            string expiry = value.ToString()!;
            
            // Remove the slash if present (MM/YY or MMYY)
            expiry = expiry.Replace("/", "").Replace(" ", "");
            
            if (expiry.Length != 4)
                return false; // Format validation should catch this, but just in case

            // Extract month and year
            if (int.TryParse(expiry.Substring(0, 2), out int month) &&
                int.TryParse(expiry.Substring(2, 2), out int year))
            {
                // Convert 2-digit year to 4-digit (assuming 00-50 = 2000-2050, 51-99 = 1951-1999)
                int fullYear = year <= 50 ? 2000 + year : 1900 + year;
                
                // Get the last day of the expiry month
                DateTime expiryDate = new DateTime(fullYear, month, DateTime.DaysInMonth(fullYear, month));
                
                // Card is valid if expiry date is at the end of the month or later
                // We check against the first day of the current month to allow cards expiring this month
                DateTime today = DateTime.Now;
                DateTime currentMonthStart = new DateTime(today.Year, today.Month, 1);
                
                return expiryDate >= currentMonthStart;
            }

            return false;
        }

        public override string FormatErrorMessage(string name)
        {
            return "This card has expired. Please enter a valid expiry date.";
        }
    }
}

