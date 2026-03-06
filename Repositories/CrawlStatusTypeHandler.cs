using System.Data;
using Dapper;
using Shared;

namespace Repositories;

/// <summary>
/// Maps CrawlStatus enum to MySQL ENUM string values ('Ok', 'UrlParsingError', etc.)
/// so Dapper sends/receives strings instead of integers.
/// </summary>
public class CrawlStatusTypeHandler : SqlMapper.TypeHandler<CrawlStatus>
{
    public override CrawlStatus Parse(object value)
    {
        if (value is null or DBNull)
            return default;
        var s = value.ToString();
        return string.IsNullOrEmpty(s) ? CrawlStatus.None : Enum.Parse<CrawlStatus>(s, ignoreCase: true);
    }

    public override void SetValue(IDbDataParameter parameter, CrawlStatus value)
    {
        parameter.DbType = DbType.String;
        parameter.Value = value.ToString();
    }
}
