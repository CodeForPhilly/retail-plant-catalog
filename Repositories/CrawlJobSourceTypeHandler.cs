using System.Data;
using Dapper;
using Shared;

namespace Repositories;

/// <summary>
/// Maps <see cref="CrawlJobSource"/> to MySQL ENUM string values ('UI', 'MCP', etc.)
/// so Dapper sends/receives strings instead of integers.
/// </summary>
public class CrawlJobSourceTypeHandler : SqlMapper.TypeHandler<CrawlJobSource>
{
    public override CrawlJobSource Parse(object value)
    {
        if (value is null or DBNull)
            return default;
        var s = value.ToString();
        return string.IsNullOrEmpty(s) ? CrawlJobSource.Other : Enum.Parse<CrawlJobSource>(s, ignoreCase: true);
    }

    public override void SetValue(IDbDataParameter parameter, CrawlJobSource value)
    {
        parameter.DbType = DbType.String;
        parameter.Value = value.ToString();
    }
}
