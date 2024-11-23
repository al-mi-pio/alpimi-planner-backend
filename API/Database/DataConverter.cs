using Dapper;

public class DateOnlyTypeHandler : SqlMapper.TypeHandler<DateOnly>
{
    public override DateOnly Parse(object value)
    {
        // Convert DateTime to DateOnly
        if (value is DateTime dateTime)
        {
            return DateOnly.FromDateTime(dateTime);
        }
        throw new InvalidCastException($"Unable to cast {value.GetType()} to DateOnly");
    }

    public override void SetValue(System.Data.IDbDataParameter parameter, DateOnly value)
    {
        // Convert DateOnly to DateTime for storage in the database
        parameter.Value = value.ToDateTime(new TimeOnly(0, 0));
    }
}
