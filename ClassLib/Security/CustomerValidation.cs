using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace ClassLib.Model;

// Partial class customer - if we re-scaffold from the database, customer.cs gets overwritten
// To the rest of the program, it is one customer
// This way, we can add our own code in a second file which will never be overwritten
public partial class Customer 
{
    /// <summary>Throws ArgumentException if a property is invalid.</summary>
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Name))
            throw new ArgumentException("Kundenavnet skal udfyldes.", nameof(Name));
        if (Name.Length > 150)
            throw new ArgumentException("Kundenavnet må ikke overskride 150 tegn.", nameof(Name));

        if (ContactPerson?.Length > 150)
            throw new ArgumentException("Kontaktperson må ikke overskride 150 tegn.", nameof(ContactPerson));

        if (!string.IsNullOrWhiteSpace(Email))
        {
            if (Email.Length > 256)
                throw new ArgumentException("E-mail må ikke overskride 256 tegn.", nameof(Email));
            if (!MailAddress.TryCreate(Email, out _))
                throw new ArgumentException("E-mail er ikke gyldig. Prøv venligst igen.", nameof(Email));
        }

        if (!string.IsNullOrWhiteSpace(Phone))
        {
            if (Phone.Length > 50)
                throw new ArgumentException("Telefon nr. må ikke overskride 50 tegn.", nameof(Phone));
            if (!Regex.IsMatch(Phone, @"^\+?[0-9 \-]{8,}$"))
                throw new ArgumentException("Telefon nr. må kun indeholde tal, mellemrum, + og - (mindst 8 tegn).", nameof(Phone));
        }

        if (Website?.Length > 255)
            throw new ArgumentException("Hjemmeside må ikke overskride 255 tegn.", nameof(Website));

        if (OwnerId <= 0)
            throw new ArgumentException("Ansvarlig-ID skal være et positivt tal.", nameof(OwnerId));
    }
}
