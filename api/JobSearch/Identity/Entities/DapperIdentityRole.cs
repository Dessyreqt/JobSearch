namespace JobSearch.Identity.Entities
{
    using System;
    using System.Collections.Generic;

    public class DapperIdentityRole : DapperIdentityRole<int>
    {
        public DapperIdentityRole()
        {
        }

        public DapperIdentityRole(string roleName)
            : this()
        {
            Name = roleName;
        }
    }

    public class DapperIdentityRole<TKey> : DapperIdentityRole<TKey, DapperIdentityUserRole<TKey>, DapperIdentityRoleClaim<TKey>> where TKey : IEquatable<TKey>
    {
        public DapperIdentityRole()
        {
        }

        public DapperIdentityRole(string roleName)
            : this()
        {
            Name = roleName;
        }
    }

    public class DapperIdentityRole<TKey, TUserRole, TRoleClaim> where TKey : IEquatable<TKey>
        where TUserRole : DapperIdentityUserRole<TKey>
        where TRoleClaim : DapperIdentityRoleClaim<TKey>
    {
        public DapperIdentityRole()
        {
        }

        public DapperIdentityRole(string roleName)
            : this()
        {
            Name = roleName;
        }

        public virtual ICollection<TUserRole> Users { get; } = new List<TUserRole>();
        public virtual ICollection<TRoleClaim> Claims { get; } = new List<TRoleClaim>();
        public virtual TKey Id { get; set; }
        public virtual string Name { get; set; }
    }
}