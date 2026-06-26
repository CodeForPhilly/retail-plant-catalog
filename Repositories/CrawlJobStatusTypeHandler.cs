using System.Data;
using Dapper;
using Shared;

namespace Repositories;

/// <summary>
/// Maps <see cref="CrawlJobStatus"/> to MySQL ENUM string values ('Queued', 'Running', etc.)
/// so Dapper sends/receives strings instead of integers. Mirrors <see cref="CrawlStatusTypeHandler"/>.
/// </summary>
public class CrawlJobStatusTypeHandler : SqlMapper.TypeHandler<CrawlJobStatus>
{
    public override CrawlJobStatus Parse(object value)
    {
        if (value is null or DBNull)
            return default;
        var s = value.ToString();
        return string.IsNullOrEmpty(s) ? CrawlJobStatus.Queued : Enum.Parse<CrawlJobStatus>(s, ignoreCase: true);
    }

    public override void SetValue(IDbDataParameter parameter, CrawlJobStatus value)
    {
        parameter.DbType = DbType.String;
        parameter.Value = value.ToString();
    }
}
