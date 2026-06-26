using Dapper.Contrib.Extensions;

namespace Shared
{
    /// <summary>
    /// A partner directory (e.g. Xerces Society) whose published vendor list PAC
    /// imports. A vendor row is associated with at most one partner via
    /// <see cref="Vendor.Partner"/> + <see cref="Vendor.ExternalKey"/>; the
    /// uniqueness of that pair is enforced at the DB level so re-running a
    /// partner sync is idempotent.
    /// </summary>
    [Table("partner")]
    public class Partner
    {
        /// <summary>Slug-style identifier, e.g. "Xerces". Foreign-keyed from vendor.Partner.</summary>
        [ExplicitKey]
        public string Id { get; set; } = "";

        public string DisplayName { get; set; } = "";

        public bool Active { get; set; } = true;

        public DateTime CreatedAt { get; set; }
    }
}
