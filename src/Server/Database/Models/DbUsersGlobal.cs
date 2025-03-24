using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Models
{
    [Table("UsersGlobal")]
    public class DbUsersGlobal
    {
        [Key] public Guid Id { get; set; }
        [StringLength(50)] public string Name { get; set; }
        public DateTime CreatedOn { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? DeletedOn { get; set; }
        public Guid? DeletedBy { get; set; }

        public ICollection<DbLinkUserCustomer>? LinkUserCustomers { get; set; }
    }
}
