using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CRUDapp.Models
{
    public interface IContact
    {
        int ID { get; set; }

        string Name { get; set; }

        string Email { get; set; }

        string Phone { get; set; }

        string Domain { get; set; }

        string Notes { get; set; }
    }

    [Table("Contact")]
    public class Contact : IContact
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        /// <summary>
        /// Contact full name
        /// </summary>
        [Required]
        [StringLength(30, ErrorMessage = "{0} length must be between {2} and {1} characters.", MinimumLength = 3)]
        public string Name { get; set; }

        /// <summary>
        /// Email address
        /// </summary>
        [Required]
        [StringLength(100)]
        [Display(Name = "Email Address")]
        [EmailAddress]
        public string Email { get; set; }

        /// <summary>
        /// Phone Number
        /// </summary>
        /// <remarks>
        /// 1. String using the enumeration method of validation 
        ///    and provides the DataTypeAttribute class to validate.
        /// 2. [Phone] => This format does make code more complex because the Phone class derives from the DataTypeAttribute class.
        /// </remarks>
        [Required]
        [StringLength(20)]
        [Display(Name = "Phone Number")]
        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }

        /// <summary>
        /// Domain name required
        /// </summary>
        [Required]
        [StringLength(100)]
        public string Domain { get; set; }

        /// <summary>
        /// Remarks
        /// </summary>
        [StringLength(1000)]
        public string Notes { get; set; }
    }
}
