using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WisdomPetMedicine.Rescue.Domain.ValueObjects
{
    public record AdopterPhoneNumber
    {
        public string Value { get; init; }
        internal AdopterPhoneNumber(string value)
        {
            Value = value;
        }

        public static AdopterPhoneNumber Create(string value)
        {
            Validate(value);
            return new AdopterPhoneNumber(value);
        }

        private static void Validate(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("Phone number must not be null");
            }

            if (value.Length > 15)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Phone number must not be longer than 15 characters");
            }
        }
    }
}
